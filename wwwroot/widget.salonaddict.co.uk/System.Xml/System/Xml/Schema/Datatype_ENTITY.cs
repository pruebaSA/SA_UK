namespace System.Xml.Schema
{
    using System.Xml;

    internal class Datatype_ENTITY : Datatype_NCName
    {
        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.ENTITY;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Entity;
    }
}

