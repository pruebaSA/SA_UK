namespace System.Web.Compilation.XmlSerializerDataSvc
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class DataSvcMapFileSerializer : XmlSerializer1
    {
        public override bool CanDeserialize(XmlReader xmlReader) => 
            xmlReader.IsStartElement("ReferenceGroup", "urn:schemas-microsoft-com:xml-dataservicemap");

        protected override object Deserialize(XmlSerializationReader reader) => 
            ((XmlSerializationReaderDataSvcMapFile) reader).Read8_ReferenceGroup();

        protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
        {
            ((XmlSerializationWriterDataSvcMapFile) writer).Write8_ReferenceGroup(objectToSerialize);
        }
    }
}

