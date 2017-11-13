namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class CollectionColumnMap : ColumnMap
    {
        private readonly ColumnMap m_element;
        private readonly SimpleColumnMap[] m_foreignKeys;
        private readonly SimpleColumnMap[] m_keys;
        private readonly SortKeyInfo[] m_sortKeys;

        internal CollectionColumnMap(TypeUsage type, string name, ColumnMap elementMap, SimpleColumnMap[] keys, SimpleColumnMap[] foreignKeys, SortKeyInfo[] sortKeys) : base(type, name)
        {
            this.m_element = elementMap;
            this.m_keys = keys ?? new SimpleColumnMap[0];
            this.m_foreignKeys = foreignKeys ?? new SimpleColumnMap[0];
            this.m_sortKeys = sortKeys ?? new SortKeyInfo[0];
        }

        internal ColumnMap Element =>
            this.m_element;

        internal SimpleColumnMap[] ForeignKeys =>
            this.m_foreignKeys;

        internal SimpleColumnMap[] Keys =>
            this.m_keys;

        internal SortKeyInfo[] SortKeys =>
            this.m_sortKeys;
    }
}

