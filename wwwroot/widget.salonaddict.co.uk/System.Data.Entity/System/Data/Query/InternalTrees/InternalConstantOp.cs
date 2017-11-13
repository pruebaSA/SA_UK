namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class InternalConstantOp : ConstantBaseOp
    {
        internal static readonly InternalConstantOp Pattern = new InternalConstantOp();

        private InternalConstantOp() : base(OpType.InternalConstant)
        {
        }

        internal InternalConstantOp(TypeUsage type, object value) : base(OpType.InternalConstant, type, value)
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

        internal override bool? IsEquivalent(Op other)
        {
            InternalConstantOp op = other as InternalConstantOp;
            if ((op == null) || (op.Value == null))
            {
                return null;
            }
            return new bool?(op.Type.EdmEquals(this.Type) && op.Value.Equals(this.Value));
        }
    }
}

