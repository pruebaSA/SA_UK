namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class AggregateOp : ScalarOp
    {
        private EdmFunction m_aggFunc;
        private bool m_distinctAgg;
        internal static readonly AggregateOp Pattern = new AggregateOp();

        private AggregateOp() : base(OpType.Aggregate)
        {
        }

        internal AggregateOp(EdmFunction aggFunc, bool distinctAgg) : base(OpType.Aggregate, aggFunc.ReturnParameter.TypeUsage)
        {
            this.m_aggFunc = aggFunc;
            this.m_distinctAgg = distinctAgg;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal EdmFunction AggFunc =>
            this.m_aggFunc;

        internal override bool IsAggregateOp =>
            true;

        internal bool IsDistinctAggregate =>
            this.m_distinctAgg;
    }
}

