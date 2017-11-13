namespace System.Data.EntityClient
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal sealed class EntityProviderServices : DbProviderServices
    {
        internal static readonly EntityProviderServices Instance = new EntityProviderServices();

        public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            EntityUtil.CheckArgumentNull<DbCommand>(prototype, "prototype");
            return ((EntityCommand) prototype).GetCommandDefinition();
        }

        internal EntityCommandDefinition CreateCommandDefinition(DbProviderFactory storeProviderFactory, DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<DbProviderFactory>(storeProviderFactory, "storeProviderFactory");
            return new EntityCommandDefinition(storeProviderFactory, commandTree);
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<DbProviderManifest>(providerManifest, "providerManifest");
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            StoreItemCollection itemCollection = (StoreItemCollection) commandTree.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            return this.CreateCommandDefinition(itemCollection.StoreProviderFactory, commandTree);
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            EntityUtil.CheckArgumentNull<string>(versionHint, "versionHint");
            return MetadataItem.EdmProviderManifest;
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            EntityUtil.CheckArgumentNull<DbConnection>(connection, "connection");
            if (connection.GetType() != typeof(EntityConnection))
            {
                throw EntityUtil.Argument(Strings.Mapping_Provider_WrongConnectionType(typeof(EntityConnection)));
            }
            return MetadataItem.EdmProviderManifest.Token;
        }
    }
}

