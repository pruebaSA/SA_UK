namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class DelegatingMessage : Message
    {
        private Message innerMessage;

        protected DelegatingMessage(Message innerMessage)
        {
            if (innerMessage == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerMessage");
            }
            this.innerMessage = innerMessage;
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            this.innerMessage.BodyToString(writer);
        }

        protected override void OnClose()
        {
            base.OnClose();
            this.innerMessage.Close();
        }

        protected override string OnGetBodyAttribute(string localName, string ns) => 
            this.innerMessage.GetBodyAttribute(localName, ns);

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            this.innerMessage.WriteBodyContents(writer);
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            this.innerMessage.WriteStartBody(writer);
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            this.innerMessage.WriteStartEnvelope(writer);
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            this.innerMessage.WriteStartHeaders(writer);
        }

        public override MessageHeaders Headers =>
            this.innerMessage.Headers;

        protected Message InnerMessage =>
            this.innerMessage;

        public override bool IsEmpty =>
            this.innerMessage.IsEmpty;

        public override bool IsFault =>
            this.innerMessage.IsFault;

        public override MessageProperties Properties =>
            this.innerMessage.Properties;

        public override MessageVersion Version =>
            this.innerMessage.Version;
    }
}

