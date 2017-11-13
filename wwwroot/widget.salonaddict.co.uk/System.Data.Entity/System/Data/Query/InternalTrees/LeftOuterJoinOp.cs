namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class LeftOuterJoinOp : JoinBaseOp
    {
        internal static readonly LeftOuterJoinOp Instance = new LeftOuterJoinOp();
        internal static readonly LeftOuterJoinOp Pattern = Instance;

        private LeftOuterJoinOp() : base(OpType.LeftOuterJoin)
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

