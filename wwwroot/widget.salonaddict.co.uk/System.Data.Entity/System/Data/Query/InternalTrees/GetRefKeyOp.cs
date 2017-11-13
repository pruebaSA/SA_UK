namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class GetRefKeyOp : ScalarOp
    {
        internal static readonly GetRefKeyOp Pattern = new GetRefKeyOp();

        private GetRefKeyOp() : base(OpType.GetRefKey)
        {
        }

        internal GetRefKeyOp(TypeUsage type) : base(OpType.GetRefKey, type)
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

