namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class CqlGenerator : InternalBase
    {
        private Dictionary<MemberPath, CaseStatement> m_caseStatements;
        private int m_currentBlockNum;
        private CqlIdentifiers m_identifiers;
        private int m_numBools;
        private MemberPathMapBase m_projectedSlotMap;
        private BoolExpression m_topLevelWhereClause;
        private CellTreeNode m_view;
        private MetadataWorkspace m_workspace;

        internal CqlGenerator(CellTreeNode view, Dictionary<MemberPath, CaseStatement> caseStatements, CqlIdentifiers identifiers, MemberPathMapBase projectedSlotMap, int numCellsInView, BoolExpression topLevelWhereClause, MetadataWorkspace workspace)
        {
            this.m_view = view;
            this.m_caseStatements = caseStatements;
            this.m_projectedSlotMap = projectedSlotMap;
            this.m_numBools = numCellsInView;
            this.m_topLevelWhereClause = topLevelWhereClause;
            this.m_identifiers = identifiers;
            this.m_workspace = workspace;
        }

        private CqlBlock ConstructCaseBlocks(CqlBlock viewBlock, IEnumerable<WithStatement> withStatements)
        {
            bool[] requiredSlots = new bool[this.TotalSlots];
            requiredSlots[0] = true;
            this.m_topLevelWhereClause.GetRequiredSlots(this.m_projectedSlotMap, requiredSlots);
            return this.ConstructCaseBlocks(viewBlock, 0, requiredSlots, withStatements);
        }

        private CqlBlock ConstructCaseBlocks(CqlBlock viewBlock, int startSlotNum, bool[] parentRequiredSlots, IEnumerable<WithStatement> withStatements)
        {
            int count = this.m_projectedSlotMap.Count;
            int memberSlotNum = this.FindNextCaseStatementSlot(startSlotNum, parentRequiredSlots, count);
            if (memberSlotNum == -1)
            {
                return viewBlock;
            }
            MemberPath member = this.m_projectedSlotMap[memberSlotNum];
            bool[] requiredSlots = new bool[this.TotalSlots];
            this.GetRequiredSlotsForCaseMember(memberSlotNum, member, requiredSlots);
            for (int i = 0; i < this.TotalSlots; i++)
            {
                if (parentRequiredSlots[i])
                {
                    requiredSlots[i] = true;
                }
            }
            CaseStatement thisCaseStatement = this.m_caseStatements[member];
            requiredSlots[memberSlotNum] = thisCaseStatement.DependsOnMemberValue;
            CqlBlock childBlock = this.ConstructCaseBlocks(viewBlock, memberSlotNum + 1, requiredSlots, null);
            SlotInfo[] slots = this.CreateSlotInfosForCaseStatement(parentRequiredSlots, memberSlotNum, childBlock, thisCaseStatement, withStatements);
            this.m_currentBlockNum++;
            BoolExpression whereClause = (startSlotNum == 0) ? this.m_topLevelWhereClause : BoolExpression.True;
            if (startSlotNum == 0)
            {
                for (int j = 1; j < slots.Length; j++)
                {
                    slots[j].ResetIsRequiredByParent();
                }
            }
            return new CaseCqlBlock(slots, memberSlotNum, childBlock, whereClause, this.m_identifiers, this.m_currentBlockNum);
        }

        private SlotInfo[] CreateSlotInfosForCaseStatement(bool[] parentRequiredSlots, int foundSlot, CqlBlock childBlock, CaseStatement thisCaseStatement, IEnumerable<WithStatement> withStatements)
        {
            int num = childBlock.Slots.Count - this.TotalSlots;
            SlotInfo[] infoArray = new SlotInfo[this.TotalSlots + num];
            for (int i = 0; i < this.TotalSlots; i++)
            {
                bool isProjected = childBlock.IsProjected(i);
                bool flag2 = parentRequiredSlots[i];
                ProjectedSlot slot = childBlock.ProjectedSlot(i);
                MemberPath memberPath = this.GetMemberPath(i);
                if (i == foundSlot)
                {
                    slot = new CaseStatementSlot(thisCaseStatement.MakeCaseWithAliasedSlots(childBlock, memberPath, i), withStatements);
                    isProjected = true;
                }
                else if (((slot != null) && isProjected) && flag2)
                {
                    slot = new AliasedSlot(childBlock, slot, memberPath, i);
                }
                infoArray[i] = new SlotInfo(flag2 && isProjected, isProjected, slot, memberPath);
            }
            for (int j = this.TotalSlots; j < (this.TotalSlots + num); j++)
            {
                SlotInfo info2 = childBlock.Slots[j];
                AliasedSlot slot2 = new AliasedSlot(childBlock, info2.SlotValue, info2.MemberPath, j);
                infoArray[j] = new SlotInfo(true, true, slot2, slot2.MemberPath);
            }
            return infoArray;
        }

        private int FindNextCaseStatementSlot(int startSlotNum, bool[] parentRequiredSlots, int numMembers)
        {
            for (int i = startSlotNum; i < numMembers; i++)
            {
                MemberPath key = this.m_projectedSlotMap[i];
                if (parentRequiredSlots[i] && this.m_caseStatements.ContainsKey(key))
                {
                    return i;
                }
            }
            return -1;
        }

        internal string GenerateCql()
        {
            CqlBlock block = this.GenerateCqlBlockTree();
            StringBuilder builder = new StringBuilder(0x400);
            block.AsCql(builder, true, 1);
            return builder.ToString();
        }

        private CqlBlock GenerateCqlBlockTree()
        {
            bool[] requiredSlots = this.GetRequiredSlots();
            List<WithStatement> withStatements = new List<WithStatement>();
            CqlBlock viewBlock = this.m_view.ToCqlBlock(requiredSlots, this.m_identifiers, ref this.m_currentBlockNum, ref withStatements);
            foreach (CaseStatement statement in this.m_caseStatements.Values)
            {
                statement.Simplify();
            }
            return this.ConstructCaseBlocks(viewBlock, withStatements);
        }

        private MemberPath GetMemberPath(int slotNum) => 
            ProjectedSlot.GetMemberPath(slotNum, this.m_projectedSlotMap, this.TotalSlots - this.m_projectedSlotMap.Count);

        private bool[] GetRequiredSlots()
        {
            bool[] requiredSlots = new bool[this.TotalSlots];
            foreach (CaseStatement statement in this.m_caseStatements.Values)
            {
                int index = this.m_projectedSlotMap.IndexOf(statement.MemberPath);
                this.GetRequiredSlotsForCaseMember(index, statement.MemberPath, requiredSlots);
            }
            for (int i = this.TotalSlots - this.m_numBools; i < this.TotalSlots; i++)
            {
                requiredSlots[i] = true;
            }
            foreach (CaseStatement statement2 in this.m_caseStatements.Values)
            {
                if (!statement2.MemberPath.IsScalarType() || !statement2.DependsOnMemberValue)
                {
                    requiredSlots[this.m_projectedSlotMap.IndexOf(statement2.MemberPath)] = false;
                }
            }
            return requiredSlots;
        }

        private void GetRequiredSlotsForCaseMember(int memberSlotNum, MemberPath member, bool[] requiredSlots)
        {
            CaseStatement statement = this.m_caseStatements[member];
            bool flag = false;
            foreach (CaseStatement.WhenThen then in statement.Clauses)
            {
                then.Condition.GetRequiredSlots(this.m_projectedSlotMap, requiredSlots);
                if (!(then.Value is ConstantSlot))
                {
                    flag = true;
                }
            }
            EdmType edmType = member.EdmType;
            if (Helper.IsEntityType(edmType) || Helper.IsComplexType(edmType))
            {
                foreach (EdmType type2 in statement.InstantiatedTypes)
                {
                    foreach (EdmMember member2 in Helper.GetAllStructuralMembers(type2))
                    {
                        int slotIndex = this.GetSlotIndex(member, member2);
                        requiredSlots[slotIndex] = true;
                    }
                }
            }
            else if (member.IsScalarType())
            {
                if (flag)
                {
                    requiredSlots[memberSlotNum] = true;
                }
            }
            else if (Helper.IsAssociationType(edmType))
            {
                AssociationSet extent = (AssociationSet) member.Extent;
                foreach (AssociationEndMember member3 in extent.ElementType.AssociationEndMembers)
                {
                    int index = this.GetSlotIndex(member, member3);
                    requiredSlots[index] = true;
                }
            }
            else
            {
                RefType type4 = edmType as RefType;
                EntityTypeBase elementType = type4.ElementType;
                MetadataHelper.GetEntitySetAtEnd((AssociationSet) member.Extent, (AssociationEndMember) member.LastMember);
                foreach (EdmMember member4 in elementType.KeyMembers)
                {
                    int num3 = this.GetSlotIndex(member, member4);
                    requiredSlots[num3] = true;
                }
            }
        }

        private int GetSlotIndex(MemberPath member, EdmMember child)
        {
            MemberPath path = new MemberPath(member, child);
            return this.m_projectedSlotMap.IndexOf(path);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("View: ");
            this.m_view.ToCompactString(builder);
            builder.Append("ProjectedSlotMap: ");
            this.m_projectedSlotMap.ToCompactString(builder);
            builder.Append("Case statements: ");
            foreach (MemberPath path in this.m_caseStatements.Keys)
            {
                this.m_caseStatements[path].ToCompactString(builder);
                builder.AppendLine();
            }
        }

        private int TotalSlots =>
            (this.m_projectedSlotMap.Count + this.m_numBools);
    }
}

