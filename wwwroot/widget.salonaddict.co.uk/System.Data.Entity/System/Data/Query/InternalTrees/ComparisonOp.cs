namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ComparisonOp : ScalarOp
    {
        internal static readonly ComparisonOp PatternEq = new ComparisonOp(OpType.EQ);

        private ComparisonOp(OpType opType) : base(opType)
        {
        }

        internal ComparisonOp(OpType opType, TypeUsage type) : base(opType, type)
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

