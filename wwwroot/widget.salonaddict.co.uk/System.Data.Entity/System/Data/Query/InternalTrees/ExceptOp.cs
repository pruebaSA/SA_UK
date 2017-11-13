namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class ExceptOp : SetOp
    {
        internal static readonly ExceptOp Pattern = new ExceptOp();

        private ExceptOp() : base(OpType.Except)
        {
        }

        internal ExceptOp(VarVec outputs, VarMap left, VarMap right) : base(OpType.Except, outputs, left, right)
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

