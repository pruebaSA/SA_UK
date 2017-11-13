namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Linq;

    internal static class Converter
    {
        internal static readonly FacetDescription CollationFacet;
        internal static readonly FacetDescription ConcurrencyModeFacet;
        internal static readonly FacetDescription StoreGeneratedPatternFacet;

        static Converter()
        {
            EnumType facetType = new EnumType("ConcurrencyMode", "Edm", DataSpace.CSpace);
            foreach (string str in Enum.GetNames(typeof(ConcurrencyMode)))
            {
                facetType.AddMember(new EnumMember(str));
            }
            EnumType type2 = new EnumType("StoreGeneratedPattern", "Edm", DataSpace.CSpace);
            foreach (string str2 in Enum.GetNames(typeof(StoreGeneratedPattern)))
            {
                type2.AddMember(new EnumMember(str2));
            }
            ConcurrencyModeFacet = new FacetDescription("ConcurrencyMode", facetType, null, null, ConcurrencyMode.None);
            StoreGeneratedPatternFacet = new FacetDescription("StoreGeneratedPattern", type2, null, null, StoreGeneratedPattern.None);
            CollationFacet = new FacetDescription("Collation", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String), null, null, string.Empty);
        }

        private static void AddOtherContent(SchemaElement element, MetadataItem item)
        {
            if (element.OtherContent.Count > 0)
            {
                item.AddMetadataProperties(element.OtherContent);
            }
        }

        private static void ApplyPrimitiveTypePropertyFacets(TypeUsage sourceType, ref TypeUsage targetType)
        {
            Dictionary<string, Facet> dictionary = targetType.Facets.ToDictionary<Facet, string>(f => f.Name);
            bool flag = false;
            foreach (Facet facet in sourceType.Facets)
            {
                Facet facet2;
                if (dictionary.TryGetValue(facet.Name, out facet2))
                {
                    if (!facet2.Description.IsConstant)
                    {
                        flag = true;
                        dictionary[facet2.Name] = Facet.Create(facet2.Description, facet.Value);
                    }
                }
                else
                {
                    flag = true;
                    dictionary.Add(facet.Name, facet);
                }
            }
            if (flag)
            {
                targetType = TypeUsage.Create(targetType.EdmType, dictionary.Values);
            }
        }

        internal static IEnumerable<GlobalItem> ConvertSchema(IList<Schema> somSchemas, DbProviderManifest providerManifest, ItemCollection itemCollection)
        {
            Dictionary<SchemaElement, GlobalItem> newGlobalItems = new Dictionary<SchemaElement, GlobalItem>();
            ConversionCache convertedItemCache = new ConversionCache(itemCollection);
            foreach (Schema schema in somSchemas)
            {
                ConvertSchema(schema, providerManifest, convertedItemCache, newGlobalItems);
            }
            return newGlobalItems.Values;
        }

        internal static IEnumerable<GlobalItem> ConvertSchema(Schema somSchema, DbProviderManifest providerManifest, ItemCollection itemCollection)
        {
            Dictionary<SchemaElement, GlobalItem> newGlobalItems = new Dictionary<SchemaElement, GlobalItem>();
            ConvertSchema(somSchema, providerManifest, new ConversionCache(itemCollection), newGlobalItems);
            return newGlobalItems.Values;
        }

        private static void ConvertSchema(Schema somSchema, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            foreach (System.Data.EntityModel.SchemaObjectModel.SchemaType type in somSchema.SchemaTypes)
            {
                LoadSchemaElement(type, providerManifest, convertedItemCache, newGlobalItems);
            }
            foreach (SchemaEntityType type2 in somSchema.SchemaTypes.OfType<SchemaEntityType>())
            {
                LoadEntityTypePhase2(type2, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (convertedItemCache.ItemCollection.DataSpace == DataSpace.CSpace)
            {
                EdmItemCollection itemCollection = (EdmItemCollection) convertedItemCache.ItemCollection;
                itemCollection.EdmVersion = Math.Max(somSchema.EdmVersion, itemCollection.EdmVersion);
            }
        }

        private static AssociationSet ConvertToAssociationSet(EntityContainerRelationshipSet relationshipSet, DbProviderManifest providerManifest, ConversionCache convertedItemCache, System.Data.Metadata.Edm.EntityContainer container, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            AssociationType associationType = (AssociationType) LoadSchemaElement((System.Data.EntityModel.SchemaObjectModel.SchemaType) relationshipSet.Relationship, providerManifest, convertedItemCache, newGlobalItems);
            AssociationSet parentSet = new AssociationSet(relationshipSet.Name, associationType);
            foreach (EntityContainerRelationshipSetEnd end in relationshipSet.Ends)
            {
                EntityType type1 = (EntityType) LoadSchemaElement(end.EntitySet.EntityType, providerManifest, convertedItemCache, newGlobalItems);
                AssociationEndMember endMember = (AssociationEndMember) associationType.Members[end.Name];
                AssociationSetEnd item = new AssociationSetEnd(GetEntitySet(end.EntitySet, container), parentSet, endMember);
                AddOtherContent(end, item);
                parentSet.AddAssociationSetEnd(item);
                if (end.Documentation != null)
                {
                    item.Documentation = ConvertToDocumentation(end.Documentation);
                }
            }
            if (relationshipSet.Documentation != null)
            {
                parentSet.Documentation = ConvertToDocumentation(relationshipSet.Documentation);
            }
            AddOtherContent(relationshipSet, parentSet);
            return parentSet;
        }

        private static AssociationType ConvertToAssociationType(Relationship element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            AssociationType type = new AssociationType(element.Name, element.Namespace, GetDataSpace(providerManifest));
            newGlobalItems.Add(element, type);
            foreach (RelationshipEnd end in element.Ends)
            {
                EntityType endMemberType = (EntityType) LoadSchemaElement(end.Type, providerManifest, convertedItemCache, newGlobalItems);
                AssociationEndMember item = InitializeAssociationEndMember(type, end, endMemberType);
                AddOtherContent(end, item);
                foreach (OnOperation operation in end.Operations)
                {
                    if (operation.Operation != Operation.Delete)
                    {
                        continue;
                    }
                    OperationAction none = OperationAction.None;
                    switch (operation.Action)
                    {
                        case System.Data.EntityModel.SchemaObjectModel.Action.None:
                            none = OperationAction.None;
                            break;

                        case System.Data.EntityModel.SchemaObjectModel.Action.Cascade:
                            none = OperationAction.Cascade;
                            break;

                        default:
                            throw EntityUtil.OperationActionNotSupported();
                    }
                    item.DeleteBehavior = none;
                }
                if (end.Documentation != null)
                {
                    item.Documentation = ConvertToDocumentation(end.Documentation);
                }
            }
            for (int i = 0; i < element.Constraints.Count; i++)
            {
                System.Data.EntityModel.SchemaObjectModel.ReferentialConstraint constraint = element.Constraints[i];
                AssociationEndMember fromRole = (AssociationEndMember) type.Members[constraint.PrincipalRole.Name];
                AssociationEndMember toRole = (AssociationEndMember) type.Members[constraint.DependentRole.Name];
                EntityTypeBase elementType = ((RefType) fromRole.TypeUsage.EdmType).ElementType;
                EntityTypeBase entityType = ((RefType) toRole.TypeUsage.EdmType).ElementType;
                System.Data.Metadata.Edm.ReferentialConstraint referentialConstraint = new System.Data.Metadata.Edm.ReferentialConstraint(fromRole, toRole, GetProperties(elementType, constraint.PrincipalRole.RoleProperties), GetProperties(entityType, constraint.DependentRole.RoleProperties));
                if (constraint.Documentation != null)
                {
                    referentialConstraint.Documentation = ConvertToDocumentation(constraint.Documentation);
                }
                if (fromRole.Documentation != null)
                {
                    referentialConstraint.FromRole.Documentation = ConvertToDocumentation(constraint.PrincipalRole.Documentation);
                }
                if (toRole.Documentation != null)
                {
                    referentialConstraint.ToRole.Documentation = ConvertToDocumentation(constraint.DependentRole.Documentation);
                }
                type.AddReferentialConstraint(referentialConstraint);
                AddOtherContent(element.Constraints[i], referentialConstraint);
            }
            if (element.Documentation != null)
            {
                type.Documentation = ConvertToDocumentation(element.Documentation);
            }
            AddOtherContent(element, type);
            return type;
        }

        private static ComplexType ConvertToComplexType(SchemaComplexType element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            ComplexType type = new ComplexType(element.Name, element.Namespace, GetDataSpace(providerManifest));
            newGlobalItems.Add(element, type);
            foreach (StructuredProperty property in element.Properties)
            {
                type.AddMember(ConvertToProperty(property, providerManifest, convertedItemCache, newGlobalItems));
            }
            type.Abstract = element.IsAbstract;
            if (element.BaseType != null)
            {
                type.BaseType = (EdmType) LoadSchemaElement(element.BaseType, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (element.Documentation != null)
            {
                type.Documentation = ConvertToDocumentation(element.Documentation);
            }
            AddOtherContent(element, type);
            return type;
        }

        private static Documentation ConvertToDocumentation(DocumentationElement element) => 
            element.MetadataDocumentation;

        private static System.Data.Metadata.Edm.EntityContainer ConvertToEntityContainer(System.Data.EntityModel.SchemaObjectModel.EntityContainer element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            System.Data.Metadata.Edm.EntityContainer container = new System.Data.Metadata.Edm.EntityContainer(element.Name, GetDataSpace(providerManifest));
            newGlobalItems.Add(element, container);
            foreach (EntityContainerEntitySet set in element.EntitySets)
            {
                container.AddEntitySetBase(ConvertToEntitySet(set, container.Name, providerManifest, convertedItemCache, newGlobalItems));
            }
            foreach (EntityContainerRelationshipSet set2 in element.RelationshipSets)
            {
                container.AddEntitySetBase(ConvertToAssociationSet(set2, providerManifest, convertedItemCache, container, newGlobalItems));
            }
            foreach (System.Data.EntityModel.SchemaObjectModel.Function function in element.FunctionImports)
            {
                container.AddFunctionImport(ConvertToFunction(function, providerManifest, convertedItemCache, container, newGlobalItems));
            }
            if (element.Documentation != null)
            {
                container.Documentation = ConvertToDocumentation(element.Documentation);
            }
            AddOtherContent(element, container);
            return container;
        }

        private static EntitySet ConvertToEntitySet(EntityContainerEntitySet set, string containerName, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            EntitySet item = new EntitySet(set.Name, set.DbSchema, set.Table, set.DefiningQuery, (EntityType) LoadSchemaElement(set.EntityType, providerManifest, convertedItemCache, newGlobalItems));
            if (set.Documentation != null)
            {
                item.Documentation = ConvertToDocumentation(set.Documentation);
            }
            AddOtherContent(set, item);
            return item;
        }

        private static EntityType ConvertToEntityType(SchemaEntityType element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            string[] keyMemberNames = null;
            if (element.DeclaredKeyProperties.Count != 0)
            {
                keyMemberNames = new string[element.DeclaredKeyProperties.Count];
                for (int i = 0; i < keyMemberNames.Length; i++)
                {
                    keyMemberNames[i] = element.DeclaredKeyProperties[i].Property.Name;
                }
            }
            EdmProperty[] members = new EdmProperty[element.Properties.Count];
            int num2 = 0;
            foreach (StructuredProperty property in element.Properties)
            {
                members[num2++] = ConvertToProperty(property, providerManifest, convertedItemCache, newGlobalItems);
            }
            EntityType item = new EntityType(element.Name, element.Namespace, GetDataSpace(providerManifest), keyMemberNames, members);
            if (element.BaseType != null)
            {
                item.BaseType = (EdmType) LoadSchemaElement(element.BaseType, providerManifest, convertedItemCache, newGlobalItems);
            }
            item.Abstract = element.IsAbstract;
            if (element.Documentation != null)
            {
                item.Documentation = ConvertToDocumentation(element.Documentation);
            }
            AddOtherContent(element, item);
            newGlobalItems.Add(element, item);
            return item;
        }

        private static EdmFunction ConvertToFunction(System.Data.EntityModel.SchemaObjectModel.Function somFunction, DbProviderManifest providerManifest, ConversionCache convertedItemCache, System.Data.Metadata.Edm.EntityContainer entityContainer, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            GlobalItem item = null;
            if (!somFunction.IsFunctionImport && newGlobalItems.TryGetValue(somFunction, out item))
            {
                return (EdmFunction) item;
            }
            FunctionParameter parameter = null;
            bool areConvertingForProviderManifest = somFunction.Schema.DataModel == SchemaDataModelOption.ProviderManifestModel;
            TypeUsage typeUsage = GetFunctionTypeUsage(somFunction.ReturnType, providerManifest, areConvertingForProviderManifest, somFunction.Type, somFunction.CollectionKind, somFunction, convertedItemCache, newGlobalItems);
            if (typeUsage != null)
            {
                parameter = new FunctionParameter("ReturnType", typeUsage, ParameterMode.ReturnValue);
            }
            EntitySet entitySet = null;
            if (somFunction.EntitySet != null)
            {
                entitySet = GetEntitySet(somFunction.EntitySet, entityContainer);
            }
            List<FunctionParameter> list = new List<FunctionParameter>();
            foreach (Parameter parameter2 in somFunction.Parameters)
            {
                TypeUsage usage2 = GetFunctionTypeUsage(parameter2, providerManifest, areConvertingForProviderManifest, parameter2.Type, parameter2.CollectionKind, parameter2, convertedItemCache, newGlobalItems);
                FunctionParameter parameter3 = new FunctionParameter(parameter2.Name, usage2, GetParameterMode(parameter2.ParameterDirection));
                AddOtherContent(parameter2, parameter3);
                if (parameter2.Documentation != null)
                {
                    parameter3.Documentation = ConvertToDocumentation(parameter2.Documentation);
                }
                list.Add(parameter3);
            }
            EdmFunctionPayload payload = new EdmFunctionPayload {
                Schema = somFunction.DbSchema,
                StoreFunctionName = somFunction.StoreFunctionName,
                CommandText = somFunction.CommandText,
                EntitySet = entitySet,
                IsAggregate = new bool?(somFunction.IsAggregate),
                IsBuiltIn = new bool?(somFunction.IsBuiltIn),
                IsNiladic = new bool?(somFunction.IsNiladicFunction),
                IsComposable = new bool?(somFunction.IsComposable),
                IsFromProviderManifest = new bool?(areConvertingForProviderManifest),
                ReturnParameter = parameter,
                Parameters = list.ToArray(),
                ParameterTypeSemantics = new ParameterTypeSemantics?(somFunction.ParameterTypeSemantics)
            };
            EdmFunction function = new EdmFunction(somFunction.Name, somFunction.Schema.Namespace, GetDataSpace(providerManifest), payload);
            if (!somFunction.IsFunctionImport)
            {
                newGlobalItems.Add(somFunction, function);
            }
            if (somFunction.Documentation != null)
            {
                function.Documentation = ConvertToDocumentation(somFunction.Documentation);
            }
            AddOtherContent(somFunction, function);
            return function;
        }

        private static System.Data.Metadata.Edm.NavigationProperty ConvertToNavigationProperty(EntityType declaringEntityType, System.Data.EntityModel.SchemaObjectModel.NavigationProperty somNavigationProperty, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            TypeUsage usage;
            EntityType endMemberType = (EntityType) LoadSchemaElement(somNavigationProperty.Type, providerManifest, convertedItemCache, newGlobalItems);
            EdmType edmType = endMemberType;
            AssociationType associationType = (AssociationType) LoadSchemaElement((Relationship) somNavigationProperty.Relationship, providerManifest, convertedItemCache, newGlobalItems);
            IRelationshipEnd end = null;
            somNavigationProperty.Relationship.TryGetEnd(somNavigationProperty.ToEnd.Name, out end);
            if (end.Multiplicity == RelationshipMultiplicity.Many)
            {
                edmType = endMemberType.GetCollectionType();
            }
            else
            {
                edmType = endMemberType;
            }
            if (end.Multiplicity == RelationshipMultiplicity.One)
            {
                FacetValues values = new FacetValues {
                    Nullable = 0
                };
                usage = TypeUsage.Create(edmType, values);
            }
            else
            {
                usage = TypeUsage.Create(edmType);
            }
            InitializeAssociationEndMember(associationType, somNavigationProperty.ToEnd, endMemberType);
            InitializeAssociationEndMember(associationType, somNavigationProperty.FromEnd, declaringEntityType);
            System.Data.Metadata.Edm.NavigationProperty item = new System.Data.Metadata.Edm.NavigationProperty(somNavigationProperty.Name, usage) {
                RelationshipType = associationType,
                ToEndMember = (RelationshipEndMember) associationType.Members[somNavigationProperty.ToEnd.Name],
                FromEndMember = (RelationshipEndMember) associationType.Members[somNavigationProperty.FromEnd.Name]
            };
            if (somNavigationProperty.Documentation != null)
            {
                item.Documentation = ConvertToDocumentation(somNavigationProperty.Documentation);
            }
            AddOtherContent(somNavigationProperty, item);
            return item;
        }

        private static EdmProperty ConvertToProperty(StructuredProperty somProperty, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            TypeUsage typeUsage = null;
            if (somProperty.Type is ScalarType)
            {
                if (somProperty.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
                {
                    typeUsage = GetCsdlPrimitiveTypeUsageWithFacets(somProperty, convertedItemCache);
                }
                else
                {
                    typeUsage = somProperty.TypeUsage;
                    UpdateSentinelValuesInFacets(ref typeUsage);
                }
            }
            else
            {
                EdmType elementType = (EdmType) LoadSchemaElement(somProperty.Type, providerManifest, convertedItemCache, newGlobalItems);
                if (somProperty.CollectionKind != CollectionKind.None)
                {
                    elementType = new CollectionType(elementType);
                }
                typeUsage = TypeUsage.Create(elementType);
            }
            PopulateGeneralFacets(somProperty, providerManifest, ref typeUsage);
            EdmProperty item = new EdmProperty(somProperty.Name, typeUsage);
            if (somProperty.Documentation != null)
            {
                item.Documentation = ConvertToDocumentation(somProperty.Documentation);
            }
            AddOtherContent(somProperty, item);
            return item;
        }

        private static TypeUsage GetCsdlPrimitiveTypeUsageWithFacets(StructuredProperty somProperty, ConversionCache convertedItemCache)
        {
            EdmType item = convertedItemCache.ItemCollection.GetItem<PrimitiveType>(somProperty.TypeUsage.EdmType.FullName);
            TypeUsage targetType = null;
            if (somProperty.CollectionKind != CollectionKind.None)
            {
                item = new CollectionType(item);
                return TypeUsage.Create(item);
            }
            targetType = TypeUsage.Create(item);
            ApplyPrimitiveTypePropertyFacets(somProperty.TypeUsage, ref targetType);
            return targetType;
        }

        private static DataSpace GetDataSpace(DbProviderManifest providerManifest)
        {
            if (providerManifest is EdmProviderManifest)
            {
                return DataSpace.CSpace;
            }
            return DataSpace.SSpace;
        }

        private static EntitySet GetEntitySet(EntityContainerEntitySet set, System.Data.Metadata.Edm.EntityContainer container) => 
            container.GetEntitySetByName(set.Name, false);

        private static TypeUsage GetFunctionTypeUsage(FacetEnabledSchemaElement somParameter, DbProviderManifest providerManifest, bool areConvertingForProviderManifest, System.Data.EntityModel.SchemaObjectModel.SchemaType type, CollectionKind collectionKind, SchemaElement schemaElement, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            EdmType ssdlPrimitiveType;
            if ((somParameter != null) && somParameter.HasUserDefinedFacets)
            {
                return somParameter.TypeUsage;
            }
            if (type == null)
            {
                return null;
            }
            if (!areConvertingForProviderManifest)
            {
                ScalarType scalarType = type as ScalarType;
                if (scalarType != null)
                {
                    ssdlPrimitiveType = GetSsdlPrimitiveType(scalarType, providerManifest);
                }
                else
                {
                    ssdlPrimitiveType = (EdmType) LoadSchemaElement(type, providerManifest, convertedItemCache, newGlobalItems);
                }
            }
            else if (type is TypeElement)
            {
                TypeElement element = type as TypeElement;
                ssdlPrimitiveType = element.PrimitiveType;
            }
            else
            {
                ScalarType type4 = type as ScalarType;
                ssdlPrimitiveType = type4.Type;
            }
            if (collectionKind != CollectionKind.None)
            {
                return convertedItemCache.GetCollectionTypeUsageWithNullFacets(ssdlPrimitiveType);
            }
            return convertedItemCache.GetTypeUsageWithNullFacets(ssdlPrimitiveType);
        }

        private static ParameterMode GetParameterMode(ParameterDirection parameterDirection)
        {
            switch (parameterDirection)
            {
                case ParameterDirection.Input:
                    return ParameterMode.In;

                case ParameterDirection.Output:
                    return ParameterMode.Out;

                case ParameterDirection.InputOutput:
                    return ParameterMode.InOut;
            }
            throw EntityUtil.MetadataGeneralError();
        }

        private static EdmProperty[] GetProperties(EntityTypeBase entityType, IList<PropertyRefElement> properties)
        {
            EdmProperty[] propertyArray = new EdmProperty[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                propertyArray[i] = (EdmProperty) entityType.Members[properties[i].Name];
            }
            return propertyArray;
        }

        private static PrimitiveType GetSsdlPrimitiveType(ScalarType scalarType, DbProviderManifest providerManifest)
        {
            string name = scalarType.Name;
            foreach (PrimitiveType type2 in providerManifest.GetStoreTypes())
            {
                if (type2.Name == name)
                {
                    return type2;
                }
            }
            return null;
        }

        private static AssociationEndMember InitializeAssociationEndMember(AssociationType associationType, IRelationshipEnd end, EntityType endMemberType)
        {
            AssociationEndMember member;
            EdmMember member2;
            if (!associationType.Members.TryGetValue(end.Name, false, out member2))
            {
                member = new AssociationEndMember(end.Name, endMemberType.GetReferenceType(), end.Multiplicity);
                associationType.AddKeyMember(member);
            }
            else
            {
                member = (AssociationEndMember) member2;
            }
            RelationshipEnd end2 = end as RelationshipEnd;
            if ((end2 != null) && (end2.Documentation != null))
            {
                member.Documentation = ConvertToDocumentation(end2.Documentation);
            }
            return member;
        }

        private static void LoadEntityTypePhase2(SchemaEntityType element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            EntityType declaringEntityType = (EntityType) newGlobalItems[element];
            foreach (System.Data.EntityModel.SchemaObjectModel.NavigationProperty property in element.NavigationProperties)
            {
                declaringEntityType.AddMember(ConvertToNavigationProperty(declaringEntityType, property, providerManifest, convertedItemCache, newGlobalItems));
            }
        }

        private static MetadataItem LoadSchemaElement(System.Data.EntityModel.SchemaObjectModel.SchemaType element, DbProviderManifest providerManifest, ConversionCache convertedItemCache, Dictionary<SchemaElement, GlobalItem> newGlobalItems)
        {
            GlobalItem item;
            if (newGlobalItems.TryGetValue(element, out item))
            {
                return item;
            }
            System.Data.EntityModel.SchemaObjectModel.EntityContainer container = element as System.Data.EntityModel.SchemaObjectModel.EntityContainer;
            if (container != null)
            {
                return ConvertToEntityContainer(container, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (element is SchemaEntityType)
            {
                return ConvertToEntityType((SchemaEntityType) element, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (element is Relationship)
            {
                return ConvertToAssociationType((Relationship) element, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (element is SchemaComplexType)
            {
                return ConvertToComplexType((SchemaComplexType) element, providerManifest, convertedItemCache, newGlobalItems);
            }
            if (element is System.Data.EntityModel.SchemaObjectModel.Function)
            {
                return ConvertToFunction((System.Data.EntityModel.SchemaObjectModel.Function) element, providerManifest, convertedItemCache, null, newGlobalItems);
            }
            return null;
        }

        private static void PopulateGeneralFacets(StructuredProperty somProperty, DbProviderManifest providerManifest, ref TypeUsage propertyTypeUsage)
        {
            bool flag = false;
            Dictionary<string, Facet> dictionary = propertyTypeUsage.Facets.ToDictionary<Facet, string>(f => f.Name);
            if (!somProperty.Nullable)
            {
                dictionary["Nullable"] = Facet.Create(MetadataItem.NullableFacetDescription, false);
                flag = true;
            }
            if (somProperty.Default != null)
            {
                dictionary["DefaultValue"] = Facet.Create(MetadataItem.DefaultValueFacetDescription, somProperty.DefaultAsObject);
                flag = true;
            }
            if (somProperty.Schema.EdmVersion == 1.1)
            {
                Facet facet = Facet.Create(MetadataItem.CollectionKindFacetDescription, somProperty.CollectionKind);
                dictionary.Add(facet.Name, facet);
                flag = true;
            }
            if (flag)
            {
                propertyTypeUsage = TypeUsage.Create(propertyTypeUsage.EdmType, dictionary.Values);
            }
        }

        private static void UpdateSentinelValuesInFacets(ref TypeUsage typeUsage)
        {
            PrimitiveType edmType = (PrimitiveType) typeUsage.EdmType;
            if ((edmType.PrimitiveTypeKind == PrimitiveTypeKind.String) || (edmType.PrimitiveTypeKind == PrimitiveTypeKind.Binary))
            {
                Facet facet = typeUsage.Facets["MaxLength"];
                if (Helper.IsUnboundedFacetValue(facet))
                {
                    FacetValues facetValues = new FacetValues {
                        MaxLength = Helper.GetFacet(edmType.FacetDescriptions, "MaxLength").MaxValue
                    };
                    typeUsage = typeUsage.ShallowCopy(facetValues);
                }
            }
        }

        private class ConversionCache
        {
            private readonly Dictionary<EdmType, TypeUsage> _nullFacetsCollectionTypeUsage;
            private readonly Dictionary<EdmType, TypeUsage> _nullFacetsTypeUsage;
            internal readonly System.Data.Metadata.Edm.ItemCollection ItemCollection;

            internal ConversionCache(System.Data.Metadata.Edm.ItemCollection itemCollection)
            {
                this.ItemCollection = itemCollection;
                this._nullFacetsTypeUsage = new Dictionary<EdmType, TypeUsage>();
                this._nullFacetsCollectionTypeUsage = new Dictionary<EdmType, TypeUsage>();
            }

            internal TypeUsage GetCollectionTypeUsageWithNullFacets(EdmType edmType)
            {
                TypeUsage usage;
                if (!this._nullFacetsCollectionTypeUsage.TryGetValue(edmType, out usage))
                {
                    usage = TypeUsage.Create(new CollectionType(this.GetTypeUsageWithNullFacets(edmType)), FacetValues.NullFacetValues);
                    this._nullFacetsCollectionTypeUsage.Add(edmType, usage);
                }
                return usage;
            }

            internal TypeUsage GetTypeUsageWithNullFacets(EdmType edmType)
            {
                TypeUsage usage;
                if (!this._nullFacetsTypeUsage.TryGetValue(edmType, out usage))
                {
                    usage = TypeUsage.Create(edmType, FacetValues.NullFacetValues);
                    this._nullFacetsTypeUsage.Add(edmType, usage);
                }
                return usage;
            }
        }
    }
}

