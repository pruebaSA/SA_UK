namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Text;

    internal class OpCellTreeNode : CellTreeNode
    {
        private Set<MemberPath> m_attrs;
        private List<CellTreeNode> m_children;
        private FragmentQuery m_leftFragmentQuery;
        private CellTreeOpType m_opType;
        private FragmentQuery m_rightFragmentQuery;

        internal OpCellTreeNode(CellNormalizer normalizer, CellTreeOpType opType) : base(normalizer)
        {
            this.m_opType = opType;
            this.m_attrs = new Set<MemberPath>(MemberPath.EqualityComparer);
            this.m_children = new List<CellTreeNode>();
        }

        internal OpCellTreeNode(CellNormalizer normalizer, CellTreeOpType opType, IEnumerable<CellTreeNode> children) : this(normalizer, opType)
        {
            foreach (CellTreeNode node in children)
            {
                this.Add(node);
            }
        }

        internal OpCellTreeNode(CellNormalizer normalizer, CellTreeOpType opType, params CellTreeNode[] children) : this(normalizer, opType, (IEnumerable<CellTreeNode>) children)
        {
        }

        internal override TOutput Accept<TInput, TOutput>(CellTreeNode.CellTreeVisitor<TInput, TOutput> visitor, TInput param)
        {
            switch (this.OpType)
            {
                case CellTreeOpType.Union:
                    return visitor.VisitUnion(this, param);

                case CellTreeOpType.FOJ:
                    return visitor.VisitFullOuterJoin(this, param);

                case CellTreeOpType.LOJ:
                    return visitor.VisitLeftOuterJoin(this, param);

                case CellTreeOpType.IJ:
                    return visitor.VisitInnerJoin(this, param);

                case CellTreeOpType.LASJ:
                    return visitor.VisitLeftAntiSemiJoin(this, param);
            }
            return visitor.VisitInnerJoin(this, param);
        }

        internal override TOutput Accept<TInput, TOutput>(CellTreeNode.SimpleCellTreeVisitor<TInput, TOutput> visitor, TInput param) => 
            visitor.VisitOpNode(this, param);

        internal void Add(CellTreeNode child)
        {
            this.Insert(this.m_children.Count, child);
        }

        private void AddCaseForOuterJoins(CaseStatement caseForOuterJoins, CqlBlock child, int slotNum, CqlIdentifiers identifiers)
        {
            ProjectedSlot slot = child.ProjectedSlot(slotNum);
            ConstantSlot slot2 = slot as ConstantSlot;
            if ((slot2 == null) || !slot2.CellConstant.IsNull())
            {
                BoolExpression @false = BoolExpression.False;
                for (int i = 0; i < this.NumBoolSlots; i++)
                {
                    int num2 = base.BoolIndexToSlot(i);
                    if (child.IsProjected(num2))
                    {
                        QualifiedCellIdBoolean literal = new QualifiedCellIdBoolean(child, identifiers, i);
                        @false = BoolExpression.CreateOr(new BoolExpression[] { @false, BoolExpression.CreateLiteral(literal, this.RightDomainMap) });
                    }
                }
                MemberPath memberPath = base.GetMemberPath(slotNum);
                AliasedSlot slot3 = new AliasedSlot(child, slot, memberPath, slotNum);
                caseForOuterJoins.AddWhenThen(@false, slot3);
            }
        }

        internal void AddFirst(CellTreeNode child)
        {
            this.Insert(0, child);
        }

        private static void AndWith(bool[] boolArray, bool[] another)
        {
            for (int i = 0; i < boolArray.Length; i++)
            {
                boolArray[i] &= another[i];
            }
        }

        private static int GetInnerJoinChildForSlot(List<CqlBlock> children, int slotNum)
        {
            int num = -1;
            for (int i = 0; i < children.Count; i++)
            {
                CqlBlock block = children[i];
                if (block.IsProjected(slotNum))
                {
                    ProjectedSlot slot = block.ProjectedSlot(slotNum);
                    ConstantSlot slot2 = slot as ConstantSlot;
                    if (slot is JoinTreeSlot)
                    {
                        num = i;
                    }
                    else if ((slot2 != null) && slot2.CellConstant.IsNull())
                    {
                        if (num == -1)
                        {
                            num = i;
                        }
                    }
                    else
                    {
                        num = i;
                    }
                }
            }
            return num;
        }

        private SlotInfo GetJoinSlotInfo(CellTreeOpType opType, bool isRequiredSlot, List<CqlBlock> children, int slotNum, CqlIdentifiers identifiers)
        {
            if (!isRequiredSlot)
            {
                return new SlotInfo(false, false, null, base.GetMemberPath(slotNum));
            }
            int innerJoinChildForSlot = -1;
            CaseStatement caseForOuterJoins = null;
            for (int i = 0; i < children.Count; i++)
            {
                CqlBlock child = children[i];
                if (child.IsProjected(slotNum))
                {
                    if (base.IsKeySlot(slotNum))
                    {
                        innerJoinChildForSlot = i;
                        break;
                    }
                    if (opType == CellTreeOpType.IJ)
                    {
                        innerJoinChildForSlot = GetInnerJoinChildForSlot(children, slotNum);
                        break;
                    }
                    if (innerJoinChildForSlot != -1)
                    {
                        if (caseForOuterJoins == null)
                        {
                            caseForOuterJoins = new CaseStatement(base.GetMemberPath(slotNum));
                            this.AddCaseForOuterJoins(caseForOuterJoins, children[innerJoinChildForSlot], slotNum, identifiers);
                        }
                        this.AddCaseForOuterJoins(caseForOuterJoins, child, slotNum, identifiers);
                    }
                    innerJoinChildForSlot = i;
                }
            }
            MemberPath memberPath = base.GetMemberPath(slotNum);
            ProjectedSlot slot = null;
            if ((caseForOuterJoins != null) && ((caseForOuterJoins.Clauses.Count > 0) || (caseForOuterJoins.ElseValue != null)))
            {
                caseForOuterJoins.Simplify();
                slot = new CaseStatementSlot(caseForOuterJoins, null);
            }
            else if (innerJoinChildForSlot >= 0)
            {
                slot = new AliasedSlot(children[innerJoinChildForSlot], children[innerJoinChildForSlot].ProjectedSlot(slotNum), memberPath, slotNum);
            }
            else if (base.IsBoolSlot(slotNum))
            {
                slot = new BooleanProjectedSlot(BoolExpression.False, identifiers, base.SlotToBoolIndex(slotNum));
            }
            else
            {
                slot = new ConstantSlot(CellConstantDomain.GetDefaultValueForMemberPath(memberPath, base.GetLeaves(), base.CellNormalizer.Config));
            }
            return new SlotInfo(true, true, slot, memberPath, base.IsBoolSlot(slotNum) && (((opType == CellTreeOpType.LOJ) && (innerJoinChildForSlot > 0)) || (opType == CellTreeOpType.FOJ)));
        }

        private void Insert(int index, CellTreeNode child)
        {
            this.m_attrs.Unite(child.Attributes);
            this.m_children.Insert(index, child);
            this.m_leftFragmentQuery = null;
            this.m_rightFragmentQuery = null;
        }

        internal override bool IsProjectedSlot(int slot)
        {
            foreach (CellTreeNode node in this.Children)
            {
                if (node.IsProjectedSlot(slot))
                {
                    return true;
                }
            }
            return false;
        }

        private CqlBlock JoinToCqlBlock(bool[] requiredSlots, CqlIdentifiers identifiers, ref int blockAliasNum, ref List<WithStatement> withStatements)
        {
            int num8;
            int length = requiredSlots.Length;
            List<CqlBlock> children = new List<CqlBlock>();
            List<AliasedSlot> list2 = new List<AliasedSlot>();
            foreach (CellTreeNode node in this.Children)
            {
                bool[] projectedSlots = node.GetProjectedSlots();
                AndWith(projectedSlots, requiredSlots);
                CqlBlock item = node.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withStatements);
                children.Add(item);
                for (int k = projectedSlots.Length; k < item.Slots.Count; k++)
                {
                    SlotInfo info = item.Slots[k];
                    list2.Add(new AliasedSlot(item, info.SlotValue, info.MemberPath, k));
                }
            }
            SlotInfo[] slotInfos = new SlotInfo[length + list2.Count];
            for (int i = 0; i < length; i++)
            {
                slotInfos[i] = this.GetJoinSlotInfo(this.OpType, requiredSlots[i], children, i, identifiers);
            }
            int num4 = 0;
            int index = length;
            while (index < (length + list2.Count))
            {
                slotInfos[index] = new SlotInfo(true, true, list2[num4], list2[num4].MemberPath);
                index++;
                num4++;
            }
            List<JoinCqlBlock.OnClause> onClauses = new List<JoinCqlBlock.OnClause>();
            for (int j = 1; j < children.Count; j++)
            {
                CqlBlock block = children[j];
                JoinCqlBlock.OnClause clause = new JoinCqlBlock.OnClause();
                foreach (int num7 in base.KeySlots)
                {
                    SlotInfo info3 = slotInfos[num7];
                    AliasedSlot firstSlot = new AliasedSlot(children[0], info3.SlotValue, info3.MemberPath, num7);
                    AliasedSlot secondSlot = new AliasedSlot(block, info3.SlotValue, info3.MemberPath, num7);
                    clause.Add(firstSlot, secondSlot);
                }
                onClauses.Add(clause);
            }
            blockAliasNum = num8 = blockAliasNum + 1;
            return new JoinCqlBlock(this.OpType, slotInfos, children, onClauses, identifiers, num8);
        }

        internal static string OpToCql(CellTreeOpType opType)
        {
            switch (opType)
            {
                case CellTreeOpType.Union:
                    return "UNION ALL";

                case CellTreeOpType.FOJ:
                    return "FULL OUTER JOIN";

                case CellTreeOpType.LOJ:
                    return "LEFT OUTER JOIN";

                case CellTreeOpType.IJ:
                    return "INNER JOIN";
            }
            return null;
        }

        internal override void ToCompactString(StringBuilder stringBuilder)
        {
            stringBuilder.Append("(");
            for (int i = 0; i < this.m_children.Count; i++)
            {
                this.m_children[i].ToCompactString(stringBuilder);
                if (i != (this.m_children.Count - 1))
                {
                    StringUtil.FormatStringBuilder(stringBuilder, " {0} ", new object[] { this.OpType });
                }
            }
            stringBuilder.Append(")");
        }

        internal override CqlBlock ToCqlBlock(bool[] requiredSlots, CqlIdentifiers identifiers, ref int blockAliasNum, ref List<WithStatement> withStatements)
        {
            if (this.OpType == CellTreeOpType.Union)
            {
                return this.UnionToCqlBlock(requiredSlots, identifiers, ref blockAliasNum, ref withStatements);
            }
            return this.JoinToCqlBlock(requiredSlots, identifiers, ref blockAliasNum, ref withStatements);
        }

        private CqlBlock UnionToCqlBlock(bool[] requiredSlots, CqlIdentifiers identifiers, ref int blockAliasNum, ref List<WithStatement> withStatements)
        {
            int num8;
            List<CqlBlock> children = new List<CqlBlock>();
            List<AliasedSlot> list2 = new List<AliasedSlot>();
            int length = requiredSlots.Length;
            foreach (CellTreeNode node in this.Children)
            {
                bool[] projectedSlots = node.GetProjectedSlots();
                AndWith(projectedSlots, requiredSlots);
                CqlBlock block = node.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withStatements);
                for (int j = projectedSlots.Length; j < block.Slots.Count; j++)
                {
                    SlotInfo info = block.Slots[j];
                    list2.Add(new AliasedSlot(block, info.SlotValue, info.MemberPath, j));
                }
                SlotInfo[] list = new SlotInfo[block.Slots.Count];
                ReadOnlyCollection<SlotInfo> slots = block.Slots;
                for (int k = 0; k < length; k++)
                {
                    if (requiredSlots[k] && !projectedSlots[k])
                    {
                        if (base.IsBoolSlot(k))
                        {
                            list[k] = new SlotInfo(true, true, new BooleanProjectedSlot(BoolExpression.False, identifiers, base.SlotToBoolIndex(k)), null);
                        }
                        else
                        {
                            list[k] = new SlotInfo(true, true, new ConstantSlot(CellConstant.Null), slots[k].MemberPath);
                        }
                    }
                    else
                    {
                        list[k] = slots[k];
                    }
                }
                block.Slots = new ReadOnlyCollection<SlotInfo>(list);
                children.Add(block);
            }
            if (list2.Count != 0)
            {
                foreach (CqlBlock block2 in children)
                {
                    SlotInfo[] array = new SlotInfo[length + list2.Count];
                    block2.Slots.CopyTo(array, 0);
                    int num4 = length;
                    foreach (AliasedSlot slot in list2)
                    {
                        if (slot.Block.Equals(block2))
                        {
                            array[num4] = new SlotInfo(true, true, slot.InnerSlot, slot.MemberPath);
                        }
                        else
                        {
                            array[num4] = new SlotInfo(true, true, new ConstantSlot(CellConstant.Null), slot.MemberPath);
                        }
                        num4++;
                    }
                    block2.Slots = new ReadOnlyCollection<SlotInfo>(array);
                }
            }
            SlotInfo[] slotInfos = new SlotInfo[length + list2.Count];
            CqlBlock block3 = children[0];
            for (int i = 0; i < length; i++)
            {
                ProjectedSlot slot2 = block3.ProjectedSlot(i);
                MemberPath memberPath = base.GetMemberPath(i);
                bool isRequiredByParent = requiredSlots[i];
                slotInfos[i] = new SlotInfo(isRequiredByParent, isRequiredByParent, slot2, memberPath);
            }
            int num6 = 0;
            int index = length;
            while (index < (length + list2.Count))
            {
                slotInfos[index] = new SlotInfo(true, true, list2[num6], list2[num6].MemberPath);
                index++;
                num6++;
            }
            blockAliasNum = num8 = blockAliasNum + 1;
            return new UnionCqlBlock(slotInfos, children, identifiers, num8);
        }

        internal override Set<MemberPath> Attributes =>
            this.m_attrs;

        internal override List<CellTreeNode> Children =>
            this.m_children;

        internal override FragmentQuery LeftFragmentQuery
        {
            get
            {
                if (this.m_leftFragmentQuery == null)
                {
                    FragmentQuery leftFragmentQuery = this.Children[0].LeftFragmentQuery;
                    FragmentQueryProcessor leftFragmentQP = base.CellNormalizer.LeftFragmentQP;
                    for (int i = 1; i < this.Children.Count; i++)
                    {
                        FragmentQuery query2 = this.Children[i].LeftFragmentQuery;
                        switch (this.OpType)
                        {
                            case CellTreeOpType.LOJ:
                                break;

                            case CellTreeOpType.IJ:
                                leftFragmentQuery = leftFragmentQP.Intersect(leftFragmentQuery, query2);
                                break;

                            case CellTreeOpType.LASJ:
                                leftFragmentQuery = leftFragmentQP.Difference(leftFragmentQuery, query2);
                                break;

                            default:
                                leftFragmentQuery = leftFragmentQP.Union(leftFragmentQuery, query2);
                                break;
                        }
                    }
                    this.m_leftFragmentQuery = leftFragmentQuery;
                }
                return this.m_leftFragmentQuery;
            }
        }

        internal override int NumBoolSlots =>
            this.m_children[0].NumBoolSlots;

        internal override int NumProjectedSlots =>
            this.m_children[0].NumProjectedSlots;

        internal override CellTreeOpType OpType =>
            this.m_opType;

        internal override MemberDomainMap RightDomainMap =>
            this.m_children[0].RightDomainMap;

        internal override FragmentQuery RightFragmentQuery
        {
            get
            {
                if (this.m_rightFragmentQuery == null)
                {
                    FragmentQuery rightFragmentQuery = this.Children[0].RightFragmentQuery;
                    FragmentQueryProcessor rightFragmentQP = base.CellNormalizer.RightFragmentQP;
                    for (int i = 1; i < this.Children.Count; i++)
                    {
                        FragmentQuery query2 = this.Children[i].RightFragmentQuery;
                        switch (this.OpType)
                        {
                            case CellTreeOpType.LOJ:
                                break;

                            case CellTreeOpType.IJ:
                                rightFragmentQuery = rightFragmentQP.Intersect(rightFragmentQuery, query2);
                                break;

                            case CellTreeOpType.LASJ:
                                rightFragmentQuery = rightFragmentQP.Difference(rightFragmentQuery, query2);
                                break;

                            default:
                                rightFragmentQuery = rightFragmentQP.Union(rightFragmentQuery, query2);
                                break;
                        }
                    }
                    this.m_rightFragmentQuery = rightFragmentQuery;
                }
                return this.m_rightFragmentQuery;
            }
        }
    }
}

