namespace Microsoft.Transactions.Wsat.Recovery
{
    using System;
    using System.ServiceModel;

    internal class LogEntry
    {
        private EndpointAddress endpoint;
        private Guid localEnlistmentId;
        private Guid localTransactionId;
        private string remoteTransactionId;

        public LogEntry(string remoteTransactionId, Guid localTransactionId, Guid localEnlistmentId) : this(remoteTransactionId, localTransactionId, localEnlistmentId, null)
        {
        }

        public LogEntry(string remoteTransactionId, Guid localTransactionId, Guid localEnlistmentId, EndpointAddress endpoint)
        {
            this.remoteTransactionId = remoteTransactionId;
            this.localTransactionId = localTransactionId;
            this.localEnlistmentId = localEnlistmentId;
            this.endpoint = endpoint;
        }

        public EndpointAddress Endpoint
        {
            get => 
                this.endpoint;
            set
            {
                this.endpoint = value;
            }
        }

        public Guid LocalEnlistmentId =>
            this.localEnlistmentId;

        public Guid LocalTransactionId =>
            this.localTransactionId;

        public string RemoteTransactionId =>
            this.remoteTransactionId;
    }
}

