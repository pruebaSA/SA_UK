namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Data.Mapping.ViewGeneration;

    internal class MemberMaps
    {
        private MemberPathMapBase m_projectedSlotMap;
        private MemberDomainMap m_queryDomainMap;
        private SchemaContext m_schemaContext;
        private MemberDomainMap m_updateDomainMap;

        internal MemberMaps(SchemaContext schemaContext, MemberPathMapBase projectedSlotMap, MemberDomainMap queryDomainMap, MemberDomainMap updateDomainMap)
        {
            this.m_projectedSlotMap = projectedSlotMap;
            this.m_queryDomainMap = queryDomainMap;
            this.m_updateDomainMap = updateDomainMap;
            this.m_schemaContext = schemaContext;
        }

        internal MemberDomainMap LeftDomainMap
        {
            get
            {
                if (this.m_schemaContext.ViewTarget != ViewTarget.QueryView)
                {
                    return this.m_updateDomainMap;
                }
                return this.m_queryDomainMap;
            }
        }

        internal MemberPathMapBase ProjectedSlotMap =>
            this.m_projectedSlotMap;

        internal MemberDomainMap QueryDomainMap =>
            this.m_queryDomainMap;

        internal MemberDomainMap RightDomainMap
        {
            get
            {
                if (this.m_schemaContext.ViewTarget != ViewTarget.QueryView)
                {
                    return this.m_queryDomainMap;
                }
                return this.m_updateDomainMap;
            }
        }

        internal MemberDomainMap UpdateDomainMap =>
            this.m_updateDomainMap;
    }
}

