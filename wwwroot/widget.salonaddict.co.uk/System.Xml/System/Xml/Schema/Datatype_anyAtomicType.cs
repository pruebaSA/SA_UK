namespace System.Xml.Schema
{
    internal class Datatype_anyAtomicType : Datatype_anySimpleType
    {
        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlAnyConverter.AnyAtomic;

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Preserve;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.AnyAtomicType;
    }
}

