﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;

    internal class ImmutableCommunicationTimeouts : IDefaultCommunicationTimeouts
    {
        private TimeSpan close;
        private TimeSpan open;
        private TimeSpan receive;
        private TimeSpan send;

        internal ImmutableCommunicationTimeouts() : this(null)
        {
        }

        internal ImmutableCommunicationTimeouts(IDefaultCommunicationTimeouts timeouts)
        {
            if (timeouts == null)
            {
                this.close = ServiceDefaults.CloseTimeout;
                this.open = ServiceDefaults.OpenTimeout;
                this.receive = ServiceDefaults.ReceiveTimeout;
                this.send = ServiceDefaults.SendTimeout;
            }
            else
            {
                this.close = timeouts.CloseTimeout;
                this.open = timeouts.OpenTimeout;
                this.receive = timeouts.ReceiveTimeout;
                this.send = timeouts.SendTimeout;
            }
        }

        TimeSpan IDefaultCommunicationTimeouts.CloseTimeout =>
            this.close;

        TimeSpan IDefaultCommunicationTimeouts.OpenTimeout =>
            this.open;

        TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout =>
            this.receive;

        TimeSpan IDefaultCommunicationTimeouts.SendTimeout =>
            this.send;
    }
}

