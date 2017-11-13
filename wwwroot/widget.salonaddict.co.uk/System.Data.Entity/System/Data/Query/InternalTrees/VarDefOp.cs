namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class VarDefOp : AncillaryOp
    {
        private System.Data.Query.InternalTrees.Var m_var;
        internal static readonly VarDefOp Pattern = new VarDefOp();

        private VarDefOp() : base(OpType.VarDef)
        {
        }

        internal VarDefOp(System.Data.Query.InternalTrees.Var v) : this()
        {
            this.m_var = v;
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

        internal System.Data.Query.InternalTrees.Var Var =>
            this.m_var;
    }
}

