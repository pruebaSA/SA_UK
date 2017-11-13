namespace System.Data.SqlClient
{
    using System;

    public sealed class SqlInfoMessageEventArgs : EventArgs
    {
        private SqlException exception;

        internal SqlInfoMessageEventArgs(SqlException exception)
        {
            this.exception = exception;
        }

        private bool ShouldSerializeErrors() => 
            ((this.exception != null) && (0 < this.exception.Errors.Count));

        public override string ToString() => 
            this.Message;

        public SqlErrorCollection Errors =>
            this.exception.Errors;

        public string Message =>
            this.exception.Message;

        public string Source =>
            this.exception.Source;
    }
}

