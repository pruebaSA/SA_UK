namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class LeafCellTreeNode : CellTreeNode
    {
        internal static readonly IEqualityComparer<LeafCellTreeNode> EqualityComparer = new LeafCellTreeNodeComparer();
        private System.Data.Mapping.ViewGeneration.Structures.LeftCellWrapper m_cellWrapper;
        private FragmentQuery m_leftFragmentQuery;
        private FragmentQuery m_rightFragmentQuery;

        internal LeafCellTreeNode(CellNormalizer normalizer, System.Data.Mapping.ViewGeneration.Structures.LeftCellWrapper cellWrapper) : base(normalizer)
        {
            this.m_cellWrapper = cellWrapper;
            this.m_leftFragmentQuery = cellWrapper.FragmentQuery;
            this.m_rightFragmentQuery = FragmentQuery.Create(cellWrapper.OriginalCellNumberString, cellWrapper.CreateRoleBoolean(), cellWrapper.RightCellQuery);
        }

        internal LeafCellTreeNode(CellNormalizer normalizer, System.Data.Mapping.ViewGeneration.Structures.LeftCellWrapper cellWrapper, FragmentQuery rightFragmentQuery) : base(normalizer)
        {
            this.m_cellWrapper = cellWrapper;
            this.m_leftFragmentQuery = cellWrapper.FragmentQuery;
            this.m_rightFragmentQuery = rightFragmentQuery;
        }

        internal override TOutput Accept<TInput, TOutput>(CellTreeNode.CellTreeVisitor<TInput, TOutput> visitor, TInput param) => 
            visitor.VisitLeaf(this, param);

        internal override TOutput Accept<TInput, TOutput>(CellTreeNode.SimpleCellTreeVisitor<TInput, TOutput> visitor, TInput param) => 
            visitor.VisitLeaf(this, param);

        private StorageEndPropertyMapping GetForeignKeyEndMapFromAssocitionMap(StorageAssociationSetMapping colocatedAssociationSetMap, EntitySetBase thisExtent)
        {
            StorageMappingFragment fragment = colocatedAssociationSetMap.TypeMappings.First<StorageTypeMapping>().MappingFragments.First<StorageMappingFragment>();
            EntitySet storeEntitySet = (EntitySet) colocatedAssociationSetMap.StoreEntitySet;
            IEnumerable<EdmMember> keyMembers = storeEntitySet.ElementType.KeyMembers;
            using (IEnumerator<StoragePropertyMapping> enumerator = fragment.Properties.GetEnumerator())
            {
                Func<StorageEndPropertyMapping, bool> predicate = null;
                StorageEndPropertyMapping endMap;
                while (enumerator.MoveNext())
                {
                    endMap = (StorageEndPropertyMapping) enumerator.Current;
                    if (endMap.StoreProperties.SequenceEqual<EdmMember>(keyMembers, EqualityComparer<EdmMember>.Default))
                    {
                        if (predicate == null)
                        {
                            predicate = eMap => !eMap.Equals(endMap);
                        }
                        return fragment.Properties.OfType<StorageEndPropertyMapping>().Where<StorageEndPropertyMapping>(predicate).First<StorageEndPropertyMapping>();
                    }
                }
            }
            return null;
        }

        internal override bool IsProjectedSlot(int slot)
        {
            CellQuery rightCellQuery = this.RightCellQuery;
            if (base.IsBoolSlot(slot))
            {
                return (rightCellQuery.GetBoolVar(base.SlotToBoolIndex(slot)) != null);
            }
            return (rightCellQuery.ProjectedSlotAt(slot) != null);
        }

        internal override void ToCompactString(StringBuilder stringBuilder)
        {
            this.m_cellWrapper.ToCompactString(stringBuilder);
        }

        internal override CqlBlock ToCqlBlock(bool[] requiredSlots, CqlIdentifiers identifiers, ref int blockAliasNum, ref List<WithStatement> withStatements)
        {
            int num5;
            int length = requiredSlots.Length;
            CellQuery rightCellQuery = this.RightCellQuery;
            SlotInfo[] first = new SlotInfo[length];
            for (int i = 0; i < rightCellQuery.NumProjectedSlots; i++)
            {
                ProjectedSlot slot = rightCellQuery.ProjectedSlotAt(i);
                if (requiredSlots[i] && (slot == null))
                {
                    ConstantSlot slot2 = new ConstantSlot(CellConstantDomain.GetDefaultValueForMemberPath(base.ProjectedSlotMap[i], base.GetLeaves(), base.CellNormalizer.Config));
                    rightCellQuery.FixMissingSlotAsDefaultConstant(i, slot2);
                    slot = slot2;
                }
                first[i] = new SlotInfo(requiredSlots[i], slot != null, slot, base.ProjectedSlotMap[i]);
            }
            for (int j = 0; j < rightCellQuery.NumBoolVars; j++)
            {
                BooleanProjectedSlot slot3;
                BoolExpression boolVar = rightCellQuery.GetBoolVar(j);
                if (boolVar != null)
                {
                    slot3 = new BooleanProjectedSlot(boolVar, identifiers, j);
                }
                else
                {
                    slot3 = new BooleanProjectedSlot(BoolExpression.False, identifiers, j);
                }
                int index = base.BoolIndexToSlot(j);
                first[index] = new SlotInfo(requiredSlots[index], boolVar != null, slot3, null);
            }
            IEnumerable<SlotInfo> source = first;
            if ((rightCellQuery.Extent.EntityContainer.DataSpace == DataSpace.SSpace) && (this.m_cellWrapper.LeftExtent.BuiltInTypeKind == BuiltInTypeKind.EntitySet))
            {
                IEnumerable<StorageAssociationSetMapping> relationshipSetMappingsFor = base.CellNormalizer.EntityContainerMapping.GetRelationshipSetMappingsFor(this.m_cellWrapper.LeftExtent, rightCellQuery.Extent);
                List<SlotInfo> foreignKeySlots = new List<SlotInfo>();
                foreach (StorageAssociationSetMapping mapping in relationshipSetMappingsFor)
                {
                    WithStatement statement;
                    if (this.TryGetWithStatement(mapping, this.m_cellWrapper.LeftExtent, rightCellQuery.JoinTreeRoot, ref foreignKeySlots, out statement))
                    {
                        withStatements.Add(statement);
                        source = first.Concat<SlotInfo>(foreignKeySlots);
                    }
                }
            }
            blockAliasNum = num5 = blockAliasNum + 1;
            return new ExtentCqlBlock(rightCellQuery.Extent, source.ToArray<SlotInfo>(), rightCellQuery.WhereClause, identifiers, num5);
        }

        private bool TryGetWithStatement(StorageAssociationSetMapping colocatedAssociationSetMap, EntitySetBase thisExtent, JoinTreeNode sRootNode, ref List<SlotInfo> foreignKeySlots, out WithStatement withStatement)
        {
            withStatement = null;
            StorageEndPropertyMapping foreignKeyEndMapFromAssocitionMap = this.GetForeignKeyEndMapFromAssocitionMap(colocatedAssociationSetMap, thisExtent);
            if ((foreignKeyEndMapFromAssocitionMap == null) || (foreignKeyEndMapFromAssocitionMap.EndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many))
            {
                return false;
            }
            List<MemberPath> memberPathsForEndMembers = new List<MemberPath>();
            AssociationSet extent = (AssociationSet) colocatedAssociationSetMap.Set;
            MemberPath prefix = new MemberPath(extent, foreignKeyEndMapFromAssocitionMap.EndMember, base.CellNormalizer.Workspace);
            EntityTypeBase elementType = ((RefType) foreignKeyEndMapFromAssocitionMap.EndMember.TypeUsage.EdmType).ElementType;
            IEnumerable<StorageScalarPropertyMapping> source = foreignKeyEndMapFromAssocitionMap.Properties.Cast<StorageScalarPropertyMapping>();
            using (ReadOnlyMetadataCollection<EdmMember>.Enumerator enumerator = elementType.KeyMembers.GetEnumerator())
            {
                Func<StorageScalarPropertyMapping, bool> predicate = null;
                EdmProperty edmProperty;
                while (enumerator.MoveNext())
                {
                    edmProperty = (EdmProperty) enumerator.Current;
                    if (predicate == null)
                    {
                        predicate = propMap => propMap.EdmProperty.Equals(edmProperty);
                    }
                    StorageScalarPropertyMapping mapping2 = source.Where<StorageScalarPropertyMapping>(predicate).First<StorageScalarPropertyMapping>();
                    JoinTreeSlot slot = new JoinTreeSlot(sRootNode.CreateAttributeNode(mapping2.ColumnProperty));
                    MemberPath item = new MemberPath(prefix, edmProperty);
                    memberPathsForEndMembers.Add(item);
                    foreignKeySlots.Add(new SlotInfo(true, true, slot, item));
                }
            }
            EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(extent, (AssociationEndMember) foreignKeyEndMapFromAssocitionMap.EndMember);
            AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd((AssociationEndMember) foreignKeyEndMapFromAssocitionMap.EndMember);
            EntityType entityTypeForToEnd = (EntityType) ((RefType) foreignKeyEndMapFromAssocitionMap.EndMember.TypeUsage.EdmType).ElementType;
            EntityType entityTypeForFromEnd = (EntityType) ((RefType) otherAssociationEnd.TypeUsage.EdmType).ElementType;
            withStatement = new WithStatement(entitySetAtEnd, entityTypeForToEnd, entityTypeForFromEnd, extent, otherAssociationEnd.Name, foreignKeyEndMapFromAssocitionMap.EndMember.Name, memberPathsForEndMembers);
            return true;
        }

        internal override System.Data.Common.Utils.Set<MemberPath> Attributes =>
            this.m_cellWrapper.Attributes;

        internal override List<CellTreeNode> Children =>
            new List<CellTreeNode>();

        internal System.Data.Mapping.ViewGeneration.Structures.LeftCellWrapper LeftCellWrapper =>
            this.m_cellWrapper;

        internal override FragmentQuery LeftFragmentQuery =>
            this.m_cellWrapper.FragmentQuery;

        internal override int NumBoolSlots =>
            this.RightCellQuery.NumBoolVars;

        internal override int NumProjectedSlots =>
            this.RightCellQuery.NumProjectedSlots;

        internal override CellTreeOpType OpType =>
            CellTreeOpType.Leaf;

        internal CellQuery RightCellQuery =>
            this.m_cellWrapper.RightCellQuery;

        internal override MemberDomainMap RightDomainMap =>
            this.m_cellWrapper.RightDomainMap;

        internal override FragmentQuery RightFragmentQuery =>
            this.m_rightFragmentQuery;

        private class LeafCellTreeNodeComparer : IEqualityComparer<LeafCellTreeNode>
        {
            public bool Equals(LeafCellTreeNode left, LeafCellTreeNode right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.m_cellWrapper.Equals(right.m_cellWrapper)));

            public int GetHashCode(LeafCellTreeNode node) => 
                node.m_cellWrapper.GetHashCode();
        }
    }
}

