namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Threading;

    public sealed class FacetDescription
    {
        private readonly object _defaultValue;
        private Facet _defaultValueFacet;
        private readonly string _facetName;
        private readonly EdmType _facetType;
        private readonly bool _isConstant;
        private readonly int? _maxValue;
        private readonly int? _minValue;
        private static object _notInitializedSentinel = new object();
        private Facet _nullValueFacet;
        private Facet[] _valueCache;

        internal FacetDescription(string facetName, EdmType facetType, int? minValue, int? maxValue, object defaultValue)
        {
            EntityUtil.CheckStringArgument(facetName, "facetName");
            EntityUtil.GenericCheckArgumentNull<EdmType>(facetType, "facetType");
            if ((minValue.HasValue || maxValue.HasValue) && minValue.HasValue)
            {
                bool hasValue = maxValue.HasValue;
            }
            this._facetName = facetName;
            this._facetType = facetType;
            this._minValue = minValue;
            this._maxValue = maxValue;
            this._defaultValue = defaultValue;
        }

        internal FacetDescription(string facetName, EdmType facetType, int? minValue, int? maxValue, object defaultValue, bool isConstant, string declaringTypeName)
        {
            this._facetName = facetName;
            this._facetType = facetType;
            this._minValue = minValue;
            this._maxValue = maxValue;
            if (defaultValue != null)
            {
                this._defaultValue = defaultValue;
            }
            else
            {
                this._defaultValue = _notInitializedSentinel;
            }
            this._isConstant = isConstant;
            this.Validate(declaringTypeName);
            if (this._isConstant)
            {
                UpdateMinMaxValueForConstant(this._facetName, this._facetType, ref this._minValue, ref this._maxValue, this._defaultValue);
            }
        }

        internal Facet GetBooleanFacet(bool value)
        {
            if (this._valueCache == null)
            {
                Facet[] facetArray = new Facet[] { Facet.Create(this, true, true), Facet.Create(this, false, true) };
                Interlocked.CompareExchange<Facet[]>(ref this._valueCache, facetArray, null);
            }
            if (!value)
            {
                return this._valueCache[1];
            }
            return this._valueCache[0];
        }

        internal static bool IsNumericType(EdmType facetType)
        {
            if (!Helper.IsPrimitiveType(facetType))
            {
                return false;
            }
            PrimitiveType type = (PrimitiveType) facetType;
            if (((type.PrimitiveTypeKind != PrimitiveTypeKind.Byte) && (type.PrimitiveTypeKind != PrimitiveTypeKind.SByte)) && (type.PrimitiveTypeKind != PrimitiveTypeKind.Int16))
            {
                return (type.PrimitiveTypeKind == PrimitiveTypeKind.Int32);
            }
            return true;
        }

        public override string ToString() => 
            this.FacetName;

        private static void UpdateMinMaxValueForConstant(string facetName, EdmType facetType, ref int? minValue, ref int? maxValue, object defaultValue)
        {
            if (IsNumericType(facetType))
            {
                if ((facetName == "Precision") || (facetName == "Scale"))
                {
                    byte? nullable = (byte?) defaultValue;
                    minValue = nullable.HasValue ? new int?(nullable.GetValueOrDefault()) : 0;
                    byte? nullable3 = (byte?) defaultValue;
                    maxValue = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault()) : 0;
                }
                else
                {
                    minValue = (int?) defaultValue;
                    maxValue = (int?) defaultValue;
                }
            }
        }

        private void Validate(string declaringTypeName)
        {
            if (this._defaultValue == _notInitializedSentinel)
            {
                if (this._isConstant)
                {
                    throw EntityUtil.MissingDefaultValueForConstantFacet(this._facetName, declaringTypeName);
                }
            }
            else if (IsNumericType(this._facetType))
            {
                if (this._isConstant)
                {
                    if ((this._minValue.HasValue != this._maxValue.HasValue) || (this._minValue.HasValue && (this._minValue.Value != this._maxValue.Value)))
                    {
                        throw EntityUtil.MinAndMaxValueMustBeSameForConstantFacet(this._facetName, declaringTypeName);
                    }
                }
                else
                {
                    if (!this._minValue.HasValue || !this._maxValue.HasValue)
                    {
                        throw EntityUtil.BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet(this._facetName, declaringTypeName);
                    }
                    if (this._minValue.Value == this._maxValue)
                    {
                        throw EntityUtil.MinAndMaxValueMustBeDifferentForNonConstantFacet(this._facetName, declaringTypeName);
                    }
                    if ((this._minValue < 0) || (this._maxValue < 0))
                    {
                        throw EntityUtil.MinAndMaxMustBePositive(this._facetName, declaringTypeName);
                    }
                    int? nullable12 = this._minValue;
                    int? nullable13 = this._maxValue;
                    if ((nullable12.GetValueOrDefault() > nullable13.GetValueOrDefault()) && (nullable12.HasValue & nullable13.HasValue))
                    {
                        throw EntityUtil.MinMustBeLessThanMax(this._minValue.ToString(), this._facetName, declaringTypeName);
                    }
                }
            }
        }

        public object DefaultValue
        {
            get
            {
                if (this._defaultValue == _notInitializedSentinel)
                {
                    return null;
                }
                return this._defaultValue;
            }
        }

        internal Facet DefaultValueFacet
        {
            get
            {
                if (this._defaultValueFacet == null)
                {
                    Facet facet = Facet.Create(this, this.DefaultValue, true);
                    Interlocked.CompareExchange<Facet>(ref this._defaultValueFacet, facet, null);
                }
                return this._defaultValueFacet;
            }
        }

        public string FacetName =>
            this._facetName;

        public EdmType FacetType =>
            this._facetType;

        public bool IsConstant =>
            this._isConstant;

        public bool IsRequired =>
            (this._defaultValue == _notInitializedSentinel);

        public int? MaxValue =>
            this._maxValue;

        public int? MinValue =>
            this._minValue;

        internal Facet NullValueFacet
        {
            get
            {
                if (this._nullValueFacet == null)
                {
                    Facet facet = Facet.Create(this, null, true);
                    Interlocked.CompareExchange<Facet>(ref this._nullValueFacet, facet, null);
                }
                return this._nullValueFacet;
            }
        }
    }
}

