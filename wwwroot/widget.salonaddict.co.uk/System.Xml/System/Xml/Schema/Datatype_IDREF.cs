namespace System.Xml.Schema
{
    using System.Xml;

    internal class Datatype_IDREF : Datatype_NCName
    {
        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.IDREF;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Idref;
    }
}

