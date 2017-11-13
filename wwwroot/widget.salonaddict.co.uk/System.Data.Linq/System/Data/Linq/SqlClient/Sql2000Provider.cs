namespace System.Data.Linq.SqlClient
{
    using System;

    public sealed class Sql2000Provider : SqlProvider
    {
        public Sql2000Provider() : base(SqlProvider.ProviderMode.Sql2000)
        {
        }
    }
}

