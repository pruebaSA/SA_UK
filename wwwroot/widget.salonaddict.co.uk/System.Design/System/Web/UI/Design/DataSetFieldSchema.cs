namespace System.Web.UI.Design
{
    using System;
    using System.Data;

    public sealed class DataSetFieldSchema : IDataSourceFieldSchema
    {
        private DataColumn _column;

        public DataSetFieldSchema(DataColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            this._column = column;
        }

        public Type DataType =>
            this._column.DataType;

        public bool Identity =>
            this._column.AutoIncrement;

        public bool IsReadOnly =>
            this._column.ReadOnly;

        public bool IsUnique =>
            this._column.Unique;

        public int Length =>
            this._column.MaxLength;

        public string Name =>
            this._column.ColumnName;

        public bool Nullable =>
            this._column.AllowDBNull;

        public int Precision =>
            -1;

        public bool PrimaryKey
        {
            get
            {
                if ((this._column.Table != null) && (this._column.Table.PrimaryKey != null))
                {
                    foreach (DataColumn column in this._column.Table.PrimaryKey)
                    {
                        if (column == this._column)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public int Scale =>
            -1;
    }
}

