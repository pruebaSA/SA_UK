namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class ScanTableOp : ScanTableBaseOp
    {
        internal static readonly ScanTableOp Pattern = new ScanTableOp();

        private ScanTableOp() : base(OpType.ScanTable)
        {
        }

        internal ScanTableOp(Table table) : base(OpType.ScanTable, table)
        {
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
            0;
    }
}

