namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class StoragePropertyMapping
    {
        private System.Data.Metadata.Edm.EdmProperty m_cdmMember;

        internal StoragePropertyMapping(System.Data.Metadata.Edm.EdmProperty cdmMember)
        {
            this.m_cdmMember = cdmMember;
        }

        internal virtual void Print(int index)
        {
        }

        internal virtual System.Data.Metadata.Edm.EdmProperty EdmProperty =>
            this.m_cdmMember;
    }
}

