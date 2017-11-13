namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class EntityKeyElement : SchemaElement
    {
        private List<PropertyRefElement> _keyProperties;

        public EntityKeyElement(SchemaEntityType parentElement) : base(parentElement)
        {
        }

        protected override bool HandleAttribute(XmlReader reader) => 
            false;

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "PropertyRef"))
            {
                this.HandlePropertyRefElement(reader);
                return true;
            }
            return false;
        }

        private void HandlePropertyRefElement(XmlReader reader)
        {
            PropertyRefElement item = new PropertyRefElement((SchemaEntityType) base.ParentElement);
            item.Parse(reader);
            this.KeyProperties.Add(item);
        }

        internal override void ResolveTopLevelNames()
        {
            foreach (PropertyRefElement element in this._keyProperties)
            {
                if (!element.ResolveNames((SchemaEntityType) base.ParentElement))
                {
                    base.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, Strings.InvalidKeyNoProperty(base.ParentElement.FQName, element.Name));
                }
            }
        }

        internal override void Validate()
        {
            Dictionary<string, PropertyRefElement> dictionary = new Dictionary<string, PropertyRefElement>(StringComparer.Ordinal);
            foreach (PropertyRefElement element in this._keyProperties)
            {
                StructuredProperty property = element.Property;
                if (dictionary.ContainsKey(property.Name))
                {
                    base.AddError(ErrorCode.DuplicatePropertySpecifiedInEntityKey, EdmSchemaErrorSeverity.Error, Strings.DuplicatePropertyNameSpecifiedInEntityKey(base.ParentElement.FQName, property.Name));
                }
                else
                {
                    dictionary.Add(property.Name, element);
                    if (property.Nullable)
                    {
                        base.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, Strings.InvalidKeyNullablePart(property.Name, base.ParentElement.Name));
                    }
                    if (!(property.Type is ScalarType) || (property.CollectionKind != CollectionKind.None))
                    {
                        base.AddError(ErrorCode.EntityKeyMustBeScalar, EdmSchemaErrorSeverity.Error, Strings.EntityKeyMustBeScalar(property.Name, base.ParentElement.Name));
                    }
                    else
                    {
                        PrimitiveTypeKind primitiveTypeKind = ((PrimitiveType) property.TypeUsage.EdmType).PrimitiveTypeKind;
                        if (!Helper.IsValidKeyType(primitiveTypeKind))
                        {
                            if (base.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
                            {
                                base.AddError(ErrorCode.BinaryEntityKeyCurrentlyNotSupported, EdmSchemaErrorSeverity.Error, Strings.EntityKeyTypeCurrentlyNotSupported(property.Name, base.ParentElement.FQName, primitiveTypeKind));
                            }
                            else
                            {
                                base.AddError(ErrorCode.BinaryEntityKeyCurrentlyNotSupported, EdmSchemaErrorSeverity.Error, Strings.EntityKeyTypeCurrentlyNotSupportedInSSDL(property.Name, base.ParentElement.FQName, property.TypeUsage.EdmType.Name, property.TypeUsage.EdmType.BaseType.FullName, primitiveTypeKind));
                            }
                        }
                    }
                }
            }
        }

        public IList<PropertyRefElement> KeyProperties
        {
            get
            {
                if (this._keyProperties == null)
                {
                    this._keyProperties = new List<PropertyRefElement>();
                }
                return this._keyProperties;
            }
        }
    }
}

