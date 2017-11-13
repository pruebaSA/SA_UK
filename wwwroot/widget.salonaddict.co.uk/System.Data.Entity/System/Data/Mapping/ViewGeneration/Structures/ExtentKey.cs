namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class ExtentKey : InternalBase
    {
        private List<MemberPath> m_keyFields;

        internal ExtentKey(IEnumerable<MemberPath> keyFields)
        {
            this.m_keyFields = new List<MemberPath>(keyFields);
        }

        internal static ExtentKey GetKeyForRelationType(MemberPath prefix, AssociationType relationType)
        {
            List<MemberPath> keyFields = new List<MemberPath>();
            foreach (AssociationEndMember member in relationType.AssociationEndMembers)
            {
                MemberPath path = new MemberPath(prefix, member);
                EntityType entityTypeForEnd = MetadataHelper.GetEntityTypeForEnd(member);
                ExtentKey primaryKeyForEntityType = GetPrimaryKeyForEntityType(path, entityTypeForEnd);
                keyFields.AddRange(primaryKeyForEntityType.KeyFields);
            }
            return new ExtentKey(keyFields);
        }

        internal static List<ExtentKey> GetKeysForEntityType(MemberPath prefix, EntityType entityType)
        {
            ExtentKey primaryKeyForEntityType = GetPrimaryKeyForEntityType(prefix, entityType);
            return new List<ExtentKey> { primaryKeyForEntityType };
        }

        internal static ExtentKey GetPrimaryKeyForEntityType(MemberPath prefix, EntityType entityType)
        {
            List<MemberPath> keyFields = new List<MemberPath>();
            foreach (EdmMember member in entityType.KeyMembers)
            {
                keyFields.Add(new MemberPath(prefix, member));
            }
            return new ExtentKey(keyFields);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.ToCommaSeparatedStringSorted(builder, this.m_keyFields);
        }

        internal string ToUserString() => 
            StringUtil.ToCommaSeparatedStringSorted(this.m_keyFields);

        internal IEnumerable<MemberPath> KeyFields =>
            this.m_keyFields;
    }
}

