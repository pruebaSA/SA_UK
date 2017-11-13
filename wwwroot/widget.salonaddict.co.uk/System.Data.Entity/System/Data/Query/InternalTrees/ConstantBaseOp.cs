namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class ConstantBaseOp : ScalarOp
    {
        private readonly object m_value;

        protected ConstantBaseOp(OpType opType) : base(opType)
        {
        }

        protected ConstantBaseOp(OpType opType, TypeUsage type, object value) : base(opType, type)
        {
            this.m_value = value;
        }

        internal override bool? IsEquivalent(Op other) => 
            null;

        internal override int Arity =>
            0;

        internal virtual object Value =>
            this.m_value;
    }
}

