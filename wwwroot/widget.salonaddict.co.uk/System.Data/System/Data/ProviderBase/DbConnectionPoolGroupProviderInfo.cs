namespace System.Data.ProviderBase
{
    using System;

    internal class DbConnectionPoolGroupProviderInfo
    {
        private DbConnectionPoolGroup _poolGroup;

        internal DbConnectionPoolGroup PoolGroup
        {
            get => 
                this._poolGroup;
            set
            {
                this._poolGroup = value;
            }
        }
    }
}

