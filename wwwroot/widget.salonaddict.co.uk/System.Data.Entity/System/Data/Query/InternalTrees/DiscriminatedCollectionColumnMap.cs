namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;

    internal class DiscriminatedCollectionColumnMap : CollectionColumnMap
    {
        private SimpleColumnMap m_discriminator;
        private object m_discriminatorValue;

        internal DiscriminatedCollectionColumnMap(TypeUsage type, string name, ColumnMap elementMap, SimpleColumnMap[] keys, SimpleColumnMap[] foreignKeys, SortKeyInfo[] sortKeys, SimpleColumnMap discriminator, object discriminatorValue) : base(type, name, elementMap, keys, foreignKeys, sortKeys)
        {
            this.m_discriminator = discriminator;
            this.m_discriminatorValue = discriminatorValue;
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
            string.Format(CultureInfo.InvariantCulture, "M{{{0}}}", new object[] { base.Element.ToString() });

        internal SimpleColumnMap Discriminator =>
            this.m_discriminator;

        internal object DiscriminatorValue =>
            this.m_discriminatorValue;
    }
}

