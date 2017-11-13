namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Diagnostics;

    [DebuggerDisplay("{Name,nq}={Value}")]
    public sealed class Facet : MetadataItem
    {
        private readonly FacetDescription _facetDescription;
        private readonly object _value;

        private Facet(FacetDescription facetDescription, object value) : base(MetadataItem.MetadataFlags.Readonly)
        {
            EntityUtil.GenericCheckArgumentNull<FacetDescription>(facetDescription, "facetDescription");
            this._facetDescription = facetDescription;
            this._value = value;
        }

        internal static Facet Create(FacetDescription facetDescription, object value) => 
            Create(facetDescription, value, false);

        internal static Facet Create(FacetDescription facetDescription, object value, bool bypassKnownValues)
        {
            EntityUtil.CheckArgumentNull<FacetDescription>(facetDescription, "facetDescription");
            if (!bypassKnownValues)
            {
                if (object.ReferenceEquals(value, null))
                {
                    return facetDescription.NullValueFacet;
                }
                if (object.Equals(facetDescription.DefaultValue, value))
                {
                    return facetDescription.DefaultValueFacet;
                }
                if (facetDescription.FacetType.Identity == "Edm.Boolean")
                {
                    bool flag = (bool) value;
                    return facetDescription.GetBooleanFacet(flag);
                }
            }
            Facet facet = new Facet(facetDescription, value);
            if (((value != null) && !Helper.IsUnboundedFacetValue(facet)) && (facet.FacetType.ClrType != null))
            {
                Type c = value.GetType();
                if ((c != facet.FacetType.ClrType) && !facet.FacetType.ClrType.IsAssignableFrom(c))
                {
                    throw EntityUtil.FacetValueHasIncorrectType(facet.Name, facet.FacetType.ClrType, c, "value");
                }
            }
            return facet;
        }

        public override string ToString() => 
            this.Name;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.Facet;

        public FacetDescription Description =>
            this._facetDescription;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType, false)]
        public EdmType FacetType =>
            this._facetDescription.FacetType;

        internal override string Identity =>
            this._facetDescription.FacetName;

        public bool IsUnbounded =>
            object.ReferenceEquals(this.Value, EdmConstants.UnboundedValue);

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._facetDescription.FacetName;

        [MetadataProperty(typeof(object), false)]
        public object Value =>
            this._value;
    }
}

