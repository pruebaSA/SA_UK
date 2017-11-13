﻿namespace System.Xml.Schema
{
    using System;

    internal class Datatype_normalizedString : Datatype_string
    {
        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Replace;

        internal override bool HasValueFacets =>
            true;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.NormalizedString;
    }
}

