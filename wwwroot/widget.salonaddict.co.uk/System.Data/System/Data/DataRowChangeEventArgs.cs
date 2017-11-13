namespace System.Data
{
    using System;

    public class DataRowChangeEventArgs : EventArgs
    {
        private DataRowAction action;
        private DataRow row;

        public DataRowChangeEventArgs(DataRow row, DataRowAction action)
        {
            this.row = row;
            this.action = action;
        }

        public DataRowAction Action =>
            this.action;

        public DataRow Row =>
            this.row;
    }
}

