namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml;

    internal sealed class EntityContainerAssociationSet : EntityContainerRelationshipSet
    {
        private Dictionary<string, EntityContainerAssociationSetEnd> _relationshipEnds;
        private List<EntityContainerAssociationSetEnd> _rolelessEnds;

        public EntityContainerAssociationSet(System.Data.EntityModel.SchemaObjectModel.EntityContainer parentElement) : base(parentElement)
        {
            this._relationshipEnds = new Dictionary<string, EntityContainerAssociationSetEnd>();
            this._rolelessEnds = new List<EntityContainerAssociationSetEnd>();
        }

        protected override void AddEnd(IRelationshipEnd relationshipEnd, EntityContainerEntitySet entitySet)
        {
            EntityContainerAssociationSetEnd end = new EntityContainerAssociationSetEnd(this) {
                Role = relationshipEnd.Name,
                RelationshipEnd = relationshipEnd,
                EntitySet = entitySet
            };
            if (end.EntitySet != null)
            {
                this._relationshipEnds.Add(end.Role, end);
            }
        }

        internal override SchemaElement Clone(SchemaElement parentElement)
        {
            EntityContainerAssociationSet set = new EntityContainerAssociationSet((System.Data.EntityModel.SchemaObjectModel.EntityContainer) parentElement) {
                Name = this.Name,
                Relationship = base.Relationship
            };
            foreach (EntityContainerAssociationSetEnd end in this.Ends)
            {
                EntityContainerAssociationSetEnd end2 = (EntityContainerAssociationSetEnd) end.Clone(set);
                set._relationshipEnds.Add(end2.Role, end2);
            }
            return set;
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Association"))
            {
                base.HandleRelationshipTypeNameAttribute(reader);
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
            if (base.CanHandleElement(reader, "End"))
            {
                this.HandleEndElement(reader);
                return true;
            }
            return false;
        }

        private void HandleEndElement(XmlReader reader)
        {
            EntityContainerAssociationSetEnd item = new EntityContainerAssociationSetEnd(this);
            item.Parse(reader);
            if (item.Role == null)
            {
                this._rolelessEnds.Add(item);
            }
            else if (this.HasEnd(item.Role))
            {
                item.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.DuplicateEndName(item.Name));
            }
            else
            {
                this._relationshipEnds.Add(item.Role, item);
            }
        }

        protected override bool HasEnd(string role) => 
            this._relationshipEnds.ContainsKey(role);

        internal override void ResolveSecondLevelNames()
        {
            base.ResolveSecondLevelNames();
            foreach (EntityContainerAssociationSetEnd end in this._rolelessEnds)
            {
                if (end.Role != null)
                {
                    if (this.HasEnd(end.Role))
                    {
                        end.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, Strings.InferRelationshipEndGivesAlreadyDefinedEnd(end.EntitySet.FQName, this.Name));
                    }
                    else
                    {
                        this._relationshipEnds.Add(end.Role, end);
                    }
                }
            }
            this._rolelessEnds.Clear();
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if ((base.Relationship != null) && (base.Relationship.RelationshipKind != RelationshipKind.Association))
            {
                base.AddError(ErrorCode.BadType, EdmSchemaErrorSeverity.Error, Strings.AssociationSetInvalidRelationshipKind(this.FQName));
            }
        }

        internal override IEnumerable<EntityContainerRelationshipSetEnd> Ends
        {
            get
            {
                foreach (EntityContainerAssociationSetEnd iteratorVariable0 in this._relationshipEnds.Values)
                {
                    yield return iteratorVariable0;
                }
                foreach (EntityContainerAssociationSetEnd iteratorVariable1 in this._rolelessEnds)
                {
                    yield return iteratorVariable1;
                }
            }
        }

    }
}

