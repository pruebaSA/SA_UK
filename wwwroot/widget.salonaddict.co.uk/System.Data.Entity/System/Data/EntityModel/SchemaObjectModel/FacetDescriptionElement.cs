namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal abstract class FacetDescriptionElement : SchemaElement
    {
        private object _defaultValue;
        private System.Data.Metadata.Edm.FacetDescription _facetDescription;
        private bool _isConstant;
        private int? _maxValue;
        private int? _minValue;

        public FacetDescriptionElement(TypeElement type, string name) : base(type, name)
        {
        }

        internal void CreateAndValidateFacetDescription(string declaringTypeName)
        {
            this._facetDescription = new System.Data.Metadata.Edm.FacetDescription(this.Name, this.FacetType, this.MinValue, this.MaxValue, this.DefaultValue, this._isConstant, declaringTypeName);
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Minimum"))
            {
                this.HandleMinimumAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Maximum"))
            {
                this.HandleMaximumAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "DefaultValue"))
            {
                this.HandleDefaultAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Constant"))
            {
                this.HandleConstantAttribute(reader);
                return true;
            }
            return false;
        }

        protected void HandleConstantAttribute(XmlReader reader)
        {
            bool field = false;
            if (base.HandleBoolAttribute(reader, ref field))
            {
                this._isConstant = field;
            }
        }

        protected abstract void HandleDefaultAttribute(XmlReader reader);
        protected void HandleMaximumAttribute(XmlReader reader)
        {
            int field = -1;
            if (base.HandleIntAttribute(reader, ref field))
            {
                this._maxValue = new int?(field);
            }
        }

        protected void HandleMinimumAttribute(XmlReader reader)
        {
            int field = -1;
            if (base.HandleIntAttribute(reader, ref field))
            {
                this._minValue = new int?(field);
            }
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        public object DefaultValue
        {
            get => 
                this._defaultValue;
            set
            {
                this._defaultValue = value;
            }
        }

        public System.Data.Metadata.Edm.FacetDescription FacetDescription =>
            this._facetDescription;

        public abstract EdmType FacetType { get; }

        public int? MaxValue =>
            this._maxValue;

        public int? MinValue =>
            this._minValue;
    }
}

