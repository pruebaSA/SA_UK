namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class CrossJoinOp : JoinBaseOp
    {
        internal static readonly CrossJoinOp Instance = new CrossJoinOp();
        internal static readonly CrossJoinOp Pattern = Instance;

        private CrossJoinOp() : base(OpType.CrossJoin)
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
            -1;
    }
}

