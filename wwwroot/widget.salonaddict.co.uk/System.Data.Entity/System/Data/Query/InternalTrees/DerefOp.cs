namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class DerefOp : ScalarOp
    {
        internal static readonly DerefOp Pattern = new DerefOp();

        private DerefOp() : base(OpType.Deref)
        {
        }

        internal DerefOp(TypeUsage type) : base(OpType.Deref, type)
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

