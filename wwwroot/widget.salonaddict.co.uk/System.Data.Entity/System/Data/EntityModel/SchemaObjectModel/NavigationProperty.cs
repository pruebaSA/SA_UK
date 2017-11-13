namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Xml;

    [DebuggerDisplay("Name={Name}, Relationship={_unresolvedRelationshipName}, FromRole={_unresolvedFromEndRole}, ToRole={_unresolvedToEndRole}")]
    internal sealed class NavigationProperty : Property
    {
        private IRelationshipEnd _fromEnd;
        private IRelationship _relationship;
        private IRelationshipEnd _toEnd;
        private string _unresolvedFromEndRole;
        private string _unresolvedRelationshipName;
        private string _unresolvedToEndRole;

        public NavigationProperty(SchemaEntityType parent) : base(parent)
        {
        }

        private void HandleAssociationAttribute(XmlReader reader)
        {
            if (this._unresolvedRelationshipName != null)
            {
                base.AddAlreadyDefinedError(reader);
            }
            else
            {
                string str;
                if (Utils.GetDottedName(base.Schema, reader, out str))
                {
                    this._unresolvedRelationshipName = str;
                }
            }
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Relationship"))
            {
                this.HandleAssociationAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "FromRole"))
            {
                this.HandleFromRoleAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "ToRole"))
            {
                this.HandleToRoleAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleFromRoleAttribute(XmlReader reader)
        {
            this._unresolvedFromEndRole = base.HandleUndottedNameAttribute(reader, this._unresolvedFromEndRole);
        }

        private void HandleToRoleAttribute(XmlReader reader)
        {
            this._unresolvedToEndRole = base.HandleUndottedNameAttribute(reader, this._unresolvedToEndRole);
        }

        internal override void ResolveTopLevelNames()
        {
            SchemaType type;
            base.ResolveTopLevelNames();
            if (base.Schema.ResolveTypeName(this, this._unresolvedRelationshipName, out type))
            {
                this._relationship = type as IRelationship;
                if (this._relationship == null)
                {
                    base.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, Strings.BadNavigationPropertyRelationshipNotRelationship(this._unresolvedRelationshipName));
                }
                else
                {
                    bool flag = true;
                    if (!this._relationship.TryGetEnd(this._unresolvedFromEndRole, out this._fromEnd))
                    {
                        base.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, Strings.BadNavigationPropertyUndefinedRole(this._unresolvedFromEndRole, this._relationship.FQName));
                        flag = false;
                    }
                    if (!this._relationship.TryGetEnd(this._unresolvedToEndRole, out this._toEnd))
                    {
                        base.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, Strings.BadNavigationPropertyUndefinedRole(this._unresolvedToEndRole, this._relationship.FQName));
                        flag = false;
                    }
                    if (flag && (this._fromEnd == this._toEnd))
                    {
                        base.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, Strings.BadNavigationPropertyRolesCannotBeTheSame);
                    }
                }
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if (this._fromEnd.Type != this.ParentElement)
            {
                base.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, Strings.BadNavigationPropertyBadFromRoleType(this.Name, this._fromEnd.Type.FQName, this._fromEnd.Name, this._relationship.FQName, this.ParentElement.FQName));
            }
            SchemaEntityType type = this._toEnd.Type;
        }

        internal IRelationshipEnd FromEnd =>
            this._fromEnd;

        public SchemaEntityType ParentElement =>
            (base.ParentElement as SchemaEntityType);

        internal IRelationship Relationship =>
            this._relationship;

        internal IRelationshipEnd ToEnd =>
            this._toEnd;

        public override SchemaType Type
        {
            get
            {
                if ((this._toEnd != null) && (this._toEnd.Type != null))
                {
                    return this._toEnd.Type;
                }
                return null;
            }
        }
    }
}

