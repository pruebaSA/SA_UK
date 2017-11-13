namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal sealed class ReferentialConstraint : SchemaElement
    {
        private ReferentialConstraintRoleElement _dependentRole;
        private ReferentialConstraintRoleElement _principalRole;
        private const char KEY_DELIMITER = ' ';

        public ReferentialConstraint(Relationship relationship) : base(relationship)
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
            if (base.CanHandleElement(reader, "Principal"))
            {
                this.HandleReferentialConstraintPrincipalRoleElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "Dependent"))
            {
                this.HandleReferentialConstraintDependentRoleElement(reader);
                return true;
            }
            return false;
        }

        internal void HandleReferentialConstraintDependentRoleElement(XmlReader reader)
        {
            this._dependentRole = new ReferentialConstraintRoleElement(this);
            this._dependentRole.Parse(reader);
        }

        internal void HandleReferentialConstraintPrincipalRoleElement(XmlReader reader)
        {
            this._principalRole = new ReferentialConstraintRoleElement(this);
            this._principalRole.Parse(reader);
        }

        private static void IsKeyProperty(ReferentialConstraintRoleElement roleElement, SchemaEntityType itemType, out bool isKeyProperty, out bool isNullable, out bool isSubsetOfKeyProperties)
        {
            isKeyProperty = true;
            isNullable = true;
            isSubsetOfKeyProperties = true;
            if (itemType.KeyProperties.Count != roleElement.RoleProperties.Count)
            {
                isKeyProperty = false;
            }
            for (int i = 0; i < roleElement.RoleProperties.Count; i++)
            {
                if (isSubsetOfKeyProperties)
                {
                    bool flag = false;
                    for (int j = 0; j < itemType.KeyProperties.Count; j++)
                    {
                        if (itemType.KeyProperties[j].Property == roleElement.RoleProperties[i].Property)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        isKeyProperty = false;
                        isSubsetOfKeyProperties = false;
                    }
                }
                isNullable &= roleElement.RoleProperties[i].Property.Nullable;
            }
        }

        private static bool ReadyForFurtherValidation(ReferentialConstraintRoleElement role)
        {
            if (role == null)
            {
                return false;
            }
            if (role.End == null)
            {
                return false;
            }
            if (role.RoleProperties.Count == 0)
            {
                return false;
            }
            foreach (PropertyRefElement element in role.RoleProperties)
            {
                if (element.Property == null)
                {
                    return false;
                }
            }
            return true;
        }

        internal override void ResolveTopLevelNames()
        {
            this._dependentRole.ResolveTopLevelNames();
            this._principalRole.ResolveTopLevelNames();
        }

        internal override void Validate()
        {
            base.Validate();
            this._principalRole.Validate();
            this._dependentRole.Validate();
            if (ReadyForFurtherValidation(this._principalRole) && ReadyForFurtherValidation(this._dependentRole))
            {
                bool flag;
                bool flag2;
                bool flag3;
                bool flag4;
                bool flag5;
                bool flag6;
                IRelationshipEnd end = this._principalRole.End;
                IRelationshipEnd end2 = this._dependentRole.End;
                if (this._principalRole.Name == this._dependentRole.Name)
                {
                    base.AddError(ErrorCode.SameRoleReferredInReferentialConstraint, EdmSchemaErrorSeverity.Error, Strings.SameRoleReferredInReferentialConstraint(this.ParentElement.Name));
                }
                IsKeyProperty(this._dependentRole, end2.Type, out flag, out flag4, out flag5);
                IsKeyProperty(this._principalRole, end.Type, out flag2, out flag3, out flag6);
                if (!flag2)
                {
                    base.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidFromPropertyInRelationshipConstraint(this.PrincipalRole.Name, end.Type.FQName, this.ParentElement.FQName));
                }
                else
                {
                    if (end.Multiplicity == RelationshipMultiplicity.Many)
                    {
                        base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityFromRoleUpperBoundMustBeOne(this._principalRole.Name, this.ParentElement.Name));
                    }
                    else if (flag4 && (end.Multiplicity == RelationshipMultiplicity.One))
                    {
                        base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityFromRoleToPropertyNullable(this._principalRole.Name, this.ParentElement.Name));
                    }
                    else if (!flag4 && (end.Multiplicity != RelationshipMultiplicity.One))
                    {
                        base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityFromRoleToPropertyNonNullable(this._principalRole.Name, this.ParentElement.Name));
                    }
                    if ((end2.Multiplicity == RelationshipMultiplicity.One) && (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel))
                    {
                        base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityToRoleLowerBoundMustBeZero(this._dependentRole.Name, this.ParentElement.Name));
                    }
                    if (!flag5 && (base.Schema.DataModel == SchemaDataModelOption.EntityDataModel))
                    {
                        base.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidToPropertyInRelationshipConstraint(this.DependentRole.Name, end2.Type.FQName, this.ParentElement.FQName));
                    }
                    if (flag)
                    {
                        if (end2.Multiplicity == RelationshipMultiplicity.Many)
                        {
                            base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityToRoleUpperBoundMustBeOne(end2.Name, this.ParentElement.Name));
                        }
                    }
                    else if (end2.Multiplicity != RelationshipMultiplicity.Many)
                    {
                        base.AddError(ErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.InvalidMultiplicityToRoleUpperBoundMustBeMany(end2.Name, this.ParentElement.Name));
                    }
                    if (this._dependentRole.RoleProperties.Count != this._principalRole.RoleProperties.Count)
                    {
                        base.AddError(ErrorCode.MismatchNumberOfPropertiesInRelationshipConstraint, EdmSchemaErrorSeverity.Error, Strings.MismatchNumberOfPropertiesinRelationshipConstraint);
                    }
                    else
                    {
                        for (int i = 0; i < this._dependentRole.RoleProperties.Count; i++)
                        {
                            if (this._dependentRole.RoleProperties[i].Property.Type != this._principalRole.RoleProperties[i].Property.Type)
                            {
                                base.AddError(ErrorCode.TypeMismatchRelationshipConstaint, EdmSchemaErrorSeverity.Error, Strings.TypeMismatchRelationshipConstaint(this._dependentRole.RoleProperties[i].Name, this._principalRole.RoleProperties[i].Name));
                            }
                        }
                    }
                }
            }
        }

        internal ReferentialConstraintRoleElement DependentRole =>
            this._dependentRole;

        internal IRelationship ParentElement =>
            ((IRelationship) base.ParentElement);

        internal ReferentialConstraintRoleElement PrincipalRole =>
            this._principalRole;
    }
}

