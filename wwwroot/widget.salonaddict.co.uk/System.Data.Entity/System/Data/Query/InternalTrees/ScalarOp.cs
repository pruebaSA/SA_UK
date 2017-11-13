namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class ScalarOp : Op
    {
        private TypeUsage m_type;

        protected ScalarOp(OpType opType) : base(opType)
        {
        }

        internal ScalarOp(OpType opType, TypeUsage type) : this(opType)
        {
            this.m_type = type;
        }

        internal override bool? IsEquivalent(Op other) => 
            new bool?((other.OpType == base.OpType) && TypeSemantics.IsEquivalent(this.Type, other.Type));

        internal virtual bool IsAggregateOp =>
            false;

        internal override bool IsScalarOp =>
            true;

        internal override TypeUsage Type
        {
            get => 
                this.m_type;
            set
            {
                this.m_type = value;
            }
        }
    }
}

