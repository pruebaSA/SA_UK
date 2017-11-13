namespace System.Xml.Schema
{
    using System.Xml;

    internal class Datatype_NMTOKEN : Datatype_token
    {
        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.NMTOKEN;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.NmToken;
    }
}

