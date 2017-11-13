namespace System.Data.Services.Design
{
    using System;
    using System.Data;

    internal static class Strings
    {
        internal static string CannotChangePropertyReturnType(object p0, object p1) => 
            EDesignRes.GetString("CannotChangePropertyReturnType", new object[] { p0, p1 });

        internal static string CannotChangePropertyReturnTypeToNull(object p0, object p1) => 
            EDesignRes.GetString("CannotChangePropertyReturnTypeToNull", new object[] { p0, p1 });

        internal static string CtorSummaryComment(object p0) => 
            EDesignRes.GetString("CtorSummaryComment", new object[] { p0 });

        internal static string DuplicateClassName(object p0, object p1, object p2) => 
            EDesignRes.GetString("DuplicateClassName", new object[] { p0, p1, p2 });

        internal static string EntitySetExistsWithDifferentCase(object p0) => 
            EDesignRes.GetString("EntitySetExistsWithDifferentCase", new object[] { p0 });

        internal static string EntityTypeAndSetAccessibilityConflict(object p0, object p1, object p2, object p3) => 
            EDesignRes.GetString("EntityTypeAndSetAccessibilityConflict", new object[] { p0, p1, p2, p3 });

        internal static string ExpectingComplexTypeForMember(object p0, object p1) => 
            EDesignRes.GetString("ExpectingComplexTypeForMember", new object[] { p0, p1 });

        internal static string FactoryMethodSummaryComment(object p0) => 
            EDesignRes.GetString("FactoryMethodSummaryComment", new object[] { p0 });

        internal static string FactoryParamCommentGeneral(object p0) => 
            EDesignRes.GetString("FactoryParamCommentGeneral", new object[] { p0 });

        internal static string GeneratedFactoryMethodNameConflict(object p0, object p1) => 
            EDesignRes.GetString("GeneratedFactoryMethodNameConflict", new object[] { p0, p1 });

        internal static string GeneratedPropertyAccessibilityConflict(object p0, object p1, object p2) => 
            EDesignRes.GetString("GeneratedPropertyAccessibilityConflict", new object[] { p0, p1, p2 });

        internal static string InvalidAttributeSuppliedForProperty(object p0) => 
            EDesignRes.GetString("InvalidAttributeSuppliedForProperty", new object[] { p0 });

        internal static string InvalidAttributeSuppliedForType(object p0) => 
            EDesignRes.GetString("InvalidAttributeSuppliedForType", new object[] { p0 });

        internal static string InvalidGetStatementSuppliedForProperty(object p0) => 
            EDesignRes.GetString("InvalidGetStatementSuppliedForProperty", new object[] { p0 });

        internal static string InvalidInterfaceSuppliedForType(object p0) => 
            EDesignRes.GetString("InvalidInterfaceSuppliedForType", new object[] { p0 });

        internal static string InvalidMemberSuppliedForType(object p0) => 
            EDesignRes.GetString("InvalidMemberSuppliedForType", new object[] { p0 });

        internal static string InvalidMetadataDataServiceVersion(object p0) => 
            EDesignRes.GetString("InvalidMetadataDataServiceVersion", new object[] { p0 });

        internal static string InvalidMetadataMultipleNamespaces(object p0, object p1) => 
            EDesignRes.GetString("InvalidMetadataMultipleNamespaces", new object[] { p0, p1 });

        internal static string InvalidSetStatementSuppliedForProperty(object p0) => 
            EDesignRes.GetString("InvalidSetStatementSuppliedForProperty", new object[] { p0 });

        internal static string InvalidStringArgument(object p0) => 
            EDesignRes.GetString("InvalidStringArgument", new object[] { p0 });

        internal static string MissingComplexTypeDocumentation(object p0) => 
            EDesignRes.GetString("MissingComplexTypeDocumentation", new object[] { p0 });

        internal static string MissingDocumentation(object p0) => 
            EDesignRes.GetString("MissingDocumentation", new object[] { p0 });

        internal static string MissingPropertyDocumentation(object p0) => 
            EDesignRes.GetString("MissingPropertyDocumentation", new object[] { p0 });

        internal static string NamespaceComments(object p0, object p1) => 
            EDesignRes.GetString("NamespaceComments", new object[] { p0, p1 });

        internal static string ObjectContext_InvalidAttributeForNonSyndicationItemsMember(object p0, object p1, object p2) => 
            EDesignRes.GetString("ObjectContext_InvalidAttributeForNonSyndicationItemsMember", new object[] { p0, p1, p2 });

        internal static string ObjectContext_InvalidAttributeForNonSyndicationItemsType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_InvalidAttributeForNonSyndicationItemsType", new object[] { p0, p1 });

        internal static string ObjectContext_InvalidValueForEpmPropertyMember(object p0, object p1, object p2) => 
            EDesignRes.GetString("ObjectContext_InvalidValueForEpmPropertyMember", new object[] { p0, p1, p2 });

        internal static string ObjectContext_InvalidValueForEpmPropertyType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_InvalidValueForEpmPropertyType", new object[] { p0, p1 });

        internal static string ObjectContext_InvalidValueForTargetTextContentKindPropertyMember(object p0, object p1, object p2) => 
            EDesignRes.GetString("ObjectContext_InvalidValueForTargetTextContentKindPropertyMember", new object[] { p0, p1, p2 });

        internal static string ObjectContext_InvalidValueForTargetTextContentKindPropertyType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_InvalidValueForTargetTextContentKindPropertyType", new object[] { p0, p1 });

        internal static string ObjectContext_MissingExtendedAttributeType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_MissingExtendedAttributeType", new object[] { p0, p1 });

        internal static string ObjectContext_MultipleValuesForSameExtendedAttributeMember(object p0, object p1, object p2) => 
            EDesignRes.GetString("ObjectContext_MultipleValuesForSameExtendedAttributeMember", new object[] { p0, p1, p2 });

        internal static string ObjectContext_MultipleValuesForSameExtendedAttributeType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_MultipleValuesForSameExtendedAttributeType", new object[] { p0, p1 });

        internal static string ObjectContext_OpenTypePropertyValueIsNotCorrect(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_OpenTypePropertyValueIsNotCorrect", new object[] { p0, p1 });

        internal static string ObjectContext_UnknownPropertyNameInEpmAttributesMember(object p0, object p1, object p2) => 
            EDesignRes.GetString("ObjectContext_UnknownPropertyNameInEpmAttributesMember", new object[] { p0, p1, p2 });

        internal static string ObjectContext_UnknownPropertyNameInEpmAttributesType(object p0, object p1) => 
            EDesignRes.GetString("ObjectContext_UnknownPropertyNameInEpmAttributesType", new object[] { p0, p1 });

        internal static string PropertyExistsWithDifferentCase(object p0) => 
            EDesignRes.GetString("PropertyExistsWithDifferentCase", new object[] { p0 });

        internal static string MissingDocumentationNoName =>
            EDesignRes.GetString("MissingDocumentationNoName");

        internal static string ObjectContext_SyndicationMappingForComplexPropertiesNotAllowed =>
            EDesignRes.GetString("ObjectContext_SyndicationMappingForComplexPropertiesNotAllowed");

        internal static string TypeMapperDescription =>
            EDesignRes.GetString("TypeMapperDescription");

        internal static string VersionV1RequiresUseDataServiceCollectionFalse =>
            EDesignRes.GetString("VersionV1RequiresUseDataServiceCollectionFalse");
    }
}

