namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal abstract class EntityContainerRelationshipSet : SchemaElement
    {
        private IRelationship _relationship;
        private string _unresolvedRelationshipTypeName;

        public EntityContainerRelationshipSet(System.Data.EntityModel.SchemaObjectModel.EntityContainer parentElement) : base(parentElement)
        {
        }

        protected abstract void AddEnd(IRelationshipEnd relationshipEnd, EntityContainerEntitySet entitySet);
        protected void HandleRelationshipTypeNameAttribute(XmlReader reader)
        {
            ReturnValue<string> value2 = base.HandleDottedNameAttribute(reader, this._unresolvedRelationshipTypeName, new Func<object, string>(Strings.PropertyTypeAlreadyDefined));
            if (value2.Succeeded)
            {
                this._unresolvedRelationshipTypeName = value2.Value;
            }
        }

        protected abstract bool HasEnd(string role);
        private void InferEnds()
        {
            foreach (IRelationshipEnd end in this.Relationship.Ends)
            {
                if (!this.HasEnd(end.Name))
                {
                    EntityContainerEntitySet entitySet = this.InferEntitySet(end);
                    if (entitySet != null)
                    {
                        this.AddEnd(end, entitySet);
                    }
                }
            }
        }

        private EntityContainerEntitySet InferEntitySet(IRelationshipEnd relationshipEnd)
        {
            List<EntityContainerEntitySet> list = new List<EntityContainerEntitySet>();
            foreach (EntityContainerEntitySet set in this.ParentElement.EntitySets)
            {
                if (relationshipEnd.Type.IsOfType(set.EntityType))
                {
                    list.Add(set);
                }
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            if (list.Count == 0)
            {
                base.AddError(ErrorCode.MissingExtentEntityContainerEnd, EdmSchemaErrorSeverity.Error, Strings.MissingEntityContainerEnd(relationshipEnd.Name, this.FQName));
            }
            else
            {
                base.AddError(ErrorCode.AmbiguousEntityContainerEnd, EdmSchemaErrorSeverity.Error, Strings.AmbiguousEntityContainerEnd(relationshipEnd.Name, this.FQName));
            }
            return null;
        }

        internal override void ResolveSecondLevelNames()
        {
            base.ResolveSecondLevelNames();
            foreach (EntityContainerRelationshipSetEnd end in this.Ends)
            {
                end.ResolveSecondLevelNames();
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (this._relationship == null)
            {
                SchemaType type;
                if (!base.Schema.ResolveTypeName(this, this._unresolvedRelationshipTypeName, out type))
                {
                    return;
                }
                this._relationship = type as IRelationship;
                if (this._relationship == null)
                {
                    base.AddError(ErrorCode.InvalidPropertyType, EdmSchemaErrorSeverity.Error, Strings.InvalidRelationshipSetType(type.Name));
                    return;
                }
            }
            foreach (EntityContainerRelationshipSetEnd end in this.Ends)
            {
                end.ResolveTopLevelNames();
            }
        }

        internal override void Validate()
        {
            base.Validate();
            this.InferEnds();
            foreach (EntityContainerRelationshipSetEnd end in this.Ends)
            {
                end.Validate();
            }
        }

        internal abstract IEnumerable<EntityContainerRelationshipSetEnd> Ends { get; }

        internal System.Data.EntityModel.SchemaObjectModel.EntityContainer ParentElement =>
            ((System.Data.EntityModel.SchemaObjectModel.EntityContainer) base.ParentElement);

        internal IRelationship Relationship
        {
            get => 
                this._relationship;
            set
            {
                this._relationship = value;
            }
        }
    }
}

