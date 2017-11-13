namespace System.Data.OracleClient
{
    using System;

    public sealed class OracleInfoMessageEventArgs : EventArgs
    {
        private OracleException exception;

        internal OracleInfoMessageEventArgs(OracleException exception)
        {
            this.exception = exception;
        }

        public override string ToString() => 
            this.Message;

        public int Code =>
            this.exception.Code;

        public string Message =>
            this.exception.Message;

        public string Source =>
            this.exception.Source;
    }
}

