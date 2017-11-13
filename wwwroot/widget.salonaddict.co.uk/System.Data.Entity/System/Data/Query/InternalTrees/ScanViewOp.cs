namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class ScanViewOp : ScanTableBaseOp
    {
        internal static readonly ScanViewOp Pattern = new ScanViewOp();

        private ScanViewOp() : base(OpType.ScanView)
        {
        }

        internal ScanViewOp(Table table) : base(OpType.ScanView, table)
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
            1;
    }
}

