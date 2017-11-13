namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal class AddressingProperty
    {
        private string action;
        private UniqueId messageId;
        private EndpointAddress replyTo;
        private Uri to;

        public AddressingProperty(MessageHeaders headers)
        {
            this.action = headers.Action;
            this.to = headers.To;
            this.replyTo = headers.ReplyTo;
            this.messageId = headers.MessageId;
        }

        public string Action =>
            this.action;

        public UniqueId MessageId =>
            this.messageId;

        public static string Name =>
            "Addressing";

        public EndpointAddress ReplyTo =>
            this.replyTo;

        public Uri To =>
            this.to;
    }
}

