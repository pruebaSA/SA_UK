namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal class RecordColumnMap : StructuredColumnMap
    {
        private SimpleColumnMap m_nullSentinel;

        internal RecordColumnMap(TypeUsage type, string name, ColumnMap[] properties, SimpleColumnMap nullSentinel) : base(type, name, properties)
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

        internal override SimpleColumnMap NullSentinel =>
            this.m_nullSentinel;
    }
}

