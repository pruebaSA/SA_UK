namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Common;
    using System.Data.Metadata.Edm;

    internal static class TypeUtils
    {
        internal static TypeUsage CreateCollectionType(TypeUsage elementType) => 
            TypeHelpers.CreateCollectionTypeUsage(elementType);

        internal static bool IsCollectionType(TypeUsage type) => 
            TypeSemantics.IsCollectionType(type);

        internal static bool IsStructuredType(TypeUsage type) => 
            (((TypeSemantics.IsReferenceType(type) || TypeSemantics.IsRowType(type)) || (TypeSemantics.IsEntityType(type) || TypeSemantics.IsRelationshipType(type))) || (TypeSemantics.IsComplexType(type) && !IsUdt(type)));

        internal static bool IsUdt(EdmType type) => 
            false;

        internal static bool IsUdt(TypeUsage type) => 
            IsUdt(type.EdmType);
    }
}

