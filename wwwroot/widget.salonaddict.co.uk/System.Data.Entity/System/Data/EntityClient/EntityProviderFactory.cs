namespace System.Data.EntityClient
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Security;
    using System.Security.Permissions;

    public sealed class EntityProviderFactory : DbProviderFactory, IServiceProvider
    {
        public static readonly EntityProviderFactory Instance = new EntityProviderFactory();

        private EntityProviderFactory()
        {
        }

        public override DbCommand CreateCommand() => 
            new EntityCommand();

        public override DbCommandBuilder CreateCommandBuilder()
        {
            throw EntityUtil.NotSupported();
        }

        public override DbConnection CreateConnection() => 
            new EntityConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new EntityConnectionStringBuilder();

        public override DbDataAdapter CreateDataAdapter()
        {
            throw EntityUtil.NotSupported();
        }

        public override DbParameter CreateParameter() => 
            new EntityParameter();

        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            throw EntityUtil.NotSupported();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            object obj2 = null;
            if (serviceType == typeof(DbProviderServices))
            {
                return EntityProviderServices.Instance;
            }
            if (serviceType == typeof(IEntityAdapter))
            {
                obj2 = new EntityAdapter();
            }
            return obj2;
        }
    }
}

