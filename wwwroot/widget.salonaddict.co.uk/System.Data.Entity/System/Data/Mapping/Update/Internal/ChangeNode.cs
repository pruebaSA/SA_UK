namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal class ChangeNode
    {
        private List<PropagatorResult> m_deleted = new List<PropagatorResult>();
        private TypeUsage m_elementType;
        private List<PropagatorResult> m_inserted = new List<PropagatorResult>();
        private PropagatorResult m_placeholder;

        internal ChangeNode(TypeUsage elementType)
        {
            this.m_elementType = elementType;
        }

        internal List<PropagatorResult> Deleted =>
            this.m_deleted;

        internal TypeUsage ElementType =>
            this.m_elementType;

        internal List<PropagatorResult> Inserted =>
            this.m_inserted;

        internal PropagatorResult Placeholder
        {
            get => 
                this.m_placeholder;
            set
            {
                this.m_placeholder = value;
            }
        }
    }
}

