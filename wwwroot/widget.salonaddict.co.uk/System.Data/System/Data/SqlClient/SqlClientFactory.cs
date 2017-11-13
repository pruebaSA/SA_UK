namespace System.Data.SqlClient
{
    using System;
    using System.Data.Common;
    using System.Data.Sql;
    using System.Security;
    using System.Security.Permissions;

    public sealed class SqlClientFactory : DbProviderFactory, IServiceProvider
    {
        public static readonly SqlClientFactory Instance = new SqlClientFactory();

        private SqlClientFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new SqlCommand();

        public override DbCommandBuilder CreateCommandBuilder() => 
            new SqlCommandBuilder();

        public override DbConnection CreateConnection() => 
            new SqlConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new SqlConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter() => 
            new SqlDataAdapter();

        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => 
            SqlDataSourceEnumerator.Instance;

        public override DbParameter CreateParameter() => 
            new SqlParameter();

        public override CodeAccessPermission CreatePermission(PermissionState state) => 
            new SqlClientPermission(state);

        object IServiceProvider.GetService(Type serviceType)
        {
            object obj2 = null;
            if (serviceType == GreenMethods.SystemDataCommonDbProviderServices_Type)
            {
                obj2 = GreenMethods.SystemDataSqlClientSqlProviderServices_Instance();
            }
            return obj2;
        }

        public override bool CanCreateDataSourceEnumerator =>
            true;
    }
}

