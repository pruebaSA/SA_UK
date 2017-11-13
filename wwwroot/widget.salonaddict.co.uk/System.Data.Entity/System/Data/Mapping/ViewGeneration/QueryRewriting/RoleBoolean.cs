namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class RoleBoolean : TrueFalseLiteral
    {
        private MetadataItem m_metadataItem;

        internal RoleBoolean(AssociationSetEnd end)
        {
            this.m_metadataItem = end;
        }

        internal RoleBoolean(EntitySetBase extent)
        {
            this.m_metadataItem = extent;
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            this.ToCompactString(builder);
            builder.Append("(");
            builder.Append(blockAlias);
            builder.Append(")");
            return builder;
        }

        internal override StringBuilder AsNegatedUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            AssociationSetEnd metadataItem = this.m_metadataItem as AssociationSetEnd;
            if (metadataItem != null)
            {
                builder.Append(Strings.ViewGen_AssociationSet_AsUserString_Negated(blockAlias, metadataItem.Name, metadataItem.ParentAssociationSet));
                return builder;
            }
            builder.Append(Strings.ViewGen_EntitySet_AsUserString_Negated(blockAlias, this.m_metadataItem.ToString()));
            return builder;
        }

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            AssociationSetEnd metadataItem = this.m_metadataItem as AssociationSetEnd;
            if (metadataItem != null)
            {
                builder.Append(Strings.ViewGen_AssociationSet_AsUserString(blockAlias, metadataItem.Name, metadataItem.ParentAssociationSet));
                return builder;
            }
            builder.Append(Strings.ViewGen_EntitySet_AsUserString(blockAlias, this.m_metadataItem.ToString()));
            return builder;
        }

        internal override bool CheckRepInvariant() => 
            true;

        protected override int GetHash() => 
            this.m_metadataItem.GetHashCode();

        internal override void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
        {
            throw new NotImplementedException();
        }

        protected override bool IsEqualTo(BoolLiteral right)
        {
            RoleBoolean flag = right as RoleBoolean;
            if (flag == null)
            {
                return false;
            }
            return (this.m_metadataItem == flag.m_metadataItem);
        }

        internal override BoolLiteral RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap) => 
            this;

        internal override void ToCompactString(StringBuilder builder)
        {
            AssociationSetEnd metadataItem = this.m_metadataItem as AssociationSetEnd;
            if (metadataItem != null)
            {
                builder.Append(string.Concat(new object[] { "InEnd:", metadataItem.ParentAssociationSet, "_", metadataItem.Name }));
            }
            else
            {
                builder.Append("InSet:" + this.m_metadataItem.ToString());
            }
        }
    }
}

