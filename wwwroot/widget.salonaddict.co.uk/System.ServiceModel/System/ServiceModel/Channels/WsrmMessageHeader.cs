namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class WsrmMessageHeader : DictionaryHeader, IMessageHeaderWithSharedNamespace
    {
        private System.ServiceModel.ReliableMessagingVersion reliableMessagingVersion;

        protected WsrmMessageHeader(System.ServiceModel.ReliableMessagingVersion reliableMessagingVersion)
        {
            this.reliableMessagingVersion = reliableMessagingVersion;
        }

        public override XmlDictionaryString DictionaryNamespace =>
            WsrmIndex.GetNamespace(this.reliableMessagingVersion);

        public override string Namespace =>
            WsrmIndex.GetNamespaceString(this.reliableMessagingVersion);

        protected System.ServiceModel.ReliableMessagingVersion ReliableMessagingVersion =>
            this.reliableMessagingVersion;

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace =>
            WsrmIndex.GetNamespace(this.reliableMessagingVersion);

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix =>
            XD.WsrmFeb2005Dictionary.Prefix;
    }
}

