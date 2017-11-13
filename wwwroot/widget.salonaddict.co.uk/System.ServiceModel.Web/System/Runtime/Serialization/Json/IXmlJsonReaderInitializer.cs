namespace System.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    public interface IXmlJsonReaderInitializer
    {
        void SetInput(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose);
        void SetInput(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose);
    }
}

