namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class LikeOp : ScalarOp
    {
        internal static readonly LikeOp Pattern = new LikeOp();

        private LikeOp() : base(OpType.Like)
        {
        }

        internal LikeOp(TypeUsage boolType) : base(OpType.Like, boolType)
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
            3;
    }
}

