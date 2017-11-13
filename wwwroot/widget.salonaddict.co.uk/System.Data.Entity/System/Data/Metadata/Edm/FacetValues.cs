namespace System.Data.Metadata.Edm
{
    using System;
    using System.Runtime.InteropServices;

    internal class FacetValues
    {
        private FacetValueContainer<bool?> _fixedLength;
        private FacetValueContainer<int?> _maxLength;
        private FacetValueContainer<bool?> _nullable;
        private FacetValueContainer<byte?> _precision;
        private FacetValueContainer<byte?> _scale;
        private FacetValueContainer<bool?> _unicode;

        internal bool TryGetFacet(FacetDescription description, out Facet facet)
        {
            if (description.FacetName == "Nullable")
            {
                if (this._nullable.HasValue)
                {
                    facet = Facet.Create(description, this._nullable.GetValueAsObject());
                    return true;
                }
            }
            else if (description.FacetName == "MaxLength")
            {
                if (this._maxLength.HasValue)
                {
                    facet = Facet.Create(description, this._maxLength.GetValueAsObject());
                    return true;
                }
            }
            else if (description.FacetName == "Unicode")
            {
                if (this._unicode.HasValue)
                {
                    facet = Facet.Create(description, this._unicode.GetValueAsObject());
                    return true;
                }
            }
            else if (description.FacetName == "FixedLength")
            {
                if (this._fixedLength.HasValue)
                {
                    facet = Facet.Create(description, this._fixedLength.GetValueAsObject());
                    return true;
                }
            }
            else if (description.FacetName == "Precision")
            {
                if (this._precision.HasValue)
                {
                    facet = Facet.Create(description, this._precision.GetValueAsObject());
                    return true;
                }
            }
            else if ((description.FacetName == "Scale") && this._scale.HasValue)
            {
                facet = Facet.Create(description, this._scale.GetValueAsObject());
                return true;
            }
            facet = null;
            return false;
        }

        internal FacetValueContainer<bool?> FixedLength
        {
            set
            {
                this._fixedLength = value;
            }
        }

        internal FacetValueContainer<int?> MaxLength
        {
            set
            {
                this._maxLength = value;
            }
        }

        internal FacetValueContainer<bool?> Nullable
        {
            set
            {
                this._nullable = value;
            }
        }

        internal static FacetValues NullFacetValues =>
            new FacetValues { 
                FixedLength=0,
                MaxLength=0,
                Precision=0,
                Scale=0,
                Unicode=0
            };

        internal FacetValueContainer<byte?> Precision
        {
            set
            {
                this._precision = value;
            }
        }

        internal FacetValueContainer<byte?> Scale
        {
            set
            {
                this._scale = value;
            }
        }

        internal FacetValueContainer<bool?> Unicode
        {
            set
            {
                this._unicode = value;
            }
        }
    }
}

