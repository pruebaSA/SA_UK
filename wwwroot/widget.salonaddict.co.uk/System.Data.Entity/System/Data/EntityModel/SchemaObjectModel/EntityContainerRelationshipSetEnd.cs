namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class EntityContainerRelationshipSetEnd : SchemaElement
    {
        private EntityContainerEntitySet _entitySet;
        private IRelationshipEnd _relationshipEnd;
        private string _unresolvedEntitySetName;

        public EntityContainerRelationshipSetEnd(EntityContainerRelationshipSet parentElement) : base(parentElement)
        {
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "EntitySet"))
            {
                this.HandleEntitySetAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleEntitySetAttribute(XmlReader reader)
        {
            if (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
            {
                this._unresolvedEntitySetName = reader.Value;
            }
            else
            {
                this._unresolvedEntitySetName = base.HandleUndottedNameAttribute(reader, this._unresolvedEntitySetName);
            }
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (this._entitySet == null)
            {
                this._entitySet = this.ParentElement.ParentElement.FindEntitySet(this._unresolvedEntitySetName);
                if (this._entitySet == null)
                {
                    base.AddError(ErrorCode.InvalidEndEntitySet, EdmSchemaErrorSeverity.Error, Strings.InvalidEntitySetNameReference(this._unresolvedEntitySetName, this.Name));
                }
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if (((this._relationshipEnd != null) && (this._entitySet != null)) && (!this._relationshipEnd.Type.IsOfType(this._entitySet.EntityType) && !this._entitySet.EntityType.IsOfType(this._relationshipEnd.Type)))
            {
                base.AddError(ErrorCode.InvalidEndEntitySet, EdmSchemaErrorSeverity.Error, Strings.InvalidEndEntitySetTypeMismatch(this._relationshipEnd.Name));
            }
        }

        public EntityContainerEntitySet EntitySet
        {
            get => 
                this._entitySet;
            internal set
            {
                this._entitySet = value;
            }
        }

        internal EntityContainerRelationshipSet ParentElement =>
            ((EntityContainerRelationshipSet) base.ParentElement);

        public IRelationshipEnd RelationshipEnd
        {
            get => 
                this._relationshipEnd;
            internal set
            {
                this._relationshipEnd = value;
            }
        }
    }
}

