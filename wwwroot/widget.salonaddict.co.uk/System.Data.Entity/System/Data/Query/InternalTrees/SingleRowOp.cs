namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class SingleRowOp : RelOp
    {
        internal static readonly SingleRowOp Instance = new SingleRowOp();
        internal static readonly SingleRowOp Pattern = Instance;

        private SingleRowOp() : base(OpType.SingleRow)
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

