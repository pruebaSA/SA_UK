namespace System.Data.Mapping
{
    using System;
    using System.Data.Common;
    using System.Data.Metadata.Edm;

    internal abstract class BaseMetadataMappingVisitor
    {
        protected BaseMetadataMappingVisitor()
        {
        }

        protected virtual void Visit(DbProviderManifest providerManifest)
        {
        }

        protected virtual void Visit(StorageComplexPropertyMapping storageComplexPropertyMapping)
        {
            this.Visit(storageComplexPropertyMapping.EdmProperty);
            foreach (StorageComplexTypeMapping mapping in storageComplexPropertyMapping.TypeMappings)
            {
                this.Visit(mapping);
            }
        }

        protected virtual void Visit(StorageComplexTypeMapping storageComplexTypeMapping)
        {
            foreach (StoragePropertyMapping mapping in storageComplexTypeMapping.AllProperties)
            {
                this.Visit(mapping);
            }
            foreach (ComplexType type in storageComplexTypeMapping.IsOfTypes)
            {
                this.Visit(type);
            }
            foreach (ComplexType type2 in storageComplexTypeMapping.Types)
            {
                this.Visit(type2);
            }
        }

        protected virtual void Visit(StorageConditionPropertyMapping storageConditionPropertyMapping)
        {
            this.Visit(storageConditionPropertyMapping.ColumnProperty);
            this.Visit(storageConditionPropertyMapping.EdmProperty);
        }

        protected virtual void Visit(StorageEntityContainerMapping storageEntityContainerMapping)
        {
            this.Visit(storageEntityContainerMapping.EdmEntityContainer);
            this.Visit(storageEntityContainerMapping.StorageEntityContainer);
            foreach (StorageSetMapping mapping in storageEntityContainerMapping.EntitySetMaps)
            {
                this.Visit(mapping);
            }
        }

        protected virtual void Visit(StorageMappingFragment storageMappingFragment)
        {
            foreach (StoragePropertyMapping mapping in storageMappingFragment.AllProperties)
            {
                this.Visit(mapping);
            }
            this.Visit((EntitySetBase) storageMappingFragment.TableSet);
        }

        protected virtual void Visit(StoragePropertyMapping storagePropertyMapping)
        {
            if (storagePropertyMapping.GetType() == typeof(StorageComplexPropertyMapping))
            {
                this.Visit((StorageComplexPropertyMapping) storagePropertyMapping);
            }
            else if (storagePropertyMapping.GetType() == typeof(StorageConditionPropertyMapping))
            {
                this.Visit((StorageConditionPropertyMapping) storagePropertyMapping);
            }
            else if (storagePropertyMapping.GetType() == typeof(StorageScalarPropertyMapping))
            {
                this.Visit((StorageScalarPropertyMapping) storagePropertyMapping);
            }
        }

        protected virtual void Visit(StorageScalarPropertyMapping storageScalarPropertyMapping)
        {
            this.Visit(storageScalarPropertyMapping.ColumnProperty);
            this.Visit(storageScalarPropertyMapping.EdmProperty);
        }

        protected virtual void Visit(StorageSetMapping storageSetMapping)
        {
            foreach (StorageTypeMapping mapping in storageSetMapping.TypeMappings)
            {
                this.Visit(mapping);
            }
            this.Visit(storageSetMapping.EntityContainerMapping);
        }

        protected virtual void Visit(StorageTypeMapping storageTypeMapping)
        {
            foreach (EdmType type in storageTypeMapping.IsOfTypes)
            {
                this.Visit(type);
            }
            foreach (StorageMappingFragment fragment in storageTypeMapping.MappingFragments)
            {
                this.Visit(fragment);
            }
            this.Visit(storageTypeMapping.SetMapping);
            foreach (EdmType type2 in storageTypeMapping.Types)
            {
                this.Visit(type2);
            }
        }

        protected virtual void Visit(AssociationEndMember associationEndMember)
        {
            this.Visit(associationEndMember.TypeUsage);
        }

        protected virtual void Visit(AssociationSet associationSet)
        {
            this.Visit(associationSet.ElementType);
            this.Visit(associationSet.EntityContainer);
            foreach (AssociationSetEnd end in associationSet.AssociationSetEnds)
            {
                this.Visit(end);
            }
        }

        protected virtual void Visit(AssociationSetEnd associationSetEnd)
        {
            this.Visit(associationSetEnd.CorrespondingAssociationEndMember);
            this.Visit(associationSetEnd.EntitySet);
            this.Visit(associationSetEnd.ParentAssociationSet);
        }

        protected virtual void Visit(AssociationType associationType)
        {
            foreach (AssociationEndMember member in associationType.AssociationEndMembers)
            {
                this.Visit(member);
            }
            this.Visit(associationType.BaseType);
            foreach (EdmMember member2 in associationType.KeyMembers)
            {
                this.Visit(member2);
            }
            foreach (EdmMember member3 in associationType.GetDeclaredOnlyMembers<EdmMember>())
            {
                this.Visit(member3);
            }
            foreach (ReferentialConstraint constraint in associationType.ReferentialConstraints)
            {
                this.Visit(constraint);
            }
            foreach (RelationshipEndMember member4 in associationType.RelationshipEndMembers)
            {
                this.Visit(member4);
            }
        }

        protected virtual void Visit(CollectionType collectionType)
        {
            this.Visit(collectionType.BaseType);
            this.Visit(collectionType.TypeUsage);
        }

        protected virtual void Visit(ComplexType complexType)
        {
            this.Visit(complexType.BaseType);
            foreach (EdmMember member in complexType.Members)
            {
                this.Visit(member);
            }
            foreach (EdmProperty property in complexType.Properties)
            {
                this.Visit(property);
            }
        }

        protected virtual void Visit(EdmFunction edmFunction)
        {
            this.Visit(edmFunction.BaseType);
            this.Visit(edmFunction.EntitySet);
            foreach (FunctionParameter parameter in edmFunction.Parameters)
            {
                this.Visit(parameter);
            }
            this.Visit(edmFunction.ReturnParameter);
        }

        protected virtual void Visit(EdmMember edmMember)
        {
            this.Visit(edmMember.TypeUsage);
        }

        protected virtual void Visit(EdmProperty edmProperty)
        {
            this.Visit(edmProperty.TypeUsage);
        }

        protected virtual void Visit(EdmType edmType)
        {
            if (edmType != null)
            {
                switch (edmType.BuiltInTypeKind)
                {
                    case BuiltInTypeKind.EntityType:
                        this.Visit((EntityType) edmType);
                        return;

                    case BuiltInTypeKind.EnumType:
                        this.Visit((EnumType) edmType);
                        return;

                    case BuiltInTypeKind.EnumMember:
                    case BuiltInTypeKind.Facet:
                    case BuiltInTypeKind.CollectionKind:
                        return;

                    case BuiltInTypeKind.EdmFunction:
                        this.Visit((EdmFunction) edmType);
                        return;

                    case BuiltInTypeKind.PrimitiveType:
                        this.Visit((PrimitiveType) edmType);
                        return;

                    case BuiltInTypeKind.RefType:
                        this.Visit((RefType) edmType);
                        return;

                    case BuiltInTypeKind.CollectionType:
                        this.Visit((CollectionType) edmType);
                        return;

                    case BuiltInTypeKind.ComplexType:
                        this.Visit((ComplexType) edmType);
                        return;

                    case BuiltInTypeKind.AssociationType:
                        this.Visit((AssociationType) edmType);
                        return;
                }
            }
        }

        protected virtual void Visit(EntityContainer entityContainer)
        {
            foreach (EntitySetBase base2 in entityContainer.BaseEntitySets)
            {
                this.Visit(base2);
            }
        }

        protected virtual void Visit(EntitySet entitySet)
        {
            this.Visit(entitySet.ElementType);
            this.Visit(entitySet.EntityContainer);
        }

        protected virtual void Visit(EntitySetBase entitySetBase)
        {
            switch (entitySetBase.BuiltInTypeKind)
            {
                case BuiltInTypeKind.AssociationSet:
                    this.Visit((AssociationSet) entitySetBase);
                    break;

                case BuiltInTypeKind.EntitySet:
                    this.Visit((EntitySet) entitySetBase);
                    break;
            }
        }

        protected virtual void Visit(EntityType entityType)
        {
            foreach (EdmMember member in entityType.KeyMembers)
            {
                this.Visit(member);
            }
            foreach (EdmMember member2 in entityType.GetDeclaredOnlyMembers<EdmMember>())
            {
                this.Visit(member2);
            }
            foreach (NavigationProperty property in entityType.NavigationProperties)
            {
                this.Visit(property);
            }
            foreach (EdmProperty property2 in entityType.Properties)
            {
                this.Visit(property2);
            }
        }

        protected virtual void Visit(EntityTypeBase entityTypeBase)
        {
            if (entityTypeBase != null)
            {
                switch (entityTypeBase.BuiltInTypeKind)
                {
                    case BuiltInTypeKind.AssociationType:
                        this.Visit((AssociationType) entityTypeBase);
                        break;

                    case BuiltInTypeKind.EntityType:
                        this.Visit((EntityType) entityTypeBase);
                        break;
                }
            }
        }

        protected virtual void Visit(EnumMember enumMember)
        {
        }

        protected virtual void Visit(EnumType enumType)
        {
            foreach (EnumMember member in enumType.EnumMembers)
            {
                this.Visit(member);
            }
        }

        protected virtual void Visit(Facet facet)
        {
            this.Visit(facet.FacetType);
        }

        protected virtual void Visit(FunctionParameter functionParameter)
        {
            this.Visit(functionParameter.DeclaringFunction);
            this.Visit(functionParameter.TypeUsage);
        }

        protected virtual void Visit(NavigationProperty navigationProperty)
        {
            this.Visit(navigationProperty.FromEndMember);
            this.Visit(navigationProperty.RelationshipType);
            this.Visit(navigationProperty.ToEndMember);
            this.Visit(navigationProperty.TypeUsage);
        }

        protected virtual void Visit(PrimitiveType primitiveType)
        {
        }

        protected virtual void Visit(ReferentialConstraint referentialConstraint)
        {
            foreach (EdmProperty property in referentialConstraint.FromProperties)
            {
                this.Visit(property);
            }
            this.Visit(referentialConstraint.FromRole);
            foreach (EdmProperty property2 in referentialConstraint.ToProperties)
            {
                this.Visit(property2);
            }
            this.Visit(referentialConstraint.ToRole);
        }

        protected virtual void Visit(RefType refType)
        {
            this.Visit(refType.BaseType);
            this.Visit(refType.ElementType);
        }

        protected virtual void Visit(RelationshipEndMember relationshipEndMember)
        {
            this.Visit(relationshipEndMember.TypeUsage);
        }

        protected virtual void Visit(RelationshipType relationshipType)
        {
            if ((relationshipType != null) && (relationshipType.BuiltInTypeKind == BuiltInTypeKind.AssociationType))
            {
                this.Visit((AssociationType) relationshipType);
            }
        }

        protected virtual void Visit(TypeUsage typeUsage)
        {
            this.Visit(typeUsage.EdmType);
            foreach (Facet facet in typeUsage.Facets)
            {
                this.Visit(facet);
            }
        }
    }
}

