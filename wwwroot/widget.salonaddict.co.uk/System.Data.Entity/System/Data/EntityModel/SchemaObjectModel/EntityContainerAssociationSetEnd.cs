namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class EntityContainerAssociationSetEnd : EntityContainerRelationshipSetEnd
    {
        private string _unresolvedRelationshipEndRole;

        public EntityContainerAssociationSetEnd(EntityContainerAssociationSet parentElement) : base(parentElement)
        {
        }

        internal override SchemaElement Clone(SchemaElement parentElement) => 
            new EntityContainerAssociationSetEnd((EntityContainerAssociationSet) parentElement) { 
                _unresolvedRelationshipEndRole = this._unresolvedRelationshipEndRole,
                EntitySet = base.EntitySet
            };

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Role"))
            {
                this.HandleRoleAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleRoleAttribute(XmlReader reader)
        {
            this._unresolvedRelationshipEndRole = base.HandleUndottedNameAttribute(reader, this._unresolvedRelationshipEndRole);
        }

        private IRelationshipEnd InferRelationshipEnd(EntityContainerEntitySet set)
        {
            if (base.ParentElement.Relationship != null)
            {
                List<IRelationshipEnd> list = new List<IRelationshipEnd>();
                foreach (IRelationshipEnd end in base.ParentElement.Relationship.Ends)
                {
                    if (set.EntityType.IsOfType(end.Type))
                    {
                        list.Add(end);
                    }
                }
                if (list.Count == 1)
                {
                    return list[0];
                }
                if (list.Count == 0)
                {
                    base.AddError(ErrorCode.FailedInference, EdmSchemaErrorSeverity.Error, Strings.InferRelationshipEndFailedNoEntitySetMatch(set.FQName, base.ParentElement.FQName, base.ParentElement.Relationship.FQName, set.EntityType.FQName, base.ParentElement.ParentElement.FQName));
                }
                else
                {
                    base.AddError(ErrorCode.FailedInference, EdmSchemaErrorSeverity.Error, Strings.InferRelationshipEndAmbigous(set.FQName, base.ParentElement.FQName, base.ParentElement.Relationship.FQName, set.EntityType.FQName, base.ParentElement.ParentElement.FQName));
                }
            }
            return null;
        }

        internal override void ResolveSecondLevelNames()
        {
            base.ResolveSecondLevelNames();
            if ((this._unresolvedRelationshipEndRole == null) && (base.EntitySet != null))
            {
                base.RelationshipEnd = this.InferRelationshipEnd(base.EntitySet);
                if (base.RelationshipEnd != null)
                {
                    this._unresolvedRelationshipEndRole = base.RelationshipEnd.Name;
                }
            }
            else if (this._unresolvedRelationshipEndRole != null)
            {
                IRelationshipEnd end;
                IRelationship relationship = base.ParentElement.Relationship;
                if (relationship.TryGetEnd(this._unresolvedRelationshipEndRole, out end))
                {
                    base.RelationshipEnd = end;
                }
                else
                {
                    base.AddError(ErrorCode.InvalidContainerTypeForEnd, EdmSchemaErrorSeverity.Error, Strings.InvalidEntityEndName(this.Role, relationship.FQName));
                }
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            IRelationship relationship = base.ParentElement.Relationship;
        }

        public override string Name =>
            this.Role;

        public string Role
        {
            get => 
                this._unresolvedRelationshipEndRole;
            set
            {
                this._unresolvedRelationshipEndRole = value;
            }
        }
    }
}

