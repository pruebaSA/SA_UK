namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;

    internal class CollectionInfo
    {
        private Var m_collectionVar;
        private System.Data.Query.InternalTrees.ColumnMap m_columnMap;
        private object m_discriminatorValue;
        private VarList m_flattenedElementVars;
        private VarVec m_keys;
        private List<SortKey> m_sortKeys;

        internal CollectionInfo(Var collectionVar, System.Data.Query.InternalTrees.ColumnMap columnMap, VarList flattenedElementVars, VarVec keys, List<SortKey> sortKeys, object discriminatorValue)
        {
            this.m_collectionVar = collectionVar;
            this.m_columnMap = columnMap;
            this.m_flattenedElementVars = flattenedElementVars;
            this.m_keys = keys;
            this.m_sortKeys = sortKeys;
            this.m_discriminatorValue = discriminatorValue;
        }

        internal Var CollectionVar =>
            this.m_collectionVar;

        internal System.Data.Query.InternalTrees.ColumnMap ColumnMap =>
            this.m_columnMap;

        internal object DiscriminatorValue =>
            this.m_discriminatorValue;

        internal VarList FlattenedElementVars =>
            this.m_flattenedElementVars;

        internal VarVec Keys =>
            this.m_keys;

        internal List<SortKey> SortKeys =>
            this.m_sortKeys;
    }
}

