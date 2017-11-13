namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ConstantPredicateOp : ConstantBaseOp
    {
        internal static readonly ConstantPredicateOp Pattern = new ConstantPredicateOp();

        private ConstantPredicateOp() : base(OpType.ConstantPredicate)
        {
        }

        internal ConstantPredicateOp(TypeUsage type, bool value) : base(OpType.ConstantPredicate, type, value)
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
            ConstantPredicateOp op = other as ConstantPredicateOp;
            return new bool?((op != null) && (this.Value == op.Value));
        }

        internal bool IsFalse =>
            !this.Value;

        internal bool IsTrue =>
            this.Value;

        internal bool Value =>
            ((bool) base.Value);
    }
}

