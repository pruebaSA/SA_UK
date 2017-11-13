namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class GetEntityRefOp : ScalarOp
    {
        internal static readonly GetEntityRefOp Pattern = new GetEntityRefOp();

        private GetEntityRefOp() : base(OpType.GetEntityRef)
        {
        }

        internal GetEntityRefOp(TypeUsage type) : base(OpType.GetEntityRef, type)
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

