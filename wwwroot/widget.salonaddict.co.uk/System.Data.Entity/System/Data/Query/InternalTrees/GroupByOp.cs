namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class GroupByOp : RelOp
    {
        private VarVec m_keys;
        private VarVec m_outputs;
        internal static readonly GroupByOp Pattern = new GroupByOp();

        private GroupByOp() : base(OpType.GroupBy)
        {
        }

        internal GroupByOp(VarVec keys, VarVec outputs) : this()
        {
            this.m_keys = keys;
            this.m_outputs = outputs;
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
            3;

        internal VarVec Keys =>
            this.m_keys;

        internal VarVec Outputs =>
            this.m_outputs;
    }
}

