namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Xml;

    internal class TypeUsageBuilder
    {
        private string _default;
        private object _defaultObject;
        private readonly SchemaElement _element;
        private readonly Dictionary<string, object> _facetValues;
        private bool _hasUserDefinedFacets;
        private bool? _nullable;
        private System.Data.Metadata.Edm.TypeUsage _typeUsage;

        internal TypeUsageBuilder(SchemaElement element)
        {
            this._element = element;
            this._facetValues = new Dictionary<string, object>();
        }

        internal bool HandleAttribute(XmlReader reader)
        {
            bool flag = this.InternalHandleAttribute(reader);
            this._hasUserDefinedFacets |= flag;
            return flag;
        }

        private void HandleCollationAttribute(XmlReader reader)
        {
            if (!string.IsNullOrEmpty(reader.Value))
            {
                this._facetValues.Add("Collation", reader.Value);
            }
        }

        internal void HandleConcurrencyModeAttribute(XmlReader reader)
        {
            ConcurrencyMode none;
            string str = reader.Value;
            if (str == "None")
            {
                none = ConcurrencyMode.None;
            }
            else if (str == "Fixed")
            {
                none = ConcurrencyMode.Fixed;
            }
            else
            {
                return;
            }
            this._facetValues.Add("ConcurrencyMode", none);
        }

        private void HandleDefaultAttribute(XmlReader reader)
        {
            this._default = reader.Value;
        }

        private void HandleIsFixedLengthAttribute(XmlReader reader)
        {
            bool field = false;
            if (this._element.HandleBoolAttribute(reader, ref field))
            {
                this._facetValues.Add("FixedLength", field);
            }
        }

        internal void HandleMaxLengthAttribute(XmlReader reader)
        {
            if (reader.Value.Trim() == "Max")
            {
                this._facetValues.Add("MaxLength", EdmConstants.UnboundedValue);
            }
            else
            {
                int field = 0;
                if (this._element.HandleIntAttribute(reader, ref field))
                {
                    this._facetValues.Add("MaxLength", field);
                }
            }
        }

        private void HandleNullableAttribute(XmlReader reader)
        {
            bool field = false;
            if (this._element.HandleBoolAttribute(reader, ref field))
            {
                this._facetValues.Add("Nullable", field);
                this._nullable = new bool?(field);
            }
        }

        private void HandlePrecisionAttribute(XmlReader reader)
        {
            byte field = 0;
            if (this._element.HandleByteAttribute(reader, ref field))
            {
                this._facetValues.Add("Precision", field);
            }
        }

        private void HandleScaleAttribute(XmlReader reader)
        {
            byte field = 0;
            if (this._element.HandleByteAttribute(reader, ref field))
            {
                this._facetValues.Add("Scale", field);
            }
        }

        internal void HandleStoreGeneratedPatternAttribute(XmlReader reader)
        {
            StoreGeneratedPattern none;
            string str = reader.Value;
            if (str == "None")
            {
                none = StoreGeneratedPattern.None;
            }
            else if (str == "Identity")
            {
                none = StoreGeneratedPattern.Identity;
            }
            else if (str == "Computed")
            {
                none = StoreGeneratedPattern.Computed;
            }
            else
            {
                return;
            }
            this._facetValues.Add("StoreGeneratedPattern", none);
        }

        private void HandleUnicodeAttribute(XmlReader reader)
        {
            bool field = false;
            if (this._element.HandleBoolAttribute(reader, ref field))
            {
                this._facetValues.Add("Unicode", field);
            }
        }

        private bool InternalHandleAttribute(XmlReader reader)
        {
            if (SchemaElement.CanHandleAttribute(reader, "Nullable"))
            {
                this.HandleNullableAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "DefaultValue"))
            {
                this.HandleDefaultAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Precision"))
            {
                this.HandlePrecisionAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Scale"))
            {
                this.HandleScaleAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "StoreGeneratedPattern"))
            {
                this.HandleStoreGeneratedPatternAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "ConcurrencyMode"))
            {
                this.HandleConcurrencyModeAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "MaxLength"))
            {
                this.HandleMaxLengthAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Unicode"))
            {
                this.HandleUnicodeAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Collation"))
            {
                this.HandleCollationAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "FixedLength"))
            {
                this.HandleIsFixedLengthAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Nullable"))
            {
                this.HandleNullableAttribute(reader);
                return true;
            }
            return false;
        }

        private void ValidateAndSetBinaryFacets(EdmType type, Dictionary<string, Facet> facets)
        {
            this.ValidateLengthFacets(type, facets);
        }

        private void ValidateAndSetDecimalFacets(EdmType type, Dictionary<string, Facet> facets)
        {
            Facet facet;
            Facet facet2;
            PrimitiveType type2 = (PrimitiveType) type;
            byte? nullable = null;
            if (facets.TryGetValue("Precision", out facet) && (facet.Value != null))
            {
                nullable = new byte?((byte) facet.Value);
                FacetDescription description = Helper.GetFacet(type2.FacetDescriptions, "Precision");
                byte? nullable2 = nullable;
                int num2 = description.MinValue.Value;
                if (!((nullable2.GetValueOrDefault() < num2) && nullable2.HasValue))
                {
                    byte? nullable4 = nullable;
                    int num3 = description.MaxValue.Value;
                    if (!((nullable4.GetValueOrDefault() > num3) && nullable4.HasValue))
                    {
                        goto Label_00EC;
                    }
                }
                this._element.AddError(ErrorCode.PrecisionOutOfRange, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.PrecisionOutOfRange(nullable, description.MinValue.Value, description.MaxValue.Value, type2.Name));
            }
        Label_00EC:
            if (facets.TryGetValue("Scale", out facet2) && (facet2.Value != null))
            {
                byte num = (byte) facet2.Value;
                FacetDescription description2 = Helper.GetFacet(type2.FacetDescriptions, "Scale");
                if ((num < description2.MinValue.Value) || (num > description2.MaxValue.Value))
                {
                    this._element.AddError(ErrorCode.ScaleOutOfRange, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.ScaleOutOfRange(num, description2.MinValue.Value, description2.MaxValue.Value, type2.Name));
                    return;
                }
                if (nullable.HasValue)
                {
                    byte? nullable12 = nullable;
                    int num4 = num;
                    if ((nullable12.GetValueOrDefault() < num4) && nullable12.HasValue)
                    {
                        this._element.AddError(ErrorCode.BadPrecisionAndScale, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.BadPrecisionAndScale(nullable, num));
                    }
                }
            }
        }

        private void ValidateAndSetStringFacets(EdmType type, Dictionary<string, Facet> facets)
        {
            this.ValidateLengthFacets(type, facets);
        }

        internal void ValidateAndSetTypeUsage(ScalarType scalar, bool complainOnMissingFacet)
        {
            bool flag = true;
            Dictionary<string, Facet> dictionary = scalar.Type.GetAssociatedFacetDescriptions().ToDictionary<FacetDescription, string, Facet>(f => f.FacetName, f => f.DefaultValueFacet);
            Dictionary<string, Facet> facets = new Dictionary<string, Facet>();
            foreach (Facet facet in dictionary.Values)
            {
                object obj2;
                if (this._facetValues.TryGetValue(facet.Name, out obj2))
                {
                    if (facet.Description.IsConstant)
                    {
                        this._element.AddError(ErrorCode.ConstantFacetSpecifiedInSchema, EdmSchemaErrorSeverity.Error, this._element, System.Data.Entity.Strings.ConstantFacetSpecifiedInSchema(facet.Name, scalar.Type.Name));
                        flag = false;
                    }
                    else
                    {
                        facets.Add(facet.Name, Facet.Create(facet.Description, obj2));
                    }
                    this._facetValues.Remove(facet.Name);
                }
                else if (complainOnMissingFacet && facet.Description.IsRequired)
                {
                    this._element.AddError(ErrorCode.RequiredFacetMissing, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.RequiredFacetMissing(facet.Name, scalar.Type.Name));
                    flag = false;
                }
                else
                {
                    facets.Add(facet.Name, facet);
                }
            }
            foreach (KeyValuePair<string, object> pair in this._facetValues)
            {
                if (pair.Key == "StoreGeneratedPattern")
                {
                    Facet facet2 = Facet.Create(Converter.StoreGeneratedPatternFacet, pair.Value);
                    facets.Add(facet2.Name, facet2);
                }
                else if (pair.Key == "ConcurrencyMode")
                {
                    Facet facet3 = Facet.Create(Converter.ConcurrencyModeFacet, pair.Value);
                    facets.Add(facet3.Name, facet3);
                }
                else if ((scalar.Type.PrimitiveTypeKind == PrimitiveTypeKind.String) && (pair.Key == "Collation"))
                {
                    Facet facet4 = Facet.Create(Converter.CollationFacet, pair.Value);
                    facets.Add(facet4.Name, facet4);
                }
                else
                {
                    this._element.AddError(ErrorCode.FacetNotAllowedByType, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.FacetNotAllowed(pair.Key, scalar.Type.Name));
                }
            }
            if (flag)
            {
                switch (scalar.TypeKind)
                {
                    case PrimitiveTypeKind.Binary:
                        this.ValidateAndSetBinaryFacets(scalar.Type, facets);
                        break;

                    case PrimitiveTypeKind.DateTime:
                    case PrimitiveTypeKind.Time:
                    case PrimitiveTypeKind.DateTimeOffset:
                        this.ValidatePrecisionFacetsForDateTimeFamily(scalar.Type, facets);
                        break;

                    case PrimitiveTypeKind.Decimal:
                        this.ValidateAndSetDecimalFacets(scalar.Type, facets);
                        break;

                    case PrimitiveTypeKind.String:
                        this.ValidateAndSetStringFacets(scalar.Type, facets);
                        break;
                }
            }
            this._typeUsage = System.Data.Metadata.Edm.TypeUsage.Create(scalar.Type, facets.Values);
        }

        private void ValidateBinaryDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                string message = System.Data.Entity.Strings.InvalidDefaultBinaryWithNoMaxLength(this._default);
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, message);
            }
        }

        private void ValidateBooleanDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultBoolean(this._default));
            }
        }

        private void ValidateDateTimeDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultDateTime(this._default, @"yyyy-MM-dd HH\:mm\:ss.fffZ".Replace(@"\", "")));
            }
        }

        private void ValidateDateTimeOffsetDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultDateTimeOffset(this._default, @"yyyy-MM-dd HH\:mm\:ss.fffffffz".Replace(@"\", "")));
            }
        }

        private void ValidateDecimalDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultDecimal(this._default, 0x26, 0x26));
            }
        }

        internal void ValidateDefaultValue(SchemaType type)
        {
            if (this._default != null)
            {
                ScalarType scalar = type as ScalarType;
                if (scalar != null)
                {
                    this.ValidateScalarMemberDefaultValue(scalar);
                }
                else
                {
                    this._element.AddError(ErrorCode.DefaultNotAllowed, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.DefaultNotAllowed);
                }
            }
        }

        private void ValidateFloatingPointDefaultValue(ScalarType scalar, double minValue, double maxValue)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultFloatingPoint(this._default, minValue, maxValue));
            }
        }

        private void ValidateGuidDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultGuid(this._default));
            }
        }

        private void ValidateIntegralDefaultValue(ScalarType scalar, long minValue, long maxValue)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultIntegral(this._default, minValue, maxValue));
            }
        }

        private void ValidateLengthFacets(EdmType type, Dictionary<string, Facet> facets)
        {
            Facet facet;
            PrimitiveType type2 = (PrimitiveType) type;
            if ((facets.TryGetValue("MaxLength", out facet) && (facet.Value != null)) && !Helper.IsUnboundedFacetValue(facet))
            {
                int num = (int) facet.Value;
                FacetDescription description = Helper.GetFacet(type2.FacetDescriptions, "MaxLength");
                int num2 = description.MaxValue.Value;
                int num3 = description.MinValue.Value;
                if ((num < num3) || (num > num2))
                {
                    this._element.AddError(ErrorCode.InvalidSize, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidSize(num, num3, num2, type2.Name));
                }
            }
        }

        private void ValidatePrecisionFacetsForDateTimeFamily(EdmType type, Dictionary<string, Facet> facets)
        {
            Facet facet;
            PrimitiveType type2 = (PrimitiveType) type;
            byte? nullable = null;
            if (facets.TryGetValue("Precision", out facet) && (facet.Value != null))
            {
                nullable = new byte?((byte) facet.Value);
                FacetDescription description = Helper.GetFacet(type2.FacetDescriptions, "Precision");
                byte? nullable2 = nullable;
                int num = description.MinValue.Value;
                if (!((nullable2.GetValueOrDefault() < num) && nullable2.HasValue))
                {
                    byte? nullable4 = nullable;
                    int num2 = description.MaxValue.Value;
                    if (!((nullable4.GetValueOrDefault() > num2) && nullable4.HasValue))
                    {
                        return;
                    }
                }
                this._element.AddError(ErrorCode.PrecisionOutOfRange, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.PrecisionOutOfRange(nullable, description.MinValue.Value, description.MaxValue.Value, type2.Name));
            }
        }

        private void ValidateScalarMemberDefaultValue(ScalarType scalar)
        {
            if (scalar != null)
            {
                switch (scalar.TypeKind)
                {
                    case PrimitiveTypeKind.Binary:
                        this.ValidateBinaryDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.Boolean:
                        this.ValidateBooleanDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.Byte:
                        this.ValidateIntegralDefaultValue(scalar, 0L, 0xffL);
                        return;

                    case PrimitiveTypeKind.DateTime:
                        this.ValidateDateTimeDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.Decimal:
                        this.ValidateDecimalDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.Double:
                        this.ValidateFloatingPointDefaultValue(scalar, double.MinValue, double.MaxValue);
                        return;

                    case PrimitiveTypeKind.Guid:
                        this.ValidateGuidDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.Single:
                        this.ValidateFloatingPointDefaultValue(scalar, -3.4028234663852886E+38, 3.4028234663852886E+38);
                        return;

                    case PrimitiveTypeKind.Int16:
                        this.ValidateIntegralDefaultValue(scalar, -32768L, 0x7fffL);
                        return;

                    case PrimitiveTypeKind.Int32:
                        this.ValidateIntegralDefaultValue(scalar, -2147483648L, 0x7fffffffL);
                        return;

                    case PrimitiveTypeKind.Int64:
                        this.ValidateIntegralDefaultValue(scalar, -9223372036854775808L, 0x7fffffffffffffffL);
                        return;

                    case PrimitiveTypeKind.String:
                        this._defaultObject = this._default;
                        return;

                    case PrimitiveTypeKind.Time:
                        this.ValidateTimeDefaultValue(scalar);
                        return;

                    case PrimitiveTypeKind.DateTimeOffset:
                        this.ValidateDateTimeOffsetDefaultValue(scalar);
                        return;
                }
                this._element.AddError(ErrorCode.DefaultNotAllowed, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.DefaultNotAllowed);
            }
        }

        private void ValidateTimeDefaultValue(ScalarType scalar)
        {
            if (!scalar.TryParse(this._default, out this._defaultObject))
            {
                this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, System.Data.Entity.Strings.InvalidDefaultTime(this._default, @"HH\:mm\:ss.fffffffZ".Replace(@"\", "")));
            }
        }

        internal string Default =>
            this._default;

        internal object DefaultAsObject =>
            this._defaultObject;

        internal bool HasUserDefinedFacets =>
            this._hasUserDefinedFacets;

        internal bool Nullable
        {
            get
            {
                if (this._nullable.HasValue)
                {
                    return this._nullable.Value;
                }
                return true;
            }
        }

        internal System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsage;
    }
}

