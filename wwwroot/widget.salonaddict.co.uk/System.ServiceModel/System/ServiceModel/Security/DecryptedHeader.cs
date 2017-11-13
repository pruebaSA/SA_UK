namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal sealed class DecryptedHeader : ReadableMessageHeader
    {
        private readonly string actor;
        private XmlDictionaryReader cachedReader;
        private readonly byte[] decryptedBuffer;
        private readonly XmlAttributeHolder[] envelopeAttributes;
        private readonly XmlAttributeHolder[] headerAttributes;
        private readonly string id;
        private readonly bool isRefParam;
        private readonly bool mustUnderstand;
        private readonly string name;
        private readonly string namespaceUri;
        private readonly XmlDictionaryReaderQuotas quotas;
        private readonly bool relay;
        private readonly MessageVersion version;

        public DecryptedHeader(byte[] decryptedBuffer, XmlAttributeHolder[] envelopeAttributes, XmlAttributeHolder[] headerAttributes, MessageVersion version, SignatureTargetIdManager idManager, XmlDictionaryReaderQuotas quotas)
        {
            if (quotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("quotas");
            }
            this.decryptedBuffer = decryptedBuffer;
            this.version = version;
            this.envelopeAttributes = envelopeAttributes;
            this.headerAttributes = headerAttributes;
            this.quotas = quotas;
            XmlDictionaryReader reader = this.CreateReader();
            reader.MoveToStartElement();
            this.name = reader.LocalName;
            this.namespaceUri = reader.NamespaceURI;
            MessageHeader.GetHeaderAttributes(reader, version, out this.actor, out this.mustUnderstand, out this.relay, out this.isRefParam);
            this.id = idManager.ExtractId(reader);
            this.cachedReader = reader;
        }

        private XmlDictionaryReader CreateReader() => 
            ContextImportHelper.CreateSplicedReader(this.decryptedBuffer, this.envelopeAttributes, this.headerAttributes, null, this.quotas);

        public override XmlDictionaryReader GetHeaderReader()
        {
            if (this.cachedReader != null)
            {
                XmlDictionaryReader cachedReader = this.cachedReader;
                this.cachedReader = null;
                return cachedReader;
            }
            XmlDictionaryReader reader2 = this.CreateReader();
            reader2.MoveToContent();
            return reader2;
        }

        public override bool IsMessageVersionSupported(MessageVersion messageVersion) => 
            this.version.Equals(this.version);

        public override string Actor =>
            this.actor;

        public string Id =>
            this.id;

        public override bool IsReferenceParameter =>
            this.isRefParam;

        public override bool MustUnderstand =>
            this.mustUnderstand;

        public override string Name =>
            this.name;

        public override string Namespace =>
            this.namespaceUri;

        public override bool Relay =>
            this.relay;
    }
}

