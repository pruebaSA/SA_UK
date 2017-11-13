namespace System.Data.Linq.SqlClient
{
    using System;

    internal abstract class DbFormatter
    {
        protected DbFormatter()
        {
        }

        internal abstract string Format(SqlNode node);
        internal abstract string Format(SqlNode node, bool isDebug);
    }
}

