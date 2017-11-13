namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml;

    internal class TypeElement : SchemaType
    {
        private List<FacetDescriptionElement> _facetDescriptions;
        private System.Data.Metadata.Edm.PrimitiveType _primitiveType;

        public TypeElement(Schema parent) : base(parent)
        {
            this._primitiveType = new System.Data.Metadata.Edm.PrimitiveType();
            this._facetDescriptions = new List<FacetDescriptionElement>();
            this._primitiveType.NamespaceName = base.Schema.Namespace;
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "PrimitiveTypeKind"))
            {
                this.HandlePrimitiveTypeKindAttribute(reader);
                return true;
            }
            return false;
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "FacetDescriptions"))
            {
                this.SkipThroughElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "Precision"))
            {
                this.HandlePrecisionElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "Scale"))
            {
                this.HandleScaleElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "MaxLength"))
            {
                this.HandleMaxLengthElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "Unicode"))
            {
                this.HandleUnicodeElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "FixedLength"))
            {
                this.HandleFixedLengthElement(reader);
                return true;
            }
            return false;
        }

        private void HandleFixedLengthElement(XmlReader reader)
        {
            BooleanFacetDescriptionElement item = new BooleanFacetDescriptionElement(this, "FixedLength");
            item.Parse(reader);
            this._facetDescriptions.Add(item);
        }

        private void HandleMaxLengthElement(XmlReader reader)
        {
            IntegerFacetDescriptionElement item = new IntegerFacetDescriptionElement(this, "MaxLength");
            item.Parse(reader);
            this._facetDescriptions.Add(item);
        }

        private void HandlePrecisionElement(XmlReader reader)
        {
            ByteFacetDescriptionElement item = new ByteFacetDescriptionElement(this, "Precision");
            item.Parse(reader);
            if ((this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Decimal) && (item.MaxValue > 0xff))
            {
                base.AddError(ErrorCode.PrecisionMoreThanAllowedMax, EdmSchemaErrorSeverity.Error, Strings.PrecisionMoreThanAllowedMax(item.MaxValue, (byte) 0xff));
            }
            else if ((((this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.DateTime) || (this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.DateTimeOffset)) || (this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Time)) && (item.MaxValue > 0xff))
            {
                base.AddError(ErrorCode.PrecisionMoreThanAllowedMax, EdmSchemaErrorSeverity.Error, Strings.PrecisionMoreThanAllowedMax(item.MaxValue, (byte) 0xff));
            }
            else
            {
                this._facetDescriptions.Add(item);
            }
        }

        private void HandlePrimitiveTypeKindAttribute(XmlReader reader)
        {
            string str = reader.Value;
            try
            {
                this._primitiveType.PrimitiveTypeKind = (PrimitiveTypeKind) Enum.Parse(typeof(PrimitiveTypeKind), str);
                this._primitiveType.BaseType = MetadataItem.EdmProviderManifest.GetPrimitiveType(this._primitiveType.PrimitiveTypeKind);
            }
            catch (ArgumentException)
            {
                base.AddError(ErrorCode.InvalidPrimitiveTypeKind, EdmSchemaErrorSeverity.Error, Strings.InvalidPrimitiveTypeKind(str));
            }
        }

        private void HandleScaleElement(XmlReader reader)
        {
            ByteFacetDescriptionElement item = new ByteFacetDescriptionElement(this, "Scale");
            item.Parse(reader);
            this._facetDescriptions.Add(item);
        }

        private void HandleUnicodeElement(XmlReader reader)
        {
            BooleanFacetDescriptionElement item = new BooleanFacetDescriptionElement(this, "Unicode");
            item.Parse(reader);
            this._facetDescriptions.Add(item);
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            foreach (FacetDescriptionElement element in this._facetDescriptions)
            {
                try
                {
                    element.CreateAndValidateFacetDescription(this.Name);
                }
                catch (ArgumentException exception)
                {
                    base.AddError(ErrorCode.InvalidFacetInProviderManifest, EdmSchemaErrorSeverity.Error, exception.Message);
                }
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if (this.ValidateSufficientFacets())
            {
                this.ValidateInterFacetConsistency();
            }
        }

        private bool ValidateInterFacetConsistency()
        {
            if (this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Decimal)
            {
                FacetDescription facet = Helper.GetFacet(this.FacetDescriptions, "Precision");
                FacetDescription description2 = Helper.GetFacet(this.FacetDescriptions, "Scale");
                if (facet.MaxValue.Value < description2.MaxValue.Value)
                {
                    base.AddError(ErrorCode.BadPrecisionAndScale, EdmSchemaErrorSeverity.Error, Strings.BadPrecisionAndScale(facet.MaxValue.Value, description2.MaxValue.Value));
                    return false;
                }
            }
            return true;
        }

        private bool ValidateSufficientFacets()
        {
            System.Data.Metadata.Edm.PrimitiveType baseType = this._primitiveType.BaseType as System.Data.Metadata.Edm.PrimitiveType;
            if (baseType == null)
            {
                return false;
            }
            bool flag = false;
            foreach (FacetDescription description in baseType.FacetDescriptions)
            {
                if (Helper.GetFacet(this.FacetDescriptions, description.FacetName) == null)
                {
                    base.AddError(ErrorCode.RequiredFacetMissing, EdmSchemaErrorSeverity.Error, Strings.MissingFacetDescription(this.PrimitiveType.Name, this.PrimitiveType.PrimitiveTypeKind, description.FacetName));
                    flag = true;
                }
            }
            return !flag;
        }

        public IEnumerable<FacetDescription> FacetDescriptions
        {
            get
            {
                foreach (FacetDescriptionElement iteratorVariable0 in this._facetDescriptions)
                {
                    yield return iteratorVariable0.FacetDescription;
                }
            }
        }

        public override string Name
        {
            get => 
                this._primitiveType.Name;
            set
            {
                this._primitiveType.Name = value;
            }
        }

        public System.Data.Metadata.Edm.PrimitiveType PrimitiveType =>
            this._primitiveType;

    }
}

