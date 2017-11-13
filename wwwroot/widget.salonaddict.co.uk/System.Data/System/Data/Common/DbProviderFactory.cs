namespace System.Data.Common
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    public abstract class DbProviderFactory
    {
        protected DbProviderFactory()
        {
        }

        public virtual DbCommand CreateCommand() => 
            null;

        public virtual DbCommandBuilder CreateCommandBuilder() => 
            null;

        public virtual DbConnection CreateConnection() => 
            null;

        public virtual DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            null;

        public virtual DbDataAdapter CreateDataAdapter() => 
            null;

        public virtual DbDataSourceEnumerator CreateDataSourceEnumerator() => 
            null;

        public virtual DbParameter CreateParameter() => 
            null;

        public virtual CodeAccessPermission CreatePermission(PermissionState state) => 
            null;

        public virtual bool CanCreateDataSourceEnumerator =>
            false;
    }
}

