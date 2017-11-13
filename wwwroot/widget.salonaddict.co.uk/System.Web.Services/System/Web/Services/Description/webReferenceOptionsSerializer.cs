namespace System.Web.Services.Description
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class webReferenceOptionsSerializer : XmlSerializer
    {
        public override bool CanDeserialize(XmlReader xmlReader) => 
            true;

        protected override XmlSerializationReader CreateReader() => 
            new WebReferenceOptionsSerializationReader();

        protected override XmlSerializationWriter CreateWriter() => 
            new WebReferenceOptionsSerializationWriter();

        protected override object Deserialize(XmlSerializationReader reader) => 
            ((WebReferenceOptionsSerializationReader) reader).Read5_webReferenceOptions();

        protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
        {
            ((WebReferenceOptionsSerializationWriter) writer).Write5_webReferenceOptions(objectToSerialize);
        }
    }
}

