namespace System.Data.SqlClient
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class SqlError
    {
        private byte errorClass;
        private int lineNumber;
        private string message;
        private int number;
        private string procedure;
        [OptionalField(VersionAdded=2)]
        private string server;
        private string source = ".Net SqlClient Data Provider";
        private byte state;

        internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber)
        {
            this.number = infoNumber;
            this.state = errorState;
            this.errorClass = errorClass;
            this.server = server;
            this.message = errorMessage;
            this.procedure = procedure;
            this.lineNumber = lineNumber;
            if (errorClass != 0)
            {
                Bid.Trace("<sc.SqlError.SqlError|ERR> infoNumber=%d, errorState=%d, errorClass=%d, errorMessage='%ls', procedure='%ls', lineNumber=%d\n", infoNumber, errorState, errorClass, errorMessage, (procedure == null) ? "None" : procedure, lineNumber);
            }
        }

        public override string ToString() => 
            (typeof(SqlError).ToString() + ": " + this.message);

        public byte Class =>
            this.errorClass;

        public int LineNumber =>
            this.lineNumber;

        public string Message =>
            this.message;

        public int Number =>
            this.number;

        public string Procedure =>
            this.procedure;

        public string Server =>
            this.server;

        public string Source =>
            this.source;

        public byte State =>
            this.state;
    }
}

