namespace System.Data
{
    using System;

    public sealed class DataTableClearEventArgs : EventArgs
    {
        private readonly DataTable dataTable;

        public DataTableClearEventArgs(DataTable dataTable)
        {
            this.dataTable = dataTable;
        }

        public DataTable Table =>
            this.dataTable;

        public string TableName =>
            this.dataTable.TableName;

        public string TableNamespace =>
            this.dataTable.Namespace;
    }
}

