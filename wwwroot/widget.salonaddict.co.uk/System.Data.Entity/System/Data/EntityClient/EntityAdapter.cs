namespace System.Data.EntityClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Mapping.Update.Internal;
    using System.Data.Objects;

    internal sealed class EntityAdapter : IEntityAdapter
    {
        private bool _acceptChangesDuringUpdate = true;
        private int? _commandTimeout;
        private EntityConnection _connection;

        private static bool IsStateManagerDirty(IEntityStateManager entityCache)
        {
            using (IEnumerator<IEntityStateEntry> enumerator = entityCache.GetEntityStateEntries(EntityState.Modified | EntityState.Deleted | EntityState.Added).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ObjectStateEntry current = (ObjectStateEntry) enumerator.Current;
                    return true;
                }
            }
            return false;
        }

        public int Update(IEntityStateManager entityCache)
        {
            EntityUtil.CheckArgumentNull<IEntityStateManager>(entityCache, "entityCache");
            if (!IsStateManagerDirty(entityCache))
            {
                return 0;
            }
            if (this._connection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_NoConnectionForAdapter);
            }
            if ((this._connection.StoreProviderFactory == null) || (this._connection.StoreConnection == null))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_NoStoreConnectionForUpdate);
            }
            if (ConnectionState.Open != this._connection.State)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ClosedConnectionForUpdate);
            }
            return UpdateTranslator.Update(entityCache, this);
        }

        public bool AcceptChangesDuringUpdate
        {
            get => 
                this._acceptChangesDuringUpdate;
            set
            {
                this._acceptChangesDuringUpdate = value;
            }
        }

        public EntityConnection Connection
        {
            get => 
                this._connection;
            set
            {
                this._connection = value;
            }
        }

        int? IEntityAdapter.CommandTimeout
        {
            get => 
                this._commandTimeout;
            set
            {
                this._commandTimeout = value;
            }
        }

        DbConnection IEntityAdapter.Connection
        {
            get => 
                this.Connection;
            set
            {
                this.Connection = (EntityConnection) value;
            }
        }
    }
}

