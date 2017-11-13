namespace System.Data
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class EDesignRes
    {
        internal const string CannotChangePropertyReturnType = "CannotChangePropertyReturnType";
        internal const string CannotChangePropertyReturnTypeToNull = "CannotChangePropertyReturnTypeToNull";
        internal const string CtorSummaryComment = "CtorSummaryComment";
        internal const string DuplicateClassName = "DuplicateClassName";
        internal const string EntitySetExistsWithDifferentCase = "EntitySetExistsWithDifferentCase";
        internal const string EntityTypeAndSetAccessibilityConflict = "EntityTypeAndSetAccessibilityConflict";
        internal const string ExpectingComplexTypeForMember = "ExpectingComplexTypeForMember";
        internal const string FactoryMethodSummaryComment = "FactoryMethodSummaryComment";
        internal const string FactoryParamCommentGeneral = "FactoryParamCommentGeneral";
        internal const string GeneratedFactoryMethodNameConflict = "GeneratedFactoryMethodNameConflict";
        internal const string GeneratedPropertyAccessibilityConflict = "GeneratedPropertyAccessibilityConflict";
        internal const string InvalidAttributeSuppliedForProperty = "InvalidAttributeSuppliedForProperty";
        internal const string InvalidAttributeSuppliedForType = "InvalidAttributeSuppliedForType";
        internal const string InvalidGetStatementSuppliedForProperty = "InvalidGetStatementSuppliedForProperty";
        internal const string InvalidInterfaceSuppliedForType = "InvalidInterfaceSuppliedForType";
        internal const string InvalidMemberSuppliedForType = "InvalidMemberSuppliedForType";
        internal const string InvalidMetadataDataServiceVersion = "InvalidMetadataDataServiceVersion";
        internal const string InvalidMetadataMultipleNamespaces = "InvalidMetadataMultipleNamespaces";
        internal const string InvalidSetStatementSuppliedForProperty = "InvalidSetStatementSuppliedForProperty";
        internal const string InvalidStringArgument = "InvalidStringArgument";
        private static EDesignRes loader;
        internal const string MissingComplexTypeDocumentation = "MissingComplexTypeDocumentation";
        internal const string MissingDocumentation = "MissingDocumentation";
        internal const string MissingDocumentationNoName = "MissingDocumentationNoName";
        internal const string MissingPropertyDocumentation = "MissingPropertyDocumentation";
        internal const string NamespaceComments = "NamespaceComments";
        internal const string ObjectContext_InvalidAttributeForNonSyndicationItemsMember = "ObjectContext_InvalidAttributeForNonSyndicationItemsMember";
        internal const string ObjectContext_InvalidAttributeForNonSyndicationItemsType = "ObjectContext_InvalidAttributeForNonSyndicationItemsType";
        internal const string ObjectContext_InvalidValueForEpmPropertyMember = "ObjectContext_InvalidValueForEpmPropertyMember";
        internal const string ObjectContext_InvalidValueForEpmPropertyType = "ObjectContext_InvalidValueForEpmPropertyType";
        internal const string ObjectContext_InvalidValueForTargetTextContentKindPropertyMember = "ObjectContext_InvalidValueForTargetTextContentKindPropertyMember";
        internal const string ObjectContext_InvalidValueForTargetTextContentKindPropertyType = "ObjectContext_InvalidValueForTargetTextContentKindPropertyType";
        internal const string ObjectContext_MissingExtendedAttributeType = "ObjectContext_MissingExtendedAttributeType";
        internal const string ObjectContext_MultipleValuesForSameExtendedAttributeMember = "ObjectContext_MultipleValuesForSameExtendedAttributeMember";
        internal const string ObjectContext_MultipleValuesForSameExtendedAttributeType = "ObjectContext_MultipleValuesForSameExtendedAttributeType";
        internal const string ObjectContext_OpenTypePropertyValueIsNotCorrect = "ObjectContext_OpenTypePropertyValueIsNotCorrect";
        internal const string ObjectContext_SyndicationMappingForComplexPropertiesNotAllowed = "ObjectContext_SyndicationMappingForComplexPropertiesNotAllowed";
        internal const string ObjectContext_UnknownPropertyNameInEpmAttributesMember = "ObjectContext_UnknownPropertyNameInEpmAttributesMember";
        internal const string ObjectContext_UnknownPropertyNameInEpmAttributesType = "ObjectContext_UnknownPropertyNameInEpmAttributesType";
        internal const string PropertyExistsWithDifferentCase = "PropertyExistsWithDifferentCase";
        private ResourceManager resources;
        internal const string TypeMapperDescription = "TypeMapperDescription";
        internal const string VersionV1RequiresUseDataServiceCollectionFalse = "VersionV1RequiresUseDataServiceCollectionFalse";

        internal EDesignRes()
        {
            this.resources = new ResourceManager("System.Data.Services.Design", base.GetType().Assembly);
        }

        private static EDesignRes GetLoader()
        {
            if (loader == null)
            {
                EDesignRes res = new EDesignRes();
                Interlocked.CompareExchange<EDesignRes>(ref loader, res, null);
            }
            return loader;
        }

        public static string GetString(string name)
        {
            EDesignRes loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            EDesignRes loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture =>
            null;
    }
}

