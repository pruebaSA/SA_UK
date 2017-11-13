namespace System.Data.SqlClient
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal sealed class Result
    {
        private _SqlMetaDataSet _metadata;
        private ArrayList _rowset;

        internal Result(_SqlMetaDataSet metadata)
        {
            this._metadata = metadata;
            this._rowset = new ArrayList();
        }

        internal void AddRow(Row row)
        {
            this._rowset.Add(row);
        }

        internal int Count =>
            this._rowset.Count;

        internal Row this[int index] =>
            ((Row) this._rowset[index]);

        internal _SqlMetaDataSet MetaData =>
            this._metadata;
    }
}

