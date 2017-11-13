namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class InnerJoinOp : JoinBaseOp
    {
        internal static readonly InnerJoinOp Instance = new InnerJoinOp();
        internal static readonly InnerJoinOp Pattern = Instance;

        private InnerJoinOp() : base(OpType.InnerJoin)
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

