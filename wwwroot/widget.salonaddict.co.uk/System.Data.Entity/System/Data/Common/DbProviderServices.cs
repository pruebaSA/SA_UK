namespace System.Data.Common
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Reflection;
    using System.Xml;

    public abstract class DbProviderServices
    {
        protected DbProviderServices()
        {
        }

        internal virtual DbCommand CreateCommand(DbCommandTree commandTree) => 
            this.CreateCommandDefinition(commandTree).CreateCommand();

        public DbCommandDefinition CreateCommandDefinition(DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            StoreItemCollection itemCollection = (StoreItemCollection) commandTree.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            return this.CreateDbCommandDefinition(itemCollection.StoreProviderManifest, commandTree);
        }

        public virtual DbCommandDefinition CreateCommandDefinition(DbCommand prototype) => 
            DbCommandDefinition.CreateCommandDefinition(prototype);

        protected abstract DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree);
        internal static XmlReader GetConceptualSchemaDescription() => 
            GetXmlResource("System.Data.Resources.DbProviderServices.ConceptualSchemaDefinition.csdl");

        protected abstract DbProviderManifest GetDbProviderManifest(string manifestToken);
        protected abstract string GetDbProviderManifestToken(DbConnection connection);
        internal static DbProviderFactory GetProviderFactory(DbConnection connection)
        {
            EntityUtil.CheckArgumentNull<DbConnection>(connection, "connection");
            DbProviderFactory providerFactory = connection.ProviderFactory;
            if (providerFactory == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_ReturnedNullOnProviderMethod("get_ProviderFactory", connection.GetType().ToString()));
            }
            return providerFactory;
        }

        internal static DbProviderFactory GetProviderFactory(string providerInvariantName)
        {
            DbProviderFactory factory;
            EntityUtil.CheckArgumentNull<string>(providerInvariantName, "providerInvariantName");
            try
            {
                factory = DbProviderFactories.GetFactory(providerInvariantName);
            }
            catch (ArgumentException exception)
            {
                throw EntityUtil.Argument(Strings.EntityClient_InvalidStoreProvider, exception);
            }
            return factory;
        }

        public DbProviderManifest GetProviderManifest(string manifestToken)
        {
            DbProviderManifest manifest2;
            try
            {
                DbProviderManifest dbProviderManifest = this.GetDbProviderManifest(manifestToken);
                if (dbProviderManifest == null)
                {
                    throw EntityUtil.ProviderIncompatible(Strings.ProviderDidNotReturnAProviderManifest);
                }
                manifest2 = dbProviderManifest;
            }
            catch (ProviderIncompatibleException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.ProviderIncompatible(Strings.ProviderDidNotReturnAProviderManifest, exception);
                }
                throw;
            }
            return manifest2;
        }

        public string GetProviderManifestToken(DbConnection connection)
        {
            string str2;
            try
            {
                string dbProviderManifestToken = this.GetDbProviderManifestToken(connection);
                if (dbProviderManifestToken == null)
                {
                    throw EntityUtil.ProviderIncompatible(Strings.ProviderDidNotReturnAProviderManifestToken);
                }
                str2 = dbProviderManifestToken;
            }
            catch (ProviderIncompatibleException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.ProviderIncompatible(Strings.ProviderDidNotReturnAProviderManifestToken, exception);
                }
                throw;
            }
            return str2;
        }

        public static DbProviderServices GetProviderServices(DbConnection connection) => 
            GetProviderServices(GetProviderFactory(connection));

        internal static DbProviderServices GetProviderServices(DbProviderFactory factory)
        {
            EntityUtil.CheckArgumentNull<DbProviderFactory>(factory, "factory");
            IServiceProvider provider = factory as IServiceProvider;
            if (provider == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_DoesNotImplementIServiceProvider(factory.GetType().ToString()));
            }
            DbProviderServices service = provider.GetService(typeof(DbProviderServices)) as DbProviderServices;
            if (service == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_ReturnedNullOnProviderMethod("GetService", factory.GetType().ToString()));
            }
            return service;
        }

        internal static XmlReader GetXmlResource(string resourceName) => 
            XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName), null, resourceName);
    }
}

