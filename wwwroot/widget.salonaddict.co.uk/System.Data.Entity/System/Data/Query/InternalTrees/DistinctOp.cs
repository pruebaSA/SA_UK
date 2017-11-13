namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class DistinctOp : RelOp
    {
        private VarVec m_keys;
        internal static readonly DistinctOp Pattern = new DistinctOp();

        private DistinctOp() : base(OpType.Distinct)
        {
        }

        internal DistinctOp(VarVec keyVars) : this()
        {
            this.m_keys = keyVars;
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

        internal VarVec Keys =>
            this.m_keys;
    }
}

