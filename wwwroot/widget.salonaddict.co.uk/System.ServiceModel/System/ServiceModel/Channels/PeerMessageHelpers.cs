namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml;

    internal static class PeerMessageHelpers
    {
        public static string GetHeaderString(MessageHeaders headers, string name, string ns)
        {
            string str = null;
            int headerIndex = headers.FindHeader(name, ns);
            if (headerIndex < 0)
            {
                return str;
            }
            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(headerIndex))
            {
                return reader.ReadElementString();
            }
        }

        public static ulong GetHeaderULong(MessageHeaders headers, int index)
        {
            ulong maxValue = ulong.MaxValue;
            if (index < 0)
            {
                return maxValue;
            }
            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(index))
            {
                return XmlConvert.ToUInt64(reader.ReadElementString());
            }
        }

        public static UniqueId GetHeaderUniqueId(MessageHeaders headers, string name, string ns)
        {
            UniqueId id = null;
            int headerIndex = headers.FindHeader(name, ns);
            if (headerIndex < 0)
            {
                return id;
            }
            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(headerIndex))
            {
                return reader.ReadElementContentAsUniqueId();
            }
        }

        public static Uri GetHeaderUri(MessageHeaders headers, string name, string ns)
        {
            Uri uri = null;
            string uriString = GetHeaderString(headers, name, ns);
            if (uriString != null)
            {
                uri = new Uri(uriString);
            }
            return uri;
        }

        public delegate void CleanupCallback(IPeerNeighbor neighbor, PeerCloseReason reason, Exception exception);
    }
}

