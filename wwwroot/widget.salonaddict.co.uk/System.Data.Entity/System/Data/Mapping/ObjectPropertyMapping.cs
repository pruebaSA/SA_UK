namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ObjectPropertyMapping : ObjectMemberMapping
    {
        internal ObjectPropertyMapping(EdmProperty edmProperty, EdmProperty clrProperty) : base(edmProperty, clrProperty)
        {
        }

        internal EdmProperty ClrProperty =>
            ((EdmProperty) base.ClrMember);

        internal override System.Data.Mapping.MemberMappingKind MemberMappingKind =>
            System.Data.Mapping.MemberMappingKind.ScalarPropertyMapping;
    }
}

