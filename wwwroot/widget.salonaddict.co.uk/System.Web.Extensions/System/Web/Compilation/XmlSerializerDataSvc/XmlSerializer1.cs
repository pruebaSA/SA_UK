namespace System.Web.Compilation.XmlSerializerDataSvc
{
    using System;
    using System.Xml.Serialization;

    internal abstract class XmlSerializer1 : XmlSerializer
    {
        protected XmlSerializer1()
        {
        }

        protected override XmlSerializationReader CreateReader() => 
            new XmlSerializationReaderDataSvcMapFile();

        protected override XmlSerializationWriter CreateWriter() => 
            new XmlSerializationWriterDataSvcMapFile();
    }
}

