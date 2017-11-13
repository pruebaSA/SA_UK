namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel.Channels;

    internal abstract class CoordinationMessage : Message
    {
        private MessageHeaders headers;
        private MessageProperties properties;

        protected CoordinationMessage(string action, MessageVersion version)
        {
            this.headers = new MessageHeaders(version);
            this.headers.Action = action;
        }

        public override MessageHeaders Headers =>
            this.headers;

        public override MessageProperties Properties
        {
            get
            {
                if (this.properties == null)
                {
                    lock (this)
                    {
                        if (this.properties == null)
                        {
                            this.properties = new MessageProperties();
                        }
                    }
                }
                return this.properties;
            }
        }

        public override MessageVersion Version =>
            this.headers.MessageVersion;
    }
}

