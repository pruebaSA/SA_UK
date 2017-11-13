namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class NavigateOp : ScalarOp
    {
        private readonly System.Data.Query.InternalTrees.RelProperty m_property;
        internal static readonly NavigateOp Pattern = new NavigateOp();

        private NavigateOp() : base(OpType.Navigate)
        {
        }

        internal NavigateOp(TypeUsage type, System.Data.Query.InternalTrees.RelProperty relProperty) : base(OpType.Navigate, type)
        {
            this.m_property = relProperty;
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

        internal RelationshipEndMember FromEnd =>
            this.m_property.FromEnd;

        internal RelationshipType Relationship =>
            this.m_property.Relationship;

        internal System.Data.Query.InternalTrees.RelProperty RelProperty =>
            this.m_property;

        internal RelationshipEndMember ToEnd =>
            this.m_property.ToEnd;
    }
}

