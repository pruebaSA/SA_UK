namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal sealed class ConstrainedSortOp : SortBaseOp
    {
        private bool _withTies;
        internal static readonly ConstrainedSortOp Pattern = new ConstrainedSortOp();

        private ConstrainedSortOp() : base(OpType.ConstrainedSort)
        {
        }

        internal ConstrainedSortOp(List<SortKey> sortKeys, bool withTies) : base(OpType.ConstrainedSort, sortKeys)
        {
            this._withTies = withTies;
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
            3;

        internal bool WithTies
        {
            get => 
                this._withTies;
            set
            {
                this._withTies = value;
            }
        }
    }
}

