namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class BasicCellRelation : CellRelation
    {
        private CellQuery m_cellQuery;
        private List<JoinTreeSlot> m_slots;
        private System.Data.Mapping.ViewGeneration.Validation.ViewCellRelation m_viewCellRelation;

        internal BasicCellRelation(CellQuery cellQuery, System.Data.Mapping.ViewGeneration.Validation.ViewCellRelation viewCellRelation, IEnumerable<JoinTreeSlot> slots) : base(viewCellRelation.CellNumber)
        {
            this.m_cellQuery = cellQuery;
            this.m_slots = new List<JoinTreeSlot>(slots);
            this.m_viewCellRelation = viewCellRelation;
        }

        private void AddKeyConstraints(IEnumerable<ExtentKey> keys, SchemaConstraints<BasicKeyConstraint> constraints)
        {
            foreach (ExtentKey key in keys)
            {
                List<JoinTreeSlot> slots = JoinTreeSlot.GetSlots(this.m_slots, key.KeyFields);
                if (slots != null)
                {
                    BasicKeyConstraint constraint = new BasicKeyConstraint(this, slots);
                    constraints.Add(constraint);
                }
            }
        }

        protected override int GetHash() => 
            this.m_cellQuery.GetHashCode();

        internal void PopulateKeyConstraints(SchemaConstraints<BasicKeyConstraint> constraints, MetadataWorkspace workspace)
        {
            if (this.m_cellQuery.Extent is EntitySet)
            {
                this.PopulateKeyConstraintsForEntitySet(constraints, workspace);
            }
            else
            {
                this.PopulateKeyConstraintsForRelationshipSet(constraints, workspace);
            }
        }

        private void PopulateKeyConstraintsForEntitySet(SchemaConstraints<BasicKeyConstraint> constraints, MetadataWorkspace workspace)
        {
            MemberPath prefix = new MemberPath(this.m_cellQuery.Extent, workspace);
            EntityType elementType = (EntityType) this.m_cellQuery.Extent.ElementType;
            List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(prefix, elementType);
            this.AddKeyConstraints(keysForEntityType, constraints);
        }

        private void PopulateKeyConstraintsForRelationshipSet(SchemaConstraints<BasicKeyConstraint> constraints, MetadataWorkspace workspace)
        {
            AssociationSet extent = this.m_cellQuery.Extent as AssociationSet;
            Set<MemberPath> keyFields = new Set<MemberPath>(MemberPath.EqualityComparer);
            bool flag = false;
            foreach (AssociationSetEnd end in extent.AssociationSetEnds)
            {
                AssociationEndMember correspondingAssociationEndMember = end.CorrespondingAssociationEndMember;
                MemberPath prefix = new MemberPath(extent, correspondingAssociationEndMember, workspace);
                List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(prefix, end.EntitySet.ElementType);
                if (MetadataHelper.DoesEndFormKey(extent, correspondingAssociationEndMember))
                {
                    this.AddKeyConstraints(keysForEntityType, constraints);
                    flag = true;
                }
                keyFields.AddRange(keysForEntityType[0].KeyFields);
            }
            if (!flag)
            {
                ExtentKey key = new ExtentKey(keyFields);
                ExtentKey[] keys = new ExtentKey[] { key };
                this.AddKeyConstraints(keys, constraints);
            }
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("BasicRel: ");
            StringUtil.FormatStringBuilder(builder, "{0}", new object[] { this.m_slots[0] });
        }

        internal System.Data.Mapping.ViewGeneration.Validation.ViewCellRelation ViewCellRelation =>
            this.m_viewCellRelation;
    }
}

