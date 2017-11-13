namespace Microsoft.Transactions.Bridge
{
    using System;

    internal abstract class TransactionManagerSettings
    {
        protected TransactionManagerSettings()
        {
        }

        public abstract bool AnyNetworkAccess { get; }

        public abstract Microsoft.Transactions.Bridge.AuthenticationLevel AuthenticationLevel { get; }

        public abstract string ClusterResourceType { get; }

        public abstract bool IsClustered { get; }

        public abstract bool NetworkAdministrationAccess { get; }

        public abstract bool NetworkClientAccess { get; }

        public abstract bool NetworkInboundAccess { get; }

        public abstract bool NetworkOutboundAccess { get; }

        public abstract bool NetworkTransactionAccess { get; }

        public abstract string VirtualServerName { get; }
    }
}

