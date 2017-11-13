namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class UnnestOp : RelOp
    {
        private System.Data.Query.InternalTrees.Table m_table;
        private System.Data.Query.InternalTrees.Var m_var;
        internal static readonly UnnestOp Pattern = new UnnestOp();

        private UnnestOp() : base(OpType.Unnest)
        {
        }

        internal UnnestOp(System.Data.Query.InternalTrees.Var v, System.Data.Query.InternalTrees.Table t) : this()
        {
            this.m_var = v;
            this.m_table = t;
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

        internal System.Data.Query.InternalTrees.Table Table =>
            this.m_table;

        internal System.Data.Query.InternalTrees.Var Var =>
            this.m_var;
    }
}

