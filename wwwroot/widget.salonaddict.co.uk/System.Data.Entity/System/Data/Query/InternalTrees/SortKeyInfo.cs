namespace System.Data.Query.InternalTrees
{
    using System;

    internal class SortKeyInfo
    {
        private bool m_asc;
        private string m_collation;
        private SimpleColumnMap m_sortKeyColumn;

        internal SortKeyInfo(SimpleColumnMap sortKeyColumn, bool asc, string collation)
        {
            this.m_sortKeyColumn = sortKeyColumn;
            this.m_asc = asc;
            this.m_collation = collation;
        }

        internal bool AscendingSort =>
            this.m_asc;

        internal string Collation =>
            this.m_collation;

        internal ColumnMap SortKeyColumn =>
            this.m_sortKeyColumn;
    }
}

