namespace System.Data.OracleClient
{
    using System;
    using System.Data.Common;
    using System.Security;
    using System.Security.Permissions;

    public sealed class OracleClientFactory : DbProviderFactory
    {
        public static readonly OracleClientFactory Instance = new OracleClientFactory();

        private OracleClientFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new OracleCommand();

        public override DbCommandBuilder CreateCommandBuilder() => 
            new OracleCommandBuilder();

        public override DbConnection CreateConnection() => 
            new OracleConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new OracleConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter() => 
            new OracleDataAdapter();

        public override DbParameter CreateParameter() => 
            new OracleParameter();

        public override CodeAccessPermission CreatePermission(PermissionState state) => 
            new OraclePermission(state);
    }
}

