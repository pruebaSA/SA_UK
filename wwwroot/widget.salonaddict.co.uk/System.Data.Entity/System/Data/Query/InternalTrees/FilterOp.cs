namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class FilterOp : RelOp
    {
        internal static readonly FilterOp Instance = new FilterOp();
        internal static readonly FilterOp Pattern = Instance;

        private FilterOp() : base(OpType.Filter)
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
            2;
    }
}

