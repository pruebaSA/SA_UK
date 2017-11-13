namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class PropertyOp : ScalarOp
    {
        private EdmMember m_property;
        internal static readonly PropertyOp Pattern = new PropertyOp();

        private PropertyOp() : base(OpType.Property)
        {
        }

        internal PropertyOp(TypeUsage type, EdmMember property) : base(OpType.Property, type)
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

        internal EdmMember PropertyInfo =>
            this.m_property;
    }
}

