namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class ObjectMemberMapping
    {
        private System.Data.Metadata.Edm.EdmMember m_clrMember;
        private System.Data.Metadata.Edm.EdmMember m_edmMember;

        protected ObjectMemberMapping(System.Data.Metadata.Edm.EdmMember edmMember, System.Data.Metadata.Edm.EdmMember clrMember)
        {
            this.m_edmMember = edmMember;
            this.m_clrMember = clrMember;
        }

        internal System.Data.Metadata.Edm.EdmMember ClrMember =>
            this.m_clrMember;

        internal System.Data.Metadata.Edm.EdmMember EdmMember =>
            this.m_edmMember;

        internal abstract System.Data.Mapping.MemberMappingKind MemberMappingKind { get; }
    }
}

