namespace System.Xml.Schema
{
    using System;

    internal class Datatype_nonPositiveInteger : Datatype_integer
    {
        private static readonly System.Xml.Schema.FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(-79228162514264337593543950335M, 0M);

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            numeric10FacetsChecker;

        internal override bool HasValueFacets =>
            true;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.NonPositiveInteger;
    }
}

