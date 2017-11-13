namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class VarRefOp : ScalarOp
    {
        private System.Data.Query.InternalTrees.Var m_var;
        internal static readonly VarRefOp Pattern = new VarRefOp();

        private VarRefOp() : base(OpType.VarRef)
        {
        }

        internal VarRefOp(System.Data.Query.InternalTrees.Var v) : base(OpType.VarRef, v.Type)
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

        internal override bool? IsEquivalent(Op other)
        {
            VarRefOp op = other as VarRefOp;
            return new bool?((op != null) && op.Var.Equals(this.Var));
        }

        internal override int Arity =>
            0;

        internal System.Data.Query.InternalTrees.Var Var =>
            this.m_var;
    }
}

