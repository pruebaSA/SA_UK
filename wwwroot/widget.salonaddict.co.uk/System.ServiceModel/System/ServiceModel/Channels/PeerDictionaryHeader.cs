namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using System.Xml;

    internal class PeerDictionaryHeader : DictionaryHeader
    {
        private XmlDictionaryString name;
        private XmlDictionaryString nameSpace;
        private string value;

        public PeerDictionaryHeader(XmlDictionaryString name, XmlDictionaryString nameSpace, string value)
        {
            this.name = name;
            this.nameSpace = nameSpace;
            this.value = value;
        }

        public PeerDictionaryHeader(XmlDictionaryString name, XmlDictionaryString nameSpace, XmlDictionaryString value)
        {
            this.name = name;
            this.nameSpace = nameSpace;
            this.value = value.Value;
        }

        internal static PeerDictionaryHeader CreateFloodRole() => 
            new PeerDictionaryHeader(XD.PeerWireStringsDictionary.FloodAction, XD.PeerWireStringsDictionary.Namespace, XD.PeerWireStringsDictionary.Demuxer);

        internal static PeerDictionaryHeader CreateHopCountHeader(ulong hopcount) => 
            new PeerDictionaryHeader(XD.PeerWireStringsDictionary.HopCount, XD.PeerWireStringsDictionary.HopCountNamespace, hopcount.ToString(CultureInfo.InvariantCulture));

        internal static PeerDictionaryHeader CreateMessageIdHeader(UniqueId messageId) => 
            new PeerDictionaryHeader(XD.AddressingDictionary.MessageId, XD.PeerWireStringsDictionary.Namespace, messageId.ToString());

        internal static PeerDictionaryHeader CreateToHeader(Uri to) => 
            new PeerDictionaryHeader(XD.PeerWireStringsDictionary.PeerTo, XD.PeerWireStringsDictionary.Namespace, to.ToString());

        internal static PeerDictionaryHeader CreateViaHeader(Uri via) => 
            new PeerDictionaryHeader(XD.PeerWireStringsDictionary.PeerVia, XD.PeerWireStringsDictionary.Namespace, via.ToString());

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteString(this.value);
        }

        public override XmlDictionaryString DictionaryName =>
            this.name;

        public override XmlDictionaryString DictionaryNamespace =>
            this.nameSpace;
    }
}

