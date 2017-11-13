namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class ProjectOp : RelOp
    {
        private VarVec m_vars;
        internal static readonly ProjectOp Pattern = new ProjectOp();

        private ProjectOp() : base(OpType.Project)
        {
        }

        internal ProjectOp(VarVec vars) : this()
        {
            this.m_vars = vars;
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
            2;

        internal VarVec Outputs =>
            this.m_vars;
    }
}

