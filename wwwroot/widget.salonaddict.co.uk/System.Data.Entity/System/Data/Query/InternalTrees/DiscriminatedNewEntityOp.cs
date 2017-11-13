namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class DiscriminatedNewEntityOp : NewEntityBaseOp
    {
        private readonly ExplicitDiscriminatorMap m_discriminatorMap;
        internal static readonly DiscriminatedNewEntityOp Pattern = new DiscriminatedNewEntityOp();

        private DiscriminatedNewEntityOp() : base(OpType.DiscriminatedNewEntity)
        {
        }

        internal DiscriminatedNewEntityOp(TypeUsage type, ExplicitDiscriminatorMap discriminatorMap, EntitySetBase entitySet, List<RelProperty> relProperties) : base(OpType.DiscriminatedNewEntity, type, entitySet, relProperties)
        {
            this.m_discriminatorMap = discriminatorMap;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal ExplicitDiscriminatorMap DiscriminatorMap =>
            this.m_discriminatorMap;
    }
}

