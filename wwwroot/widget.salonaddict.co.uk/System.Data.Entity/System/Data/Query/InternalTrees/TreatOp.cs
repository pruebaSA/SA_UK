namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class TreatOp : ScalarOp
    {
        private bool m_isFake;
        internal static readonly TreatOp Pattern = new TreatOp();

        private TreatOp() : base(OpType.Treat)
        {
        }

        internal TreatOp(TypeUsage type, bool isFake) : base(OpType.Treat, type)
        {
            this.m_isFake = isFake;
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

        internal bool IsFakeTreat =>
            this.m_isFake;
    }
}

