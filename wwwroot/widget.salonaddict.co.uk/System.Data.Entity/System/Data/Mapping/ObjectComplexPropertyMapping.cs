namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ObjectComplexPropertyMapping : ObjectPropertyMapping
    {
        private readonly ObjectTypeMapping m_objectTypeMapping;

        internal ObjectComplexPropertyMapping(EdmProperty edmProperty, EdmProperty clrProperty, ObjectTypeMapping complexTypeMapping) : base(edmProperty, clrProperty)
        {
            this.m_objectTypeMapping = complexTypeMapping;
        }

        internal override System.Data.Mapping.MemberMappingKind MemberMappingKind =>
            System.Data.Mapping.MemberMappingKind.ComplexPropertyMapping;
    }
}

