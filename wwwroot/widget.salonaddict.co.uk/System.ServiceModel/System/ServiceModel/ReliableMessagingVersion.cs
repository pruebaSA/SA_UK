namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;
    using System.Xml;

    public abstract class ReliableMessagingVersion
    {
        private XmlDictionaryString dictionaryNs;
        private string ns;

        internal ReliableMessagingVersion(string ns, XmlDictionaryString dictionaryNs)
        {
            this.ns = ns;
            this.dictionaryNs = dictionaryNs;
        }

        internal static bool IsDefined(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion != WSReliableMessaging11)
            {
                return (reliableMessagingVersion == WSReliableMessagingFebruary2005);
            }
            return true;
        }

        public static ReliableMessagingVersion Default =>
            ReliableSessionDefaults.ReliableMessagingVersion;

        internal XmlDictionaryString DictionaryNamespace =>
            this.dictionaryNs;

        internal string Namespace =>
            this.ns;

        public static ReliableMessagingVersion WSReliableMessaging11 =>
            WSReliableMessaging11Version.Instance;

        public static ReliableMessagingVersion WSReliableMessagingFebruary2005 =>
            WSReliableMessagingFebruary2005Version.Instance;
    }
}

