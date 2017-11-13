namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;

    internal class EntityColumnMap : TypedColumnMap
    {
        private System.Data.Query.InternalTrees.EntityIdentity m_entityIdentity;

        internal EntityColumnMap(TypeUsage type, string name, ColumnMap[] properties, System.Data.Query.InternalTrees.EntityIdentity entityIdentity) : base(type, name, properties)
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

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "E{0}", new object[] { base.ToString() });

        internal System.Data.Query.InternalTrees.EntityIdentity EntityIdentity =>
            this.m_entityIdentity;
    }
}

