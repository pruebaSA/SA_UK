namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal class PhysicalProjectOp : PhysicalOp
    {
        private SimpleCollectionColumnMap m_columnMap;
        private VarList m_outputVars;
        internal static readonly PhysicalProjectOp Pattern = new PhysicalProjectOp();

        private PhysicalProjectOp() : base(OpType.PhysicalProject)
        {
        }

        internal PhysicalProjectOp(VarList outputVars, SimpleCollectionColumnMap columnMap) : this()
        {
            this.m_outputVars = outputVars;
            this.m_columnMap = columnMap;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal SimpleCollectionColumnMap ColumnMap =>
            this.m_columnMap;

        internal VarList Outputs =>
            this.m_outputVars;
    }
}

