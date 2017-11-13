namespace System.Data.Linq.SqlClient
{
    using System;

    internal abstract class SqlNodeAnnotation
    {
        private string message;

        internal SqlNodeAnnotation(string message)
        {
            this.message = message;
        }

        internal string Message =>
            this.message;
    }
}

