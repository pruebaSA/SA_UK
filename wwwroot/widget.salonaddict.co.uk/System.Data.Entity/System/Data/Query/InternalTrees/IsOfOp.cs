namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class IsOfOp : ScalarOp
    {
        private bool m_isOfOnly;
        private TypeUsage m_isOfType;
        internal static readonly IsOfOp Pattern = new IsOfOp();

        private IsOfOp() : base(OpType.IsOf)
        {
        }

        internal IsOfOp(TypeUsage isOfType, bool isOfOnly, TypeUsage type) : base(OpType.IsOf, type)
        {
            this.m_isOfType = isOfType;
            this.m_isOfOnly = isOfOnly;
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

        internal bool IsOfOnly =>
            this.m_isOfOnly;

        internal TypeUsage IsOfType =>
            this.m_isOfType;
    }
}

