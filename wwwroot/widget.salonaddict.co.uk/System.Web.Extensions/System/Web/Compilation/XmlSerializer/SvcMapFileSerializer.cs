namespace System.Web.Compilation.XmlSerializer
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class SvcMapFileSerializer : XmlSerializer1
    {
        public override bool CanDeserialize(XmlReader xmlReader) => 
            xmlReader.IsStartElement("ReferenceGroup", "urn:schemas-microsoft-com:xml-wcfservicemap");

        protected override object Deserialize(XmlSerializationReader reader) => 
            ((XmlSerializationReaderSvcMapFile) reader).Read16_ReferenceGroup();

        protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
        {
            ((XmlSerializationWriterSvcMapFile) writer).Write16_ReferenceGroup(objectToSerialize);
        }
    }
}

