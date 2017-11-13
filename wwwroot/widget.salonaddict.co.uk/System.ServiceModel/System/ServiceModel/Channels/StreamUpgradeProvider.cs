﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    public abstract class StreamUpgradeProvider : CommunicationObject
    {
        private TimeSpan closeTimeout;
        private TimeSpan openTimeout;

        protected StreamUpgradeProvider() : this(null)
        {
        }

        protected StreamUpgradeProvider(IDefaultCommunicationTimeouts timeouts)
        {
            if (timeouts != null)
            {
                this.closeTimeout = timeouts.CloseTimeout;
                this.openTimeout = timeouts.OpenTimeout;
            }
            else
            {
                this.closeTimeout = ServiceDefaults.CloseTimeout;
                this.openTimeout = ServiceDefaults.OpenTimeout;
            }
        }

        public abstract StreamUpgradeAcceptor CreateUpgradeAcceptor();
        public abstract StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via);
        internal virtual T GetProperty<T>() where T: class => 
            default(T);

        protected override TimeSpan DefaultCloseTimeout =>
            this.closeTimeout;

        protected override TimeSpan DefaultOpenTimeout =>
            this.closeTimeout;
    }
}

