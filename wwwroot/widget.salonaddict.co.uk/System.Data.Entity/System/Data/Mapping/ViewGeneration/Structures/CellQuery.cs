namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class CellQuery : InternalBase
    {
        private System.Data.Mapping.ViewGeneration.Validation.BasicCellRelation m_basicCellRelation;
        private List<BoolExpression> m_boolExprs;
        private DuplicateElimination m_eliminateDuplicates;
        private ExtentJoinTreeNode m_joinTreeRoot;
        private BoolExpression m_originalWhereClause;
        private ProjectedSlot[] m_projectedSlots;
        private SchemaContext m_schemaContext;
        private BoolExpression m_whereClause;

        internal CellQuery(CellQuery source)
        {
            this.m_basicCellRelation = source.m_basicCellRelation;
            this.m_boolExprs = source.m_boolExprs;
            this.m_eliminateDuplicates = source.m_eliminateDuplicates;
            this.m_joinTreeRoot = source.m_joinTreeRoot;
            this.m_originalWhereClause = source.m_originalWhereClause;
            this.m_projectedSlots = source.m_projectedSlots;
            this.m_schemaContext = source.m_schemaContext;
            this.m_whereClause = source.m_whereClause;
        }

        private CellQuery(CellQuery existing, ProjectedSlot[] newSlots) : this(existing.m_schemaContext, newSlots, existing.m_whereClause, existing.m_boolExprs, existing.m_eliminateDuplicates, existing.m_joinTreeRoot)
        {
        }

        internal CellQuery(List<ProjectedSlot> slots, BoolExpression whereClause, ExtentJoinTreeNode joinTreeRoot) : this(null, slots.ToArray(), whereClause, new List<BoolExpression>(), DuplicateElimination.No, joinTreeRoot)
        {
        }

        private CellQuery(SchemaContext schemaContext, ProjectedSlot[] projectedSlots, BoolExpression whereClause, List<BoolExpression> boolExprs, DuplicateElimination elimDupl, ExtentJoinTreeNode joinTreeRoot)
        {
            this.m_schemaContext = schemaContext;
            this.m_boolExprs = boolExprs;
            this.m_projectedSlots = projectedSlots;
            this.m_whereClause = whereClause;
            this.m_originalWhereClause = whereClause;
            this.m_eliminateDuplicates = elimDupl;
            this.m_joinTreeRoot = joinTreeRoot;
        }

        private bool AreSlotsEquivalentViaRefConstraints(ReadOnlyCollection<int> cSideSlotIndexes)
        {
            if (!(this.Extent is AssociationSet))
            {
                return false;
            }
            if (cSideSlotIndexes.Count > 2)
            {
                return false;
            }
            JoinTreeSlot slot = (JoinTreeSlot) this.m_projectedSlots[cSideSlotIndexes[0]];
            JoinTreeSlot slot2 = (JoinTreeSlot) this.m_projectedSlots[cSideSlotIndexes[1]];
            return slot.MemberPath.IsEquivalentViaRefConstraint(slot2.MemberPath);
        }

        internal ErrorLog.Record CheckForDuplicateFields(CellQuery cQuery, Cell sourceCell)
        {
            KeyToListMap<JoinTreeSlot, int> map = new KeyToListMap<JoinTreeSlot, int>(JoinTreeSlot.SpecificEqualityComparer);
            for (int i = 0; i < this.m_projectedSlots.Length; i++)
            {
                ProjectedSlot slot = this.m_projectedSlots[i];
                JoinTreeSlot key = slot as JoinTreeSlot;
                map.Add(key, i);
            }
            StringBuilder builder = null;
            bool flag = false;
            foreach (JoinTreeSlot slot3 in map.Keys)
            {
                ReadOnlyCollection<int> cSideSlotIndexes = map.ListForKey(slot3);
                if ((cSideSlotIndexes.Count > 1) && !cQuery.AreSlotsEquivalentViaRefConstraints(cSideSlotIndexes))
                {
                    flag = true;
                    if (builder == null)
                    {
                        builder = new StringBuilder(Strings.ViewGen_Duplicate_CProperties_0(this.Extent.Name));
                        builder.AppendLine();
                    }
                    StringBuilder builder2 = new StringBuilder();
                    for (int j = 0; j < cSideSlotIndexes.Count; j++)
                    {
                        int index = cSideSlotIndexes[j];
                        if (j != 0)
                        {
                            builder2.Append(", ");
                        }
                        builder2.Append(((JoinTreeSlot) cQuery.m_projectedSlots[index]).ToUserString());
                    }
                    builder.AppendLine(Strings.ViewGen_Duplicate_CProperties_IsMapped_1(slot3.ToUserString(), builder2.ToString()));
                }
            }
            if (!flag)
            {
                return null;
            }
            return new ErrorLog.Record(true, ViewGenErrorCode.DuplicateCPropertiesMapped, builder.ToString(), sourceCell, string.Empty);
        }

        internal ErrorLog.Record CheckForProjectedNotNullSlots(Cell sourceCell)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            foreach (OneOfConst @const in this.GetConjunctsFromOriginalWhereClause())
            {
                if (@const.Values.ContainsNotNull() && (JoinTreeSlot.GetSlotForMember(this.m_projectedSlots, @const.Slot.MemberPath) == null))
                {
                    builder.AppendLine(Strings.ViewGen_NotNull_No_Projected_Slot_0(@const.Slot.MemberPath.PathToString(false)));
                    flag = true;
                }
            }
            if (!flag)
            {
                return null;
            }
            return new ErrorLog.Record(true, ViewGenErrorCode.NotNullNoProjectedSlot, builder.ToString(), sourceCell, string.Empty);
        }

        internal override bool CheckRepInvariant()
        {
            foreach (BoolExpression expression in this.m_boolExprs)
            {
                expression.CheckRepInvariant();
            }
            this.m_whereClause.CheckRepInvariant();
            this.m_joinTreeRoot.CheckRepInvariant();
            return true;
        }

        internal bool CheckRepInvariant(CellQuery other)
        {
            this.CheckRepInvariant();
            other.CheckRepInvariant();
            ExceptionHelpers.CheckAndThrowResArgs(this.m_projectedSlots.Length == other.m_projectedSlots.Length, new Func<object, object, string>(Strings.ViewGen_SlotNumber_Mismatch_1), this, other);
            return true;
        }

        internal void CreateBasicCellRelation(ViewCellRelation viewCellRelation)
        {
            List<JoinTreeSlot> allQuerySlots = this.GetAllQuerySlots();
            this.m_basicCellRelation = new System.Data.Mapping.ViewGeneration.Validation.BasicCellRelation(this, viewCellRelation, allQuerySlots);
        }

        internal void CreateFieldAlignedCellQueries(CellQuery otherQuery, MemberPathMapBase projectedSlotMap, out CellQuery newMainQuery, out CellQuery newOtherQuery)
        {
            int count = projectedSlotMap.Count;
            ProjectedSlot[] newSlots = new ProjectedSlot[count];
            ProjectedSlot[] slotArray2 = new ProjectedSlot[count];
            for (int i = 0; i < this.m_projectedSlots.Length; i++)
            {
                JoinTreeSlot slot = this.m_projectedSlots[i] as JoinTreeSlot;
                int index = projectedSlotMap.IndexOf(slot.MemberPath);
                newSlots[index] = this.m_projectedSlots[i];
                slotArray2[index] = otherQuery.m_projectedSlots[i];
            }
            newMainQuery = new CellQuery(this, newSlots);
            newOtherQuery = new CellQuery(otherQuery, slotArray2);
        }

        internal void FixCellConstantDomains(MemberDomainMap domainMap, ViewTarget viewTarget)
        {
            List<BoolExpression> list = new List<BoolExpression>();
            foreach (BoolExpression expression in this.WhereClause.Atoms)
            {
                OneOfConst asLiteral = expression.AsLiteral as OneOfConst;
                IEnumerable<CellConstant> domain = domainMap.GetDomain(asLiteral.Slot.MemberPath);
                OneOfConst literal = OneOfConst.CreateFullOneOfConst(asLiteral, domain);
                OneOfScalarConst const3 = asLiteral as OneOfScalarConst;
                bool flag = (((const3 != null) && !const3.Values.Contains(CellConstant.Null)) && !const3.Values.Contains(CellConstant.NotNull)) && !const3.Values.Contains(CellConstant.Undefined);
                if (flag)
                {
                    domainMap.AddSentinel(literal.Slot.MemberPath);
                }
                list.Add(BoolExpression.CreateLiteral(literal, domainMap));
                if (flag)
                {
                    domainMap.RemoveSentinel(literal.Slot.MemberPath);
                }
            }
            if (list.Count > 0)
            {
                this.m_whereClause = BoolExpression.CreateAnd(list.ToArray());
            }
        }

        internal void FixMissingSlotAsDefaultConstant(int slotNumber, ConstantSlot slot)
        {
            this.m_projectedSlots[slotNumber] = slot;
        }

        internal List<JoinTreeSlot> GetAllQuerySlots()
        {
            List<JoinTreeSlot> slots = new List<JoinTreeSlot>();
            this.m_joinTreeRoot.GatherDescendantSlots(slots);
            return slots;
        }

        internal List<int> GetAssociationEndSlots(AssociationEndMember endMember)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < this.m_projectedSlots.Length; i++)
            {
                JoinTreeSlot slot = this.m_projectedSlots[i] as JoinTreeSlot;
                if ((slot != null) && slot.MemberPath.FirstMember.Equals(endMember))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        internal BoolExpression GetBoolVar(int varNum) => 
            this.m_boolExprs[varNum];

        internal IEnumerable<OneOfConst> GetConjunctsFromOriginalWhereClause() => 
            this.GetConjunctsFromWhereClause(this.m_originalWhereClause);

        internal IEnumerable<OneOfConst> GetConjunctsFromWhereClause() => 
            this.GetConjunctsFromWhereClause(this.m_whereClause);

        private IEnumerable<OneOfConst> GetConjunctsFromWhereClause(BoolExpression whereClause)
        {
            foreach (BoolExpression iteratorVariable0 in whereClause.Atoms)
            {
                if (!iteratorVariable0.IsTrue)
                {
                    OneOfConst asLiteral = iteratorVariable0.AsLiteral as OneOfConst;
                    yield return asLiteral;
                }
            }
        }

        internal void GetIdentifiers(CqlIdentifiers identifiers)
        {
            foreach (ProjectedSlot slot in this.m_projectedSlots)
            {
                JoinTreeSlot slot2 = slot as JoinTreeSlot;
                if (slot2 != null)
                {
                    slot2.MemberPath.GetIdentifiers(identifiers);
                }
            }
            this.m_joinTreeRoot.GetIdentifiers(identifiers);
        }

        internal Set<MemberPath> GetNonNullSlots()
        {
            Set<MemberPath> set = new Set<MemberPath>(MemberPath.EqualityComparer);
            foreach (ProjectedSlot slot in this.m_projectedSlots)
            {
                if (slot != null)
                {
                    JoinTreeSlot slot2 = slot as JoinTreeSlot;
                    set.Add(slot2.MemberPath);
                }
            }
            return set;
        }

        internal IEnumerable<MemberPath> GetProjectedMembers()
        {
            foreach (JoinTreeSlot iteratorVariable0 in this.GetProjectedSlots())
            {
                yield return iteratorVariable0.MemberPath;
            }
        }

        internal int GetProjectedPosition(JoinTreeSlot slot)
        {
            for (int i = 0; i < this.m_projectedSlots.Length; i++)
            {
                if (ProjectedSlot.EqualityComparer.Equals(slot, this.m_projectedSlots[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        internal List<int> GetProjectedPositions(IEnumerable<MemberPath> paths)
        {
            List<int> list = new List<int>();
            foreach (MemberPath path in paths)
            {
                List<int> projectedPositions = this.GetProjectedPositions(path);
                if (projectedPositions.Count == 0)
                {
                    return null;
                }
                list.Add(projectedPositions[0]);
            }
            return list;
        }

        internal List<int> GetProjectedPositions(MemberPath member)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < this.m_projectedSlots.Length; i++)
            {
                JoinTreeSlot slot = this.m_projectedSlots[i] as JoinTreeSlot;
                if ((slot != null) && MemberPath.EqualityComparer.Equals(member, slot.MemberPath))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        internal List<int> GetProjectedPositions(IEnumerable<MemberPath> paths, List<int> slotsToSearchFrom)
        {
            List<int> list = new List<int>();
            foreach (MemberPath path in paths)
            {
                List<int> projectedPositions = this.GetProjectedPositions(path);
                if (projectedPositions.Count == 0)
                {
                    return null;
                }
                int item = -1;
                if (projectedPositions.Count > 1)
                {
                    for (int i = 0; i < projectedPositions.Count; i++)
                    {
                        if (slotsToSearchFrom.Contains(projectedPositions[i]))
                        {
                            item = projectedPositions[i];
                        }
                    }
                    if (item == -1)
                    {
                        return null;
                    }
                }
                else
                {
                    item = projectedPositions[0];
                }
                list.Add(item);
            }
            return list;
        }

        private IEnumerable<JoinTreeSlot> GetProjectedSlots()
        {
            foreach (ProjectedSlot iteratorVariable0 in this.m_projectedSlots)
            {
                JoinTreeSlot iteratorVariable1 = iteratorVariable0 as JoinTreeSlot;
                if (iteratorVariable1 != null)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        internal void InitializeBoolExpressions(int numBoolVars, int cellNum)
        {
            this.m_boolExprs = new List<BoolExpression>(numBoolVars);
            for (int i = 0; i < numBoolVars; i++)
            {
                this.m_boolExprs.Add(null);
            }
            this.m_boolExprs[cellNum] = BoolExpression.True;
        }

        private static List<BoolExpression> MergeBoolExpressions(CellQuery query1, CellQuery query2, BoolExpression conjunct1, BoolExpression conjunct2, CellTreeOpType opType, bool canBooleansOverlap)
        {
            List<BoolExpression> boolExprs = query1.m_boolExprs;
            List<BoolExpression> bools = query2.m_boolExprs;
            if (!conjunct1.IsTrue)
            {
                boolExprs = BoolExpression.AddConjunctionToBools(boolExprs, conjunct1);
            }
            if (!conjunct2.IsTrue)
            {
                bools = BoolExpression.AddConjunctionToBools(bools, conjunct2);
            }
            List<BoolExpression> list3 = new List<BoolExpression>();
            for (int i = 0; i < boolExprs.Count; i++)
            {
                BoolExpression item = null;
                if (boolExprs[i] == null)
                {
                    item = bools[i];
                }
                else if (bools[i] == null)
                {
                    item = boolExprs[i];
                }
                else if (opType == CellTreeOpType.IJ)
                {
                    item = BoolExpression.CreateAnd(new BoolExpression[] { boolExprs[i], bools[i] });
                }
                else if (opType == CellTreeOpType.Union)
                {
                    item = BoolExpression.CreateOr(new BoolExpression[] { boolExprs[i], bools[i] });
                }
                else if (opType == CellTreeOpType.LASJ)
                {
                    item = BoolExpression.CreateAnd(new BoolExpression[] { boolExprs[i], BoolExpression.CreateNot(bools[i]) });
                }
                if (item != null)
                {
                    item.ExpensiveSimplify();
                }
                list3.Add(item);
            }
            return list3;
        }

        private static DuplicateElimination MergeDupl(DuplicateElimination d1, DuplicateElimination d2)
        {
            if ((d1 != DuplicateElimination.Yes) && (d2 != DuplicateElimination.Yes))
            {
                return DuplicateElimination.No;
            }
            return DuplicateElimination.Yes;
        }

        internal ProjectedSlot ProjectedSlotAt(int slotNum) => 
            this.m_projectedSlots[slotNum];

        internal override void ToCompactString(StringBuilder stringBuilder)
        {
            List<BoolExpression> boolExprs = this.m_boolExprs;
            int num = 0;
            bool flag = true;
            using (List<BoolExpression>.Enumerator enumerator = boolExprs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        if (!flag)
                        {
                            stringBuilder.Append(",");
                        }
                        else
                        {
                            stringBuilder.Append("[");
                        }
                        StringUtil.FormatStringBuilder(stringBuilder, "C{0}", new object[] { num });
                        flag = false;
                    }
                    num++;
                }
            }
            if (flag)
            {
                this.ToFullString(stringBuilder);
            }
            else
            {
                stringBuilder.Append("]");
            }
        }

        internal string ToESqlString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\n\tSELECT ");
            foreach (ProjectedSlot slot in this.m_projectedSlots)
            {
                JoinTreeSlot slot2 = slot as JoinTreeSlot;
                StructuralType declaringType = slot2.MemberPath.LastMember.DeclaringType;
                StringBuilder inputBuilder = new StringBuilder();
                slot2.MemberPath.AsCql(inputBuilder, "e");
                builder.AppendFormat("{0}, ", inputBuilder.ToString());
            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append("\n\tFROM ");
            EntitySetBase extent = this.m_joinTreeRoot.Extent;
            CqlWriter.AppendEscapedQualifiedName(builder, extent.EntityContainer.Name, extent.Name);
            builder.Append(" AS e");
            if (!this.m_whereClause.IsTrue)
            {
                builder.Append("\n\tWHERE ");
                StringBuilder builder3 = new StringBuilder();
                this.m_whereClause.AsCql(builder3, "e");
                builder.Append(builder3.ToString());
            }
            builder.Append("\n    ");
            return builder.ToString();
        }

        internal override void ToFullString(StringBuilder builder)
        {
            builder.Append("SELECT ");
            StringUtil.ToSeparatedString(builder, this.m_projectedSlots, ", ", "_");
            if (this.m_boolExprs.Count > 0)
            {
                builder.Append(", Bool[");
                StringUtil.ToSeparatedString(builder, this.m_boolExprs, ", ", "_");
                builder.Append("]");
            }
            builder.Append(" FROM ");
            this.m_joinTreeRoot.ToFullString(builder);
            if (!this.m_whereClause.IsTrue)
            {
                builder.Append(" WHERE ");
                this.m_whereClause.ToFullString(builder);
            }
        }

        public override string ToString() => 
            this.ToFullString();

        internal bool TryMerge(CellQuery query2, CellTreeOpType opType, bool canBooleansOverlap, MemberDomainMap memberDomainMap, out CellQuery mergedQuery)
        {
            ProjectedSlot[] slotArray;
            mergedQuery = null;
            BoolExpression expression = null;
            BoolExpression expression2 = null;
            switch (opType)
            {
                case CellTreeOpType.Union:
                case CellTreeOpType.FOJ:
                    expression = BoolExpression.True;
                    expression2 = BoolExpression.True;
                    break;

                case CellTreeOpType.LOJ:
                case CellTreeOpType.LASJ:
                    expression2 = BoolExpression.True;
                    break;
            }
            Dictionary<JoinTreeNode, JoinTreeNode> remap = new Dictionary<JoinTreeNode, JoinTreeNode>(EqualityComparer<JoinTreeNode>.Default);
            CellQuery query = this;
            ExtentJoinTreeNode joinTreeRoot = (ExtentJoinTreeNode) query.m_joinTreeRoot.TryMergeNode(query2.m_joinTreeRoot, opType, ref expression, ref expression2, remap, memberDomainMap);
            if (joinTreeRoot == null)
            {
                return false;
            }
            BoolExpression @true = BoolExpression.True;
            BoolExpression expression4 = BoolExpression.True;
            BoolExpression whereClause = null;
            switch (opType)
            {
                case CellTreeOpType.Union:
                case CellTreeOpType.FOJ:
                    @true = BoolExpression.CreateAnd(new BoolExpression[] { query.m_whereClause, expression });
                    expression4 = BoolExpression.CreateAnd(new BoolExpression[] { query2.m_whereClause, expression2 });
                    whereClause = BoolExpression.CreateOr(new BoolExpression[] { BoolExpression.CreateAnd(new BoolExpression[] { query.m_whereClause, expression }), BoolExpression.CreateAnd(new BoolExpression[] { query2.m_whereClause, expression2 }) });
                    break;

                case CellTreeOpType.LOJ:
                    expression4 = BoolExpression.CreateAnd(new BoolExpression[] { query2.m_whereClause, expression2 });
                    whereClause = query.m_whereClause;
                    break;

                case CellTreeOpType.IJ:
                    whereClause = BoolExpression.CreateAnd(new BoolExpression[] { query.m_whereClause, query2.m_whereClause });
                    break;

                case CellTreeOpType.LASJ:
                    expression4 = BoolExpression.CreateAnd(new BoolExpression[] { query2.m_whereClause, expression2 });
                    whereClause = BoolExpression.CreateAnd(new BoolExpression[] { query.m_whereClause, BoolExpression.CreateNot(expression4) });
                    break;
            }
            List<BoolExpression> bools = MergeBoolExpressions(query, query2, @true, expression4, opType, canBooleansOverlap);
            BoolExpression.RemapBools(bools, remap);
            if (!ProjectedSlot.TryMergeRemapSlots(query.m_projectedSlots, query2.m_projectedSlots, remap, out slotArray))
            {
                return false;
            }
            whereClause = whereClause.RemapBool(remap);
            DuplicateElimination elimDupl = MergeDupl(query.m_eliminateDuplicates, query2.m_eliminateDuplicates);
            whereClause.ExpensiveSimplify();
            mergedQuery = new CellQuery(query.m_schemaContext, slotArray, whereClause, bools, elimDupl, joinTreeRoot);
            return true;
        }

        internal ErrorLog.Record VerifyKeysPresent(Cell ownerCell, Func<object, object, string> formatEntitySetMessage, Func<object, object, object, string> formatAssociationSetMessage, ViewGenErrorCode errorCode)
        {
            List<MemberPath> list = new List<MemberPath>(1);
            List<ExtentKey> list2 = new List<ExtentKey>(1);
            if (this.Extent is EntitySet)
            {
                MemberPath item = new MemberPath(this.Extent, this.m_joinTreeRoot.MetadataWorkspace);
                list.Add(item);
                EntityType elementType = (EntityType) this.Extent.ElementType;
                List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(item, elementType);
                list2.Add(keysForEntityType[0]);
            }
            else
            {
                AssociationSet extent = (AssociationSet) this.Extent;
                foreach (AssociationSetEnd end in extent.AssociationSetEnds)
                {
                    AssociationEndMember correspondingAssociationEndMember = end.CorrespondingAssociationEndMember;
                    MemberPath path2 = new MemberPath(extent, correspondingAssociationEndMember, this.m_joinTreeRoot.MetadataWorkspace);
                    list.Add(path2);
                    List<ExtentKey> list4 = ExtentKey.GetKeysForEntityType(path2, MetadataHelper.GetEntityTypeForEnd(correspondingAssociationEndMember));
                    list2.Add(list4[0]);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                MemberPath prefix = list[i];
                if (JoinTreeSlot.GetKeySlots(this.GetProjectedSlots(), prefix) == null)
                {
                    string str;
                    ExtentKey key = list2[i];
                    if (this.Extent is EntitySet)
                    {
                        string str2 = MemberPath.PropertiesToUserString(key.KeyFields, true);
                        str = formatEntitySetMessage(str2, this.Extent.Name);
                    }
                    else
                    {
                        string name = prefix.FirstMember.Name;
                        string str4 = MemberPath.PropertiesToUserString(key.KeyFields, false);
                        str = formatAssociationSetMessage(str4, name, this.Extent.Name);
                    }
                    return new ErrorLog.Record(true, errorCode, str, ownerCell, string.Empty);
                }
            }
            return null;
        }

        internal void WhereClauseToUserString(StringBuilder builder, MetadataWorkspace workspace)
        {
            bool flag = true;
            foreach (OneOfConst @const in this.GetConjunctsFromWhereClause())
            {
                if (!flag)
                {
                    builder.Append(Strings.ViewGen_AND);
                }
                @const.ToUserString(false, builder, workspace);
            }
        }

        internal System.Data.Mapping.ViewGeneration.Validation.BasicCellRelation BasicCellRelation =>
            this.m_basicCellRelation;

        internal EntitySetBase Extent =>
            this.m_joinTreeRoot.Extent;

        internal JoinTreeNode JoinTreeRoot =>
            this.m_joinTreeRoot;

        internal int NumBoolVars =>
            this.m_boolExprs.Count;

        internal int NumProjectedSlots =>
            this.m_projectedSlots.Length;

        internal BoolExpression OriginalWhereClause =>
            this.m_originalWhereClause;

        internal BoolExpression WhereClause =>
            this.m_whereClause;




        private enum DuplicateElimination
        {
            Yes,
            No
        }
    }
}

