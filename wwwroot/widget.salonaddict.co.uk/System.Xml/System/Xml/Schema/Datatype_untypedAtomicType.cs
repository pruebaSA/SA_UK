namespace System.Xml.Schema
{
    internal class Datatype_untypedAtomicType : Datatype_anyAtomicType
    {
        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlUntypedConverter.Untyped;

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Preserve;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.UntypedAtomic;
    }
}

