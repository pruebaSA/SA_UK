namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ConstantOp : ConstantBaseOp
    {
        internal static readonly ConstantOp Pattern = new ConstantOp();

        private ConstantOp() : base(OpType.Constant)
        {
        }

        internal ConstantOp(TypeUsage type, object value) : base(OpType.Constant, type, value)
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

