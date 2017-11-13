namespace System.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Text;
    using System.Xml;

    public static class JsonReaderWriterFactory
    {
        public static XmlDictionaryReader CreateJsonReader(Stream stream, XmlDictionaryReaderQuotas quotas) => 
            CreateJsonReader(stream, null, quotas, null);

        public static XmlDictionaryReader CreateJsonReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
            }
            return CreateJsonReader(buffer, 0, buffer.Length, null, quotas, null);
        }

        public static XmlDictionaryReader CreateJsonReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas) => 
            CreateJsonReader(buffer, offset, count, null, quotas, null);

        public static XmlDictionaryReader CreateJsonReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
        {
            XmlJsonReader reader = new XmlJsonReader();
            reader.SetInput(stream, encoding, quotas, onClose);
            return reader;
        }

        public static XmlDictionaryReader CreateJsonReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
        {
            XmlJsonReader reader = new XmlJsonReader();
            reader.SetInput(buffer, offset, count, encoding, quotas, onClose);
            return reader;
        }

        public static XmlDictionaryWriter CreateJsonWriter(Stream stream) => 
            CreateJsonWriter(stream, Encoding.UTF8, true);

        public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding) => 
            CreateJsonWriter(stream, encoding, true);

        public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding, bool ownsStream)
        {
            XmlJsonWriter writer = new XmlJsonWriter();
            writer.SetOutput(stream, encoding, ownsStream);
            return writer;
        }
    }
}

