namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal abstract class DelegatingHeader : MessageHeader
    {
        private MessageHeader innerHeader;

        protected DelegatingHeader(MessageHeader innerHeader)
        {
            if (innerHeader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerHeader");
            }
            this.innerHeader = innerHeader;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            this.innerHeader.WriteHeaderContents(writer, messageVersion);
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            this.innerHeader.WriteStartHeader(writer, messageVersion);
        }

        public override string Actor =>
            this.innerHeader.Actor;

        protected MessageHeader InnerHeader =>
            this.innerHeader;

        public override bool MustUnderstand =>
            this.innerHeader.MustUnderstand;

        public override string Name =>
            this.innerHeader.Name;

        public override string Namespace =>
            this.innerHeader.Namespace;

        public override bool Relay =>
            this.innerHeader.Relay;
    }
}

