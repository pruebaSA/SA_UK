namespace System.Data.Linq.SqlClient
{
    using System;

    public sealed class Sql2005Provider : SqlProvider
    {
        public Sql2005Provider() : base(SqlProvider.ProviderMode.Sql2005)
        {
        }
    }
}

