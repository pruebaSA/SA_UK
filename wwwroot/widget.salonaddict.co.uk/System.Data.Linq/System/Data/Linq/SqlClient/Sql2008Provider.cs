namespace System.Data.Linq.SqlClient
{
    using System;

    public sealed class Sql2008Provider : SqlProvider
    {
        public Sql2008Provider() : base(SqlProvider.ProviderMode.Sql2008)
        {
        }
    }
}

