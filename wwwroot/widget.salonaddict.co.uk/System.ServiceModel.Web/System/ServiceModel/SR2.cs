namespace System.ServiceModel
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), DebuggerNonUserCode]
    internal sealed class SR2
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal SR2()
        {
        }

        internal static string GetString(string name, params object[] args) => 
            GetString(resourceCulture, name, args);

        internal static string GetString(CultureInfo culture, string name, params object[] args)
        {
            if ((args != null) && (args.Length > 0))
            {
                return string.Format(culture, name, args);
            }
            return name;
        }

        internal static string AtMostOneRequestBodyParameterAllowedForStream =>
            ResourceManager.GetString("AtMostOneRequestBodyParameterAllowedForStream", resourceCulture);

        internal static string AtMostOneRequestBodyParameterAllowedForUnwrappedMessages =>
            ResourceManager.GetString("AtMostOneRequestBodyParameterAllowedForUnwrappedMessages", resourceCulture);

        internal static string Atom10SpecRequiresTextConstruct =>
            ResourceManager.GetString("Atom10SpecRequiresTextConstruct", resourceCulture);

        internal static string BindUriTemplateToNullOrEmptyPathParam =>
            ResourceManager.GetString("BindUriTemplateToNullOrEmptyPathParam", resourceCulture);

        internal static string BodyStyleNotSupportedByWebScript =>
            ResourceManager.GetString("BodyStyleNotSupportedByWebScript", resourceCulture);

        internal static string CannotDeserializeBody =>
            ResourceManager.GetString("CannotDeserializeBody", resourceCulture);

        internal static string CannotMoveToAttribute1 =>
            ResourceManager.GetString("CannotMoveToAttribute1", resourceCulture);

        internal static string CannotMoveToAttribute2 =>
            ResourceManager.GetString("CannotMoveToAttribute2", resourceCulture);

        internal static string CannotSerializeType =>
            ResourceManager.GetString("CannotSerializeType", resourceCulture);

        internal static string ChangingFullTypeNameNotSupported =>
            ResourceManager.GetString("ChangingFullTypeNameNotSupported", resourceCulture);

        internal static string ChannelDispatcherMustBePresent =>
            ResourceManager.GetString("ChannelDispatcherMustBePresent", resourceCulture);

        internal static string CollectionAssignedToIncompatibleInterface =>
            ResourceManager.GetString("CollectionAssignedToIncompatibleInterface", resourceCulture);

        internal static string ConfigInvalidBindingConfigurationName =>
            ResourceManager.GetString("ConfigInvalidBindingConfigurationName", resourceCulture);

        internal static string ConfigInvalidWebContentTypeMapper =>
            ResourceManager.GetString("ConfigInvalidWebContentTypeMapper", resourceCulture);

        internal static string ConfigWebContentTypeMapperNoConstructor =>
            ResourceManager.GetString("ConfigWebContentTypeMapperNoConstructor", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => 
                resourceCulture;
            set
            {
                resourceCulture = value;
            }
        }

        internal static string DefaultContentFormatNotAllowedInProperty =>
            ResourceManager.GetString("DefaultContentFormatNotAllowedInProperty", resourceCulture);

        internal static string DocumentFormatterDoesNotHaveDocument =>
            ResourceManager.GetString("DocumentFormatterDoesNotHaveDocument", resourceCulture);

        internal static string EndElementWithoutStartElement =>
            ResourceManager.GetString("EndElementWithoutStartElement", resourceCulture);

        internal static string EndpointAddressCannotBeNull =>
            ResourceManager.GetString("EndpointAddressCannotBeNull", resourceCulture);

        internal static string ErrorEncounteredInContentTypeMapper =>
            ResourceManager.GetString("ErrorEncounteredInContentTypeMapper", resourceCulture);

        internal static string ErrorInLine =>
            ResourceManager.GetString("ErrorInLine", resourceCulture);

        internal static string ErrorParsingDateTime =>
            ResourceManager.GetString("ErrorParsingDateTime", resourceCulture);

        internal static string ErrorParsingDocument =>
            ResourceManager.GetString("ErrorParsingDocument", resourceCulture);

        internal static string ErrorParsingFeed =>
            ResourceManager.GetString("ErrorParsingFeed", resourceCulture);

        internal static string ErrorParsingItem =>
            ResourceManager.GetString("ErrorParsingItem", resourceCulture);

        internal static string ExtensionNameNotSpecified =>
            ResourceManager.GetString("ExtensionNameNotSpecified", resourceCulture);

        internal static string FeedAuthorsIgnoredOnWrite =>
            ResourceManager.GetString("FeedAuthorsIgnoredOnWrite", resourceCulture);

        internal static string FeedContributorsIgnoredOnWrite =>
            ResourceManager.GetString("FeedContributorsIgnoredOnWrite", resourceCulture);

        internal static string FeedCreatedNullCategory =>
            ResourceManager.GetString("FeedCreatedNullCategory", resourceCulture);

        internal static string FeedCreatedNullItem =>
            ResourceManager.GetString("FeedCreatedNullItem", resourceCulture);

        internal static string FeedCreatedNullPerson =>
            ResourceManager.GetString("FeedCreatedNullPerson", resourceCulture);

        internal static string FeedFormatterDoesNotHaveFeed =>
            ResourceManager.GetString("FeedFormatterDoesNotHaveFeed", resourceCulture);

        internal static string FeedHasNonContiguousItems =>
            ResourceManager.GetString("FeedHasNonContiguousItems", resourceCulture);

        internal static string FeedIdIgnoredOnWrite =>
            ResourceManager.GetString("FeedIdIgnoredOnWrite", resourceCulture);

        internal static string FeedLinksIgnoredOnWrite =>
            ResourceManager.GetString("FeedLinksIgnoredOnWrite", resourceCulture);

        internal static string FormatterCannotBeUsedForReplyMessages =>
            ResourceManager.GetString("FormatterCannotBeUsedForReplyMessages", resourceCulture);

        internal static string FormatterCannotBeUsedForRequestMessages =>
            ResourceManager.GetString("FormatterCannotBeUsedForRequestMessages", resourceCulture);

        internal static string GETCannotHaveBody =>
            ResourceManager.GetString("GETCannotHaveBody", resourceCulture);

        internal static string GETCannotHaveMCParameter =>
            ResourceManager.GetString("GETCannotHaveMCParameter", resourceCulture);

        internal static string HelpPageLayout =>
            ResourceManager.GetString("HelpPageLayout", resourceCulture);

        internal static string HelpPageMethodNotAllowedText =>
            ResourceManager.GetString("HelpPageMethodNotAllowedText", resourceCulture);

        internal static string HelpPageText =>
            ResourceManager.GetString("HelpPageText", resourceCulture);

        internal static string HelpPageTitleText =>
            ResourceManager.GetString("HelpPageTitleText", resourceCulture);

        internal static string HttpContextNoIncomingMessageProperty =>
            ResourceManager.GetString("HttpContextNoIncomingMessageProperty", resourceCulture);

        internal static string HttpTransferServiceHostBadAuthSchemes =>
            ResourceManager.GetString("HttpTransferServiceHostBadAuthSchemes", resourceCulture);

        internal static string HttpTransferServiceHostMultipleContracts =>
            ResourceManager.GetString("HttpTransferServiceHostMultipleContracts", resourceCulture);

        internal static string HttpUnhandledOperationInvokerCalledWithoutMessage =>
            ResourceManager.GetString("HttpUnhandledOperationInvokerCalledWithoutMessage", resourceCulture);

        internal static string InvalidCharacterEncountered =>
            ResourceManager.GetString("InvalidCharacterEncountered", resourceCulture);

        internal static string InvalidHttpMessageFormat =>
            ResourceManager.GetString("InvalidHttpMessageFormat", resourceCulture);

        internal static string InvalidHttpMessageFormat3 =>
            ResourceManager.GetString("InvalidHttpMessageFormat3", resourceCulture);

        internal static string InvalidMessageContractWithoutWrapperName =>
            ResourceManager.GetString("InvalidMessageContractWithoutWrapperName", resourceCulture);

        internal static string InvalidMethodWithSOAPHeaders =>
            ResourceManager.GetString("InvalidMethodWithSOAPHeaders", resourceCulture);

        internal static string InvalidObjectTypePassed =>
            ResourceManager.GetString("InvalidObjectTypePassed", resourceCulture);

        internal static string InvalidXmlCharactersInNameUsedWithPOSTMethod =>
            ResourceManager.GetString("InvalidXmlCharactersInNameUsedWithPOSTMethod", resourceCulture);

        internal static string ItemAuthorsIgnoredOnWrite =>
            ResourceManager.GetString("ItemAuthorsIgnoredOnWrite", resourceCulture);

        internal static string ItemContentIgnoredOnWrite =>
            ResourceManager.GetString("ItemContentIgnoredOnWrite", resourceCulture);

        internal static string ItemContributorsIgnoredOnWrite =>
            ResourceManager.GetString("ItemContributorsIgnoredOnWrite", resourceCulture);

        internal static string ItemCopyrightIgnoredOnWrite =>
            ResourceManager.GetString("ItemCopyrightIgnoredOnWrite", resourceCulture);

        internal static string ItemCreatedNullCategory =>
            ResourceManager.GetString("ItemCreatedNullCategory", resourceCulture);

        internal static string ItemCreatedNullPerson =>
            ResourceManager.GetString("ItemCreatedNullPerson", resourceCulture);

        internal static string ItemFormatterDoesNotHaveItem =>
            ResourceManager.GetString("ItemFormatterDoesNotHaveItem", resourceCulture);

        internal static string ItemLastUpdatedTimeIgnoredOnWrite =>
            ResourceManager.GetString("ItemLastUpdatedTimeIgnoredOnWrite", resourceCulture);

        internal static string ItemLinksIgnoredOnWrite =>
            ResourceManager.GetString("ItemLinksIgnoredOnWrite", resourceCulture);

        internal static string JsonAttributeAlreadyWritten =>
            ResourceManager.GetString("JsonAttributeAlreadyWritten", resourceCulture);

        internal static string JsonAttributeMustHaveElement =>
            ResourceManager.GetString("JsonAttributeMustHaveElement", resourceCulture);

        internal static string JsonCannotWriteStandaloneTextAfterQuotedText =>
            ResourceManager.GetString("JsonCannotWriteStandaloneTextAfterQuotedText", resourceCulture);

        internal static string JsonCannotWriteTextAfterNonTextAttribute =>
            ResourceManager.GetString("JsonCannotWriteTextAfterNonTextAttribute", resourceCulture);

        internal static string JsonDateTimeOutOfRange =>
            ResourceManager.GetString("JsonDateTimeOutOfRange", resourceCulture);

        internal static string JsonDuplicateMemberInInput =>
            ResourceManager.GetString("JsonDuplicateMemberInInput", resourceCulture);

        internal static string JsonDuplicateMemberNames =>
            ResourceManager.GetString("JsonDuplicateMemberNames", resourceCulture);

        internal static string JsonEncodingNotSupported =>
            ResourceManager.GetString("JsonEncodingNotSupported", resourceCulture);

        internal static string JsonEncounteredUnexpectedCharacter =>
            ResourceManager.GetString("JsonEncounteredUnexpectedCharacter", resourceCulture);

        internal static string JsonEndElementNoOpenNodes =>
            ResourceManager.GetString("JsonEndElementNoOpenNodes", resourceCulture);

        internal static string JsonExpectedEncoding =>
            ResourceManager.GetString("JsonExpectedEncoding", resourceCulture);

        internal static string JsonFormatRequiresDataContract =>
            ResourceManager.GetString("JsonFormatRequiresDataContract", resourceCulture);

        internal static string JsonFormatterExpectedAttributeNull =>
            ResourceManager.GetString("JsonFormatterExpectedAttributeNull", resourceCulture);

        internal static string JsonFormatterExpectedAttributeObject =>
            ResourceManager.GetString("JsonFormatterExpectedAttributeObject", resourceCulture);

        internal static string JsonInvalidBytes =>
            ResourceManager.GetString("JsonInvalidBytes", resourceCulture);

        internal static string JsonInvalidDataTypeSpecifiedForServerType =>
            ResourceManager.GetString("JsonInvalidDataTypeSpecifiedForServerType", resourceCulture);

        internal static string JsonInvalidDateTimeString =>
            ResourceManager.GetString("JsonInvalidDateTimeString", resourceCulture);

        internal static string JsonInvalidFFFE =>
            ResourceManager.GetString("JsonInvalidFFFE", resourceCulture);

        internal static string JsonInvalidItemNameForArrayElement =>
            ResourceManager.GetString("JsonInvalidItemNameForArrayElement", resourceCulture);

        internal static string JsonInvalidLocalNameEmpty =>
            ResourceManager.GetString("JsonInvalidLocalNameEmpty", resourceCulture);

        internal static string JsonInvalidMethodBetweenStartEndAttribute =>
            ResourceManager.GetString("JsonInvalidMethodBetweenStartEndAttribute", resourceCulture);

        internal static string JsonInvalidRootElementName =>
            ResourceManager.GetString("JsonInvalidRootElementName", resourceCulture);

        internal static string JsonInvalidStartElementCall =>
            ResourceManager.GetString("JsonInvalidStartElementCall", resourceCulture);

        internal static string JsonInvalidWriteState =>
            ResourceManager.GetString("JsonInvalidWriteState", resourceCulture);

        internal static string JsonMethodNotSupported =>
            ResourceManager.GetString("JsonMethodNotSupported", resourceCulture);

        internal static string JsonMultipleRootElementsNotAllowedOnWriter =>
            ResourceManager.GetString("JsonMultipleRootElementsNotAllowedOnWriter", resourceCulture);

        internal static string JsonMustSpecifyDataType =>
            ResourceManager.GetString("JsonMustSpecifyDataType", resourceCulture);

        internal static string JsonMustUseWriteStringForWritingAttributeValues =>
            ResourceManager.GetString("JsonMustUseWriteStringForWritingAttributeValues", resourceCulture);

        internal static string JsonNamespaceMustBeEmpty =>
            ResourceManager.GetString("JsonNamespaceMustBeEmpty", resourceCulture);

        internal static string JsonNestedArraysNotSupported =>
            ResourceManager.GetString("JsonNestedArraysNotSupported", resourceCulture);

        internal static string JsonNodeTypeArrayOrObjectNotSpecified =>
            ResourceManager.GetString("JsonNodeTypeArrayOrObjectNotSpecified", resourceCulture);

        internal static string JsonNoEndpointAtMetadataAddress =>
            ResourceManager.GetString("JsonNoEndpointAtMetadataAddress", resourceCulture);

        internal static string JsonNoMatchingStartAttribute =>
            ResourceManager.GetString("JsonNoMatchingStartAttribute", resourceCulture);

        internal static string JsonOffsetExceedsBufferSize =>
            ResourceManager.GetString("JsonOffsetExceedsBufferSize", resourceCulture);

        internal static string JsonOneRequiredMemberNotFound =>
            ResourceManager.GetString("JsonOneRequiredMemberNotFound", resourceCulture);

        internal static string JsonOnlySupportsMessageVersionNone =>
            ResourceManager.GetString("JsonOnlySupportsMessageVersionNone", resourceCulture);

        internal static string JsonOnlyWhitespace =>
            ResourceManager.GetString("JsonOnlyWhitespace", resourceCulture);

        internal static string JsonOpenAttributeMustBeClosedFirst =>
            ResourceManager.GetString("JsonOpenAttributeMustBeClosedFirst", resourceCulture);

        internal static string JsonPrefixMustBeNullOrEmpty =>
            ResourceManager.GetString("JsonPrefixMustBeNullOrEmpty", resourceCulture);

        internal static string JsonRequiredMembersNotFound =>
            ResourceManager.GetString("JsonRequiredMembersNotFound", resourceCulture);

        internal static string JsonServerTypeSpecifiedForInvalidDataType =>
            ResourceManager.GetString("JsonServerTypeSpecifiedForInvalidDataType", resourceCulture);

        internal static string JsonSizeExceedsRemainingBufferSpace =>
            ResourceManager.GetString("JsonSizeExceedsRemainingBufferSpace", resourceCulture);

        internal static string JsonTypeNotSupportedByDataContractJsonSerializer =>
            ResourceManager.GetString("JsonTypeNotSupportedByDataContractJsonSerializer", resourceCulture);

        internal static string JsonUnexpectedAttributeLocalName =>
            ResourceManager.GetString("JsonUnexpectedAttributeLocalName", resourceCulture);

        internal static string JsonUnexpectedAttributeValue =>
            ResourceManager.GetString("JsonUnexpectedAttributeValue", resourceCulture);

        internal static string JsonUnexpectedEndOfFile =>
            ResourceManager.GetString("JsonUnexpectedEndOfFile", resourceCulture);

        internal static string JsonUnsupportedForIsReference =>
            ResourceManager.GetString("JsonUnsupportedForIsReference", resourceCulture);

        internal static string JsonValueMustBeInRange =>
            ResourceManager.GetString("JsonValueMustBeInRange", resourceCulture);

        internal static string JsonWebScriptServiceHostOneServiceContract =>
            ResourceManager.GetString("JsonWebScriptServiceHostOneServiceContract", resourceCulture);

        internal static string JsonWriteArrayNotSupported =>
            ResourceManager.GetString("JsonWriteArrayNotSupported", resourceCulture);

        internal static string JsonWriterClosed =>
            ResourceManager.GetString("JsonWriterClosed", resourceCulture);

        internal static string JsonXmlInvalidDeclaration =>
            ResourceManager.GetString("JsonXmlInvalidDeclaration", resourceCulture);

        internal static string JsonXmlProcessingInstructionNotSupported =>
            ResourceManager.GetString("JsonXmlProcessingInstructionNotSupported", resourceCulture);

        internal static string ManualAddressingCannotBeFalseWithTransportBindingElement =>
            ResourceManager.GetString("ManualAddressingCannotBeFalseWithTransportBindingElement", resourceCulture);

        internal static string MaxReceivedMessageSizeExceeded =>
            ResourceManager.GetString("MaxReceivedMessageSizeExceeded", resourceCulture);

        internal static string MaxSentMessageSizeExceeded =>
            ResourceManager.GetString("MaxSentMessageSizeExceeded", resourceCulture);

        internal static string MCAtMostOneRequestBodyParameterAllowedForUnwrappedMessages =>
            ResourceManager.GetString("MCAtMostOneRequestBodyParameterAllowedForUnwrappedMessages", resourceCulture);

        internal static string MessageBodyIsStream =>
            ResourceManager.GetString("MessageBodyIsStream", resourceCulture);

        internal static string MessageBufferIsClosed =>
            ResourceManager.GetString("MessageBufferIsClosed", resourceCulture);

        internal static string MessageClosed =>
            ResourceManager.GetString("MessageClosed", resourceCulture);

        internal static string MessageFormatPropertyNotFound =>
            ResourceManager.GetString("MessageFormatPropertyNotFound", resourceCulture);

        internal static string MessageFormatPropertyNotFound2 =>
            ResourceManager.GetString("MessageFormatPropertyNotFound2", resourceCulture);

        internal static string MessageFormatPropertyNotFound3 =>
            ResourceManager.GetString("MessageFormatPropertyNotFound3", resourceCulture);

        internal static string MultipleOperationsInContractWithPathMethod =>
            ResourceManager.GetString("MultipleOperationsInContractWithPathMethod", resourceCulture);

        internal static string MultipleWebAttributes =>
            ResourceManager.GetString("MultipleWebAttributes", resourceCulture);

        internal static string NoOutOrRefParametersAllowedWithStreamResult =>
            ResourceManager.GetString("NoOutOrRefParametersAllowedWithStreamResult", resourceCulture);

        internal static string NoOutOrRefStreamParametersAllowed =>
            ResourceManager.GetString("NoOutOrRefStreamParametersAllowed", resourceCulture);

        internal static string OffsetExceedsBufferSize =>
            ResourceManager.GetString("OffsetExceedsBufferSize", resourceCulture);

        internal static string OnlyDataContractAndXmlSerializerTypesInUnWrappedMode =>
            ResourceManager.GetString("OnlyDataContractAndXmlSerializerTypesInUnWrappedMode", resourceCulture);

        internal static string OnlyReturnValueBodyParameterAllowedForUnwrappedMessages =>
            ResourceManager.GetString("OnlyReturnValueBodyParameterAllowedForUnwrappedMessages", resourceCulture);

        internal static string OuterElementNameNotSpecified =>
            ResourceManager.GetString("OuterElementNameNotSpecified", resourceCulture);

        internal static string ParameterIsNotStreamType =>
            ResourceManager.GetString("ParameterIsNotStreamType", resourceCulture);

        internal static string QueryStringFormatterOperationNotSupportedClientSide =>
            ResourceManager.GetString("QueryStringFormatterOperationNotSupportedClientSide", resourceCulture);

        internal static string QueryStringFormatterOperationNotSupportedServerSide =>
            ResourceManager.GetString("QueryStringFormatterOperationNotSupportedServerSide", resourceCulture);

        internal static string ReaderNotPositionedAtByteStream =>
            ResourceManager.GetString("ReaderNotPositionedAtByteStream", resourceCulture);

        internal static string RedirectPageText =>
            ResourceManager.GetString("RedirectPageText", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("System.ServiceModel.SR2", typeof(SR2).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static string RpcEncodedNotSupportedForNoneMessageVersion =>
            ResourceManager.GetString("RpcEncodedNotSupportedForNoneMessageVersion", resourceCulture);

        internal static string SerializingReplyNotSupportedByFormatter =>
            ResourceManager.GetString("SerializingReplyNotSupportedByFormatter", resourceCulture);

        internal static string SerializingRequestNotSupportedByFormatter =>
            ResourceManager.GetString("SerializingRequestNotSupportedByFormatter", resourceCulture);

        internal static string ServerErrorProcessingRequest =>
            ResourceManager.GetString("ServerErrorProcessingRequest", resourceCulture);

        internal static string ServerErrorProcessingRequestWithDetails =>
            ResourceManager.GetString("ServerErrorProcessingRequestWithDetails", resourceCulture);

        internal static string SizeExceedsRemainingBufferSpace =>
            ResourceManager.GetString("SizeExceedsRemainingBufferSpace", resourceCulture);

        internal static string StreamBodyMemberNotSupported =>
            ResourceManager.GetString("StreamBodyMemberNotSupported", resourceCulture);

        internal static string TraceCodeSyndicationFeedReadBegin =>
            ResourceManager.GetString("TraceCodeSyndicationFeedReadBegin", resourceCulture);

        internal static string TraceCodeSyndicationFeedReadEnd =>
            ResourceManager.GetString("TraceCodeSyndicationFeedReadEnd", resourceCulture);

        internal static string TraceCodeSyndicationFeedWriteBegin =>
            ResourceManager.GetString("TraceCodeSyndicationFeedWriteBegin", resourceCulture);

        internal static string TraceCodeSyndicationFeedWriteEnd =>
            ResourceManager.GetString("TraceCodeSyndicationFeedWriteEnd", resourceCulture);

        internal static string TraceCodeSyndicationItemReadBegin =>
            ResourceManager.GetString("TraceCodeSyndicationItemReadBegin", resourceCulture);

        internal static string TraceCodeSyndicationItemReadEnd =>
            ResourceManager.GetString("TraceCodeSyndicationItemReadEnd", resourceCulture);

        internal static string TraceCodeSyndicationItemWriteBegin =>
            ResourceManager.GetString("TraceCodeSyndicationItemWriteBegin", resourceCulture);

        internal static string TraceCodeSyndicationItemWriteEnd =>
            ResourceManager.GetString("TraceCodeSyndicationItemWriteEnd", resourceCulture);

        internal static string TraceCodeSyndicationProtocolElementIgnoredOnRead =>
            ResourceManager.GetString("TraceCodeSyndicationProtocolElementIgnoredOnRead", resourceCulture);

        internal static string TraceCodeSyndicationReadCategoriesDocumentBegin =>
            ResourceManager.GetString("TraceCodeSyndicationReadCategoriesDocumentBegin", resourceCulture);

        internal static string TraceCodeSyndicationReadCategoriesDocumentEnd =>
            ResourceManager.GetString("TraceCodeSyndicationReadCategoriesDocumentEnd", resourceCulture);

        internal static string TraceCodeSyndicationReadServiceDocumentBegin =>
            ResourceManager.GetString("TraceCodeSyndicationReadServiceDocumentBegin", resourceCulture);

        internal static string TraceCodeSyndicationReadServiceDocumentEnd =>
            ResourceManager.GetString("TraceCodeSyndicationReadServiceDocumentEnd", resourceCulture);

        internal static string TraceCodeSyndicationWriteCategoriesDocumentBegin =>
            ResourceManager.GetString("TraceCodeSyndicationWriteCategoriesDocumentBegin", resourceCulture);

        internal static string TraceCodeSyndicationWriteCategoriesDocumentEnd =>
            ResourceManager.GetString("TraceCodeSyndicationWriteCategoriesDocumentEnd", resourceCulture);

        internal static string TraceCodeSyndicationWriteServiceDocumentBegin =>
            ResourceManager.GetString("TraceCodeSyndicationWriteServiceDocumentBegin", resourceCulture);

        internal static string TraceCodeSyndicationWriteServiceDocumentEnd =>
            ResourceManager.GetString("TraceCodeSyndicationWriteServiceDocumentEnd", resourceCulture);

        internal static string TraceCodeWebRequestMatchesOperation =>
            ResourceManager.GetString("TraceCodeWebRequestMatchesOperation", resourceCulture);

        internal static string TraceCodeWebRequestRedirect =>
            ResourceManager.GetString("TraceCodeWebRequestRedirect", resourceCulture);

        internal static string TraceCodeWebRequestUnknownQueryParameterIgnored =>
            ResourceManager.GetString("TraceCodeWebRequestUnknownQueryParameterIgnored", resourceCulture);

        internal static string TypeIsNotParameterTypeAndIsNotPresentInKnownTypes =>
            ResourceManager.GetString("TypeIsNotParameterTypeAndIsNotPresentInKnownTypes", resourceCulture);

        internal static string TypeNotSupportedByQueryStringConverter =>
            ResourceManager.GetString("TypeNotSupportedByQueryStringConverter", resourceCulture);

        internal static string UnbufferedItemsCannotBeCloned =>
            ResourceManager.GetString("UnbufferedItemsCannotBeCloned", resourceCulture);

        internal static string UnknownDocumentXml =>
            ResourceManager.GetString("UnknownDocumentXml", resourceCulture);

        internal static string UnknownFeedXml =>
            ResourceManager.GetString("UnknownFeedXml", resourceCulture);

        internal static string UnknownItemXml =>
            ResourceManager.GetString("UnknownItemXml", resourceCulture);

        internal static string UnknownWebEncodingFormat =>
            ResourceManager.GetString("UnknownWebEncodingFormat", resourceCulture);

        internal static string UnrecognizedHttpMessageFormat =>
            ResourceManager.GetString("UnrecognizedHttpMessageFormat", resourceCulture);

        internal static string UnsupportedRssVersion =>
            ResourceManager.GetString("UnsupportedRssVersion", resourceCulture);

        internal static string UriTemplateMissingVar =>
            ResourceManager.GetString("UriTemplateMissingVar", resourceCulture);

        internal static string UriTemplatePathVarMustBeString =>
            ResourceManager.GetString("UriTemplatePathVarMustBeString", resourceCulture);

        internal static string UriTemplateQueryVarMustBeConvertible =>
            ResourceManager.GetString("UriTemplateQueryVarMustBeConvertible", resourceCulture);

        internal static string UriTemplateVarCaseDistinction =>
            ResourceManager.GetString("UriTemplateVarCaseDistinction", resourceCulture);

        internal static string UTAdditionalDefaultIsInvalid =>
            ResourceManager.GetString("UTAdditionalDefaultIsInvalid", resourceCulture);

        internal static string UTBadBaseAddress =>
            ResourceManager.GetString("UTBadBaseAddress", resourceCulture);

        internal static string UTBindByNameCalledWithEmptyKey =>
            ResourceManager.GetString("UTBindByNameCalledWithEmptyKey", resourceCulture);

        internal static string UTBindByPositionNoVariables =>
            ResourceManager.GetString("UTBindByPositionNoVariables", resourceCulture);

        internal static string UTBindByPositionWrongCount =>
            ResourceManager.GetString("UTBindByPositionWrongCount", resourceCulture);

        internal static string UTBothLiteralAndNameValueCollectionKey =>
            ResourceManager.GetString("UTBothLiteralAndNameValueCollectionKey", resourceCulture);

        internal static string UTCSRLookupBeforeMatch =>
            ResourceManager.GetString("UTCSRLookupBeforeMatch", resourceCulture);

        internal static string UTDefaultValuesAreImmutable =>
            ResourceManager.GetString("UTDefaultValuesAreImmutable", resourceCulture);

        internal static string UTDefaultValueToCompoundSegmentVar =>
            ResourceManager.GetString("UTDefaultValueToCompoundSegmentVar", resourceCulture);

        internal static string UTDefaultValueToCompoundSegmentVarFromAdditionalDefaults =>
            ResourceManager.GetString("UTDefaultValueToCompoundSegmentVarFromAdditionalDefaults", resourceCulture);

        internal static string UTDefaultValueToQueryVar =>
            ResourceManager.GetString("UTDefaultValueToQueryVar", resourceCulture);

        internal static string UTDefaultValueToQueryVarFromAdditionalDefaults =>
            ResourceManager.GetString("UTDefaultValueToQueryVarFromAdditionalDefaults", resourceCulture);

        internal static string UTDoesNotSupportAdjacentVarsInCompoundSegment =>
            ResourceManager.GetString("UTDoesNotSupportAdjacentVarsInCompoundSegment", resourceCulture);

        internal static string UTInvalidDefaultPathValue =>
            ResourceManager.GetString("UTInvalidDefaultPathValue", resourceCulture);

        internal static string UTInvalidFormatSegmentOrQueryPart =>
            ResourceManager.GetString("UTInvalidFormatSegmentOrQueryPart", resourceCulture);

        internal static string UTInvalidVarDeclaration =>
            ResourceManager.GetString("UTInvalidVarDeclaration", resourceCulture);

        internal static string UTInvalidWildcardInVariableOrLiteral =>
            ResourceManager.GetString("UTInvalidWildcardInVariableOrLiteral", resourceCulture);

        internal static string UTNullableDefaultAtAdditionalDefaults =>
            ResourceManager.GetString("UTNullableDefaultAtAdditionalDefaults", resourceCulture);

        internal static string UTNullableDefaultMustBeFollowedWithNullables =>
            ResourceManager.GetString("UTNullableDefaultMustBeFollowedWithNullables", resourceCulture);

        internal static string UTNullableDefaultMustNotBeFollowedWithLiteral =>
            ResourceManager.GetString("UTNullableDefaultMustNotBeFollowedWithLiteral", resourceCulture);

        internal static string UTNullableDefaultMustNotBeFollowedWithWildcard =>
            ResourceManager.GetString("UTNullableDefaultMustNotBeFollowedWithWildcard", resourceCulture);

        internal static string UTParamsDoNotComposeWithMessage =>
            ResourceManager.GetString("UTParamsDoNotComposeWithMessage", resourceCulture);

        internal static string UTParamsDoNotComposeWithMessageContract =>
            ResourceManager.GetString("UTParamsDoNotComposeWithMessageContract", resourceCulture);

        internal static string UTQueryCannotEndInAmpersand =>
            ResourceManager.GetString("UTQueryCannotEndInAmpersand", resourceCulture);

        internal static string UTQueryCannotHaveCompoundValue =>
            ResourceManager.GetString("UTQueryCannotHaveCompoundValue", resourceCulture);

        internal static string UTQueryCannotHaveEmptyName =>
            ResourceManager.GetString("UTQueryCannotHaveEmptyName", resourceCulture);

        internal static string UTQueryMustHaveLiteralNames =>
            ResourceManager.GetString("UTQueryMustHaveLiteralNames", resourceCulture);

        internal static string UTQueryNamesMustBeUnique =>
            ResourceManager.GetString("UTQueryNamesMustBeUnique", resourceCulture);

        internal static string UTStarVariableWithDefaults =>
            ResourceManager.GetString("UTStarVariableWithDefaults", resourceCulture);

        internal static string UTStarVariableWithDefaultsFromAdditionalDefaults =>
            ResourceManager.GetString("UTStarVariableWithDefaultsFromAdditionalDefaults", resourceCulture);

        internal static string UTTAmbiguousQueries =>
            ResourceManager.GetString("UTTAmbiguousQueries", resourceCulture);

        internal static string UTTBaseAddressMustBeAbsolute =>
            ResourceManager.GetString("UTTBaseAddressMustBeAbsolute", resourceCulture);

        internal static string UTTBaseAddressNotSet =>
            ResourceManager.GetString("UTTBaseAddressNotSet", resourceCulture);

        internal static string UTTCannotChangeBaseAddress =>
            ResourceManager.GetString("UTTCannotChangeBaseAddress", resourceCulture);

        internal static string UTTDuplicate =>
            ResourceManager.GetString("UTTDuplicate", resourceCulture);

        internal static string UTTEmptyKeyValuePairs =>
            ResourceManager.GetString("UTTEmptyKeyValuePairs", resourceCulture);

        internal static string UTTInvalidTemplateKey =>
            ResourceManager.GetString("UTTInvalidTemplateKey", resourceCulture);

        internal static string UTTMultipleMatches =>
            ResourceManager.GetString("UTTMultipleMatches", resourceCulture);

        internal static string UTTMustBeAbsolute =>
            ResourceManager.GetString("UTTMustBeAbsolute", resourceCulture);

        internal static string UTTNullTemplateKey =>
            ResourceManager.GetString("UTTNullTemplateKey", resourceCulture);

        internal static string UTTOtherAmbiguousQueries =>
            ResourceManager.GetString("UTTOtherAmbiguousQueries", resourceCulture);

        internal static string UTVarNamesMustBeUnique =>
            ResourceManager.GetString("UTVarNamesMustBeUnique", resourceCulture);

        internal static string ValueMustBeNonNegative =>
            ResourceManager.GetString("ValueMustBeNonNegative", resourceCulture);

        internal static string ValueMustBePositive =>
            ResourceManager.GetString("ValueMustBePositive", resourceCulture);

        internal static string WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadMessageVersion =>
            ResourceManager.GetString("WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadMessageVersion", resourceCulture);

        internal static string WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadScheme =>
            ResourceManager.GetString("WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadScheme", resourceCulture);

        internal static string WebBodyFormatPropertyToString =>
            ResourceManager.GetString("WebBodyFormatPropertyToString", resourceCulture);

        internal static string WebErrorPageTitleText =>
            ResourceManager.GetString("WebErrorPageTitleText", resourceCulture);

        internal static string WebHttpServiceEndpointCannotHaveMessageHeaders =>
            ResourceManager.GetString("WebHttpServiceEndpointCannotHaveMessageHeaders", resourceCulture);

        internal static string WebRequestDidNotMatchMethod =>
            ResourceManager.GetString("WebRequestDidNotMatchMethod", resourceCulture);

        internal static string WebRequestDidNotMatchOperation =>
            ResourceManager.GetString("WebRequestDidNotMatchOperation", resourceCulture);

        internal static string WebScriptInvalidHttpRequestMethod =>
            ResourceManager.GetString("WebScriptInvalidHttpRequestMethod", resourceCulture);

        internal static string WebScriptNotSupportedForXmlSerializerFormat =>
            ResourceManager.GetString("WebScriptNotSupportedForXmlSerializerFormat", resourceCulture);

        internal static string WebScriptOutRefOperationsNotSupported =>
            ResourceManager.GetString("WebScriptOutRefOperationsNotSupported", resourceCulture);

        internal static string XmlObjectAssignedToIncompatibleInterface =>
            ResourceManager.GetString("XmlObjectAssignedToIncompatibleInterface", resourceCulture);

        internal static string XmlSerializersCreatedBeforeRegistration =>
            ResourceManager.GetString("XmlSerializersCreatedBeforeRegistration", resourceCulture);

        internal static string XmlWriterClosed =>
            ResourceManager.GetString("XmlWriterClosed", resourceCulture);
    }
}

