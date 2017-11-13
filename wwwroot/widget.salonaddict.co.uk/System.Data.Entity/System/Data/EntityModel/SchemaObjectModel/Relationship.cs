namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal sealed class Relationship : SchemaType, IRelationship
    {
        private List<System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint> _constraints;
        private RelationshipEndCollection _ends;
        private System.Data.Objects.DataClasses.RelationshipKind _relationshipKind;

        public Relationship(Schema parent, System.Data.Objects.DataClasses.RelationshipKind kind) : base(parent)
        {
            this.RelationshipKind = kind;
        }

        private void HandleConstraintElement(XmlReader reader)
        {
            System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint item = new System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint(this);
            item.Parse(reader);
            this.Constraints.Add(item);
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
            if (base.CanHandleElement(reader, "ReferentialConstraint"))
            {
                this.HandleConstraintElement(reader);
                return true;
            }
            return false;
        }

        private void HandleEndElement(XmlReader reader)
        {
            RelationshipEnd item = new RelationshipEnd(this);
            item.Parse(reader);
            if (this.Ends.Count == 2)
            {
                base.AddError(ErrorCode.InvalidAssociation, EdmSchemaErrorSeverity.Error, Strings.TooManyAssociationEnds(this.FQName));
            }
            else
            {
                this.Ends.Add(item);
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            foreach (RelationshipEnd end in this.Ends)
            {
                end.ResolveTopLevelNames();
            }
            foreach (System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint constraint in this.Constraints)
            {
                constraint.ResolveTopLevelNames();
            }
        }

        public bool TryGetEnd(string roleName, out IRelationshipEnd end) => 
            this._ends.TryGetEnd(roleName, out end);

        internal override void Validate()
        {
            base.Validate();
            bool flag = false;
            foreach (RelationshipEnd end in this.Ends)
            {
                end.Validate();
                if ((this.RelationshipKind == System.Data.Objects.DataClasses.RelationshipKind.Association) && (end.Operations.Count > 0))
                {
                    if (flag)
                    {
                        end.AddError(ErrorCode.InvalidOperation, EdmSchemaErrorSeverity.Error, Strings.InvalidOperationMultipleEndsInAssociation);
                    }
                    flag = true;
                }
            }
            if ((this.Constraints.Count == 0) && (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel))
            {
                base.AddError(ErrorCode.MissingConstraintOnRelationshipType, EdmSchemaErrorSeverity.Error, Strings.MissingConstraintOnRelationshipType(this.FQName));
            }
            else
            {
                foreach (System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint constraint in this.Constraints)
                {
                    constraint.Validate();
                }
            }
        }

        public IList<System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint> Constraints
        {
            get
            {
                if (this._constraints == null)
                {
                    this._constraints = new List<System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint>();
                }
                return this._constraints;
            }
        }

        public IList<IRelationshipEnd> Ends
        {
            get
            {
                if (this._ends == null)
                {
                    this._ends = new RelationshipEndCollection();
                }
                return this._ends;
            }
        }

        public System.Data.Objects.DataClasses.RelationshipKind RelationshipKind
        {
            get => 
                this._relationshipKind;
            private set
            {
                this._relationshipKind = value;
            }
        }
    }
}

