namespace System.Data.Metadata.Edm
{
    using System;

    internal static class EdmConstants
    {
        internal const string Abstract = "Abstract";
        internal const string AssociationEnd = "AssociationEnd";
        internal const string AssociationSet = "AssociationSet";
        internal const string AssociationSetEnds = "AssociationSetEnds";
        internal const string AssociationSetEndType = "AssociationSetEndType";
        internal const string AssociationSetType = "AssocationSetType";
        internal const string AssociationType = "AssociationType";
        internal const string BagCollectionKind = "Bag";
        internal const string BaseEntitySetType = "BaseEntitySetType";
        internal const string BaseType = "BaseType";
        internal const string Binary = "Binary";
        internal const string Boolean = "Boolean";
        internal const string BuiltInAttribute = "BuiltInAttribute";
        internal const string Byte = "Byte";
        internal const string CanonicalFunctionNamespace = "Edm";
        internal const string Cascade = "Cascade";
        internal const string Child = "Child";
        internal const string ClrPrimitiveTypeNamespace = "System";
        internal const string CollectionKind = "CollectionKind";
        internal const string CollectionType = "CollectionType";
        internal const string CommandTextFunctionAttribyte = "CommandText";
        internal const string ComplexType = "ComplexType";
        internal const string DateTime = "DateTime";
        internal const string DateTimeOffset = "DateTimeOffset";
        internal const string Decimal = "Decimal";
        internal const string DefaultValue = "DefaultValue";
        internal const string DeleteAction = "DeleteAction";
        internal const string DeleteBehavior = "DeleteBehavior";
        internal const string Documentation = "Documentation";
        internal const string Double = "Double";
        internal const string EdmNamespace = "Edm";
        internal const string EdmType = "EdmType";
        internal const string ElementType = "ElementType";
        internal const string Ends = "Ends";
        internal const string EntityContainerType = "EntityContainerType";
        internal const string EntitySet = "EntitySet";
        internal const string EntitySets = "EntitySets";
        internal const string EntitySetType = "EntitySetType";
        internal const string EntityType = "EntityType";
        internal const string EnumerationMember = "EnumMember";
        internal const string EnumerationType = "EnumType";
        internal const string EnumMembers = "EnumMembers";
        internal const string Facet = "Facet";
        internal const string Facets = "Facets";
        internal const string FacetType = "FacetType";
        internal const string FromProperties = "FromProperties";
        internal const string FromRole = "FromRole";
        internal const string Function = "EdmFunction";
        internal const string FunctionParameter = "FunctionParameter";
        internal const string GlobalItem = "GlobalItem";
        internal const string Guid = "Guid";
        internal const string In = "In";
        internal const string InOut = "InOut";
        internal const string Int16 = "Int16";
        internal const string Int32 = "Int32";
        internal const string Int64 = "Int64";
        internal const string IsComposableFunctionAttribute = "IsComposable";
        internal const string IsParent = "IsParent";
        internal const string ItemAttribute = "MetadataProperty";
        internal const string ItemAttributes = "MetadataProperties";
        internal const string ItemType = "ItemType";
        internal const string KeyMembers = "KeyMembers";
        internal const string ListCollectionKind = "List";
        internal const string Local = "Local";
        internal const string LongDescription = "LongDescription";
        internal const string Many = "Many";
        internal const int MaxLength = 0x100;
        internal const string MaxMaxLength = "Max";
        internal const string Member = "EdmMember";
        internal const string Members = "Members";
        internal const string Mode = "Mode";
        internal const string Money = "Money";
        internal const string Name = "Name";
        internal const string Namespace = "Namespace";
        internal const string NavigationProperty = "NavigationProperty";
        internal const string NiladicFunctionAttribute = "NiladicFunctionAttribute";
        internal const string None = "None";
        internal const string NoneCollectionKind = "None";
        internal const string Null = "Null";
        internal const string Nullable = "Nullable";
        internal const int NumBuiltInTypes = 40;
        internal const int NumPrimitiveTypes = 15;
        internal const string One = "One";
        internal const string OperationBehavior = "OperationBehavior";
        internal const string OperationBehaviors = "OperationBehaviors";
        internal const string Out = "Out";
        internal const string ParameterMode = "ParameterMode";
        internal const string Parameters = "Parameters";
        internal const string ParameterTypeSemantics = "ParameterTypeSemantics";
        internal const string ParameterTypeSemanticsAttribute = "ParameterTypeSemanticsAttribute";
        internal const string Parent = "Parent";
        internal const string PrimitiveType = "PrimitiveType";
        internal const string PrimitiveTypeKind = "PrimitiveTypeKind";
        internal const string Properties = "Properties";
        internal const string Property = "EdmProperty";
        internal const string ProviderManifest = "ProviderManifest";
        internal const string ReferentialConstraint = "ReferentialConstraint";
        internal const string ReferentialConstraints = "ReferentialConstraints";
        internal const string RefType = "RefType";
        internal const string RelationshipEnd = "RelationshipEnd";
        internal const string RelationshipMultiplicity = "RelationshipMultiplicity";
        internal const string RelationshipSet = "RelationshipSet";
        internal const string RelationshipType = "RelationshipType";
        internal const string RelationshipTypeName = "RelationshipTypeName";
        internal const string Restrict = "Restrict";
        internal const string ReturnParameter = "ReturnParameter";
        internal const string ReturnType = "ReturnType";
        internal const string Role = "Role";
        internal const string RowType = "RowType";
        internal const string SByte = "SByte";
        internal const string Schema = "Schema";
        internal const string Sealed = "Sealed";
        internal const string SimpleType = "SimpleType";
        internal const string Single = "Single";
        internal const string StoreFunctionNameAttribute = "StoreFunctionNameAttribute";
        internal const string StoreFunctionNamespace = "StoreFunctionNamespace";
        internal const string String = "String";
        internal const string StructuralType = "StructuralType";
        internal const string Summary = "Summary";
        internal const string Table = "Table";
        internal const string Time = "Time";
        internal const string ToEndMemberName = "ToEndMemberName";
        internal const string ToProperties = "ToProperties";
        internal const string ToRole = "ToRole";
        internal const string TransientNamespace = "Transient";
        internal const string Type = "Type";
        internal const string TypeUsage = "TypeUsage";
        internal const string UInt16 = "UInt16";
        internal const string UInt32 = "UInt32";
        internal const string UInt64 = "UInt64";
        internal static readonly Unbounded UnboundedValue = Unbounded.Instance;
        internal const string Unspecified = "Unspecified";
        internal const string Utc = "Utc";
        internal const string Value = "Value";
        internal const string WebHomeSymbol = "~";
        internal const string Xml = "Xml";
        internal const string ZeroToOne = "ZeroToOne";

        internal class Unbounded
        {
            private static readonly EdmConstants.Unbounded _instance = new EdmConstants.Unbounded();

            private Unbounded()
            {
            }

            public override string ToString() => 
                "Max";

            internal static EdmConstants.Unbounded Instance =>
                _instance;
        }
    }
}

