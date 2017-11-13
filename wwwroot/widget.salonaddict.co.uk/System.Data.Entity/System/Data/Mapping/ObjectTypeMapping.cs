namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal class ObjectTypeMapping : Map
    {
        private static readonly Dictionary<string, ObjectMemberMapping> EmptyMemberMapping = new Dictionary<string, ObjectMemberMapping>(0);
        private readonly string identity;
        private readonly System.Data.Metadata.Edm.EdmType m_cdmType;
        private readonly System.Data.Metadata.Edm.EdmType m_clrType;
        private readonly Dictionary<string, ObjectMemberMapping> m_memberMapping;

        internal ObjectTypeMapping(System.Data.Metadata.Edm.EdmType clrType, System.Data.Metadata.Edm.EdmType cdmType)
        {
            this.m_clrType = clrType;
            this.m_cdmType = cdmType;
            this.identity = clrType.Identity + ':' + cdmType.Identity;
            if (Helper.IsStructuralType(cdmType))
            {
                this.m_memberMapping = new Dictionary<string, ObjectMemberMapping>(((StructuralType) cdmType).Members.Count);
            }
            else
            {
                this.m_memberMapping = EmptyMemberMapping;
            }
        }

        internal void AddMemberMap(ObjectMemberMapping memberMapping)
        {
            this.m_memberMapping.Add(memberMapping.EdmMember.Name, memberMapping);
        }

        private ObjectMemberMapping GetMemberMap(string propertyName, bool ignoreCase)
        {
            EntityUtil.CheckStringArgument(propertyName, "propertyName");
            ObjectMemberMapping mapping = null;
            if (!ignoreCase)
            {
                this.m_memberMapping.TryGetValue(propertyName, out mapping);
                return mapping;
            }
            foreach (KeyValuePair<string, ObjectMemberMapping> pair in this.m_memberMapping)
            {
                if (pair.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    if (mapping != null)
                    {
                        throw new MappingException(Strings.Mapping_Duplicate_PropertyMap_CaseInsensitive(propertyName));
                    }
                    mapping = pair.Value;
                }
            }
            return mapping;
        }

        internal ObjectMemberMapping GetMemberMapForClrMember(string clrMemberName, bool ignoreCase) => 
            this.GetMemberMap(clrMemberName, ignoreCase);

        internal ObjectPropertyMapping GetPropertyMap(string propertyName)
        {
            ObjectMemberMapping memberMap = this.GetMemberMap(propertyName, false);
            if (((memberMap == null) || (memberMap.MemberMappingKind != MemberMappingKind.ScalarPropertyMapping)) && (memberMap.MemberMappingKind != MemberMappingKind.ComplexPropertyMapping))
            {
                return null;
            }
            return (ObjectPropertyMapping) memberMap;
        }

        public override string ToString() => 
            this.Identity;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem;

        internal System.Data.Metadata.Edm.EdmType ClrType =>
            this.m_clrType;

        internal override MetadataItem EdmItem =>
            this.EdmType;

        internal System.Data.Metadata.Edm.EdmType EdmType =>
            this.m_cdmType;

        internal override string Identity =>
            this.identity;
    }
}

