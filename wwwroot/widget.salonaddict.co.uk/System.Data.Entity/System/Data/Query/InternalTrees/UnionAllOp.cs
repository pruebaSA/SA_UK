namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class UnionAllOp : SetOp
    {
        private Var m_branchDiscriminator;
        internal static readonly UnionAllOp Pattern = new UnionAllOp();

        private UnionAllOp() : base(OpType.UnionAll)
        {
        }

        internal UnionAllOp(VarVec outputs, VarMap left, VarMap right, Var branchDiscriminator) : base(OpType.UnionAll, outputs, left, right)
        {
            this.m_branchDiscriminator = branchDiscriminator;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal Var BranchDiscriminator =>
            this.m_branchDiscriminator;
    }
}

