namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;

    internal class ComplexTypeColumnMap : TypedColumnMap
    {
        private SimpleColumnMap m_nullSentinel;

        internal ComplexTypeColumnMap(TypeUsage type, string name, ColumnMap[] properties, SimpleColumnMap nullSentinel) : base(type, name, properties)
        {
            this.m_nullSentinel = nullSentinel;
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
            string.Format(CultureInfo.InvariantCulture, "C{0}", new object[] { base.ToString() });

        internal override SimpleColumnMap NullSentinel =>
            this.m_nullSentinel;
    }
}

