namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal abstract class NewEntityBaseOp : ScalarOp
    {
        private readonly EntitySetBase m_entitySet;
        private readonly List<RelProperty> m_relProperties;

        protected NewEntityBaseOp(OpType opType) : base(opType)
        {
        }

        internal NewEntityBaseOp(OpType opType, TypeUsage type, EntitySetBase entitySet, List<RelProperty> relProperties) : base(opType, type)
        {
            this.m_entitySet = entitySet;
            this.m_relProperties = relProperties;
        }

        internal EntitySetBase EntitySet =>
            this.m_entitySet;

        internal List<RelProperty> RelationshipProperties =>
            this.m_relProperties;
    }
}

