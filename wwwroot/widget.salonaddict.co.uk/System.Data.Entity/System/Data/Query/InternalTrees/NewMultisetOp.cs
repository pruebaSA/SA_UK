namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class NewMultisetOp : ScalarOp
    {
        internal static readonly NewMultisetOp Pattern = new NewMultisetOp();

        private NewMultisetOp() : base(OpType.NewMultiset)
        {
        }

        internal NewMultisetOp(TypeUsage type) : base(OpType.NewMultiset, type)
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

