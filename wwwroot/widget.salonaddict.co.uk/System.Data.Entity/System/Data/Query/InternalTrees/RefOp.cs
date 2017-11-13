namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class RefOp : ScalarOp
    {
        private System.Data.Metadata.Edm.EntitySet m_entitySet;
        internal static readonly RefOp Pattern = new RefOp();

        private RefOp() : base(OpType.Ref)
        {
        }

        internal RefOp(System.Data.Metadata.Edm.EntitySet entitySet, TypeUsage type) : base(OpType.Ref, type)
        {
            this.m_entitySet = entitySet;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal override int Arity =>
            1;

        internal System.Data.Metadata.Edm.EntitySet EntitySet =>
            this.m_entitySet;
    }
}

