namespace System.Data.Odbc
{
    using System;
    using System.Data.Common;
    using System.Security;
    using System.Security.Permissions;

    public sealed class OdbcFactory : DbProviderFactory
    {
        public static readonly OdbcFactory Instance = new OdbcFactory();

        private OdbcFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new OdbcCommand();

        public override DbCommandBuilder CreateCommandBuilder() => 
            new OdbcCommandBuilder();

        public override DbConnection CreateConnection() => 
            new OdbcConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new OdbcConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter() => 
            new OdbcDataAdapter();

        public override DbParameter CreateParameter() => 
            new OdbcParameter();

        public override CodeAccessPermission CreatePermission(PermissionState state) => 
            new OdbcPermission(state);
    }
}

