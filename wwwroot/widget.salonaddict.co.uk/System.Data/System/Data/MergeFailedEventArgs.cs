namespace System.Data
{
    using System;

    public class MergeFailedEventArgs : EventArgs
    {
        private string conflict;
        private DataTable table;

        public MergeFailedEventArgs(DataTable table, string conflict)
        {
            this.table = table;
            this.conflict = conflict;
        }

        public string Conflict =>
            this.conflict;

        public DataTable Table =>
            this.table;
    }
}

