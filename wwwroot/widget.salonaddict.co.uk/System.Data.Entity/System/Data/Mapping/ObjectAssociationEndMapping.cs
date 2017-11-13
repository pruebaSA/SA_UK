namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ObjectAssociationEndMapping : ObjectMemberMapping
    {
        internal ObjectAssociationEndMapping(AssociationEndMember edmAssociationEnd, AssociationEndMember clrAssociationEnd) : base(edmAssociationEnd, clrAssociationEnd)
        {
        }

        internal override System.Data.Mapping.MemberMappingKind MemberMappingKind =>
            System.Data.Mapping.MemberMappingKind.AssociationEndMapping;
    }
}

