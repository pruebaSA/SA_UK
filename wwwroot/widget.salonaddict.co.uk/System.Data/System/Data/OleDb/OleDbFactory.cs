namespace System.Data.OleDb
{
    using System;
    using System.Data.Common;
    using System.Security;
    using System.Security.Permissions;

    public sealed class OleDbFactory : DbProviderFactory
    {
        public static readonly OleDbFactory Instance = new OleDbFactory();

        private OleDbFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new OleDbCommand();

        public override DbCommandBuilder CreateCommandBuilder() => 
            new OleDbCommandBuilder();

        public override DbConnection CreateConnection() => 
            new OleDbConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new OleDbConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter() => 
            new OleDbDataAdapter();

        public override DbParameter CreateParameter() => 
            new OleDbParameter();

        public override CodeAccessPermission CreatePermission(PermissionState state) => 
            new OleDbPermission(state);
    }
}

