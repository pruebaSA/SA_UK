namespace System.Data.ProviderBase
{
    using System;

    internal sealed class DbConnectionPoolGroupOptions
    {
        private readonly int _creationTimeout;
        private readonly bool _hasTransactionAffinity;
        private readonly TimeSpan _loadBalanceTimeout;
        private readonly int _maxPoolSize;
        private readonly int _minPoolSize;
        private readonly bool _poolByIdentity;
        private readonly bool _useDeactivateQueue;
        private readonly bool _useLoadBalancing;

        public DbConnectionPoolGroupOptions(bool poolByIdentity, int minPoolSize, int maxPoolSize, int creationTimeout, int loadBalanceTimeout, bool hasTransactionAffinity, bool useDeactivateQueue)
        {
            this._poolByIdentity = poolByIdentity;
            this._minPoolSize = minPoolSize;
            this._maxPoolSize = maxPoolSize;
            this._creationTimeout = creationTimeout;
            if (loadBalanceTimeout != 0)
            {
                this._loadBalanceTimeout = new TimeSpan(0, 0, loadBalanceTimeout);
                this._useLoadBalancing = true;
            }
            this._hasTransactionAffinity = hasTransactionAffinity;
            this._useDeactivateQueue = useDeactivateQueue;
        }

        public int CreationTimeout =>
            this._creationTimeout;

        public bool HasTransactionAffinity =>
            this._hasTransactionAffinity;

        public TimeSpan LoadBalanceTimeout =>
            this._loadBalanceTimeout;

        public int MaxPoolSize =>
            this._maxPoolSize;

        public int MinPoolSize =>
            this._minPoolSize;

        public bool PoolByIdentity =>
            this._poolByIdentity;

        public bool UseDeactivateQueue =>
            this._useDeactivateQueue;

        public bool UseLoadBalancing =>
            this._useLoadBalancing;
    }
}

