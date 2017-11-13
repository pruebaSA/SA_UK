namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal sealed class SortOp : SortBaseOp
    {
        internal static readonly SortOp Pattern = new SortOp();

        private SortOp() : base(OpType.Sort)
        {
        }

        internal SortOp(List<SortKey> sortKeys) : base(OpType.Sort, sortKeys)
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

