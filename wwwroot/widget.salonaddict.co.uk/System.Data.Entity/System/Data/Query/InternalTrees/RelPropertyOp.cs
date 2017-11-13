namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class RelPropertyOp : ScalarOp
    {
        private readonly RelProperty m_property;
        internal static readonly RelPropertyOp Pattern = new RelPropertyOp();

        private RelPropertyOp() : base(OpType.RelProperty)
        {
        }

        internal RelPropertyOp(TypeUsage type, RelProperty property) : base(OpType.RelProperty, type)
        {
            this.m_property = property;
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

        public RelProperty PropertyInfo =>
            this.m_property;
    }
}

