namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class NewInstanceOp : ScalarOp
    {
        internal static readonly NewInstanceOp Pattern = new NewInstanceOp();

        private NewInstanceOp() : base(OpType.NewInstance)
        {
        }

        internal NewInstanceOp(TypeUsage type) : base(OpType.NewInstance, type)
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

