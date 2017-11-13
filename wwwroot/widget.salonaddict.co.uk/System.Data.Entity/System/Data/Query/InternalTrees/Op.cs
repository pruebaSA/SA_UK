namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal abstract class Op
    {
        internal const int ArityVarying = -1;
        private System.Data.Query.InternalTrees.OpType m_opType;

        internal Op(System.Data.Query.InternalTrees.OpType opType)
        {
            this.m_opType = opType;
        }

        [DebuggerNonUserCode]
        internal virtual void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal virtual TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal virtual bool? IsEquivalent(Op other) => 
            false;

        internal virtual int Arity =>
            -1;

        internal virtual bool IsAncillaryOp =>
            false;

        internal virtual bool IsPhysicalOp =>
            false;

        internal virtual bool IsRelOp =>
            false;

        internal virtual bool IsRulePatternOp =>
            false;

        internal virtual bool IsScalarOp =>
            false;

        internal System.Data.Query.InternalTrees.OpType OpType =>
            this.m_opType;

        internal virtual TypeUsage Type
        {
            get => 
                null;
            set
            {
                throw Error.NotSupported();
            }
        }
    }
}

