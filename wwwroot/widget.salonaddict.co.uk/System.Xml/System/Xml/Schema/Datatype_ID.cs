namespace System.Xml.Schema
{
    using System.Xml;

    internal class Datatype_ID : Datatype_NCName
    {
        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.ID;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Id;
    }
}

