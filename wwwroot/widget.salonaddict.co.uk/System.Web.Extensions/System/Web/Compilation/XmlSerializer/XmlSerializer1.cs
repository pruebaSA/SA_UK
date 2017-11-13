namespace System.Web.Compilation.XmlSerializer
{
    using System;
    using System.Xml.Serialization;

    internal abstract class XmlSerializer1 : XmlSerializer
    {
        protected XmlSerializer1()
        {
        }

        protected override XmlSerializationReader CreateReader() => 
            new XmlSerializationReaderSvcMapFile();

        protected override XmlSerializationWriter CreateWriter() => 
            new XmlSerializationWriterSvcMapFile();
    }
}

