namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class ReferentialConstraintRoleElement : SchemaElement
    {
        private IRelationshipEnd _end;
        private List<PropertyRefElement> _roleProperties;

        public ReferentialConstraintRoleElement(System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint parentElement) : base(parentElement)
        {
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (SchemaElement.CanHandleAttribute(reader, "Role"))
            {
                this.HandleRoleAttribute(reader);
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
            if (base.CanHandleElement(reader, "PropertyRef"))
            {
                this.HandlePropertyRefElement(reader);
                return true;
            }
            return false;
        }

        private void HandlePropertyRefElement(XmlReader reader)
        {
            PropertyRefElement item = new PropertyRefElement(base.ParentElement);
            item.Parse(reader);
            this.RoleProperties.Add(item);
        }

        private void HandleRoleAttribute(XmlReader reader)
        {
            string str;
            Utils.GetString(base.Schema, reader, out str);
            this.Name = str;
        }

        internal override void ResolveTopLevelNames()
        {
            IRelationship parentElement = (IRelationship) base.ParentElement.ParentElement;
            if (!parentElement.TryGetEnd(this.Name, out this._end))
            {
                base.AddError(ErrorCode.InvalidRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidEndRoleInRelationshipConstraint(this.Name, parentElement.Name));
            }
            else
            {
                SchemaEntityType type = this._end.Type;
            }
        }

        internal override void Validate()
        {
            base.Validate();
            foreach (PropertyRefElement element in this._roleProperties)
            {
                if (!element.ResolveNames(this._end.Type))
                {
                    base.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidPropertyInRelationshipConstraint(element.Name, this.Name));
                }
            }
        }

        public IRelationshipEnd End =>
            this._end;

        public IList<PropertyRefElement> RoleProperties
        {
            get
            {
                if (this._roleProperties == null)
                {
                    this._roleProperties = new List<PropertyRefElement>();
                }
                return this._roleProperties;
            }
        }
    }
}

