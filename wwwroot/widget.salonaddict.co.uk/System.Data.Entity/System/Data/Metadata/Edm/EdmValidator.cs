namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Objects.DataClasses;

    internal class EdmValidator
    {
        private bool _skipReadOnlyItems;

        private void AddError(List<EdmItemError> errors, EdmItemError newError)
        {
            ValidationErrorEventArgs e = new ValidationErrorEventArgs(newError);
            this.OnValidationError(e);
            errors.Add(e.ValidationError);
        }

        protected virtual IEnumerable<EdmItemError> CustomValidate(MetadataItem item) => 
            null;

        private void InternalValidate(MetadataItem item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            if ((!item.IsReadOnly || !this.SkipReadOnlyItems) && !validatedItems.Contains(item))
            {
                validatedItems.Add(item);
                if (string.IsNullOrEmpty(item.Identity))
                {
                    this.AddError(errors, new EdmItemError(Strings.Validator_EmptyIdentity, item));
                }
                switch (item.BuiltInTypeKind)
                {
                    case BuiltInTypeKind.CollectionType:
                        this.ValidateCollectionType((CollectionType) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.ComplexType:
                        this.ValidateComplexType((ComplexType) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.EntityType:
                        this.ValidateEntityType((EntityType) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.Facet:
                        this.ValidateFacet((Facet) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.MetadataProperty:
                        this.ValidateMetadataProperty((MetadataProperty) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.NavigationProperty:
                        this.ValidateNavigationProperty((NavigationProperty) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.PrimitiveType:
                        this.ValidatePrimitiveType((PrimitiveType) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.EdmProperty:
                        this.ValidateEdmProperty((EdmProperty) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.RefType:
                        this.ValidateRefType((RefType) item, errors, validatedItems);
                        break;

                    case BuiltInTypeKind.TypeUsage:
                        this.ValidateTypeUsage((TypeUsage) item, errors, validatedItems);
                        break;
                }
                IEnumerable<EdmItemError> collection = this.CustomValidate(item);
                if (collection != null)
                {
                    errors.AddRange(collection);
                }
            }
        }

        protected virtual void OnValidationError(ValidationErrorEventArgs e)
        {
        }

        public void Validate<T>(IEnumerable<T> items, List<EdmItemError> ospaceErrors) where T: EdmType
        {
            EntityUtil.CheckArgumentNull<IEnumerable<T>>(items, "items");
            EntityUtil.CheckArgumentNull<IEnumerable<T>>(items, "ospaceErrors");
            HashSet<MetadataItem> validatedItems = new HashSet<MetadataItem>();
            foreach (MetadataItem item in items)
            {
                this.InternalValidate(item, ospaceErrors, validatedItems);
            }
        }

        private void ValidateCollectionType(CollectionType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateEdmType(item, errors, validatedItems);
            if (item.BaseType != null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_CollectionTypesCannotHaveBaseType, item));
            }
            if (item.TypeUsage == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_CollectionHasNoTypeUsage, item));
            }
            else
            {
                this.InternalValidate(item.TypeUsage, errors, validatedItems);
            }
        }

        private void ValidateComplexType(ComplexType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateStructuralType(item, errors, validatedItems);
        }

        private void ValidateEdmMember(EdmMember item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateItem(item, errors, validatedItems);
            if (string.IsNullOrEmpty(item.Name))
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNoName, item));
            }
            if (item.DeclaringType == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNullDeclaringType, item));
            }
            else
            {
                this.InternalValidate(item.DeclaringType, errors, validatedItems);
            }
            if (item.TypeUsage == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNullTypeUsage, item));
            }
            else
            {
                this.InternalValidate(item.TypeUsage, errors, validatedItems);
            }
        }

        private void ValidateEdmProperty(EdmProperty item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateEdmMember(item, errors, validatedItems);
        }

        private void ValidateEdmType(EdmType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateItem(item, errors, validatedItems);
            if (string.IsNullOrEmpty(item.Name))
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_TypeHasNoName, item));
            }
            if ((item.NamespaceName == null) || ((item.DataSpace != DataSpace.OSpace) && (string.Empty == item.NamespaceName)))
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_TypeHasNoNamespace, item));
            }
            if (item.BaseType != null)
            {
                this.InternalValidate(item.BaseType, errors, validatedItems);
            }
        }

        private void ValidateEntityType(EntityType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            if (item.BaseType == null)
            {
                if (item.KeyMembers.Count < 1)
                {
                    this.AddError(errors, new EdmItemError(Strings.Validator_NoKeyMembers, item));
                }
                else
                {
                    foreach (EdmProperty property in item.KeyMembers)
                    {
                        if (property.Nullable)
                        {
                            this.AddError(errors, new EdmItemError(Strings.Validator_NullableEntityKeyProperty(property.Name, item.FullName), property));
                        }
                    }
                }
            }
            else
            {
                this.ValidateStructuralType(item, errors, validatedItems);
            }
        }

        private void ValidateFacet(Facet item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateItem(item, errors, validatedItems);
            if (string.IsNullOrEmpty(item.Name))
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_FacetHasNoName, item));
            }
            if (item.FacetType == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_FacetTypeIsNull, item));
            }
            else
            {
                this.InternalValidate(item.FacetType, errors, validatedItems);
            }
        }

        private void ValidateItem(MetadataItem item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            if (item.RawMetadataProperties != null)
            {
                foreach (MetadataProperty property in item.MetadataProperties)
                {
                    this.InternalValidate(property, errors, validatedItems);
                }
            }
        }

        private void ValidateMetadataProperty(MetadataProperty item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            if (item.PropertyKind == PropertyKind.Extended)
            {
                this.ValidateItem(item, errors, validatedItems);
                if (string.IsNullOrEmpty(item.Name))
                {
                    this.AddError(errors, new EdmItemError(Strings.Validator_MetadataPropertyHasNoName, item));
                }
                if (item.TypeUsage == null)
                {
                    this.AddError(errors, new EdmItemError(Strings.Validator_ItemAttributeHasNullTypeUsage, item));
                }
                else
                {
                    this.InternalValidate(item.TypeUsage, errors, validatedItems);
                }
            }
        }

        private void ValidateNavigationProperty(NavigationProperty item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            if (!typeof(IEntityWithRelationships).IsAssignableFrom(item.DeclaringType.ClrType))
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_NavPropWithoutIEntityWithRelationships(item.Name, item.DeclaringType.FullName), item));
            }
            this.ValidateEdmMember(item, errors, validatedItems);
        }

        private void ValidatePrimitiveType(PrimitiveType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateSimpleType(item, errors, validatedItems);
        }

        private void ValidateRefType(RefType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateEdmType(item, errors, validatedItems);
            if (item.BaseType != null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_RefTypesCannotHaveBaseType, item));
            }
            if (item.ElementType == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_RefTypeHasNullEntityType, null));
            }
            else
            {
                this.InternalValidate(item.ElementType, errors, validatedItems);
            }
        }

        private void ValidateSimpleType(System.Data.Metadata.Edm.SimpleType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateEdmType(item, errors, validatedItems);
        }

        private void ValidateStructuralType(StructuralType item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateEdmType(item, errors, validatedItems);
            Dictionary<string, EdmMember> dictionary = new Dictionary<string, EdmMember>();
            foreach (EdmMember member in item.Members)
            {
                EdmMember member2 = null;
                if (dictionary.TryGetValue(member.Name, out member2))
                {
                    this.AddError(errors, new EdmItemError(Strings.Validator_BaseTypeHasMemberOfSameName, item));
                }
                else
                {
                    dictionary.Add(member.Name, member);
                }
                this.InternalValidate(member, errors, validatedItems);
            }
        }

        private void ValidateTypeUsage(TypeUsage item, List<EdmItemError> errors, HashSet<MetadataItem> validatedItems)
        {
            this.ValidateItem(item, errors, validatedItems);
            if (item.EdmType == null)
            {
                this.AddError(errors, new EdmItemError(Strings.Validator_TypeUsageHasNullEdmType, item));
            }
            else
            {
                this.InternalValidate(item.EdmType, errors, validatedItems);
            }
            foreach (Facet facet in item.Facets)
            {
                this.InternalValidate(facet, errors, validatedItems);
            }
        }

        internal bool SkipReadOnlyItems
        {
            get => 
                this._skipReadOnlyItems;
            set
            {
                this._skipReadOnlyItems = value;
            }
        }
    }
}

