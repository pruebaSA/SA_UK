namespace System.Xml.Schema
{
    internal class Datatype_token : Datatype_normalizedString
    {
        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Token;
    }
}

