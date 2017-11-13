namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ObjectNavigationPropertyMapping : ObjectMemberMapping
    {
        internal ObjectNavigationPropertyMapping(NavigationProperty edmNavigationProperty, NavigationProperty clrNavigationProperty) : base(edmNavigationProperty, clrNavigationProperty)
        {
        }

        internal override System.Data.Mapping.MemberMappingKind MemberMappingKind =>
            System.Data.Mapping.MemberMappingKind.NavigationPropertyMapping;
    }
}

