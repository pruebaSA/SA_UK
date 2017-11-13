namespace System.Data.OleDb
{
    using System;

    public sealed class OleDbInfoMessageEventArgs : EventArgs
    {
        private readonly OleDbException exception;

        internal OleDbInfoMessageEventArgs(OleDbException exception)
        {
            this.exception = exception;
        }

        internal bool ShouldSerializeErrors() => 
            this.exception.ShouldSerializeErrors();

        public override string ToString() => 
            this.Message;

        public int ErrorCode =>
            this.exception.ErrorCode;

        public OleDbErrorCollection Errors =>
            this.exception.Errors;

        public string Message =>
            this.exception.Message;

        public string Source =>
            this.exception.Source;
    }
}

