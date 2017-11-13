namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Common;

    internal interface IConnectionManager
    {
        void ReleaseConnection(IConnectionUser user);
        DbConnection UseConnection(IConnectionUser user);
    }
}

