namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class SingleRowTableOp : RelOp
    {
        internal static readonly SingleRowTableOp Instance = new SingleRowTableOp();
        internal static readonly SingleRowTableOp Pattern = Instance;

        private SingleRowTableOp() : base(OpType.SingleRowTable)
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
            0;
    }
}

