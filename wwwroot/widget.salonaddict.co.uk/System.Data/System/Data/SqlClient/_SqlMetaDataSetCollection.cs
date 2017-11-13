namespace System.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class _SqlMetaDataSetCollection
    {
        private readonly List<_SqlMetaDataSet> altMetaDataSetArray = new List<_SqlMetaDataSet>();
        internal _SqlMetaDataSet metaDataSet;

        internal _SqlMetaDataSetCollection()
        {
        }

        internal void Add(_SqlMetaDataSet altMetaDataSet)
        {
            this.altMetaDataSetArray.Add(altMetaDataSet);
        }

        internal _SqlMetaDataSet this[int id]
        {
            get
            {
                foreach (_SqlMetaDataSet set in this.altMetaDataSetArray)
                {
                    if (set.id == id)
                    {
                        return set;
                    }
                }
                return null;
            }
        }
    }
}

