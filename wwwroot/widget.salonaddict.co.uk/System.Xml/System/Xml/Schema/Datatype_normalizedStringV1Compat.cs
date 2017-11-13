namespace System.Xml.Schema
{
    using System;

    internal class Datatype_normalizedStringV1Compat : Datatype_string
    {
        internal override bool HasValueFacets =>
            true;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.NormalizedString;
    }
}

