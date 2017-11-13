namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class SoftCastOp : ScalarOp
    {
        internal static readonly SoftCastOp Pattern = new SoftCastOp();

        private SoftCastOp() : base(OpType.SoftCast)
        {
        }

        internal SoftCastOp(TypeUsage type) : base(OpType.SoftCast, type)
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

