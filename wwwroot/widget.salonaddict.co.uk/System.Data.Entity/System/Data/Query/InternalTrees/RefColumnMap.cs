namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal class RefColumnMap : ColumnMap
    {
        private System.Data.Query.InternalTrees.EntityIdentity m_entityIdentity;

        internal RefColumnMap(TypeUsage type, string name, System.Data.Query.InternalTrees.EntityIdentity entityIdentity) : base(type, name)
        {
            this.m_entityIdentity = entityIdentity;
        }

        [DebuggerNonUserCode]
        internal override void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg)
        {
            visitor.Visit(this, arg);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType, TArgType>(ColumnMapVisitorWithResults<TResultType, TArgType> visitor, TArgType arg) => 
            visitor.Visit(this, arg);

        internal System.Data.Query.InternalTrees.EntityIdentity EntityIdentity =>
            this.m_entityIdentity;
    }
}

