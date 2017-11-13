namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class EntityContainerEntitySet : SchemaElement
    {
        private EntityContainerEntitySetDefiningQuery _definingQueryElement;
        private SchemaEntityType _entityType;
        private string _schema;
        private string _table;
        private string _unresolvedEntityTypeName;

        public EntityContainerEntitySet(System.Data.EntityModel.SchemaObjectModel.EntityContainer parentElement) : base(parentElement)
        {
        }

        internal override SchemaElement Clone(SchemaElement parentElement) => 
            new EntityContainerEntitySet((System.Data.EntityModel.SchemaObjectModel.EntityContainer) parentElement) { 
                _definingQueryElement = this._definingQueryElement,
                _entityType = this._entityType,
                _schema = this._schema,
                _table = this._table,
                Name = this.Name
            };

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "EntityType"))
            {
                this.HandleEntityTypeAttribute(reader);
                return true;
            }
            if (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
            {
                if (SchemaElement.CanHandleAttribute(reader, "Schema"))
                {
                    this.HandleDbSchemaAttribute(reader);
                    return true;
                }
                if (SchemaElement.CanHandleAttribute(reader, "Table"))
                {
                    this.HandleTableAttribute(reader);
                    return true;
                }
            }
            return false;
        }

        private void HandleDbSchemaAttribute(XmlReader reader)
        {
            this._schema = reader.Value;
        }

        private void HandleDefiningQueryElement(XmlReader reader)
        {
            EntityContainerEntitySetDefiningQuery query = new EntityContainerEntitySetDefiningQuery(this);
            query.Parse(reader);
            this._definingQueryElement = query;
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if ((base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel) && base.CanHandleElement(reader, "DefiningQuery"))
            {
                this.HandleDefiningQueryElement(reader);
                return true;
            }
            return false;
        }

        private void HandleEntityTypeAttribute(XmlReader reader)
        {
            ReturnValue<string> value2 = base.HandleDottedNameAttribute(reader, this._unresolvedEntityTypeName, new Func<object, string>(Strings.PropertyTypeAlreadyDefined));
            if (value2.Succeeded)
            {
                this._unresolvedEntityTypeName = value2.Value;
            }
        }

        protected override void HandleNameAttribute(XmlReader reader)
        {
            if (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
            {
                this.Name = reader.Value;
            }
            else
            {
                base.HandleNameAttribute(reader);
            }
        }

        private void HandleTableAttribute(XmlReader reader)
        {
            this._table = reader.Value;
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (this._entityType == null)
            {
                SchemaType type = null;
                if (base.Schema.ResolveTypeName(this, this._unresolvedEntityTypeName, out type))
                {
                    this._entityType = type as SchemaEntityType;
                    if (this._entityType == null)
                    {
                        base.AddError(ErrorCode.InvalidPropertyType, EdmSchemaErrorSeverity.Error, Strings.InvalidEntitySetType(this._unresolvedEntityTypeName));
                    }
                }
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if (this._entityType.KeyProperties.Count == 0)
            {
                base.AddError(ErrorCode.EntitySetTypeHasNoKeys, EdmSchemaErrorSeverity.Error, Strings.EntitySetTypeHasNoKeys(this.Name, this._entityType.FQName));
            }
            if (this._definingQueryElement != null)
            {
                this._definingQueryElement.Validate();
                if ((this.DbSchema != null) || (this.Table != null))
                {
                    base.AddError(ErrorCode.TableAndSchemaAreMutuallyExclusiveWithDefiningQuery, EdmSchemaErrorSeverity.Error, Strings.TableAndSchemaAreMutuallyExclusiveWithDefiningQuery(this.FQName));
                }
            }
        }

        public string DbSchema =>
            this._schema;

        public string DefiningQuery
        {
            get
            {
                if (this._definingQueryElement != null)
                {
                    return this._definingQueryElement.Query;
                }
                return null;
            }
        }

        public SchemaEntityType EntityType =>
            this._entityType;

        public string Table =>
            this._table;
    }
}

