namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ConditionalOp : ScalarOp
    {
        internal static readonly ConditionalOp PatternAnd = new ConditionalOp(OpType.And);
        internal static readonly ConditionalOp PatternIsNull = new ConditionalOp(OpType.IsNull);
        internal static readonly ConditionalOp PatternNot = new ConditionalOp(OpType.Not);
        internal static readonly ConditionalOp PatternOr = new ConditionalOp(OpType.Or);

        private ConditionalOp(OpType opType) : base(opType)
        {
        }

        internal ConditionalOp(OpType optype, TypeUsage type) : base(optype, type)
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
    }
}

