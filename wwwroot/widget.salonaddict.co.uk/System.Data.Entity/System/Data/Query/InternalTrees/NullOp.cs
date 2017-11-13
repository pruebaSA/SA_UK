namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class NullOp : ConstantBaseOp
    {
        internal static readonly NullOp Pattern = new NullOp();

        private NullOp() : base(OpType.Null)
        {
        }

        internal NullOp(TypeUsage type) : base(OpType.Null, type, null)
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

