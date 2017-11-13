namespace System.Data.Entity
{
    using System;
    using System.Data;

    internal static class Strings
    {
        internal static string ADP_CollectionParameterElementIsNull(object p0) => 
            EntityRes.GetString("ADP_CollectionParameterElementIsNull", new object[] { p0 });

        internal static string ADP_CollectionParameterElementIsNullOrEmpty(object p0) => 
            EntityRes.GetString("ADP_CollectionParameterElementIsNullOrEmpty", new object[] { p0 });

        internal static string ADP_DataReaderClosed(object p0) => 
            EntityRes.GetString("ADP_DataReaderClosed", new object[] { p0 });

        internal static string ADP_InternalProviderError(object p0) => 
            EntityRes.GetString("ADP_InternalProviderError", new object[] { p0 });

        internal static string ADP_InvalidBufferSizeOrIndex(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidBufferSizeOrIndex", new object[] { p0, p1 });

        internal static string ADP_InvalidDataLength(object p0) => 
            EntityRes.GetString("ADP_InvalidDataLength", new object[] { p0 });

        internal static string ADP_InvalidDataReaderMissingColumnForType(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidDataReaderMissingColumnForType", new object[] { p0, p1 });

        internal static string ADP_InvalidDataReaderMissingDiscriminatorColumn(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidDataReaderMissingDiscriminatorColumn", new object[] { p0, p1 });

        internal static string ADP_InvalidDataType(object p0) => 
            EntityRes.GetString("ADP_InvalidDataType", new object[] { p0 });

        internal static string ADP_InvalidDestinationBufferIndex(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidDestinationBufferIndex", new object[] { p0, p1 });

        internal static string ADP_InvalidEnumerationValue(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidEnumerationValue", new object[] { p0, p1 });

        internal static string ADP_InvalidSourceBufferIndex(object p0, object p1) => 
            EntityRes.GetString("ADP_InvalidSourceBufferIndex", new object[] { p0, p1 });

        internal static string ADP_KeysRequiredForJoinOverNest(object p0) => 
            EntityRes.GetString("ADP_KeysRequiredForJoinOverNest", new object[] { p0 });

        internal static string ADP_NestingNotSupported(object p0, object p1) => 
            EntityRes.GetString("ADP_NestingNotSupported", new object[] { p0, p1 });

        internal static string ADP_NonSequentialChunkAccess(object p0, object p1, object p2) => 
            EntityRes.GetString("ADP_NonSequentialChunkAccess", new object[] { p0, p1, p2 });

        internal static string ADP_NonSequentialColumnAccess(object p0, object p1) => 
            EntityRes.GetString("ADP_NonSequentialColumnAccess", new object[] { p0, p1 });

        internal static string ADP_NoQueryMappingView(object p0, object p1) => 
            EntityRes.GetString("ADP_NoQueryMappingView", new object[] { p0, p1 });

        internal static string ADP_UnknownDataTypeCode(object p0, object p1) => 
            EntityRes.GetString("ADP_UnknownDataTypeCode", new object[] { p0, p1 });

        internal static string AliasNameAlreadyUsed(object p0) => 
            EntityRes.GetString("AliasNameAlreadyUsed", new object[] { p0 });

        internal static string AliasNameIsAlreadyDefined(object p0) => 
            EntityRes.GetString("AliasNameIsAlreadyDefined", new object[] { p0 });

        internal static string AllArtifactsMustTargetSameProvider_InvariantName(object p0, object p1) => 
            EntityRes.GetString("AllArtifactsMustTargetSameProvider_InvariantName", new object[] { p0, p1 });

        internal static string AllArtifactsMustTargetSameProvider_ManifestToken(object p0, object p1) => 
            EntityRes.GetString("AllArtifactsMustTargetSameProvider_ManifestToken", new object[] { p0, p1 });

        internal static string AlreadyDefined(object p0) => 
            EntityRes.GetString("AlreadyDefined", new object[] { p0 });

        internal static string AmbiguousCanonicalFunction(object p0, object p1, object p2) => 
            EntityRes.GetString("AmbiguousCanonicalFunction", new object[] { p0, p1, p2 });

        internal static string AmbiguousEntityContainerEnd(object p0, object p1) => 
            EntityRes.GetString("AmbiguousEntityContainerEnd", new object[] { p0, p1 });

        internal static string AmbiguousFunction(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("AmbiguousFunction", new object[] { p0, p1, p2, p3 });

        internal static string AmbiguousFunctionMethodCtorName(object p0) => 
            EntityRes.GetString("AmbiguousFunctionMethodCtorName", new object[] { p0 });

        internal static string AmbiguousFunctionOverload(object p0) => 
            EntityRes.GetString("AmbiguousFunctionOverload", new object[] { p0 });

        internal static string AmbiguousFunctionReturnType(object p0, object p1) => 
            EntityRes.GetString("AmbiguousFunctionReturnType", new object[] { p0, p1 });

        internal static string AmbiguousName(object p0) => 
            EntityRes.GetString("AmbiguousName", new object[] { p0 });

        internal static string AmbiguousTypeName(object p0) => 
            EntityRes.GetString("AmbiguousTypeName", new object[] { p0 });

        internal static string ArgumentOutOfRange(object p0) => 
            EntityRes.GetString("ArgumentOutOfRange", new object[] { p0 });

        internal static string ArgumentOutOfRangeExpectedPostiveNumber(object p0) => 
            EntityRes.GetString("ArgumentOutOfRangeExpectedPostiveNumber", new object[] { p0 });

        internal static string ArgumentTypesAreIncompatible(object p0, object p1) => 
            EntityRes.GetString("ArgumentTypesAreIncompatible", new object[] { p0, p1 });

        internal static string AssemblyMissingFromAssembliesToConsider(object p0) => 
            EntityRes.GetString("AssemblyMissingFromAssembliesToConsider", new object[] { p0 });

        internal static string AssociationSetInvalidRelationshipKind(object p0) => 
            EntityRes.GetString("AssociationSetInvalidRelationshipKind", new object[] { p0 });

        internal static string BadNamespaceOrAlias(object p0) => 
            EntityRes.GetString("BadNamespaceOrAlias", new object[] { p0 });

        internal static string BadNavigationPropertyBadFromRoleType(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("BadNavigationPropertyBadFromRoleType", new object[] { p0, p1, p2, p3, p4 });

        internal static string BadNavigationPropertyRelationshipNotRelationship(object p0) => 
            EntityRes.GetString("BadNavigationPropertyRelationshipNotRelationship", new object[] { p0 });

        internal static string BadNavigationPropertyUndefinedRole(object p0, object p1) => 
            EntityRes.GetString("BadNavigationPropertyUndefinedRole", new object[] { p0, p1 });

        internal static string BadParameterDirection(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("BadParameterDirection", new object[] { p0, p1, p2, p3 });

        internal static string BadPrecisionAndScale(object p0, object p1) => 
            EntityRes.GetString("BadPrecisionAndScale", new object[] { p0, p1 });

        internal static string BadTypeModifier(object p0, object p1) => 
            EntityRes.GetString("BadTypeModifier", new object[] { p0, p1 });

        internal static string BetweenLimitsTypesAreNotCompatible(object p0, object p1) => 
            EntityRes.GetString("BetweenLimitsTypesAreNotCompatible", new object[] { p0, p1 });

        internal static string BetweenLimitsTypesAreNotOrderComparable(object p0, object p1) => 
            EntityRes.GetString("BetweenLimitsTypesAreNotOrderComparable", new object[] { p0, p1 });

        internal static string BetweenValueIsNotOrderComparable(object p0, object p1) => 
            EntityRes.GetString("BetweenValueIsNotOrderComparable", new object[] { p0, p1 });

        internal static string BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet(object p0, object p1) => 
            EntityRes.GetString("BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet", new object[] { p0, p1 });

        internal static string CannotConvertNumericLiteral(object p0, object p1) => 
            EntityRes.GetString("CannotConvertNumericLiteral", new object[] { p0, p1 });

        internal static string CannotInstantiateAbstractType(object p0) => 
            EntityRes.GetString("CannotInstantiateAbstractType", new object[] { p0 });

        internal static string CannotReferTypeAcrossSchema(object p0) => 
            EntityRes.GetString("CannotReferTypeAcrossSchema", new object[] { p0 });

        internal static string CannotResolveNameToFunction(object p0) => 
            EntityRes.GetString("CannotResolveNameToFunction", new object[] { p0 });

        internal static string CannotUseCanonicalNamespace(object p0) => 
            EntityRes.GetString("CannotUseCanonicalNamespace", new object[] { p0 });

        internal static string CannotUseSystemNamespaceAsAlias(object p0) => 
            EntityRes.GetString("CannotUseSystemNamespaceAsAlias", new object[] { p0 });

        internal static string CanonicalFunctionNotSupportedPriorSql10(object p0) => 
            EntityRes.GetString("CanonicalFunctionNotSupportedPriorSql10", new object[] { p0 });

        internal static string CanonicalTypeCannotBeMapped(object p0) => 
            EntityRes.GetString("CanonicalTypeCannotBeMapped", new object[] { p0 });

        internal static string CanonicalTypeNameNotFound(object p0, object p1) => 
            EntityRes.GetString("CanonicalTypeNameNotFound", new object[] { p0, p1 });

        internal static string CheckArgumentContainsNullFailed(object p0) => 
            EntityRes.GetString("CheckArgumentContainsNullFailed", new object[] { p0 });

        internal static string CodeGen_MissingMethod(object p0) => 
            EntityRes.GetString("CodeGen_MissingMethod", new object[] { p0 });

        internal static string Collections_CannotFillTryDifferentMergeOption(object p0, object p1) => 
            EntityRes.GetString("Collections_CannotFillTryDifferentMergeOption", new object[] { p0, p1 });

        internal static string Collections_ExpectedCollectionGotReference(object p0, object p1, object p2) => 
            EntityRes.GetString("Collections_ExpectedCollectionGotReference", new object[] { p0, p1, p2 });

        internal static string Collections_InvalidEntityStateLoad(object p0) => 
            EntityRes.GetString("Collections_InvalidEntityStateLoad", new object[] { p0 });

        internal static string Collections_NoRelationshipSetMatched(object p0) => 
            EntityRes.GetString("Collections_NoRelationshipSetMatched", new object[] { p0 });

        internal static string CompiledELinq_UnsupportedParameterType(object p0) => 
            EntityRes.GetString("CompiledELinq_UnsupportedParameterType", new object[] { p0 });

        internal static string ComplexObject_ComplexChangeRequestedOnScalarProperty(object p0) => 
            EntityRes.GetString("ComplexObject_ComplexChangeRequestedOnScalarProperty", new object[] { p0 });

        internal static string ComplexObject_InvalidComplexDataRecordObject(object p0) => 
            EntityRes.GetString("ComplexObject_InvalidComplexDataRecordObject", new object[] { p0 });

        internal static string ComplexObject_NullableComplexTypesNotSupported(object p0) => 
            EntityRes.GetString("ComplexObject_NullableComplexTypesNotSupported", new object[] { p0 });

        internal static string ConcurrencyRedefinedOnSubTypeOfEntitySetType(object p0, object p1, object p2) => 
            EntityRes.GetString("ConcurrencyRedefinedOnSubTypeOfEntitySetType", new object[] { p0, p1, p2 });

        internal static string ConstantFacetSpecifiedInSchema(object p0, object p1) => 
            EntityRes.GetString("ConstantFacetSpecifiedInSchema", new object[] { p0, p1 });

        internal static string CouldNotResolveIdentifier(object p0) => 
            EntityRes.GetString("CouldNotResolveIdentifier", new object[] { p0 });

        internal static string Cqt_Binary_CollectionsRequired(object p0) => 
            EntityRes.GetString("Cqt_Binary_CollectionsRequired", new object[] { p0 });

        internal static string Cqt_Cast_InvalidCast(object p0, object p1) => 
            EntityRes.GetString("Cqt_Cast_InvalidCast", new object[] { p0, p1 });

        internal static string Cqt_CommandTree_InvalidParameterName(object p0) => 
            EntityRes.GetString("Cqt_CommandTree_InvalidParameterName", new object[] { p0 });

        internal static string Cqt_Constant_InvalidConstantType(object p0) => 
            EntityRes.GetString("Cqt_Constant_InvalidConstantType", new object[] { p0 });

        internal static string Cqt_Constant_InvalidValueForType(object p0) => 
            EntityRes.GetString("Cqt_Constant_InvalidValueForType", new object[] { p0 });

        internal static string Cqt_Copier_EndNotFound(object p0, object p1) => 
            EntityRes.GetString("Cqt_Copier_EndNotFound", new object[] { p0, p1 });

        internal static string Cqt_Copier_EntityContainerNotFound(object p0) => 
            EntityRes.GetString("Cqt_Copier_EntityContainerNotFound", new object[] { p0 });

        internal static string Cqt_Copier_EntitySetNotFound(object p0, object p1) => 
            EntityRes.GetString("Cqt_Copier_EntitySetNotFound", new object[] { p0, p1 });

        internal static string Cqt_Copier_FunctionNotFound(object p0) => 
            EntityRes.GetString("Cqt_Copier_FunctionNotFound", new object[] { p0 });

        internal static string Cqt_Copier_PropertyNotFound(object p0, object p1) => 
            EntityRes.GetString("Cqt_Copier_PropertyNotFound", new object[] { p0, p1 });

        internal static string Cqt_Copier_TypeNotFound(object p0) => 
            EntityRes.GetString("Cqt_Copier_TypeNotFound", new object[] { p0 });

        internal static string Cqt_CrossJoin_DuplicateVariableNames(object p0, object p1, object p2) => 
            EntityRes.GetString("Cqt_CrossJoin_DuplicateVariableNames", new object[] { p0, p1, p2 });

        internal static string Cqt_ExpressionLink_NullTypeInvalid(object p0) => 
            EntityRes.GetString("Cqt_ExpressionLink_NullTypeInvalid", new object[] { p0 });

        internal static string Cqt_ExpressionLink_TypeMismatch(object p0, object p1) => 
            EntityRes.GetString("Cqt_ExpressionLink_TypeMismatch", new object[] { p0, p1 });

        internal static string Cqt_General_PolymorphicArgRequired(object p0) => 
            EntityRes.GetString("Cqt_General_PolymorphicArgRequired", new object[] { p0 });

        internal static string Cqt_General_PolymorphicTypeRequired(object p0) => 
            EntityRes.GetString("Cqt_General_PolymorphicTypeRequired", new object[] { p0 });

        internal static string Cqt_General_UnsupportedExpression(object p0) => 
            EntityRes.GetString("Cqt_General_UnsupportedExpression", new object[] { p0 });

        internal static string Cqt_GroupBy_AggregateColumnExistsAsGroupColumn(object p0) => 
            EntityRes.GetString("Cqt_GroupBy_AggregateColumnExistsAsGroupColumn", new object[] { p0 });

        internal static string Cqt_GroupBy_KeyNotEqualityComparable(object p0) => 
            EntityRes.GetString("Cqt_GroupBy_KeyNotEqualityComparable", new object[] { p0 });

        internal static string Cqt_InvalidTypeForSetOperation(object p0, object p1) => 
            EntityRes.GetString("Cqt_InvalidTypeForSetOperation", new object[] { p0, p1 });

        internal static string Cqt_NewInstance_CannotInstantiateAbstractType(object p0) => 
            EntityRes.GetString("Cqt_NewInstance_CannotInstantiateAbstractType", new object[] { p0 });

        internal static string Cqt_NewInstance_CannotInstantiateMemberlessType(object p0) => 
            EntityRes.GetString("Cqt_NewInstance_CannotInstantiateMemberlessType", new object[] { p0 });

        internal static string Cqt_RelNav_WrongSourceType(object p0) => 
            EntityRes.GetString("Cqt_RelNav_WrongSourceType", new object[] { p0 });

        internal static string Cqt_Unary_CollectionRequired(object p0) => 
            EntityRes.GetString("Cqt_Unary_CollectionRequired", new object[] { p0 });

        internal static string Cqt_Util_CheckListDuplicateName(object p0, object p1, object p2) => 
            EntityRes.GetString("Cqt_Util_CheckListDuplicateName", new object[] { p0, p1, p2 });

        internal static string Cqt_Validator_VarRefInvalid(object p0) => 
            EntityRes.GetString("Cqt_Validator_VarRefInvalid", new object[] { p0 });

        internal static string Cqt_Validator_VarRefTypeMismatch(object p0) => 
            EntityRes.GetString("Cqt_Validator_VarRefTypeMismatch", new object[] { p0 });

        internal static string CreateRefTypeIdentifierMustBeASubOrSuperType(object p0, object p1) => 
            EntityRes.GetString("CreateRefTypeIdentifierMustBeASubOrSuperType", new object[] { p0, p1 });

        internal static string CreateRefTypeIdentifierMustSpecifyAnEntityType(object p0, object p1) => 
            EntityRes.GetString("CreateRefTypeIdentifierMustSpecifyAnEntityType", new object[] { p0, p1 });

        internal static string CtxFunction(object p0) => 
            EntityRes.GetString("CtxFunction", new object[] { p0 });

        internal static string CtxMethodTerm(object p0) => 
            EntityRes.GetString("CtxMethodTerm", new object[] { p0 });

        internal static string CtxTypeCtorWithType(object p0) => 
            EntityRes.GetString("CtxTypeCtorWithType", new object[] { p0 });

        internal static string CycleInTypeHierarchy(object p0) => 
            EntityRes.GetString("CycleInTypeHierarchy", new object[] { p0 });

        internal static string DecimalPrecisionMustBeGreaterThanScale(object p0) => 
            EntityRes.GetString("DecimalPrecisionMustBeGreaterThanScale", new object[] { p0 });

        internal static string DeRefArgIsNotOfRefType(object p0) => 
            EntityRes.GetString("DeRefArgIsNotOfRefType", new object[] { p0 });

        internal static string DuplicatedFunctionoverloads(object p0, object p1) => 
            EntityRes.GetString("DuplicatedFunctionoverloads", new object[] { p0, p1 });

        internal static string DuplicateEndName(object p0) => 
            EntityRes.GetString("DuplicateEndName", new object[] { p0 });

        internal static string DuplicateEntityContainerMemberName(object p0) => 
            EntityRes.GetString("DuplicateEntityContainerMemberName", new object[] { p0 });

        internal static string DuplicateEntitySetTable(object p0, object p1, object p2) => 
            EntityRes.GetString("DuplicateEntitySetTable", new object[] { p0, p1, p2 });

        internal static string DuplicateMemberName(object p0, object p1, object p2) => 
            EntityRes.GetString("DuplicateMemberName", new object[] { p0, p1, p2 });

        internal static string DuplicateMemberNameInExtendedEntityContainer(object p0, object p1, object p2) => 
            EntityRes.GetString("DuplicateMemberNameInExtendedEntityContainer", new object[] { p0, p1, p2 });

        internal static string DuplicatePropertyNameSpecifiedInEntityKey(object p0, object p1) => 
            EntityRes.GetString("DuplicatePropertyNameSpecifiedInEntityKey", new object[] { p0, p1 });

        internal static string DuplicationOperation(object p0) => 
            EntityRes.GetString("DuplicationOperation", new object[] { p0 });

        internal static string EdmVersionNotSupportedByRuntime(object p0, object p1) => 
            EntityRes.GetString("EdmVersionNotSupportedByRuntime", new object[] { p0, p1 });

        internal static string ELinq_InvalidOfTypeResult(object p0) => 
            EntityRes.GetString("ELinq_InvalidOfTypeResult", new object[] { p0 });

        internal static string ELinq_NotPropertyOrField(object p0) => 
            EntityRes.GetString("ELinq_NotPropertyOrField", new object[] { p0 });

        internal static string ELinq_UnboundParameterExpression(object p0) => 
            EntityRes.GetString("ELinq_UnboundParameterExpression", new object[] { p0 });

        internal static string ELinq_UnexpectedTypeForNavigationProperty(object p0, object p1, object p2) => 
            EntityRes.GetString("ELinq_UnexpectedTypeForNavigationProperty", new object[] { p0, p1, p2 });

        internal static string ELinq_UnhandledBindingType(object p0) => 
            EntityRes.GetString("ELinq_UnhandledBindingType", new object[] { p0 });

        internal static string ELinq_UnhandledExpressionType(object p0) => 
            EntityRes.GetString("ELinq_UnhandledExpressionType", new object[] { p0 });

        internal static string ELinq_UnrecognizedMember(object p0) => 
            EntityRes.GetString("ELinq_UnrecognizedMember", new object[] { p0 });

        internal static string ELinq_UnresolvableCanonicalFunctionForExpression(object p0) => 
            EntityRes.GetString("ELinq_UnresolvableCanonicalFunctionForExpression", new object[] { p0 });

        internal static string ELinq_UnresolvableCanonicalFunctionForMember(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnresolvableCanonicalFunctionForMember", new object[] { p0, p1 });

        internal static string ELinq_UnresolvableCanonicalFunctionForMethod(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnresolvableCanonicalFunctionForMethod", new object[] { p0, p1 });

        internal static string ELinq_UnresolvableStoreFunctionForExpression(object p0) => 
            EntityRes.GetString("ELinq_UnresolvableStoreFunctionForExpression", new object[] { p0 });

        internal static string ELinq_UnresolvableStoreFunctionForMember(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnresolvableStoreFunctionForMember", new object[] { p0, p1 });

        internal static string ELinq_UnresolvableStoreFunctionForMethod(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnresolvableStoreFunctionForMethod", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedCast(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedCast", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedComparison(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedComparison", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedConstant(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedConstant", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedEnumerableType(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedEnumerableType", new object[] { p0 });

        internal static string ELinq_UnsupportedExpressionType(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedExpressionType", new object[] { p0 });

        internal static string ELinq_UnsupportedHeterogeneousInitializers(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedHeterogeneousInitializers", new object[] { p0 });

        internal static string ELinq_UnsupportedIsOrAs(object p0, object p1, object p2) => 
            EntityRes.GetString("ELinq_UnsupportedIsOrAs", new object[] { p0, p1, p2 });

        internal static string ELinq_UnsupportedKeySelector(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedKeySelector", new object[] { p0 });

        internal static string ELinq_UnsupportedMethod(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedMethod", new object[] { p0 });

        internal static string ELinq_UnsupportedMethodSuggestedAlternative(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedMethodSuggestedAlternative", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedNominalType(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedNominalType", new object[] { p0 });

        internal static string ELinq_UnsupportedPassthrough(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedPassthrough", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedRowComparison(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedRowComparison", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedRowMemberComparison(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedRowMemberComparison", new object[] { p0 });

        internal static string ELinq_UnsupportedRowTypeComparison(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedRowTypeComparison", new object[] { p0 });

        internal static string ELinq_UnsupportedStringRemoveCase(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedStringRemoveCase", new object[] { p0, p1 });

        internal static string ELinq_UnsupportedTrimStartTrimEndCase(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedTrimStartTrimEndCase", new object[] { p0 });

        internal static string ELinq_UnsupportedType(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedType", new object[] { p0 });

        internal static string ELinq_UnsupportedUseOfContextParameter(object p0) => 
            EntityRes.GetString("ELinq_UnsupportedUseOfContextParameter", new object[] { p0 });

        internal static string ELinq_UnsupportedVBDatePartInvalidInterval(object p0, object p1, object p2) => 
            EntityRes.GetString("ELinq_UnsupportedVBDatePartInvalidInterval", new object[] { p0, p1, p2 });

        internal static string ELinq_UnsupportedVBDatePartNonConstantInterval(object p0, object p1) => 
            EntityRes.GetString("ELinq_UnsupportedVBDatePartNonConstantInterval", new object[] { p0, p1 });

        internal static string EmptyFile(object p0) => 
            EntityRes.GetString("EmptyFile", new object[] { p0 });

        internal static string EmptyName(object p0) => 
            EntityRes.GetString("EmptyName", new object[] { p0 });

        internal static string EndNameAlreadyDefinedDuplicate(object p0) => 
            EntityRes.GetString("EndNameAlreadyDefinedDuplicate", new object[] { p0 });

        internal static string EndWithManyMultiplicityCannotHaveOperationsSpecified(object p0, object p1) => 
            EntityRes.GetString("EndWithManyMultiplicityCannotHaveOperationsSpecified", new object[] { p0, p1 });

        internal static string EntityClient_ConnectionAndMetadataProviderMismatch(object p0, object p1) => 
            EntityRes.GetString("EntityClient_ConnectionAndMetadataProviderMismatch", new object[] { p0, p1 });

        internal static string EntityClient_ConnectionStringMissingInfo(object p0) => 
            EntityRes.GetString("EntityClient_ConnectionStringMissingInfo", new object[] { p0 });

        internal static string EntityClient_DbConnectionHasNoProvider(object p0) => 
            EntityRes.GetString("EntityClient_DbConnectionHasNoProvider", new object[] { p0 });

        internal static string EntityClient_DoesNotImplementIServiceProvider(object p0) => 
            EntityRes.GetString("EntityClient_DoesNotImplementIServiceProvider", new object[] { p0 });

        internal static string EntityClient_DuplicateParameterNames(object p0) => 
            EntityRes.GetString("EntityClient_DuplicateParameterNames", new object[] { p0 });

        internal static string EntityClient_ExecutingOnClosedConnection(object p0) => 
            EntityRes.GetString("EntityClient_ExecutingOnClosedConnection", new object[] { p0 });

        internal static string EntityClient_FailedToGetInformation(object p0) => 
            EntityRes.GetString("EntityClient_FailedToGetInformation", new object[] { p0 });

        internal static string EntityClient_IncompatibleNavigationPropertyResult(object p0, object p1) => 
            EntityRes.GetString("EntityClient_IncompatibleNavigationPropertyResult", new object[] { p0, p1 });

        internal static string EntityClient_InvalidParameterDirection(object p0) => 
            EntityRes.GetString("EntityClient_InvalidParameterDirection", new object[] { p0 });

        internal static string EntityClient_InvalidParameterName(object p0) => 
            EntityRes.GetString("EntityClient_InvalidParameterName", new object[] { p0 });

        internal static string EntityClient_ItemCollectionsNotRegisteredInWorkspace(object p0) => 
            EntityRes.GetString("EntityClient_ItemCollectionsNotRegisteredInWorkspace", new object[] { p0 });

        internal static string EntityClient_KeywordNotSupported(object p0) => 
            EntityRes.GetString("EntityClient_KeywordNotSupported", new object[] { p0 });

        internal static string EntityClient_NestedNamedConnection(object p0) => 
            EntityRes.GetString("EntityClient_NestedNamedConnection", new object[] { p0 });

        internal static string EntityClient_NoSuitableType(object p0) => 
            EntityRes.GetString("EntityClient_NoSuitableType", new object[] { p0 });

        internal static string EntityClient_ProviderSpecificError(object p0) => 
            EntityRes.GetString("EntityClient_ProviderSpecificError", new object[] { p0 });

        internal static string EntityClient_ReturnedNullOnProviderMethod(object p0, object p1) => 
            EntityRes.GetString("EntityClient_ReturnedNullOnProviderMethod", new object[] { p0, p1 });

        internal static string EntityClient_UnableToFindFunctionImport(object p0, object p1) => 
            EntityRes.GetString("EntityClient_UnableToFindFunctionImport", new object[] { p0, p1 });

        internal static string EntityClient_UnableToFindFunctionImportContainer(object p0) => 
            EntityRes.GetString("EntityClient_UnableToFindFunctionImportContainer", new object[] { p0 });

        internal static string EntityClient_UnknownParameterType(object p0) => 
            EntityRes.GetString("EntityClient_UnknownParameterType", new object[] { p0 });

        internal static string EntityClient_UnsupportedDbType(object p0, object p1) => 
            EntityRes.GetString("EntityClient_UnsupportedDbType", new object[] { p0, p1 });

        internal static string EntityContainerAlreadyExists(object p0) => 
            EntityRes.GetString("EntityContainerAlreadyExists", new object[] { p0 });

        internal static string EntityContainerCannotExtendItself(object p0) => 
            EntityRes.GetString("EntityContainerCannotExtendItself", new object[] { p0 });

        internal static string EntityKey_DoesntMatchKeyOnEntity(object p0) => 
            EntityRes.GetString("EntityKey_DoesntMatchKeyOnEntity", new object[] { p0 });

        internal static string EntityKey_EntitySetDoesNotMatch(object p0) => 
            EntityRes.GetString("EntityKey_EntitySetDoesNotMatch", new object[] { p0 });

        internal static string EntityKey_EntityTypesDoNotMatch(object p0, object p1) => 
            EntityRes.GetString("EntityKey_EntityTypesDoNotMatch", new object[] { p0, p1 });

        internal static string EntityKey_IncorrectNumberOfKeyValuePairs(object p0, object p1, object p2) => 
            EntityRes.GetString("EntityKey_IncorrectNumberOfKeyValuePairs", new object[] { p0, p1, p2 });

        internal static string EntityKey_IncorrectValueType(object p0, object p1, object p2) => 
            EntityRes.GetString("EntityKey_IncorrectValueType", new object[] { p0, p1, p2 });

        internal static string EntityKey_InvalidName(object p0) => 
            EntityRes.GetString("EntityKey_InvalidName", new object[] { p0 });

        internal static string EntityKey_MissingKeyValue(object p0, object p1) => 
            EntityRes.GetString("EntityKey_MissingKeyValue", new object[] { p0, p1 });

        internal static string EntityKeyMustBeScalar(object p0, object p1) => 
            EntityRes.GetString("EntityKeyMustBeScalar", new object[] { p0, p1 });

        internal static string EntityKeyTypeCurrentlyNotSupported(object p0, object p1, object p2) => 
            EntityRes.GetString("EntityKeyTypeCurrentlyNotSupported", new object[] { p0, p1, p2 });

        internal static string EntityKeyTypeCurrentlyNotSupportedInSSDL(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("EntityKeyTypeCurrentlyNotSupportedInSSDL", new object[] { p0, p1, p2, p3, p4 });

        internal static string EntityReference_ExpectedReferenceGotCollection(object p0, object p1, object p2) => 
            EntityRes.GetString("EntityReference_ExpectedReferenceGotCollection", new object[] { p0, p1, p2 });

        internal static string EntityReference_TryDifferentMergeOption(object p0) => 
            EntityRes.GetString("EntityReference_TryDifferentMergeOption", new object[] { p0 });

        internal static string EntitySetIsDoesNotBelongToEntityContainer(object p0, object p1) => 
            EntityRes.GetString("EntitySetIsDoesNotBelongToEntityContainer", new object[] { p0, p1 });

        internal static string EntitySetNotInCSPace(object p0) => 
            EntityRes.GetString("EntitySetNotInCSPace", new object[] { p0 });

        internal static string EntitySetTypeHasNoKeys(object p0, object p1) => 
            EntityRes.GetString("EntitySetTypeHasNoKeys", new object[] { p0, p1 });

        internal static string EquivalentMemberName(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("EquivalentMemberName", new object[] { p0, p1, p2, p3 });

        internal static string ExpressionTypeMustBeEntityType(object p0, object p1, object p2) => 
            EntityRes.GetString("ExpressionTypeMustBeEntityType", new object[] { p0, p1, p2 });

        internal static string ExpressionTypeMustBeNominalType(object p0, object p1, object p2) => 
            EntityRes.GetString("ExpressionTypeMustBeNominalType", new object[] { p0, p1, p2 });

        internal static string FacetNotAllowed(object p0, object p1) => 
            EntityRes.GetString("FacetNotAllowed", new object[] { p0, p1 });

        internal static string FacetValueHasIncorrectType(object p0, object p1, object p2) => 
            EntityRes.GetString("FacetValueHasIncorrectType", new object[] { p0, p1, p2 });

        internal static string FailedToFindClrTypeMapping(object p0) => 
            EntityRes.GetString("FailedToFindClrTypeMapping", new object[] { p0 });

        internal static string FailedToFindCSpaceTypeMapping(object p0) => 
            EntityRes.GetString("FailedToFindCSpaceTypeMapping", new object[] { p0 });

        internal static string FailedToFindOSpaceTypeMapping(object p0) => 
            EntityRes.GetString("FailedToFindOSpaceTypeMapping", new object[] { p0 });

        internal static string FailedToResolveAggregateFunction(object p0) => 
            EntityRes.GetString("FailedToResolveAggregateFunction", new object[] { p0 });

        internal static string FunctionImportEntityTypeDoesNotMatchEntitySet(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("FunctionImportEntityTypeDoesNotMatchEntitySet", new object[] { p0, p1, p2, p3 });

        internal static string FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(object p0) => 
            EntityRes.GetString("FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet", new object[] { p0 });

        internal static string FunctionImportSpecifiesEntitySetButNotEntityType(object p0, object p1) => 
            EntityRes.GetString("FunctionImportSpecifiesEntitySetButNotEntityType", new object[] { p0, p1 });

        internal static string FunctionImportUnknownEntitySet(object p0, object p1) => 
            EntityRes.GetString("FunctionImportUnknownEntitySet", new object[] { p0, p1 });

        internal static string FunctionImportWithUnsupportedReturnTypeV1(object p0) => 
            EntityRes.GetString("FunctionImportWithUnsupportedReturnTypeV1", new object[] { p0 });

        internal static string FunctionImportWithUnsupportedReturnTypeV1_1(object p0) => 
            EntityRes.GetString("FunctionImportWithUnsupportedReturnTypeV1_1", new object[] { p0 });

        internal static string FunctionWithNonEdmTypeNotSupported(object p0, object p1) => 
            EntityRes.GetString("FunctionWithNonEdmTypeNotSupported", new object[] { p0, p1 });

        internal static string FunctionWithNonScalarTypeNotSupported(object p0, object p1) => 
            EntityRes.GetString("FunctionWithNonScalarTypeNotSupported", new object[] { p0, p1 });

        internal static string GeneralExceptionAsQueryInnerException(object p0) => 
            EntityRes.GetString("GeneralExceptionAsQueryInnerException", new object[] { p0 });

        internal static string Generated_View_Type_Super_Class_1(object p0) => 
            EntityRes.GetString("Generated_View_Type_Super_Class_1", new object[] { p0 });

        internal static string Generated_Views_Invalid_Extent_1(object p0) => 
            EntityRes.GetString("Generated_Views_Invalid_Extent_1", new object[] { p0 });

        internal static string InferRelationshipEndAmbigous(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("InferRelationshipEndAmbigous", new object[] { p0, p1, p2, p3, p4 });

        internal static string InferRelationshipEndFailedNoEntitySetMatch(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("InferRelationshipEndFailedNoEntitySetMatch", new object[] { p0, p1, p2, p3, p4 });

        internal static string InferRelationshipEndGivesAlreadyDefinedEnd(object p0, object p1) => 
            EntityRes.GetString("InferRelationshipEndGivesAlreadyDefinedEnd", new object[] { p0, p1 });

        internal static string InvalidAction(object p0, object p1) => 
            EntityRes.GetString("InvalidAction", new object[] { p0, p1 });

        internal static string InvalidAliasName(object p0) => 
            EntityRes.GetString("InvalidAliasName", new object[] { p0 });

        internal static string InvalidAssociationTypeForUnion(object p0) => 
            EntityRes.GetString("InvalidAssociationTypeForUnion", new object[] { p0 });

        internal static string InvalidBaseTypeForItemType(object p0, object p1) => 
            EntityRes.GetString("InvalidBaseTypeForItemType", new object[] { p0, p1 });

        internal static string InvalidBaseTypeForNestedType(object p0, object p1) => 
            EntityRes.GetString("InvalidBaseTypeForNestedType", new object[] { p0, p1 });

        internal static string InvalidBaseTypeForStructuredType(object p0, object p1) => 
            EntityRes.GetString("InvalidBaseTypeForStructuredType", new object[] { p0, p1 });

        internal static string InvalidBoolean(object p0, object p1) => 
            EntityRes.GetString("InvalidBoolean", new object[] { p0, p1 });

        internal static string InvalidCast(object p0, object p1) => 
            EntityRes.GetString("InvalidCast", new object[] { p0, p1 });

        internal static string InvalidCollectionForMapping(object p0) => 
            EntityRes.GetString("InvalidCollectionForMapping", new object[] { p0 });

        internal static string InvalidCollectionSpecified(object p0) => 
            EntityRes.GetString("InvalidCollectionSpecified", new object[] { p0 });

        internal static string InvalidComplexType(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("InvalidComplexType", new object[] { p0, p1, p2, p3 });

        internal static string InvalidCtorArgumentType(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidCtorArgumentType", new object[] { p0, p1, p2 });

        internal static string InvalidCtorUseOnType(object p0) => 
            EntityRes.GetString("InvalidCtorUseOnType", new object[] { p0 });

        internal static string InvalidDatePartArgumentExpression(object p0, object p1) => 
            EntityRes.GetString("InvalidDatePartArgumentExpression", new object[] { p0, p1 });

        internal static string InvalidDatePartArgumentValue(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDatePartArgumentValue", new object[] { p0, p1, p2 });

        internal static string InvalidDateTimeOffsetLiteral(object p0) => 
            EntityRes.GetString("InvalidDateTimeOffsetLiteral", new object[] { p0 });

        internal static string InvalidDay(object p0, object p1) => 
            EntityRes.GetString("InvalidDay", new object[] { p0, p1 });

        internal static string InvalidDayInMonth(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDayInMonth", new object[] { p0, p1, p2 });

        internal static string InvalidDefaultBinaryWithNoMaxLength(object p0) => 
            EntityRes.GetString("InvalidDefaultBinaryWithNoMaxLength", new object[] { p0 });

        internal static string InvalidDefaultBoolean(object p0) => 
            EntityRes.GetString("InvalidDefaultBoolean", new object[] { p0 });

        internal static string InvalidDefaultDateTime(object p0, object p1) => 
            EntityRes.GetString("InvalidDefaultDateTime", new object[] { p0, p1 });

        internal static string InvalidDefaultDateTimeOffset(object p0, object p1) => 
            EntityRes.GetString("InvalidDefaultDateTimeOffset", new object[] { p0, p1 });

        internal static string InvalidDefaultDecimal(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDefaultDecimal", new object[] { p0, p1, p2 });

        internal static string InvalidDefaultFloatingPoint(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDefaultFloatingPoint", new object[] { p0, p1, p2 });

        internal static string InvalidDefaultGuid(object p0) => 
            EntityRes.GetString("InvalidDefaultGuid", new object[] { p0 });

        internal static string InvalidDefaultIntegral(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDefaultIntegral", new object[] { p0, p1, p2 });

        internal static string InvalidDefaultTime(object p0, object p1) => 
            EntityRes.GetString("InvalidDefaultTime", new object[] { p0, p1 });

        internal static string InvalidDeRefProperty(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidDeRefProperty", new object[] { p0, p1, p2 });

        internal static string InvalidEndEntitySetTypeMismatch(object p0) => 
            EntityRes.GetString("InvalidEndEntitySetTypeMismatch", new object[] { p0 });

        internal static string InvalidEndRoleInRelationshipConstraint(object p0, object p1) => 
            EntityRes.GetString("InvalidEndRoleInRelationshipConstraint", new object[] { p0, p1 });

        internal static string InvalidEntityContainerNameInExtends(object p0) => 
            EntityRes.GetString("InvalidEntityContainerNameInExtends", new object[] { p0 });

        internal static string InvalidEntityEndName(object p0, object p1) => 
            EntityRes.GetString("InvalidEntityEndName", new object[] { p0, p1 });

        internal static string InvalidEntityRootTypeArgument(object p0, object p1) => 
            EntityRes.GetString("InvalidEntityRootTypeArgument", new object[] { p0, p1 });

        internal static string InvalidEntitySetName(object p0) => 
            EntityRes.GetString("InvalidEntitySetName", new object[] { p0 });

        internal static string InvalidEntitySetNameReference(object p0, object p1) => 
            EntityRes.GetString("InvalidEntitySetNameReference", new object[] { p0, p1 });

        internal static string InvalidEntitySetType(object p0) => 
            EntityRes.GetString("InvalidEntitySetType", new object[] { p0 });

        internal static string InvalidEntityTypeArgument(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("InvalidEntityTypeArgument", new object[] { p0, p1, p2, p3 });

        internal static string InvalidEscapedIdentifier(object p0) => 
            EntityRes.GetString("InvalidEscapedIdentifier", new object[] { p0 });

        internal static string InvalidEscapedIdentifierUnbalanced(object p0) => 
            EntityRes.GetString("InvalidEscapedIdentifierUnbalanced", new object[] { p0 });

        internal static string InvalidFileExtension(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidFileExtension", new object[] { p0, p1, p2 });

        internal static string InvalidFromPropertyInRelationshipConstraint(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidFromPropertyInRelationshipConstraint", new object[] { p0, p1, p2 });

        internal static string InvalidGroupIdentifierReference(object p0) => 
            EntityRes.GetString("InvalidGroupIdentifierReference", new object[] { p0 });

        internal static string InvalidHour(object p0, object p1) => 
            EntityRes.GetString("InvalidHour", new object[] { p0, p1 });

        internal static string InvalidImplicitRelationshipFromEnd(object p0) => 
            EntityRes.GetString("InvalidImplicitRelationshipFromEnd", new object[] { p0 });

        internal static string InvalidImplicitRelationshipToEnd(object p0) => 
            EntityRes.GetString("InvalidImplicitRelationshipToEnd", new object[] { p0 });

        internal static string InvalidInExprArgs(object p0, object p1) => 
            EntityRes.GetString("InvalidInExprArgs", new object[] { p0, p1 });

        internal static string InvalidKeyArgument(object p0) => 
            EntityRes.GetString("InvalidKeyArgument", new object[] { p0 });

        internal static string InvalidKeyKeyDefinedInBaseClass(object p0, object p1) => 
            EntityRes.GetString("InvalidKeyKeyDefinedInBaseClass", new object[] { p0, p1 });

        internal static string InvalidKeyMember(object p0) => 
            EntityRes.GetString("InvalidKeyMember", new object[] { p0 });

        internal static string InvalidKeyNoProperty(object p0, object p1) => 
            EntityRes.GetString("InvalidKeyNoProperty", new object[] { p0, p1 });

        internal static string InvalidKeyNullablePart(object p0, object p1) => 
            EntityRes.GetString("InvalidKeyNullablePart", new object[] { p0, p1 });

        internal static string InvalidKeyTypeForCollation(object p0) => 
            EntityRes.GetString("InvalidKeyTypeForCollation", new object[] { p0 });

        internal static string InvalidLiteralFormat(object p0, object p1) => 
            EntityRes.GetString("InvalidLiteralFormat", new object[] { p0, p1 });

        internal static string InvalidMemberNameMatchesTypeName(object p0, object p1) => 
            EntityRes.GetString("InvalidMemberNameMatchesTypeName", new object[] { p0, p1 });

        internal static string InvalidMethodPathElement(object p0, object p1) => 
            EntityRes.GetString("InvalidMethodPathElement", new object[] { p0, p1 });

        internal static string InvalidMinute(object p0, object p1) => 
            EntityRes.GetString("InvalidMinute", new object[] { p0, p1 });

        internal static string InvalidMonth(object p0, object p1) => 
            EntityRes.GetString("InvalidMonth", new object[] { p0, p1 });

        internal static string InvalidMultiplicityFromRoleToPropertyNonNullable(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityFromRoleToPropertyNonNullable", new object[] { p0, p1 });

        internal static string InvalidMultiplicityFromRoleToPropertyNullable(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityFromRoleToPropertyNullable", new object[] { p0, p1 });

        internal static string InvalidMultiplicityFromRoleUpperBoundMustBeOne(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityFromRoleUpperBoundMustBeOne", new object[] { p0, p1 });

        internal static string InvalidMultiplicityToRoleLowerBoundMustBeZero(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityToRoleLowerBoundMustBeZero", new object[] { p0, p1 });

        internal static string InvalidMultiplicityToRoleUpperBoundMustBeMany(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityToRoleUpperBoundMustBeMany", new object[] { p0, p1 });

        internal static string InvalidMultiplicityToRoleUpperBoundMustBeOne(object p0, object p1) => 
            EntityRes.GetString("InvalidMultiplicityToRoleUpperBoundMustBeOne", new object[] { p0, p1 });

        internal static string InvalidName(object p0, object p1) => 
            EntityRes.GetString("InvalidName", new object[] { p0, p1 });

        internal static string InvalidNamespaceInUsing(object p0) => 
            EntityRes.GetString("InvalidNamespaceInUsing", new object[] { p0 });

        internal static string InvalidNamespaceOrAliasSpecified(object p0) => 
            EntityRes.GetString("InvalidNamespaceOrAliasSpecified", new object[] { p0 });

        internal static string InvalidNullLiteralForNonNullableMember(object p0, object p1) => 
            EntityRes.GetString("InvalidNullLiteralForNonNullableMember", new object[] { p0, p1 });

        internal static string InvalidNumberOfParametersForAggregateFunction(object p0) => 
            EntityRes.GetString("InvalidNumberOfParametersForAggregateFunction", new object[] { p0 });

        internal static string InvalidParameterFormat(object p0) => 
            EntityRes.GetString("InvalidParameterFormat", new object[] { p0 });

        internal static string InvalidParameterType(object p0) => 
            EntityRes.GetString("InvalidParameterType", new object[] { p0 });

        internal static string InvalidParameterTypeForAggregateFunction(object p0, object p1) => 
            EntityRes.GetString("InvalidParameterTypeForAggregateFunction", new object[] { p0, p1 });

        internal static string InvalidPlaceholderRootTypeArgument(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("InvalidPlaceholderRootTypeArgument", new object[] { p0, p1, p2, p3 });

        internal static string InvalidPlaceholderTypeArgument(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("InvalidPlaceholderTypeArgument", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string InvalidPrimitiveTypeKind(object p0) => 
            EntityRes.GetString("InvalidPrimitiveTypeKind", new object[] { p0 });

        internal static string InvalidPropertyInRelationshipConstraint(object p0, object p1) => 
            EntityRes.GetString("InvalidPropertyInRelationshipConstraint", new object[] { p0, p1 });

        internal static string InvalidPropertyType(object p0) => 
            EntityRes.GetString("InvalidPropertyType", new object[] { p0 });

        internal static string InvalidQualifiedName(object p0, object p1) => 
            EntityRes.GetString("InvalidQualifiedName", new object[] { p0, p1 });

        internal static string InvalidQueryResultType(object p0) => 
            EntityRes.GetString("InvalidQueryResultType", new object[] { p0 });

        internal static string InvalidRelationshipEndMultiplicity(object p0, object p1) => 
            EntityRes.GetString("InvalidRelationshipEndMultiplicity", new object[] { p0, p1 });

        internal static string InvalidRelationshipEndType(object p0, object p1) => 
            EntityRes.GetString("InvalidRelationshipEndType", new object[] { p0, p1 });

        internal static string InvalidRelationshipMember(object p0, object p1) => 
            EntityRes.GetString("InvalidRelationshipMember", new object[] { p0, p1 });

        internal static string InvalidRelationshipSetName(object p0) => 
            EntityRes.GetString("InvalidRelationshipSetName", new object[] { p0 });

        internal static string InvalidRelationshipSetType(object p0) => 
            EntityRes.GetString("InvalidRelationshipSetType", new object[] { p0 });

        internal static string InvalidRelationshipTypeName(object p0) => 
            EntityRes.GetString("InvalidRelationshipTypeName", new object[] { p0 });

        internal static string InvalidRootComplexType(object p0, object p1) => 
            EntityRes.GetString("InvalidRootComplexType", new object[] { p0, p1 });

        internal static string InvalidRootRowType(object p0, object p1) => 
            EntityRes.GetString("InvalidRootRowType", new object[] { p0, p1 });

        internal static string InvalidRowType(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("InvalidRowType", new object[] { p0, p1, p2, p3 });

        internal static string InvalidSchemaEncountered(object p0) => 
            EntityRes.GetString("InvalidSchemaEncountered", new object[] { p0 });

        internal static string InvalidSecond(object p0, object p1) => 
            EntityRes.GetString("InvalidSecond", new object[] { p0, p1 });

        internal static string InvalidSimpleIdentifier(object p0) => 
            EntityRes.GetString("InvalidSimpleIdentifier", new object[] { p0 });

        internal static string InvalidSimpleIdentifierNonASCII(object p0) => 
            EntityRes.GetString("InvalidSimpleIdentifierNonASCII", new object[] { p0 });

        internal static string InvalidSize(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("InvalidSize", new object[] { p0, p1, p2, p3 });

        internal static string InvalidStringArgument(object p0) => 
            EntityRes.GetString("InvalidStringArgument", new object[] { p0 });

        internal static string InvalidToPropertyInRelationshipConstraint(object p0, object p1, object p2) => 
            EntityRes.GetString("InvalidToPropertyInRelationshipConstraint", new object[] { p0, p1, p2 });

        internal static string InvalidUnarySetOpArgument(object p0) => 
            EntityRes.GetString("InvalidUnarySetOpArgument", new object[] { p0 });

        internal static string InvalidUnsignedTypeForUnaryMinusOperation(object p0) => 
            EntityRes.GetString("InvalidUnsignedTypeForUnaryMinusOperation", new object[] { p0 });

        internal static string InvalidUseOfWebPath(object p0) => 
            EntityRes.GetString("InvalidUseOfWebPath", new object[] { p0 });

        internal static string InvalidValueForParameterTypeSemanticsAttribute(object p0) => 
            EntityRes.GetString("InvalidValueForParameterTypeSemanticsAttribute", new object[] { p0 });

        internal static string InvalidWithRelationshipTargetEndMultiplicity(object p0, object p1) => 
            EntityRes.GetString("InvalidWithRelationshipTargetEndMultiplicity", new object[] { p0, p1 });

        internal static string InvalidYear(object p0, object p1) => 
            EntityRes.GetString("InvalidYear", new object[] { p0, p1 });

        internal static string Iqt_General_UnsupportedOp(object p0) => 
            EntityRes.GetString("Iqt_General_UnsupportedOp", new object[] { p0 });

        internal static string ItemCollectionAlreadyRegistered(object p0) => 
            EntityRes.GetString("ItemCollectionAlreadyRegistered", new object[] { p0 });

        internal static string ItemDuplicateIdentity(object p0) => 
            EntityRes.GetString("ItemDuplicateIdentity", new object[] { p0 });

        internal static string ItemInvalidIdentity(object p0) => 
            EntityRes.GetString("ItemInvalidIdentity", new object[] { p0 });

        internal static string KeyMissingOnEntityType(object p0) => 
            EntityRes.GetString("KeyMissingOnEntityType", new object[] { p0 });

        internal static string KeyMustBeCorrelated(object p0) => 
            EntityRes.GetString("KeyMustBeCorrelated", new object[] { p0 });

        internal static string LiteralTypeNotFoundInMetadata(object p0) => 
            EntityRes.GetString("LiteralTypeNotFoundInMetadata", new object[] { p0 });

        internal static string LiteralTypeNotSupported(object p0) => 
            EntityRes.GetString("LiteralTypeNotSupported", new object[] { p0 });

        internal static string MalformedXml(object p0, object p1) => 
            EntityRes.GetString("MalformedXml", new object[] { p0, p1 });

        internal static string Mapping_AbstractTypeMappingToNonAbstractType(object p0, object p1) => 
            EntityRes.GetString("Mapping_AbstractTypeMappingToNonAbstractType", new object[] { p0, p1 });

        internal static string Mapping_AllQueryViewAtCompileTime(object p0) => 
            EntityRes.GetString("Mapping_AllQueryViewAtCompileTime", new object[] { p0 });

        internal static string Mapping_AlreadyMapped_StorageEntityContainer_1(object p0) => 
            EntityRes.GetString("Mapping_AlreadyMapped_StorageEntityContainer_1", new object[] { p0 });

        internal static string Mapping_CannotMapCLRTypeMultipleTimes(object p0) => 
            EntityRes.GetString("Mapping_CannotMapCLRTypeMultipleTimes", new object[] { p0 });

        internal static string Mapping_Default_OCMapping_Clr_Member_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_Default_OCMapping_Clr_Member_3", new object[] { p0, p1, p2 });

        internal static string Mapping_Default_OCMapping_Invalid_MemberType_6(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Mapping_Default_OCMapping_Invalid_MemberType_6", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Mapping_Default_OCMapping_Member_Count_Mismatch_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Default_OCMapping_Member_Count_Mismatch_2", new object[] { p0, p1 });

        internal static string Mapping_Default_OCMapping_Member_Type_Mismatch(object p0, object p1, object p2, object p3, object p4, object p5, object p6) => 
            EntityRes.GetString("Mapping_Default_OCMapping_Member_Type_Mismatch", new object[] { p0, p1, p2, p3, p4, p5, p6 });

        internal static string Mapping_Default_OCMapping_MemberKind_Mismatch_6(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Mapping_Default_OCMapping_MemberKind_Mismatch_6", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Mapping_Default_OCMapping_MultiplicityMismatch_6(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Mapping_Default_OCMapping_MultiplicityMismatch_6", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Mapping_Duplicate_CdmAssociationSet_StorageMap_1(object p0) => 
            EntityRes.GetString("Mapping_Duplicate_CdmAssociationSet_StorageMap_1", new object[] { p0 });

        internal static string Mapping_Duplicate_PropertyMap_CaseInsensitive(object p0) => 
            EntityRes.GetString("Mapping_Duplicate_PropertyMap_CaseInsensitive", new object[] { p0 });

        internal static string Mapping_Duplicate_Type_1(object p0) => 
            EntityRes.GetString("Mapping_Duplicate_Type_1", new object[] { p0 });

        internal static string Mapping_Empty_QueryView_1(object p0) => 
            EntityRes.GetString("Mapping_Empty_QueryView_1", new object[] { p0 });

        internal static string Mapping_Empty_QueryView_OfType_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Empty_QueryView_OfType_2", new object[] { p0, p1 });

        internal static string Mapping_Empty_QueryView_OfTypeOnly_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Empty_QueryView_OfTypeOnly_2", new object[] { p0, p1 });

        internal static string Mapping_Enum_EmptyValue_1(object p0) => 
            EntityRes.GetString("Mapping_Enum_EmptyValue_1", new object[] { p0 });

        internal static string Mapping_Enum_InvalidValue_1(object p0) => 
            EntityRes.GetString("Mapping_Enum_InvalidValue_1", new object[] { p0 });

        internal static string Mapping_FunctionImport_ConditionValueTypeMismatch(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_FunctionImport_ConditionValueTypeMismatch", new object[] { p0, p1, p2 });

        internal static string Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet", new object[] { p0, p1 });

        internal static string Mapping_FunctionImport_FunctionImportDoesNotExist(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_FunctionImportDoesNotExist", new object[] { p0, p1 });

        internal static string Mapping_FunctionImport_FunctionImportMappedMultipleTimes(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_FunctionImportMappedMultipleTimes", new object[] { p0 });

        internal static string Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter", new object[] { p0 });

        internal static string Mapping_FunctionImport_IncompatibleParameterMode(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_FunctionImport_IncompatibleParameterMode", new object[] { p0, p1, p2 });

        internal static string Mapping_FunctionImport_IncompatibleParameterType(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_FunctionImport_IncompatibleParameterType", new object[] { p0, p1, p2 });

        internal static string Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet", new object[] { p0, p1, p2, p3 });

        internal static string Mapping_FunctionImport_MultipleConditionsOnSingleColumn(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_MultipleConditionsOnSingleColumn", new object[] { p0 });

        internal static string Mapping_FunctionImport_RowsAffectedParameterDoesNotExist_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_RowsAffectedParameterDoesNotExist_2", new object[] { p0, p1 });

        internal static string Mapping_FunctionImport_RowsAffectedParameterHasWrongMode_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_FunctionImport_RowsAffectedParameterHasWrongMode_3", new object[] { p0, p1, p2 });

        internal static string Mapping_FunctionImport_RowsAffectedParameterHasWrongType_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_FunctionImport_RowsAffectedParameterHasWrongType_3", new object[] { p0, p1, p2 });

        internal static string Mapping_FunctionImport_StoreFunctionAmbiguous(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_StoreFunctionAmbiguous", new object[] { p0 });

        internal static string Mapping_FunctionImport_StoreFunctionDoesNotExist(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_StoreFunctionDoesNotExist", new object[] { p0 });

        internal static string Mapping_FunctionImport_TargetFunctionMustBeComposable(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_TargetFunctionMustBeComposable", new object[] { p0 });

        internal static string Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter(object p0) => 
            EntityRes.GetString("Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter", new object[] { p0 });

        internal static string Mapping_FunctionImport_UnreachableIsTypeOf(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_UnreachableIsTypeOf", new object[] { p0, p1 });

        internal static string Mapping_FunctionImport_UnreachableType(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_UnreachableType", new object[] { p0, p1 });

        internal static string Mapping_FunctionImport_UnsupportedType(object p0, object p1) => 
            EntityRes.GetString("Mapping_FunctionImport_UnsupportedType", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Association_Type_For_Association_Set_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_Invalid_Association_Type_For_Association_Set_3", new object[] { p0, p1, p2 });

        internal static string Mapping_Invalid_CSide_ScalarProperty_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_CSide_ScalarProperty_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AmbiguousFunction_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AmbiguousFunction_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AmbiguousResultBinding_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AmbiguousResultBinding_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationEndMappingInvalidForEntityType_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationEndMappingInvalidForEntityType_3", new object[] { p0, p1, p2 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetAmbiguous_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetAmbiguous_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetCardinality_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetCardinality_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetDoesNotExist_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetDoesNotExist_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetFromRoleIsNotEntitySet_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetFromRoleIsNotEntitySet_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetNotMappedForOperation_4(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetNotMappedForOperation_4", new object[] { p0, p1, p2, p3 });

        internal static string Mapping_Invalid_Function_Mapping_AssociationSetRoleDoesNotExist_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_AssociationSetRoleDoesNotExist_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_ComplexTypeNotFound_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_ComplexTypeNotFound_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_MissingEntityType_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_MissingEntityType_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_MissingParameter_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_MissingParameter_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Function_Mapping_MissingSetClosure_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_MissingSetClosure_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_MultipleEndsOfAssociationMapped_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_MultipleEndsOfAssociationMapped_3", new object[] { p0, p1, p2 });

        internal static string Mapping_Invalid_Function_Mapping_NotValidFunction_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_NotValidFunction_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_NotValidFunctionParameter_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_NotValidFunctionParameter_3", new object[] { p0, p1, p2 });

        internal static string Mapping_Invalid_Function_Mapping_ParameterBoundTwice_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_ParameterBoundTwice_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_ParameterNotFound_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_ParameterNotFound_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Function_Mapping_PropertyNotFound_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_PropertyNotFound_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Function_Mapping_PropertyNotKey_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_PropertyNotKey_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Function_Mapping_PropertyParameterTypeMismatch_6(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_PropertyParameterTypeMismatch_6", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Mapping_Invalid_Function_Mapping_RedundantEntityTypeMapping_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_RedundantEntityTypeMapping_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_UnknownFunction_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_UnknownFunction_1", new object[] { p0 });

        internal static string Mapping_Invalid_Function_Mapping_WrongComplexType_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_WrongComplexType_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_Member_Mapping_6(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Mapping_Invalid_Member_Mapping_6", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Mapping_Invalid_Query_Views_MissingSetClosure_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_Query_Views_MissingSetClosure_1", new object[] { p0 });

        internal static string Mapping_Invalid_QueryView_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_Invalid_QueryView_2", new object[] { p0, p1 });

        internal static string Mapping_Invalid_QueryView_Type_1(object p0) => 
            EntityRes.GetString("Mapping_Invalid_QueryView_Type_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_AbstractEntity_FunctionMapping_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_AbstractEntity_FunctionMapping_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_AbstractEntity_IsOfType_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_AbstractEntity_IsOfType_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_AbstractEntity_Type_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_AbstractEntity_Type_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Association_Set_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Association_Set_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Association_Type_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Association_Type_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_AssociationSet_Condition_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_AssociationSet_Condition_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Cdm_Member_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Cdm_Member_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Column_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Column_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Complex_Type_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Complex_Type_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_ConditionMapping_Computed(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_Computed", new object[] { p0 });

        internal static string Mapping_InvalidContent_ConditionMapping_InvalidMember_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_InvalidMember_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind_2", new object[] { p0, p1 });

        internal static string Mapping_InvalidContent_Duplicate_Condition_Member_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Duplicate_Condition_Member_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Emtpty_SetMap_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Emtpty_SetMap_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_End_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_End_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Entity_Set_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Entity_Set_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Entity_Type_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Entity_Type_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Entity_Type_For_Entity_Set_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_InvalidContent_Entity_Type_For_Entity_Set_3", new object[] { p0, p1, p2 });

        internal static string Mapping_InvalidContent_EntityContainer_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_EntityContainer_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_ImplicitMappingForAbstractReturnType_FunctionMapping_1(object p0, object p1) => 
            EntityRes.GetString("Mapping_InvalidContent_ImplicitMappingForAbstractReturnType_FunctionMapping_1", new object[] { p0, p1 });

        internal static string Mapping_InvalidContent_StorageEntityContainer_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_StorageEntityContainer_1", new object[] { p0 });

        internal static string Mapping_InvalidContent_Table_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidContent_Table_1", new object[] { p0 });

        internal static string Mapping_InvalidMappingSchema_Parsing_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidMappingSchema_Parsing_1", new object[] { p0 });

        internal static string Mapping_InvalidMappingSchema_validation_1(object p0) => 
            EntityRes.GetString("Mapping_InvalidMappingSchema_validation_1", new object[] { p0 });

        internal static string Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace(object p0) => 
            EntityRes.GetString("Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace", new object[] { p0 });

        internal static string Mapping_NotFound_EntityContainer(object p0) => 
            EntityRes.GetString("Mapping_NotFound_EntityContainer", new object[] { p0 });

        internal static string Mapping_Object_InvalidType(object p0) => 
            EntityRes.GetString("Mapping_Object_InvalidType", new object[] { p0 });

        internal static string Mapping_Provider_WrongConnectionType(object p0) => 
            EntityRes.GetString("Mapping_Provider_WrongConnectionType", new object[] { p0 });

        internal static string Mapping_Provider_WrongManifestType(object p0) => 
            EntityRes.GetString("Mapping_Provider_WrongManifestType", new object[] { p0 });

        internal static string Mapping_ProviderReturnsNullType(object p0) => 
            EntityRes.GetString("Mapping_ProviderReturnsNullType", new object[] { p0 });

        internal static string Mapping_QueryView_Duplicate_OfType(object p0, object p1) => 
            EntityRes.GetString("Mapping_QueryView_Duplicate_OfType", new object[] { p0, p1 });

        internal static string Mapping_QueryView_Duplicate_OfTypeOnly(object p0, object p1) => 
            EntityRes.GetString("Mapping_QueryView_Duplicate_OfTypeOnly", new object[] { p0, p1 });

        internal static string Mapping_QueryView_For_Base_Type(object p0, object p1) => 
            EntityRes.GetString("Mapping_QueryView_For_Base_Type", new object[] { p0, p1 });

        internal static string Mapping_QueryView_PropertyMaps_1(object p0) => 
            EntityRes.GetString("Mapping_QueryView_PropertyMaps_1", new object[] { p0 });

        internal static string Mapping_QueryView_TypeName_Not_Defined(object p0) => 
            EntityRes.GetString("Mapping_QueryView_TypeName_Not_Defined", new object[] { p0 });

        internal static string Mapping_QueryViewMultipleTypeInTypeName(object p0) => 
            EntityRes.GetString("Mapping_QueryViewMultipleTypeInTypeName", new object[] { p0 });

        internal static string Mapping_Storage_InvalidSpace_1(object p0) => 
            EntityRes.GetString("Mapping_Storage_InvalidSpace_1", new object[] { p0 });

        internal static string Mapping_StoreTypeMismatch_ScalarPropertyMapping_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_StoreTypeMismatch_ScalarPropertyMapping_2", new object[] { p0, p1 });

        internal static string Mapping_TableName_QueryView_1(object p0) => 
            EntityRes.GetString("Mapping_TableName_QueryView_1", new object[] { p0 });

        internal static string Mapping_UnsupportedExpressionKind_QueryView_2(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_UnsupportedExpressionKind_QueryView_2", new object[] { p0, p1, p2 });

        internal static string Mapping_UnsupportedInitialization_QueryView_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_UnsupportedInitialization_QueryView_2", new object[] { p0, p1 });

        internal static string Mapping_UnsupportedPropertyKind_QueryView_3(object p0, object p1, object p2) => 
            EntityRes.GetString("Mapping_UnsupportedPropertyKind_QueryView_3", new object[] { p0, p1, p2 });

        internal static string Mapping_UnsupportedScanTarget_QueryView_2(object p0, object p1) => 
            EntityRes.GetString("Mapping_UnsupportedScanTarget_QueryView_2", new object[] { p0, p1 });

        internal static string Mapping_Views_For_Extent_Not_Generated(object p0, object p1) => 
            EntityRes.GetString("Mapping_Views_For_Extent_Not_Generated", new object[] { p0, p1 });

        internal static string Materializer_AddedEntityAlreadyExists(object p0) => 
            EntityRes.GetString("Materializer_AddedEntityAlreadyExists", new object[] { p0 });

        internal static string Materializer_InvalidCastNullable(object p0, object p1) => 
            EntityRes.GetString("Materializer_InvalidCastNullable", new object[] { p0, p1 });

        internal static string Materializer_InvalidCastReference(object p0, object p1) => 
            EntityRes.GetString("Materializer_InvalidCastReference", new object[] { p0, p1 });

        internal static string Materializer_NullReferenceCast(object p0) => 
            EntityRes.GetString("Materializer_NullReferenceCast", new object[] { p0 });

        internal static string Materializer_PropertyIsNotNullableWithName(object p0) => 
            EntityRes.GetString("Materializer_PropertyIsNotNullableWithName", new object[] { p0 });

        internal static string Materializer_RecyclingEntity(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("Materializer_RecyclingEntity", new object[] { p0, p1, p2, p3 });

        internal static string Materializer_SetInvalidValue(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("Materializer_SetInvalidValue", new object[] { p0, p1, p2, p3 });

        internal static string Materializer_UnexpectedMetdataType(object p0) => 
            EntityRes.GetString("Materializer_UnexpectedMetdataType", new object[] { p0 });

        internal static string Materializer_UnsupportedCollectionType(object p0) => 
            EntityRes.GetString("Materializer_UnsupportedCollectionType", new object[] { p0 });

        internal static string MemberInvalidIdentity(object p0) => 
            EntityRes.GetString("MemberInvalidIdentity", new object[] { p0 });

        internal static string MinAndMaxMustBePositive(object p0, object p1) => 
            EntityRes.GetString("MinAndMaxMustBePositive", new object[] { p0, p1 });

        internal static string MinAndMaxValueMustBeDifferentForNonConstantFacet(object p0, object p1) => 
            EntityRes.GetString("MinAndMaxValueMustBeDifferentForNonConstantFacet", new object[] { p0, p1 });

        internal static string MinAndMaxValueMustBeSameForConstantFacet(object p0, object p1) => 
            EntityRes.GetString("MinAndMaxValueMustBeSameForConstantFacet", new object[] { p0, p1 });

        internal static string MinMustBeLessThanMax(object p0, object p1, object p2) => 
            EntityRes.GetString("MinMustBeLessThanMax", new object[] { p0, p1, p2 });

        internal static string MissingAssemblyAttribute(object p0, object p1) => 
            EntityRes.GetString("MissingAssemblyAttribute", new object[] { p0, p1 });

        internal static string MissingAssemblyResource(object p0) => 
            EntityRes.GetString("MissingAssemblyResource", new object[] { p0 });

        internal static string MissingConstraintOnRelationshipType(object p0) => 
            EntityRes.GetString("MissingConstraintOnRelationshipType", new object[] { p0 });

        internal static string MissingDefaultValueForConstantFacet(object p0, object p1) => 
            EntityRes.GetString("MissingDefaultValueForConstantFacet", new object[] { p0, p1 });

        internal static string MissingEntityContainerEnd(object p0, object p1) => 
            EntityRes.GetString("MissingEntityContainerEnd", new object[] { p0, p1 });

        internal static string MissingEntitySetName(object p0) => 
            EntityRes.GetString("MissingEntitySetName", new object[] { p0 });

        internal static string MissingFacetDescription(object p0, object p1, object p2) => 
            EntityRes.GetString("MissingFacetDescription", new object[] { p0, p1, p2 });

        internal static string MoreThanOneItemMatchesIdentity(object p0) => 
            EntityRes.GetString("MoreThanOneItemMatchesIdentity", new object[] { p0 });

        internal static string MultipleDefinitionsOfParameter(object p0) => 
            EntityRes.GetString("MultipleDefinitionsOfParameter", new object[] { p0 });

        internal static string MultipleDefinitionsOfVariable(object p0) => 
            EntityRes.GetString("MultipleDefinitionsOfVariable", new object[] { p0 });

        internal static string MultipleMatchesForName(object p0, object p1) => 
            EntityRes.GetString("MultipleMatchesForName", new object[] { p0, p1 });

        internal static string NamespaceAliasAlreadyUsed(object p0) => 
            EntityRes.GetString("NamespaceAliasAlreadyUsed", new object[] { p0 });

        internal static string NamespaceNameAlreadyDeclared(object p0) => 
            EntityRes.GetString("NamespaceNameAlreadyDeclared", new object[] { p0 });

        internal static string NavigationPropertyRelationshipEndTypeMismatch(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("NavigationPropertyRelationshipEndTypeMismatch", new object[] { p0, p1, p2, p3, p4 });

        internal static string NeedNotUseSystemNamespaceInUsing(object p0) => 
            EntityRes.GetString("NeedNotUseSystemNamespaceInUsing", new object[] { p0 });

        internal static string NestedClassNotSupported(object p0, object p1) => 
            EntityRes.GetString("NestedClassNotSupported", new object[] { p0, p1 });

        internal static string NewTypeConflictsWithExistingType(object p0, object p1) => 
            EntityRes.GetString("NewTypeConflictsWithExistingType", new object[] { p0, p1 });

        internal static string NoAggrFunctionOverloadMatch(object p0, object p1, object p2) => 
            EntityRes.GetString("NoAggrFunctionOverloadMatch", new object[] { p0, p1, p2 });

        internal static string NoCanonicalAggrFunctionOverloadMatch(object p0, object p1, object p2) => 
            EntityRes.GetString("NoCanonicalAggrFunctionOverloadMatch", new object[] { p0, p1, p2 });

        internal static string NoCanonicalFunctionOverloadMatch(object p0, object p1, object p2) => 
            EntityRes.GetString("NoCanonicalFunctionOverloadMatch", new object[] { p0, p1, p2 });

        internal static string NoCollectionForSpace(object p0) => 
            EntityRes.GetString("NoCollectionForSpace", new object[] { p0 });

        internal static string NoFunctionOverloadMatch(object p0, object p1, object p2) => 
            EntityRes.GetString("NoFunctionOverloadMatch", new object[] { p0, p1, p2 });

        internal static string NoStoreTypeForEdmType(object p0, object p1) => 
            EntityRes.GetString("NoStoreTypeForEdmType", new object[] { p0, p1 });

        internal static string NotAMemberOfCollection(object p0, object p1) => 
            EntityRes.GetString("NotAMemberOfCollection", new object[] { p0, p1 });

        internal static string NotAMemberOfType(object p0, object p1) => 
            EntityRes.GetString("NotAMemberOfType", new object[] { p0, p1 });

        internal static string NotASuperOrSubType(object p0, object p1) => 
            EntityRes.GetString("NotASuperOrSubType", new object[] { p0, p1 });

        internal static string NotInNamespaceAlias(object p0, object p1, object p2) => 
            EntityRes.GetString("NotInNamespaceAlias", new object[] { p0, p1, p2 });

        internal static string NotInNamespaceNoAlias(object p0, object p1) => 
            EntityRes.GetString("NotInNamespaceNoAlias", new object[] { p0, p1 });

        internal static string NotNamespaceQualified(object p0) => 
            EntityRes.GetString("NotNamespaceQualified", new object[] { p0 });

        internal static string NullableComplexType(object p0) => 
            EntityRes.GetString("NullableComplexType", new object[] { p0 });

        internal static string NullParameterForEdmRelationshipAttribute(object p0, object p1) => 
            EntityRes.GetString("NullParameterForEdmRelationshipAttribute", new object[] { p0, p1 });

        internal static string NullRelationshipNameforEdmRelationshipAttribute(object p0) => 
            EntityRes.GetString("NullRelationshipNameforEdmRelationshipAttribute", new object[] { p0 });

        internal static string NumberOfTypeCtorIsLessThenFormalSpec(object p0) => 
            EntityRes.GetString("NumberOfTypeCtorIsLessThenFormalSpec", new object[] { p0 });

        internal static string NumberOfTypeCtorIsMoreThenFormalSpec(object p0) => 
            EntityRes.GetString("NumberOfTypeCtorIsMoreThenFormalSpec", new object[] { p0 });

        internal static string ObjectContext_AcceptAllChangesFailure(object p0) => 
            EntityRes.GetString("ObjectContext_AcceptAllChangesFailure", new object[] { p0 });

        internal static string ObjectContext_ClientEntityRemovedFromStore(object p0) => 
            EntityRes.GetString("ObjectContext_ClientEntityRemovedFromStore", new object[] { p0 });

        internal static string ObjectContext_DoesNotImplementIEntityWithChangeTracker(object p0) => 
            EntityRes.GetString("ObjectContext_DoesNotImplementIEntityWithChangeTracker", new object[] { p0 });

        internal static string ObjectContext_EntitiesHaveDifferentType(object p0, object p1) => 
            EntityRes.GetString("ObjectContext_EntitiesHaveDifferentType", new object[] { p0, p1 });

        internal static string ObjectContext_EntityContainerNotFoundForName(object p0) => 
            EntityRes.GetString("ObjectContext_EntityContainerNotFoundForName", new object[] { p0 });

        internal static string ObjectContext_EntityMustBeUnchangedOrModified(object p0) => 
            EntityRes.GetString("ObjectContext_EntityMustBeUnchangedOrModified", new object[] { p0 });

        internal static string ObjectContext_EntitySetNotFoundForName(object p0) => 
            EntityRes.GetString("ObjectContext_EntitySetNotFoundForName", new object[] { p0 });

        internal static string ObjectContext_ExecuteFunctionCalledWithNonQueryFunction(object p0) => 
            EntityRes.GetString("ObjectContext_ExecuteFunctionCalledWithNonQueryFunction", new object[] { p0 });

        internal static string ObjectContext_ExecuteFunctionCalledWithNullParameter(object p0) => 
            EntityRes.GetString("ObjectContext_ExecuteFunctionCalledWithNullParameter", new object[] { p0 });

        internal static string ObjectContext_ExecuteFunctionCalledWithScalarFunction(object p0, object p1) => 
            EntityRes.GetString("ObjectContext_ExecuteFunctionCalledWithScalarFunction", new object[] { p0, p1 });

        internal static string ObjectContext_ExecuteFunctionTypeMismatch(object p0, object p1) => 
            EntityRes.GetString("ObjectContext_ExecuteFunctionTypeMismatch", new object[] { p0, p1 });

        internal static string ObjectContext_InvalidDefaultContainerName(object p0) => 
            EntityRes.GetString("ObjectContext_InvalidDefaultContainerName", new object[] { p0 });

        internal static string ObjectContext_InvalidEntitySetInKey(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("ObjectContext_InvalidEntitySetInKey", new object[] { p0, p1, p2, p3 });

        internal static string ObjectContext_InvalidEntitySetInKeyFromName(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("ObjectContext_InvalidEntitySetInKeyFromName", new object[] { p0, p1, p2, p3, p4 });

        internal static string ObjectContext_InvalidEntitySetOnEntity(object p0, object p1) => 
            EntityRes.GetString("ObjectContext_InvalidEntitySetOnEntity", new object[] { p0, p1 });

        internal static string ObjectContext_NoMappingForEntityType(object p0) => 
            EntityRes.GetString("ObjectContext_NoMappingForEntityType", new object[] { p0 });

        internal static string ObjectContext_NonEntityType(object p0) => 
            EntityRes.GetString("ObjectContext_NonEntityType", new object[] { p0 });

        internal static string ObjectContext_NthElementInAddedState(object p0) => 
            EntityRes.GetString("ObjectContext_NthElementInAddedState", new object[] { p0 });

        internal static string ObjectContext_NthElementIsDuplicate(object p0) => 
            EntityRes.GetString("ObjectContext_NthElementIsDuplicate", new object[] { p0 });

        internal static string ObjectContext_NthElementIsNull(object p0) => 
            EntityRes.GetString("ObjectContext_NthElementIsNull", new object[] { p0 });

        internal static string ObjectContext_NthElementNotInObjectStateManager(object p0) => 
            EntityRes.GetString("ObjectContext_NthElementNotInObjectStateManager", new object[] { p0 });

        internal static string ObjectParameter_InvalidParameterName(object p0) => 
            EntityRes.GetString("ObjectParameter_InvalidParameterName", new object[] { p0 });

        internal static string ObjectParameter_InvalidParameterType(object p0) => 
            EntityRes.GetString("ObjectParameter_InvalidParameterType", new object[] { p0 });

        internal static string ObjectParameterCollection_DuplicateParameterName(object p0) => 
            EntityRes.GetString("ObjectParameterCollection_DuplicateParameterName", new object[] { p0 });

        internal static string ObjectParameterCollection_ParameterAlreadyExists(object p0) => 
            EntityRes.GetString("ObjectParameterCollection_ParameterAlreadyExists", new object[] { p0 });

        internal static string ObjectParameterCollection_ParameterNameNotFound(object p0) => 
            EntityRes.GetString("ObjectParameterCollection_ParameterNameNotFound", new object[] { p0 });

        internal static string ObjectQuery_InvalidQueryName(object p0) => 
            EntityRes.GetString("ObjectQuery_InvalidQueryName", new object[] { p0 });

        internal static string ObjectQuery_QueryBuilder_InvalidResultType(object p0) => 
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidResultType", new object[] { p0 });

        internal static string ObjectQuery_Span_NoNavProp(object p0, object p1) => 
            EntityRes.GetString("ObjectQuery_Span_NoNavProp", new object[] { p0, p1 });

        internal static string ObjectStateEntry_CannotModifyKeyProperty(object p0) => 
            EntityRes.GetString("ObjectStateEntry_CannotModifyKeyProperty", new object[] { p0 });

        internal static string ObjectStateEntry_ChangedInDifferentStateFromChanging(object p0, object p1) => 
            EntityRes.GetString("ObjectStateEntry_ChangedInDifferentStateFromChanging", new object[] { p0, p1 });

        internal static string ObjectStateEntry_ChangeOnUnmappedComplexProperty(object p0) => 
            EntityRes.GetString("ObjectStateEntry_ChangeOnUnmappedComplexProperty", new object[] { p0 });

        internal static string ObjectStateEntry_ChangeOnUnmappedProperty(object p0) => 
            EntityRes.GetString("ObjectStateEntry_ChangeOnUnmappedProperty", new object[] { p0 });

        internal static string ObjectStateEntry_SetModifiedOnInvalidProperty(object p0) => 
            EntityRes.GetString("ObjectStateEntry_SetModifiedOnInvalidProperty", new object[] { p0 });

        internal static string ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(object p0) => 
            EntityRes.GetString("ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity", new object[] { p0 });

        internal static string ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType(object p0, object p1) => 
            EntityRes.GetString("ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType", new object[] { p0, p1 });

        internal static string ObjectStateManager_GetEntityKeyRequiresObjectToHaveAKey(object p0) => 
            EntityRes.GetString("ObjectStateManager_GetEntityKeyRequiresObjectToHaveAKey", new object[] { p0 });

        internal static string ObjectStateManager_NoEntryExistsForObject(object p0) => 
            EntityRes.GetString("ObjectStateManager_NoEntryExistsForObject", new object[] { p0 });

        internal static string ObjectView_CannotResolveTheEntitySet(object p0) => 
            EntityRes.GetString("ObjectView_CannotResolveTheEntitySet", new object[] { p0 });

        internal static string OfTypeExpressionElementTypeMustBeEntityType(object p0, object p1) => 
            EntityRes.GetString("OfTypeExpressionElementTypeMustBeEntityType", new object[] { p0, p1 });

        internal static string OfTypeExpressionElementTypeMustBeNominalType(object p0, object p1) => 
            EntityRes.GetString("OfTypeExpressionElementTypeMustBeNominalType", new object[] { p0, p1 });

        internal static string OfTypeOnlyTypeArgumentCannotBeAbstract(object p0) => 
            EntityRes.GetString("OfTypeOnlyTypeArgumentCannotBeAbstract", new object[] { p0 });

        internal static string ParameterNameAlreadyDefinedDuplicate(object p0) => 
            EntityRes.GetString("ParameterNameAlreadyDefinedDuplicate", new object[] { p0 });

        internal static string ParameterWasNotDefined(object p0) => 
            EntityRes.GetString("ParameterWasNotDefined", new object[] { p0 });

        internal static string PlaceholderExpressionMustBeCompatibleWithEdm64(object p0, object p1) => 
            EntityRes.GetString("PlaceholderExpressionMustBeCompatibleWithEdm64", new object[] { p0, p1 });

        internal static string PlaceholderExpressionMustBeGreaterThanOrEqualToZero(object p0) => 
            EntityRes.GetString("PlaceholderExpressionMustBeGreaterThanOrEqualToZero", new object[] { p0 });

        internal static string PlaceholderSetArgTypeIsNotEqualComparable(object p0, object p1, object p2) => 
            EntityRes.GetString("PlaceholderSetArgTypeIsNotEqualComparable", new object[] { p0, p1, p2 });

        internal static string PrecisionMoreThanAllowedMax(object p0, object p1) => 
            EntityRes.GetString("PrecisionMoreThanAllowedMax", new object[] { p0, p1 });

        internal static string PrecisionOutOfRange(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("PrecisionOutOfRange", new object[] { p0, p1, p2, p3 });

        internal static string PrimitiveTypeNotSupportedPriorSql10(object p0) => 
            EntityRes.GetString("PrimitiveTypeNotSupportedPriorSql10", new object[] { p0 });

        internal static string PropertyNameAlreadyDefinedDuplicate(object p0) => 
            EntityRes.GetString("PropertyNameAlreadyDefinedDuplicate", new object[] { p0 });

        internal static string PropertyTypeAlreadyDefined(object p0) => 
            EntityRes.GetString("PropertyTypeAlreadyDefined", new object[] { p0 });

        internal static string ProviderDoesNotSupportType(object p0) => 
            EntityRes.GetString("ProviderDoesNotSupportType", new object[] { p0 });

        internal static string ProviderReturnedNullForGetDbInformation(object p0) => 
            EntityRes.GetString("ProviderReturnedNullForGetDbInformation", new object[] { p0 });

        internal static string RefArgIsNotOfEntityType(object p0) => 
            EntityRes.GetString("RefArgIsNotOfEntityType", new object[] { p0 });

        internal static string RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(object p0) => 
            EntityRes.GetString("RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities", new object[] { p0 });

        internal static string RelatedEnd_EntitySetIsNotValidForRelationship(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("RelatedEnd_EntitySetIsNotValidForRelationship", new object[] { p0, p1, p2, p3, p4 });

        internal static string RelatedEnd_InvalidContainedType_Collection(object p0, object p1) => 
            EntityRes.GetString("RelatedEnd_InvalidContainedType_Collection", new object[] { p0, p1 });

        internal static string RelatedEnd_InvalidContainedType_Reference(object p0, object p1) => 
            EntityRes.GetString("RelatedEnd_InvalidContainedType_Reference", new object[] { p0, p1 });

        internal static string RelatedEnd_InvalidNthElementContextForAttach(object p0) => 
            EntityRes.GetString("RelatedEnd_InvalidNthElementContextForAttach", new object[] { p0 });

        internal static string RelatedEnd_InvalidNthElementNullForAttach(object p0) => 
            EntityRes.GetString("RelatedEnd_InvalidNthElementNullForAttach", new object[] { p0 });

        internal static string RelatedEnd_InvalidNthElementStateForAttach(object p0) => 
            EntityRes.GetString("RelatedEnd_InvalidNthElementStateForAttach", new object[] { p0 });

        internal static string RelatedEnd_MismatchedMergeOptionOnLoad(object p0) => 
            EntityRes.GetString("RelatedEnd_MismatchedMergeOptionOnLoad", new object[] { p0 });

        internal static string RelatedEnd_RelatedEndNotAttachedToContext(object p0) => 
            EntityRes.GetString("RelatedEnd_RelatedEndNotAttachedToContext", new object[] { p0 });

        internal static string RelationshipManager_CannotNavigateRelationshipForDetachedEntityWithoutKey(object p0) => 
            EntityRes.GetString("RelationshipManager_CannotNavigateRelationshipForDetachedEntityWithoutKey", new object[] { p0 });

        internal static string RelationshipManager_CollectionAlreadyInitialized(object p0) => 
            EntityRes.GetString("RelationshipManager_CollectionAlreadyInitialized", new object[] { p0 });

        internal static string RelationshipManager_CollectionRelationshipManagerAttached(object p0) => 
            EntityRes.GetString("RelationshipManager_CollectionRelationshipManagerAttached", new object[] { p0 });

        internal static string RelationshipManager_InvalidTargetRole(object p0, object p1) => 
            EntityRes.GetString("RelationshipManager_InvalidTargetRole", new object[] { p0, p1 });

        internal static string RelationshipManager_OwnerIsNotSourceType(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("RelationshipManager_OwnerIsNotSourceType", new object[] { p0, p1, p2, p3 });

        internal static string RelationshipManager_ReferenceAlreadyInitialized(object p0) => 
            EntityRes.GetString("RelationshipManager_ReferenceAlreadyInitialized", new object[] { p0 });

        internal static string RelationshipManager_RelationshipManagerAttached(object p0) => 
            EntityRes.GetString("RelationshipManager_RelationshipManagerAttached", new object[] { p0 });

        internal static string RelationshipManager_UnableToFindRelationshipTypeInMetadata(object p0) => 
            EntityRes.GetString("RelationshipManager_UnableToFindRelationshipTypeInMetadata", new object[] { p0 });

        internal static string RelationshipNameInNavigationPropertyNotValid(object p0, object p1, object p2) => 
            EntityRes.GetString("RelationshipNameInNavigationPropertyNotValid", new object[] { p0, p1, p2 });

        internal static string RelationshipTargetMustBeUnique(object p0) => 
            EntityRes.GetString("RelationshipTargetMustBeUnique", new object[] { p0 });

        internal static string RelationshipTypeIsNotCompatibleWithEntity(object p0, object p1) => 
            EntityRes.GetString("RelationshipTypeIsNotCompatibleWithEntity", new object[] { p0, p1 });

        internal static string RequiredFacetMissing(object p0, object p1) => 
            EntityRes.GetString("RequiredFacetMissing", new object[] { p0, p1 });

        internal static string RoleTypeInEdmRelationshipAttributeIsInvalidType(object p0, object p1, object p2) => 
            EntityRes.GetString("RoleTypeInEdmRelationshipAttributeIsInvalidType", new object[] { p0, p1, p2 });

        internal static string SameRoleNameOnRelationshipAttribute(object p0, object p1) => 
            EntityRes.GetString("SameRoleNameOnRelationshipAttribute", new object[] { p0, p1 });

        internal static string SameRoleReferredInReferentialConstraint(object p0) => 
            EntityRes.GetString("SameRoleReferredInReferentialConstraint", new object[] { p0 });

        internal static string ScaleOutOfRange(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("ScaleOutOfRange", new object[] { p0, p1, p2, p3 });

        internal static string SimilarRelationshipEnd(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("SimilarRelationshipEnd", new object[] { p0, p1, p2, p3, p4 });

        internal static string SourceTypeMustBePromotoableToFromEndRelationType(object p0, object p1) => 
            EntityRes.GetString("SourceTypeMustBePromotoableToFromEndRelationType", new object[] { p0, p1 });

        internal static string StaticMembersAreNotSupported(object p0, object p1) => 
            EntityRes.GetString("StaticMembersAreNotSupported", new object[] { p0, p1 });

        internal static string StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping(object p0, object p1, object p2) => 
            EntityRes.GetString("StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping", new object[] { p0, p1, p2 });

        internal static string StoreItemCollectionMustHaveOneArtifact(object p0) => 
            EntityRes.GetString("StoreItemCollectionMustHaveOneArtifact", new object[] { p0 });

        internal static string SystemNamespaceEncountered(object p0) => 
            EntityRes.GetString("SystemNamespaceEncountered", new object[] { p0 });

        internal static string TableAndSchemaAreMutuallyExclusiveWithDefiningQuery(object p0) => 
            EntityRes.GetString("TableAndSchemaAreMutuallyExclusiveWithDefiningQuery", new object[] { p0 });

        internal static string TargetRoleNameInNavigationPropertyNotValid(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("TargetRoleNameInNavigationPropertyNotValid", new object[] { p0, p1, p2, p3 });

        internal static string TextNotAllowed(object p0) => 
            EntityRes.GetString("TextNotAllowed", new object[] { p0 });

        internal static string TooManyAssociationEnds(object p0) => 
            EntityRes.GetString("TooManyAssociationEnds", new object[] { p0 });

        internal static string TypeDoesNotSupportParameters(object p0) => 
            EntityRes.GetString("TypeDoesNotSupportParameters", new object[] { p0 });

        internal static string TypeDoesNotSupportPrecisionOrScale(object p0, object p1) => 
            EntityRes.GetString("TypeDoesNotSupportPrecisionOrScale", new object[] { p0, p1 });

        internal static string TypeKindMismatch(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("TypeKindMismatch", new object[] { p0, p1, p2, p3 });

        internal static string TypeMismatchRelationshipConstaint(object p0, object p1) => 
            EntityRes.GetString("TypeMismatchRelationshipConstaint", new object[] { p0, p1 });

        internal static string TypeMustBeEntityType(object p0, object p1, object p2) => 
            EntityRes.GetString("TypeMustBeEntityType", new object[] { p0, p1, p2 });

        internal static string TypeMustBeNominalType(object p0, object p1, object p2) => 
            EntityRes.GetString("TypeMustBeNominalType", new object[] { p0, p1, p2 });

        internal static string TypeNameAlreadyDefinedDuplicate(object p0) => 
            EntityRes.GetString("TypeNameAlreadyDefinedDuplicate", new object[] { p0 });

        internal static string TypeNameNotFound(object p0) => 
            EntityRes.GetString("TypeNameNotFound", new object[] { p0 });

        internal static string TypeNotInAssociationSet(object p0, object p1, object p2) => 
            EntityRes.GetString("TypeNotInAssociationSet", new object[] { p0, p1, p2 });

        internal static string TypeNotInEntitySet(object p0, object p1, object p2) => 
            EntityRes.GetString("TypeNotInEntitySet", new object[] { p0, p1, p2 });

        internal static string TypeSpecBellowMin(object p0) => 
            EntityRes.GetString("TypeSpecBellowMin", new object[] { p0 });

        internal static string TypeSpecExceedsMax(object p0) => 
            EntityRes.GetString("TypeSpecExceedsMax", new object[] { p0 });

        internal static string UnableToFindReflectedType(object p0, object p1) => 
            EntityRes.GetString("UnableToFindReflectedType", new object[] { p0, p1 });

        internal static string UnableToResolveAssembly(object p0) => 
            EntityRes.GetString("UnableToResolveAssembly", new object[] { p0 });

        internal static string UnacceptableUri(object p0) => 
            EntityRes.GetString("UnacceptableUri", new object[] { p0 });

        internal static string UnexpectedRootElement(object p0, object p1, object p2) => 
            EntityRes.GetString("UnexpectedRootElement", new object[] { p0, p1, p2 });

        internal static string UnexpectedRootElementNoNamespace(object p0, object p1, object p2) => 
            EntityRes.GetString("UnexpectedRootElementNoNamespace", new object[] { p0, p1, p2 });

        internal static string UnexpectedTypeInCollection(object p0, object p1) => 
            EntityRes.GetString("UnexpectedTypeInCollection", new object[] { p0, p1 });

        internal static string UnexpectedXmlAttribute(object p0) => 
            EntityRes.GetString("UnexpectedXmlAttribute", new object[] { p0 });

        internal static string UnexpectedXmlElement(object p0) => 
            EntityRes.GetString("UnexpectedXmlElement", new object[] { p0 });

        internal static string UnexpectedXmlNodeType(object p0) => 
            EntityRes.GetString("UnexpectedXmlNodeType", new object[] { p0 });

        internal static string Update_ConcurrencyError(object p0) => 
            EntityRes.GetString("Update_ConcurrencyError", new object[] { p0 });

        internal static string Update_GeneratedDependent(object p0) => 
            EntityRes.GetString("Update_GeneratedDependent", new object[] { p0 });

        internal static string Update_MappingNotFound(object p0) => 
            EntityRes.GetString("Update_MappingNotFound", new object[] { p0 });

        internal static string Update_MissingEntity(object p0, object p1) => 
            EntityRes.GetString("Update_MissingEntity", new object[] { p0, p1 });

        internal static string Update_MissingModifiedProperty(object p0, object p1) => 
            EntityRes.GetString("Update_MissingModifiedProperty", new object[] { p0, p1 });

        internal static string Update_MissingRequiredEntity(object p0, object p1) => 
            EntityRes.GetString("Update_MissingRequiredEntity", new object[] { p0, p1 });

        internal static string Update_MissingRequiredRelationshipValue(object p0, object p1) => 
            EntityRes.GetString("Update_MissingRequiredRelationshipValue", new object[] { p0, p1 });

        internal static string Update_MissingResultColumn(object p0) => 
            EntityRes.GetString("Update_MissingResultColumn", new object[] { p0 });

        internal static string Update_ModifyingIdentityColumn(object p0, object p1, object p2) => 
            EntityRes.GetString("Update_ModifyingIdentityColumn", new object[] { p0, p1, p2 });

        internal static string Update_NonEquatableColumnTypeInClause(object p0, object p1) => 
            EntityRes.GetString("Update_NonEquatableColumnTypeInClause", new object[] { p0, p1 });

        internal static string Update_NotSupportedComputedKeyColumn(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("Update_NotSupportedComputedKeyColumn", new object[] { p0, p1, p2, p3, p4 });

        internal static string Update_NotSupportedIdentityType(object p0, object p1) => 
            EntityRes.GetString("Update_NotSupportedIdentityType", new object[] { p0, p1 });

        internal static string Update_NotSupportedServerGenKey(object p0) => 
            EntityRes.GetString("Update_NotSupportedServerGenKey", new object[] { p0 });

        internal static string Update_NullReturnValueForNonNullableMember(object p0, object p1) => 
            EntityRes.GetString("Update_NullReturnValueForNonNullableMember", new object[] { p0, p1 });

        internal static string Update_NullValue(object p0) => 
            EntityRes.GetString("Update_NullValue", new object[] { p0 });

        internal static string Update_RelationshipCardinalityConstraintViolation(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Update_RelationshipCardinalityConstraintViolation", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Update_RelationshipCardinalityConstraintViolationSingleValue(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("Update_RelationshipCardinalityConstraintViolationSingleValue", new object[] { p0, p1, p2, p3, p4 });

        internal static string Update_RelationshipCardinalityViolation(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("Update_RelationshipCardinalityViolation", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string Update_ReturnValueHasUnexpectedType(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("Update_ReturnValueHasUnexpectedType", new object[] { p0, p1, p2, p3 });

        internal static string Update_SqlEntitySetWithoutDmlFunctions(object p0, object p1, object p2) => 
            EntityRes.GetString("Update_SqlEntitySetWithoutDmlFunctions", new object[] { p0, p1, p2 });

        internal static string Update_UnableToConvertRowsAffectedParameterToInt32(object p0, object p1) => 
            EntityRes.GetString("Update_UnableToConvertRowsAffectedParameterToInt32", new object[] { p0, p1 });

        internal static string Update_UnsupportedCastArgument(object p0) => 
            EntityRes.GetString("Update_UnsupportedCastArgument", new object[] { p0 });

        internal static string Update_UnsupportedExpressionKind(object p0, object p1) => 
            EntityRes.GetString("Update_UnsupportedExpressionKind", new object[] { p0, p1 });

        internal static string Update_UnsupportedExtentType(object p0, object p1) => 
            EntityRes.GetString("Update_UnsupportedExtentType", new object[] { p0, p1 });

        internal static string Update_UnsupportedJoinType(object p0) => 
            EntityRes.GetString("Update_UnsupportedJoinType", new object[] { p0 });

        internal static string Update_UnsupportedProjection(object p0) => 
            EntityRes.GetString("Update_UnsupportedProjection", new object[] { p0 });

        internal static string Validator_NavPropWithoutIEntityWithRelationships(object p0, object p1) => 
            EntityRes.GetString("Validator_NavPropWithoutIEntityWithRelationships", new object[] { p0, p1 });

        internal static string Validator_NullableEntityKeyProperty(object p0, object p1) => 
            EntityRes.GetString("Validator_NullableEntityKeyProperty", new object[] { p0, p1 });

        internal static string Validator_OSpace_ComplexPropertyNotComplex(object p0, object p1, object p2) => 
            EntityRes.GetString("Validator_OSpace_ComplexPropertyNotComplex", new object[] { p0, p1, p2 });

        internal static string Validator_OSpace_InvalidNavPropReturnType(object p0, object p1, object p2) => 
            EntityRes.GetString("Validator_OSpace_InvalidNavPropReturnType", new object[] { p0, p1, p2 });

        internal static string Validator_OSpace_ScalarPropertyNotPrimitive(object p0, object p1, object p2) => 
            EntityRes.GetString("Validator_OSpace_ScalarPropertyNotPrimitive", new object[] { p0, p1, p2 });

        internal static string ValueNotUnderstood(object p0, object p1) => 
            EntityRes.GetString("ValueNotUnderstood", new object[] { p0, p1 });

        internal static string ViewGen_AssociationSet_AsUserString(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_AssociationSet_AsUserString", new object[] { p0, p1, p2 });

        internal static string ViewGen_AssociationSet_AsUserString_Negated(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_AssociationSet_AsUserString_Negated", new object[] { p0, p1, p2 });

        internal static string ViewGen_AssociationSetKey_Missing_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_AssociationSetKey_Missing_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_Cannot_Disambiguate_MultiConstant_2(object p0, object p1) => 
            EntityRes.GetString("ViewGen_Cannot_Disambiguate_MultiConstant_2", new object[] { p0, p1 });

        internal static string ViewGen_Cannot_Recover_Attributes_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_Cannot_Recover_Attributes_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_Cannot_Recover_Types_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_Cannot_Recover_Types_1", new object[] { p0, p1 });

        internal static string ViewGen_Concurrency_Derived_Class_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_Concurrency_Derived_Class_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_Concurrency_Invalid_Condition_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_Concurrency_Invalid_Condition_1", new object[] { p0, p1 });

        internal static string Viewgen_ConfigurationErrorMsg(object p0) => 
            EntityRes.GetString("Viewgen_ConfigurationErrorMsg", new object[] { p0 });

        internal static string ViewGen_CQ_DomainConstraint_1(object p0) => 
            EntityRes.GetString("ViewGen_CQ_DomainConstraint_1", new object[] { p0 });

        internal static string ViewGen_CQ_PartitionConstraint_1(object p0) => 
            EntityRes.GetString("ViewGen_CQ_PartitionConstraint_1", new object[] { p0 });

        internal static string ViewGen_Duplicate_CProperties_0(object p0) => 
            EntityRes.GetString("ViewGen_Duplicate_CProperties_0", new object[] { p0 });

        internal static string ViewGen_Duplicate_CProperties_IsMapped_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_Duplicate_CProperties_IsMapped_1", new object[] { p0, p1 });

        internal static string ViewGen_EntitySet_AsUserString(object p0, object p1) => 
            EntityRes.GetString("ViewGen_EntitySet_AsUserString", new object[] { p0, p1 });

        internal static string ViewGen_EntitySet_AsUserString_Negated(object p0, object p1) => 
            EntityRes.GetString("ViewGen_EntitySet_AsUserString_Negated", new object[] { p0, p1 });

        internal static string ViewGen_EntitySetKey_Missing_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_EntitySetKey_Missing_1", new object[] { p0, p1 });

        internal static string ViewGen_ErrorLog_0(object p0) => 
            EntityRes.GetString("ViewGen_ErrorLog_0", new object[] { p0 });

        internal static string ViewGen_ErrorLog_1(object p0) => 
            EntityRes.GetString("ViewGen_ErrorLog_1", new object[] { p0 });

        internal static string Viewgen_ErrorPattern_ConditionMemberIsMapped(object p0) => 
            EntityRes.GetString("Viewgen_ErrorPattern_ConditionMemberIsMapped", new object[] { p0 });

        internal static string Viewgen_ErrorPattern_DuplicateConditionValue(object p0) => 
            EntityRes.GetString("Viewgen_ErrorPattern_DuplicateConditionValue", new object[] { p0 });

        internal static string Viewgen_ErrorPattern_TableMappedToMultipleES(object p0, object p1, object p2) => 
            EntityRes.GetString("Viewgen_ErrorPattern_TableMappedToMultipleES", new object[] { p0, p1, p2 });

        internal static string ViewGen_Foreign_Key_4(object p0, object p1, object p2, object p3, object p4) => 
            EntityRes.GetString("ViewGen_Foreign_Key_4", new object[] { p0, p1, p2, p3, p4 });

        internal static string ViewGen_Foreign_Key_ColumnOrder_Incorrect_8(object p0, object p1, object p2, object p3, object p4, object p5, object p6) => 
            EntityRes.GetString("ViewGen_Foreign_Key_ColumnOrder_Incorrect_8", new object[] { p0, p1, p2, p3, p4, p5, p6 });

        internal static string ViewGen_Foreign_Key_LowerBound_MustBeOne_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_Foreign_Key_LowerBound_MustBeOne_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_Foreign_Key_Missing_Relationship_Mapping_0(object p0) => 
            EntityRes.GetString("ViewGen_Foreign_Key_Missing_Relationship_Mapping_0", new object[] { p0 });

        internal static string ViewGen_Foreign_Key_Missing_Table_Mapping_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_Foreign_Key_Missing_Table_Mapping_1", new object[] { p0, p1 });

        internal static string ViewGen_Foreign_Key_Not_Guaranteed_InCSpace_0(object p0) => 
            EntityRes.GetString("ViewGen_Foreign_Key_Not_Guaranteed_InCSpace_0", new object[] { p0 });

        internal static string ViewGen_Foreign_Key_ParentTable_NotMappedToEnd_5(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("ViewGen_Foreign_Key_ParentTable_NotMappedToEnd_5", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string ViewGen_Foreign_Key_UpperBound_MustBeOne_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_Foreign_Key_UpperBound_MustBeOne_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_HashOnMappingClosure_Not_Matching(object p0) => 
            EntityRes.GetString("ViewGen_HashOnMappingClosure_Not_Matching", new object[] { p0 });

        internal static string ViewGen_InputCells_NotIsolated_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_InputCells_NotIsolated_1", new object[] { p0, p1 });

        internal static string ViewGen_InvalidCondition_0(object p0) => 
            EntityRes.GetString("ViewGen_InvalidCondition_0", new object[] { p0 });

        internal static string ViewGen_KeyConstraint_Update_Violation_AssociationSet_2(object p0, object p1, object p2) => 
            EntityRes.GetString("ViewGen_KeyConstraint_Update_Violation_AssociationSet_2", new object[] { p0, p1, p2 });

        internal static string ViewGen_KeyConstraint_Update_Violation_EntitySet_3(object p0, object p1, object p2, object p3) => 
            EntityRes.GetString("ViewGen_KeyConstraint_Update_Violation_EntitySet_3", new object[] { p0, p1, p2, p3 });

        internal static string ViewGen_KeyConstraint_Violation_5(object p0, object p1, object p2, object p3, object p4, object p5) => 
            EntityRes.GetString("ViewGen_KeyConstraint_Violation_5", new object[] { p0, p1, p2, p3, p4, p5 });

        internal static string ViewGen_Missing_Set_Mapping_0(object p0) => 
            EntityRes.GetString("ViewGen_Missing_Set_Mapping_0", new object[] { p0 });

        internal static string ViewGen_Missing_Sets_Mapping_0(object p0) => 
            EntityRes.GetString("ViewGen_Missing_Sets_Mapping_0", new object[] { p0 });

        internal static string ViewGen_Missing_Type_Mapping_0(object p0) => 
            EntityRes.GetString("ViewGen_Missing_Type_Mapping_0", new object[] { p0 });

        internal static string ViewGen_NegatedCellConstant_0(object p0) => 
            EntityRes.GetString("ViewGen_NegatedCellConstant_0", new object[] { p0 });

        internal static string ViewGen_No_Default_Value_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_No_Default_Value_1", new object[] { p0, p1 });

        internal static string ViewGen_No_Default_Value_For_Configuration_0(object p0) => 
            EntityRes.GetString("ViewGen_No_Default_Value_For_Configuration_0", new object[] { p0 });

        internal static string ViewGen_NonKeyProjectedWithOverlappingPartitions_0(object p0) => 
            EntityRes.GetString("ViewGen_NonKeyProjectedWithOverlappingPartitions_0", new object[] { p0 });

        internal static string ViewGen_NotNull_No_Projected_Slot_0(object p0) => 
            EntityRes.GetString("ViewGen_NotNull_No_Projected_Slot_0", new object[] { p0 });

        internal static string Viewgen_NullableMappingForNonNullableColumn(object p0, object p1) => 
            EntityRes.GetString("Viewgen_NullableMappingForNonNullableColumn", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_IsEqualTo_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsEqualTo_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_IsNonNullable_0(object p0) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsNonNullable_0", new object[] { p0 });

        internal static string ViewGen_OneOfConst_IsNotEqualTo_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsNotEqualTo_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_IsNotOneOf_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsNotOneOf_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_IsOneOf_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsOneOf_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_IsOneOfTypes_0(object p0) => 
            EntityRes.GetString("ViewGen_OneOfConst_IsOneOfTypes_0", new object[] { p0 });

        internal static string ViewGen_OneOfConst_MustBeEqualTo_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustBeEqualTo_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_MustBeNonNullable_0(object p0) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustBeNonNullable_0", new object[] { p0 });

        internal static string ViewGen_OneOfConst_MustBeNull_0(object p0) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustBeNull_0", new object[] { p0 });

        internal static string ViewGen_OneOfConst_MustBeOneOf_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustBeOneOf_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_MustNotBeEqualTo_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustNotBeEqualTo_1", new object[] { p0, p1 });

        internal static string ViewGen_OneOfConst_MustNotBeOneOf_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_OneOfConst_MustNotBeOneOf_1", new object[] { p0, p1 });

        internal static string Viewgen_QV_RewritingNotFound(object p0) => 
            EntityRes.GetString("Viewgen_QV_RewritingNotFound", new object[] { p0 });

        internal static string Viewgen_RightSideNotDisjoint(object p0) => 
            EntityRes.GetString("Viewgen_RightSideNotDisjoint", new object[] { p0 });

        internal static string ViewGen_SlotNumber_Mismatch_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_SlotNumber_Mismatch_1", new object[] { p0, p1 });

        internal static string ViewGen_TableKey_Missing_1(object p0, object p1) => 
            EntityRes.GetString("ViewGen_TableKey_Missing_1", new object[] { p0, p1 });

        internal static string ADP_ClosedDataReaderError =>
            EntityRes.GetString("ADP_ClosedDataReaderError");

        internal static string ADP_EntitySetForNonEntityType =>
            EntityRes.GetString("ADP_EntitySetForNonEntityType");

        internal static string ADP_GetSchemaTableIsNotSupported =>
            EntityRes.GetString("ADP_GetSchemaTableIsNotSupported");

        internal static string ADP_ImplicitlyClosedDataReaderError =>
            EntityRes.GetString("ADP_ImplicitlyClosedDataReaderError");

        internal static string ADP_InvalidDataReaderFieldCountForPrimitiveType =>
            EntityRes.GetString("ADP_InvalidDataReaderFieldCountForPrimitiveType");

        internal static string ADP_InvalidDataReaderUnableToDetermineType =>
            EntityRes.GetString("ADP_InvalidDataReaderUnableToDetermineType");

        internal static string ADP_KeysRequiredForNesting =>
            EntityRes.GetString("ADP_KeysRequiredForNesting");

        internal static string ADP_MustUseSequentialAccess =>
            EntityRes.GetString("ADP_MustUseSequentialAccess");

        internal static string ADP_NoData =>
            EntityRes.GetString("ADP_NoData");

        internal static string ADP_ProviderDoesNotSupportCommandTrees =>
            EntityRes.GetString("ADP_ProviderDoesNotSupportCommandTrees");

        internal static string AllElementsMustBeInSchema =>
            EntityRes.GetString("AllElementsMustBeInSchema");

        internal static string AmbiguousFunctionArguments =>
            EntityRes.GetString("AmbiguousFunctionArguments");

        internal static string ApplyNotSupportedOnSql8 =>
            EntityRes.GetString("ApplyNotSupportedOnSql8");

        internal static string ArgumentMustBeCSpaceType =>
            EntityRes.GetString("ArgumentMustBeCSpaceType");

        internal static string ArgumentMustBeOSpaceType =>
            EntityRes.GetString("ArgumentMustBeOSpaceType");

        internal static string ArrayTooSmall =>
            EntityRes.GetString("ArrayTooSmall");

        internal static string AssociationInvalidMembers =>
            EntityRes.GetString("AssociationInvalidMembers");

        internal static string AtleastOneSSDLNeeded =>
            EntityRes.GetString("AtleastOneSSDLNeeded");

        internal static string BadNavigationPropertyRolesCannotBeTheSame =>
            EntityRes.GetString("BadNavigationPropertyRolesCannotBeTheSame");

        internal static string BetweenLimitsCannotBeUntypedNulls =>
            EntityRes.GetString("BetweenLimitsCannotBeUntypedNulls");

        internal static string CannotCreateEmptyMultiset =>
            EntityRes.GetString("CannotCreateEmptyMultiset");

        internal static string CannotCreateMultisetofNulls =>
            EntityRes.GetString("CannotCreateMultisetofNulls");

        internal static string CodeGen_ConstructorNoParameterless =>
            EntityRes.GetString("CodeGen_ConstructorNoParameterless");

        internal static string CodeGen_PropertyDeclaringTypeIsValueType =>
            EntityRes.GetString("CodeGen_PropertyDeclaringTypeIsValueType");

        internal static string CodeGen_PropertyIsIndexed =>
            EntityRes.GetString("CodeGen_PropertyIsIndexed");

        internal static string CodeGen_PropertyIsStatic =>
            EntityRes.GetString("CodeGen_PropertyIsStatic");

        internal static string CodeGen_PropertyNoGetter =>
            EntityRes.GetString("CodeGen_PropertyNoGetter");

        internal static string CodeGen_PropertyNoSetter =>
            EntityRes.GetString("CodeGen_PropertyNoSetter");

        internal static string CodeGen_PropertyStrongNameIdentity =>
            EntityRes.GetString("CodeGen_PropertyStrongNameIdentity");

        internal static string CodeGen_PropertyUnsupportedForm =>
            EntityRes.GetString("CodeGen_PropertyUnsupportedForm");

        internal static string CodeGen_PropertyUnsupportedType =>
            EntityRes.GetString("CodeGen_PropertyUnsupportedType");

        internal static string Collections_FoundMoreThanOneRelatedEnd =>
            EntityRes.GetString("Collections_FoundMoreThanOneRelatedEnd");

        internal static string Collections_InvalidEntityStateSource =>
            EntityRes.GetString("Collections_InvalidEntityStateSource");

        internal static string Collections_UnableToMergeCollections =>
            EntityRes.GetString("Collections_UnableToMergeCollections");

        internal static string CommandTextFunctionsCannotDeclareStoreFunctionName =>
            EntityRes.GetString("CommandTextFunctionsCannotDeclareStoreFunctionName");

        internal static string CommandTextFunctionsNotComposable =>
            EntityRes.GetString("CommandTextFunctionsNotComposable");

        internal static string CommandTreeCanOnlyBeSetOnce =>
            EntityRes.GetString("CommandTreeCanOnlyBeSetOnce");

        internal static string ComplexObject_ComplexObjectAlreadyAttachedToParent =>
            EntityRes.GetString("ComplexObject_ComplexObjectAlreadyAttachedToParent");

        internal static string ComplexTypeInvalidMembers =>
            EntityRes.GetString("ComplexTypeInvalidMembers");

        internal static string ComposableFunctionMustDeclareReturnType =>
            EntityRes.GetString("ComposableFunctionMustDeclareReturnType");

        internal static string ConcatBuiltinNotSupported =>
            EntityRes.GetString("ConcatBuiltinNotSupported");

        internal static string CouldNotFindAggregateKey =>
            EntityRes.GetString("CouldNotFindAggregateKey");

        internal static string Cqt_Aggregate_InvalidFunction =>
            EntityRes.GetString("Cqt_Aggregate_InvalidFunction");

        internal static string Cqt_And_BooleanArgumentsRequired =>
            EntityRes.GetString("Cqt_And_BooleanArgumentsRequired");

        internal static string Cqt_Apply_DuplicateVariableNames =>
            EntityRes.GetString("Cqt_Apply_DuplicateVariableNames");

        internal static string Cqt_Arithmetic_NumericCommonType =>
            EntityRes.GetString("Cqt_Arithmetic_NumericCommonType");

        internal static string Cqt_Binding_CollectionRequired =>
            EntityRes.GetString("Cqt_Binding_CollectionRequired");

        internal static string Cqt_Binding_VariableNameNotValid =>
            EntityRes.GetString("Cqt_Binding_VariableNameNotValid");

        internal static string Cqt_Case_AtLeastOneClause =>
            EntityRes.GetString("Cqt_Case_AtLeastOneClause");

        internal static string Cqt_Case_InvalidResultType =>
            EntityRes.GetString("Cqt_Case_InvalidResultType");

        internal static string Cqt_Case_WhensMustEqualThens =>
            EntityRes.GetString("Cqt_Case_WhensMustEqualThens");

        internal static string Cqt_CommandTree_Import_NullCommandTreeInvalid =>
            EntityRes.GetString("Cqt_CommandTree_Import_NullCommandTreeInvalid");

        internal static string Cqt_CommandTree_InvalidDataSpace =>
            EntityRes.GetString("Cqt_CommandTree_InvalidDataSpace");

        internal static string Cqt_CommandTree_NoParameterExists =>
            EntityRes.GetString("Cqt_CommandTree_NoParameterExists");

        internal static string Cqt_CommandTree_ParameterExists =>
            EntityRes.GetString("Cqt_CommandTree_ParameterExists");

        internal static string Cqt_Comparison_ComparableRequired =>
            EntityRes.GetString("Cqt_Comparison_ComparableRequired");

        internal static string Cqt_Constant_InvalidType =>
            EntityRes.GetString("Cqt_Constant_InvalidType");

        internal static string Cqt_CrossJoin_AtLeastTwoInputs =>
            EntityRes.GetString("Cqt_CrossJoin_AtLeastTwoInputs");

        internal static string Cqt_DeRef_RefRequired =>
            EntityRes.GetString("Cqt_DeRef_RefRequired");

        internal static string Cqt_Distinct_InvalidCollection =>
            EntityRes.GetString("Cqt_Distinct_InvalidCollection");

        internal static string Cqt_Element_InvalidArgumentForUnwrapSingleProperty =>
            EntityRes.GetString("Cqt_Element_InvalidArgumentForUnwrapSingleProperty");

        internal static string Cqt_Except_LeftNullTypeInvalid =>
            EntityRes.GetString("Cqt_Except_LeftNullTypeInvalid");

        internal static string Cqt_Exceptions_InvalidCommandTree =>
            EntityRes.GetString("Cqt_Exceptions_InvalidCommandTree");

        internal static string Cqt_ExpressionList_IncorrectElementCount =>
            EntityRes.GetString("Cqt_ExpressionList_IncorrectElementCount");

        internal static string Cqt_Factory_IncompatibleRelationEnds =>
            EntityRes.GetString("Cqt_Factory_IncompatibleRelationEnds");

        internal static string Cqt_Factory_NewCollectionElementsRequired =>
            EntityRes.GetString("Cqt_Factory_NewCollectionElementsRequired");

        internal static string Cqt_Factory_NewCollectionInvalidCommonType =>
            EntityRes.GetString("Cqt_Factory_NewCollectionInvalidCommonType");

        internal static string Cqt_Factory_NoSuchProperty =>
            EntityRes.GetString("Cqt_Factory_NoSuchProperty");

        internal static string Cqt_Factory_NoSuchRelationEnd =>
            EntityRes.GetString("Cqt_Factory_NoSuchRelationEnd");

        internal static string Cqt_Function_BodyOnlyValidForLambda =>
            EntityRes.GetString("Cqt_Function_BodyOnlyValidForLambda");

        internal static string Cqt_Function_CommandTextInExpression =>
            EntityRes.GetString("Cqt_Function_CommandTextInExpression");

        internal static string Cqt_Function_NonComposableInExpression =>
            EntityRes.GetString("Cqt_Function_NonComposableInExpression");

        internal static string Cqt_Function_VoidResultInvalid =>
            EntityRes.GetString("Cqt_Function_VoidResultInvalid");

        internal static string Cqt_General_MetadataNotReadOnly =>
            EntityRes.GetString("Cqt_General_MetadataNotReadOnly");

        internal static string Cqt_General_NoProviderBooleanType =>
            EntityRes.GetString("Cqt_General_NoProviderBooleanType");

        internal static string Cqt_General_NoProviderIntegerType =>
            EntityRes.GetString("Cqt_General_NoProviderIntegerType");

        internal static string Cqt_General_NoProviderStringType =>
            EntityRes.GetString("Cqt_General_NoProviderStringType");

        internal static string Cqt_General_NullTypeInvalid =>
            EntityRes.GetString("Cqt_General_NullTypeInvalid");

        internal static string Cqt_General_TreeMismatch =>
            EntityRes.GetString("Cqt_General_TreeMismatch");

        internal static string Cqt_General_TypeUsageNullEdmTypeInvalid =>
            EntityRes.GetString("Cqt_General_TypeUsageNullEdmTypeInvalid");

        internal static string Cqt_GetEntityRef_EntityRequired =>
            EntityRes.GetString("Cqt_GetEntityRef_EntityRequired");

        internal static string Cqt_GetRefKey_InvalidRef =>
            EntityRes.GetString("Cqt_GetRefKey_InvalidRef");

        internal static string Cqt_GetRefKey_RefRequired =>
            EntityRes.GetString("Cqt_GetRefKey_RefRequired");

        internal static string Cqt_GroupBinding_CollectionRequired =>
            EntityRes.GetString("Cqt_GroupBinding_CollectionRequired");

        internal static string Cqt_GroupBinding_GroupVariableNameNotValid =>
            EntityRes.GetString("Cqt_GroupBinding_GroupVariableNameNotValid");

        internal static string Cqt_GroupBy_AggregateTreeMismatch =>
            EntityRes.GetString("Cqt_GroupBy_AggregateTreeMismatch");

        internal static string Cqt_GroupBy_AtLeastOneKeyOrAggregate =>
            EntityRes.GetString("Cqt_GroupBy_AtLeastOneKeyOrAggregate");

        internal static string Cqt_IsNull_CollectionNotAllowed =>
            EntityRes.GetString("Cqt_IsNull_CollectionNotAllowed");

        internal static string Cqt_IsNull_InvalidType =>
            EntityRes.GetString("Cqt_IsNull_InvalidType");

        internal static string Cqt_Join_DuplicateVariableNames =>
            EntityRes.GetString("Cqt_Join_DuplicateVariableNames");

        internal static string Cqt_Limit_ConstantOrParameterRefRequired =>
            EntityRes.GetString("Cqt_Limit_ConstantOrParameterRefRequired");

        internal static string Cqt_Limit_IntegerRequired =>
            EntityRes.GetString("Cqt_Limit_IntegerRequired");

        internal static string Cqt_Limit_NonNegativeLimitRequired =>
            EntityRes.GetString("Cqt_Limit_NonNegativeLimitRequired");

        internal static string Cqt_Metadata_EdmMemberDeclaringTypeNull =>
            EntityRes.GetString("Cqt_Metadata_EdmMemberDeclaringTypeNull");

        internal static string Cqt_Metadata_EdmMemberIncorrectSpace =>
            EntityRes.GetString("Cqt_Metadata_EdmMemberIncorrectSpace");

        internal static string Cqt_Metadata_EdmMemberNameNull =>
            EntityRes.GetString("Cqt_Metadata_EdmMemberNameNull");

        internal static string Cqt_Metadata_EdmMemberTypeNull =>
            EntityRes.GetString("Cqt_Metadata_EdmMemberTypeNull");

        internal static string Cqt_Metadata_EntitySetElementTypeNull =>
            EntityRes.GetString("Cqt_Metadata_EntitySetElementTypeNull");

        internal static string Cqt_Metadata_EntitySetEntityContainerNull =>
            EntityRes.GetString("Cqt_Metadata_EntitySetEntityContainerNull");

        internal static string Cqt_Metadata_EntitySetIncorrectSpace =>
            EntityRes.GetString("Cqt_Metadata_EntitySetIncorrectSpace");

        internal static string Cqt_Metadata_EntitySetNameNull =>
            EntityRes.GetString("Cqt_Metadata_EntitySetNameNull");

        internal static string Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid =>
            EntityRes.GetString("Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid");

        internal static string Cqt_Metadata_EntityTypeNullKeyMembersInvalid =>
            EntityRes.GetString("Cqt_Metadata_EntityTypeNullKeyMembersInvalid");

        internal static string Cqt_Metadata_FunctionIncorrectSpace =>
            EntityRes.GetString("Cqt_Metadata_FunctionIncorrectSpace");

        internal static string Cqt_Metadata_FunctionNameNull =>
            EntityRes.GetString("Cqt_Metadata_FunctionNameNull");

        internal static string Cqt_Metadata_FunctionParameterIncorrectSpace =>
            EntityRes.GetString("Cqt_Metadata_FunctionParameterIncorrectSpace");

        internal static string Cqt_Metadata_FunctionParameterNameNull =>
            EntityRes.GetString("Cqt_Metadata_FunctionParameterNameNull");

        internal static string Cqt_Metadata_FunctionParametersNull =>
            EntityRes.GetString("Cqt_Metadata_FunctionParametersNull");

        internal static string Cqt_Metadata_FunctionParameterTypeNull =>
            EntityRes.GetString("Cqt_Metadata_FunctionParameterTypeNull");

        internal static string Cqt_Metadata_FunctionReturnParameterNull =>
            EntityRes.GetString("Cqt_Metadata_FunctionReturnParameterNull");

        internal static string Cqt_Metadata_TypeUsageIncorrectSpace =>
            EntityRes.GetString("Cqt_Metadata_TypeUsageIncorrectSpace");

        internal static string Cqt_NewInstance_CollectionTypeRequired =>
            EntityRes.GetString("Cqt_NewInstance_CollectionTypeRequired");

        internal static string Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid =>
            EntityRes.GetString("Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid");

        internal static string Cqt_NewInstance_IncompatibleRelatedEntity_TargetEntityNotValid =>
            EntityRes.GetString("Cqt_NewInstance_IncompatibleRelatedEntity_TargetEntityNotValid");

        internal static string Cqt_NewInstance_StructuralTypeRequired =>
            EntityRes.GetString("Cqt_NewInstance_StructuralTypeRequired");

        internal static string Cqt_Not_BooleanArgumentRequired =>
            EntityRes.GetString("Cqt_Not_BooleanArgumentRequired");

        internal static string Cqt_Or_BooleanArgumentsRequired =>
            EntityRes.GetString("Cqt_Or_BooleanArgumentsRequired");

        internal static string Cqt_Property_InstanceRequiredForInstance =>
            EntityRes.GetString("Cqt_Property_InstanceRequiredForInstance");

        internal static string Cqt_QueryTree_NullQueryInvalid =>
            EntityRes.GetString("Cqt_QueryTree_NullQueryInvalid");

        internal static string Cqt_Ref_PolymorphicArgRequired =>
            EntityRes.GetString("Cqt_Ref_PolymorphicArgRequired");

        internal static string Cqt_RelatedEntityRef_TargetEndFromDifferentRelationship =>
            EntityRes.GetString("Cqt_RelatedEntityRef_TargetEndFromDifferentRelationship");

        internal static string Cqt_RelatedEntityRef_TargetEndMustBeAtMostOne =>
            EntityRes.GetString("Cqt_RelatedEntityRef_TargetEndMustBeAtMostOne");

        internal static string Cqt_RelatedEntityRef_TargetEndSameAsSourceEnd =>
            EntityRes.GetString("Cqt_RelatedEntityRef_TargetEndSameAsSourceEnd");

        internal static string Cqt_RelatedEntityRef_TargetEntityNotCompatible =>
            EntityRes.GetString("Cqt_RelatedEntityRef_TargetEntityNotCompatible");

        internal static string Cqt_RelatedEntityRef_TargetEntityNotRef =>
            EntityRes.GetString("Cqt_RelatedEntityRef_TargetEntityNotRef");

        internal static string Cqt_RelNav_NoCompositions =>
            EntityRes.GetString("Cqt_RelNav_NoCompositions");

        internal static string Cqt_Skip_ConstantOrParameterRefRequired =>
            EntityRes.GetString("Cqt_Skip_ConstantOrParameterRefRequired");

        internal static string Cqt_Skip_IntegerRequired =>
            EntityRes.GetString("Cqt_Skip_IntegerRequired");

        internal static string Cqt_Skip_NonNegativeCountRequired =>
            EntityRes.GetString("Cqt_Skip_NonNegativeCountRequired");

        internal static string Cqt_SkipSort_AtLeastOneKey =>
            EntityRes.GetString("Cqt_SkipSort_AtLeastOneKey");

        internal static string Cqt_Sort_EmptyCollationInvalid =>
            EntityRes.GetString("Cqt_Sort_EmptyCollationInvalid");

        internal static string Cqt_Sort_NonStringCollationInvalid =>
            EntityRes.GetString("Cqt_Sort_NonStringCollationInvalid");

        internal static string Cqt_Sort_OrderComparable =>
            EntityRes.GetString("Cqt_Sort_OrderComparable");

        internal static string Cqt_Util_CheckListEmptyInvalid =>
            EntityRes.GetString("Cqt_Util_CheckListEmptyInvalid");

        internal static string Cqt_Validator_CycleDetected =>
            EntityRes.GetString("Cqt_Validator_CycleDetected");

        internal static string CtxAlias =>
            EntityRes.GetString("CtxAlias");

        internal static string CtxAliasedNamespaceDeclaration =>
            EntityRes.GetString("CtxAliasedNamespaceDeclaration");

        internal static string CtxAnd =>
            EntityRes.GetString("CtxAnd");

        internal static string CtxAnyElement =>
            EntityRes.GetString("CtxAnyElement");

        internal static string CtxApplyClause =>
            EntityRes.GetString("CtxApplyClause");

        internal static string CtxBetween =>
            EntityRes.GetString("CtxBetween");

        internal static string CtxCase =>
            EntityRes.GetString("CtxCase");

        internal static string CtxCaseElse =>
            EntityRes.GetString("CtxCaseElse");

        internal static string CtxCaseWhenThen =>
            EntityRes.GetString("CtxCaseWhenThen");

        internal static string CtxCast =>
            EntityRes.GetString("CtxCast");

        internal static string CtxCollatedOrderByClauseItem =>
            EntityRes.GetString("CtxCollatedOrderByClauseItem");

        internal static string CtxCommandExpression =>
            EntityRes.GetString("CtxCommandExpression");

        internal static string CtxCreateRef =>
            EntityRes.GetString("CtxCreateRef");

        internal static string CtxDeref =>
            EntityRes.GetString("CtxDeref");

        internal static string CtxDivide =>
            EntityRes.GetString("CtxDivide");

        internal static string CtxDot =>
            EntityRes.GetString("CtxDot");

        internal static string CtxElement =>
            EntityRes.GetString("CtxElement");

        internal static string CtxEquals =>
            EntityRes.GetString("CtxEquals");

        internal static string CtxEscapedIdentifier =>
            EntityRes.GetString("CtxEscapedIdentifier");

        internal static string CtxExcept =>
            EntityRes.GetString("CtxExcept");

        internal static string CtxExists =>
            EntityRes.GetString("CtxExists");

        internal static string CtxExpressionList =>
            EntityRes.GetString("CtxExpressionList");

        internal static string CtxFlatten =>
            EntityRes.GetString("CtxFlatten");

        internal static string CtxFromApplyClause =>
            EntityRes.GetString("CtxFromApplyClause");

        internal static string CtxFromClause =>
            EntityRes.GetString("CtxFromClause");

        internal static string CtxFromClauseItem =>
            EntityRes.GetString("CtxFromClauseItem");

        internal static string CtxFromClauseList =>
            EntityRes.GetString("CtxFromClauseList");

        internal static string CtxFromJoinClause =>
            EntityRes.GetString("CtxFromJoinClause");

        internal static string CtxGenericFunctionCall =>
            EntityRes.GetString("CtxGenericFunctionCall");

        internal static string CtxGenericTypeCtor =>
            EntityRes.GetString("CtxGenericTypeCtor");

        internal static string CtxGreaterThan =>
            EntityRes.GetString("CtxGreaterThan");

        internal static string CtxGreaterThanEqual =>
            EntityRes.GetString("CtxGreaterThanEqual");

        internal static string CtxGroupByClause =>
            EntityRes.GetString("CtxGroupByClause");

        internal static string CtxHavingClause =>
            EntityRes.GetString("CtxHavingClause");

        internal static string CtxIdentifier =>
            EntityRes.GetString("CtxIdentifier");

        internal static string CtxIn =>
            EntityRes.GetString("CtxIn");

        internal static string CtxIntersect =>
            EntityRes.GetString("CtxIntersect");

        internal static string CtxIsNotNull =>
            EntityRes.GetString("CtxIsNotNull");

        internal static string CtxIsNotOf =>
            EntityRes.GetString("CtxIsNotOf");

        internal static string CtxIsNull =>
            EntityRes.GetString("CtxIsNull");

        internal static string CtxIsOf =>
            EntityRes.GetString("CtxIsOf");

        internal static string CtxJoinClause =>
            EntityRes.GetString("CtxJoinClause");

        internal static string CtxJoinOnClause =>
            EntityRes.GetString("CtxJoinOnClause");

        internal static string CtxKey =>
            EntityRes.GetString("CtxKey");

        internal static string CtxLessThan =>
            EntityRes.GetString("CtxLessThan");

        internal static string CtxLessThanEqual =>
            EntityRes.GetString("CtxLessThanEqual");

        internal static string CtxLike =>
            EntityRes.GetString("CtxLike");

        internal static string CtxLimitSubClause =>
            EntityRes.GetString("CtxLimitSubClause");

        internal static string CtxLiteral =>
            EntityRes.GetString("CtxLiteral");

        internal static string CtxMethod =>
            EntityRes.GetString("CtxMethod");

        internal static string CtxMinus =>
            EntityRes.GetString("CtxMinus");

        internal static string CtxModulus =>
            EntityRes.GetString("CtxModulus");

        internal static string CtxMultipartIdentifier =>
            EntityRes.GetString("CtxMultipartIdentifier");

        internal static string CtxMultiply =>
            EntityRes.GetString("CtxMultiply");

        internal static string CtxMultisetCtor =>
            EntityRes.GetString("CtxMultisetCtor");

        internal static string CtxNamespaceDeclaration =>
            EntityRes.GetString("CtxNamespaceDeclaration");

        internal static string CtxNavigate =>
            EntityRes.GetString("CtxNavigate");

        internal static string CtxNot =>
            EntityRes.GetString("CtxNot");

        internal static string CtxNotBetween =>
            EntityRes.GetString("CtxNotBetween");

        internal static string CtxNotEqual =>
            EntityRes.GetString("CtxNotEqual");

        internal static string CtxNotIn =>
            EntityRes.GetString("CtxNotIn");

        internal static string CtxNotLike =>
            EntityRes.GetString("CtxNotLike");

        internal static string CtxNullLiteral =>
            EntityRes.GetString("CtxNullLiteral");

        internal static string CtxOfType =>
            EntityRes.GetString("CtxOfType");

        internal static string CtxOfTypeOnly =>
            EntityRes.GetString("CtxOfTypeOnly");

        internal static string CtxOr =>
            EntityRes.GetString("CtxOr");

        internal static string CtxOrderByClause =>
            EntityRes.GetString("CtxOrderByClause");

        internal static string CtxOrderByClauseItem =>
            EntityRes.GetString("CtxOrderByClauseItem");

        internal static string CtxOverlaps =>
            EntityRes.GetString("CtxOverlaps");

        internal static string CtxPlus =>
            EntityRes.GetString("CtxPlus");

        internal static string CtxQueryExpression =>
            EntityRes.GetString("CtxQueryExpression");

        internal static string CtxRef =>
            EntityRes.GetString("CtxRef");

        internal static string CtxRelationship =>
            EntityRes.GetString("CtxRelationship");

        internal static string CtxRelationshipList =>
            EntityRes.GetString("CtxRelationshipList");

        internal static string CtxRowCtor =>
            EntityRes.GetString("CtxRowCtor");

        internal static string CtxSelectRowClause =>
            EntityRes.GetString("CtxSelectRowClause");

        internal static string CtxSelectValueClause =>
            EntityRes.GetString("CtxSelectValueClause");

        internal static string CtxSet =>
            EntityRes.GetString("CtxSet");

        internal static string CtxSimpleIdentifier =>
            EntityRes.GetString("CtxSimpleIdentifier");

        internal static string CtxSkipSubClause =>
            EntityRes.GetString("CtxSkipSubClause");

        internal static string CtxTopSubClause =>
            EntityRes.GetString("CtxTopSubClause");

        internal static string CtxTreat =>
            EntityRes.GetString("CtxTreat");

        internal static string CtxTypeIdentifier =>
            EntityRes.GetString("CtxTypeIdentifier");

        internal static string CtxUnaryMinus =>
            EntityRes.GetString("CtxUnaryMinus");

        internal static string CtxUnaryPlus =>
            EntityRes.GetString("CtxUnaryPlus");

        internal static string CtxUnion =>
            EntityRes.GetString("CtxUnion");

        internal static string CtxUnionAll =>
            EntityRes.GetString("CtxUnionAll");

        internal static string CtxWhereClause =>
            EntityRes.GetString("CtxWhereClause");

        internal static string DataCategory_Data =>
            EntityRes.GetString("DataCategory_Data");

        internal static string DataCategory_Update =>
            EntityRes.GetString("DataCategory_Update");

        internal static string DbParameter_Direction =>
            EntityRes.GetString("DbParameter_Direction");

        internal static string DbParameter_Offset =>
            EntityRes.GetString("DbParameter_Offset");

        internal static string DbParameter_Size =>
            EntityRes.GetString("DbParameter_Size");

        internal static string DbParameter_SourceColumn =>
            EntityRes.GetString("DbParameter_SourceColumn");

        internal static string DbParameter_SourceVersion =>
            EntityRes.GetString("DbParameter_SourceVersion");

        internal static string DefaultNotAllowed =>
            EntityRes.GetString("DefaultNotAllowed");

        internal static string DefiningTypeDoesNotSupportMethodCalls =>
            EntityRes.GetString("DefiningTypeDoesNotSupportMethodCalls");

        internal static string EdmMembersDefiningTypeDoNotAgreeWithMetadataType =>
            EntityRes.GetString("EdmMembersDefiningTypeDoNotAgreeWithMetadataType");

        internal static string ElementOperatorIsNotSupported =>
            EntityRes.GetString("ElementOperatorIsNotSupported");

        internal static string ELinq_AnonymousType =>
            EntityRes.GetString("ELinq_AnonymousType");

        internal static string ELinq_ClosureType =>
            EntityRes.GetString("ELinq_ClosureType");

        internal static string ELinq_CreateOrderedEnumerableNotSupported =>
            EntityRes.GetString("ELinq_CreateOrderedEnumerableNotSupported");

        internal static string ELinq_CycleDetected =>
            EntityRes.GetString("ELinq_CycleDetected");

        internal static string ELinq_ExpressionMustBeIQueryable =>
            EntityRes.GetString("ELinq_ExpressionMustBeIQueryable");

        internal static string ELinq_PrimitiveTypesSample =>
            EntityRes.GetString("ELinq_PrimitiveTypesSample");

        internal static string ELinq_PropertyIndexNotSupported =>
            EntityRes.GetString("ELinq_PropertyIndexNotSupported");

        internal static string ELinq_SkipWithoutOrder =>
            EntityRes.GetString("ELinq_SkipWithoutOrder");

        internal static string ELinq_ThenByDoesNotFollowOrderBy =>
            EntityRes.GetString("ELinq_ThenByDoesNotFollowOrderBy");

        internal static string ELinq_UnsupportedBinding =>
            EntityRes.GetString("ELinq_UnsupportedBinding");

        internal static string ELinq_UnsupportedCastToDecimal =>
            EntityRes.GetString("ELinq_UnsupportedCastToDecimal");

        internal static string ELinq_UnsupportedConstructor =>
            EntityRes.GetString("ELinq_UnsupportedConstructor");

        internal static string ELinq_UnsupportedDifferentContexts =>
            EntityRes.GetString("ELinq_UnsupportedDifferentContexts");

        internal static string ELinq_UnsupportedInclude =>
            EntityRes.GetString("ELinq_UnsupportedInclude");

        internal static string ELinq_UnsupportedNestedFirst =>
            EntityRes.GetString("ELinq_UnsupportedNestedFirst");

        internal static string ELinq_UnsupportedObjectQueryMaterialization =>
            EntityRes.GetString("ELinq_UnsupportedObjectQueryMaterialization");

        internal static string ELinq_UnsupportedQueryableMethod =>
            EntityRes.GetString("ELinq_UnsupportedQueryableMethod");

        internal static string ELinq_UnsupportedSingle =>
            EntityRes.GetString("ELinq_UnsupportedSingle");

        internal static string EmptyCommandText =>
            EntityRes.GetString("EmptyCommandText");

        internal static string EmptyDefiningQuery =>
            EntityRes.GetString("EmptyDefiningQuery");

        internal static string EmptyIdentity =>
            EntityRes.GetString("EmptyIdentity");

        internal static string EmptySchemaTextReader =>
            EntityRes.GetString("EmptySchemaTextReader");

        internal static string Entity_EntityCantHaveMultipleChangeTrackers =>
            EntityRes.GetString("Entity_EntityCantHaveMultipleChangeTrackers");

        internal static string EntityClient_CannotCloneStoreProvider =>
            EntityRes.GetString("EntityClient_CannotCloneStoreProvider");

        internal static string EntityClient_CannotDeduceDbType =>
            EntityRes.GetString("EntityClient_CannotDeduceDbType");

        internal static string EntityClient_CannotReopenConnection =>
            EntityRes.GetString("EntityClient_CannotReopenConnection");

        internal static string EntityClient_ClosedConnectionForUpdate =>
            EntityRes.GetString("EntityClient_ClosedConnectionForUpdate");

        internal static string EntityClient_CommandDefinitionExecutionFailed =>
            EntityRes.GetString("EntityClient_CommandDefinitionExecutionFailed");

        internal static string EntityClient_CommandDefinitionPreparationFailed =>
            EntityRes.GetString("EntityClient_CommandDefinitionPreparationFailed");

        internal static string EntityClient_CommandExecutionFailed =>
            EntityRes.GetString("EntityClient_CommandExecutionFailed");

        internal static string EntityClient_CommandTreeMetadataIncompatible =>
            EntityRes.GetString("EntityClient_CommandTreeMetadataIncompatible");

        internal static string EntityClient_ConnectionMustBeClosed =>
            EntityRes.GetString("EntityClient_ConnectionMustBeClosed");

        internal static string EntityClient_ConnectionNotOpen =>
            EntityRes.GetString("EntityClient_ConnectionNotOpen");

        internal static string EntityClient_ConnectionStateBroken =>
            EntityRes.GetString("EntityClient_ConnectionStateBroken");

        internal static string EntityClient_ConnectionStateClosed =>
            EntityRes.GetString("EntityClient_ConnectionStateClosed");

        internal static string EntityClient_ConnectionStringNeededBeforeOperation =>
            EntityRes.GetString("EntityClient_ConnectionStringNeededBeforeOperation");

        internal static string EntityClient_DataReaderIsStillOpen =>
            EntityRes.GetString("EntityClient_DataReaderIsStillOpen");

        internal static string EntityClient_EmptyParameterName =>
            EntityRes.GetString("EntityClient_EmptyParameterName");

        internal static string EntityClient_ErrorInBeginningTransaction =>
            EntityRes.GetString("EntityClient_ErrorInBeginningTransaction");

        internal static string EntityClient_ErrorInClosingConnection =>
            EntityRes.GetString("EntityClient_ErrorInClosingConnection");

        internal static string EntityClient_ExtraParametersWithNamedConnection =>
            EntityRes.GetString("EntityClient_ExtraParametersWithNamedConnection");

        internal static string EntityClient_FunctionImportEmptyCommandText =>
            EntityRes.GetString("EntityClient_FunctionImportEmptyCommandText");

        internal static string EntityClient_InvalidNamedConnection =>
            EntityRes.GetString("EntityClient_InvalidNamedConnection");

        internal static string EntityClient_InvalidStoredProcedureCommandText =>
            EntityRes.GetString("EntityClient_InvalidStoredProcedureCommandText");

        internal static string EntityClient_InvalidStoreProvider =>
            EntityRes.GetString("EntityClient_InvalidStoreProvider");

        internal static string EntityClient_InvalidTransactionForCommand =>
            EntityRes.GetString("EntityClient_InvalidTransactionForCommand");

        internal static string EntityClient_NoCommandText =>
            EntityRes.GetString("EntityClient_NoCommandText");

        internal static string EntityClient_NoConnectionForAdapter =>
            EntityRes.GetString("EntityClient_NoConnectionForAdapter");

        internal static string EntityClient_NoConnectionForCommand =>
            EntityRes.GetString("EntityClient_NoConnectionForCommand");

        internal static string EntityClient_NoStoreConnectionForUpdate =>
            EntityRes.GetString("EntityClient_NoStoreConnectionForUpdate");

        internal static string EntityClient_ProviderGeneralError =>
            EntityRes.GetString("EntityClient_ProviderGeneralError");

        internal static string EntityClient_SettingsCannotBeChangedOnOpenConnection =>
            EntityRes.GetString("EntityClient_SettingsCannotBeChangedOnOpenConnection");

        internal static string EntityClient_StoreReaderFailed =>
            EntityRes.GetString("EntityClient_StoreReaderFailed");

        internal static string EntityClient_TooFewColumns =>
            EntityRes.GetString("EntityClient_TooFewColumns");

        internal static string EntityClient_TransactionAlreadyStarted =>
            EntityRes.GetString("EntityClient_TransactionAlreadyStarted");

        internal static string EntityClient_UnmappedFunctionImport =>
            EntityRes.GetString("EntityClient_UnmappedFunctionImport");

        internal static string EntityClient_UnsupportedCommandType =>
            EntityRes.GetString("EntityClient_UnsupportedCommandType");

        internal static string EntityClient_ValueNotString =>
            EntityRes.GetString("EntityClient_ValueNotString");

        internal static string EntityConnectionString_Metadata =>
            EntityRes.GetString("EntityConnectionString_Metadata");

        internal static string EntityConnectionString_Name =>
            EntityRes.GetString("EntityConnectionString_Name");

        internal static string EntityConnectionString_Provider =>
            EntityRes.GetString("EntityConnectionString_Provider");

        internal static string EntityConnectionString_ProviderConnectionString =>
            EntityRes.GetString("EntityConnectionString_ProviderConnectionString");

        internal static string EntityDataCategory_Context =>
            EntityRes.GetString("EntityDataCategory_Context");

        internal static string EntityDataCategory_NamedConnectionString =>
            EntityRes.GetString("EntityDataCategory_NamedConnectionString");

        internal static string EntityDataCategory_Source =>
            EntityRes.GetString("EntityDataCategory_Source");

        internal static string EntityKey_CannotChangeKey =>
            EntityRes.GetString("EntityKey_CannotChangeKey");

        internal static string EntityKey_DataRecordMustBeEntity =>
            EntityRes.GetString("EntityKey_DataRecordMustBeEntity");

        internal static string EntityKey_EntityKeyMustHaveValues =>
            EntityRes.GetString("EntityKey_EntityKeyMustHaveValues");

        internal static string EntityKey_InvalidQualifiedEntitySetName =>
            EntityRes.GetString("EntityKey_InvalidQualifiedEntitySetName");

        internal static string EntityKey_MissingEntitySetName =>
            EntityRes.GetString("EntityKey_MissingEntitySetName");

        internal static string EntityKey_NoNullsAllowedInKeyValuePairs =>
            EntityRes.GetString("EntityKey_NoNullsAllowedInKeyValuePairs");

        internal static string EntityKey_UnexpectedNull =>
            EntityRes.GetString("EntityKey_UnexpectedNull");

        internal static string EntityReference_CannotAddMoreThanOneEntityToEntityReference =>
            EntityRes.GetString("EntityReference_CannotAddMoreThanOneEntityToEntityReference");

        internal static string EntityReference_CannotChangeReferentialConstraintProperty =>
            EntityRes.GetString("EntityReference_CannotChangeReferentialConstraintProperty");

        internal static string EntityReference_CannotSetSpecialKeys =>
            EntityRes.GetString("EntityReference_CannotSetSpecialKeys");

        internal static string EntityReference_EntityIsNotPartOfRelationship =>
            EntityRes.GetString("EntityReference_EntityIsNotPartOfRelationship");

        internal static string EntityReference_EntityKeyValueMismatch =>
            EntityRes.GetString("EntityReference_EntityKeyValueMismatch");

        internal static string EntityReference_LessThanExpectedRelatedEntitiesFound =>
            EntityRes.GetString("EntityReference_LessThanExpectedRelatedEntitiesFound");

        internal static string EntityReference_MoreThanExpectedRelatedEntitiesFound =>
            EntityRes.GetString("EntityReference_MoreThanExpectedRelatedEntitiesFound");

        internal static string EntitySetInAnotherContainer =>
            EntityRes.GetString("EntitySetInAnotherContainer");

        internal static string EntityTypeInvalidMembers =>
            EntityRes.GetString("EntityTypeInvalidMembers");

        internal static string EntityTypesDoNotAgree =>
            EntityRes.GetString("EntityTypesDoNotAgree");

        internal static string ExpressionCannotBeNull =>
            EntityRes.GetString("ExpressionCannotBeNull");

        internal static string ExpressionMustBeCollection =>
            EntityRes.GetString("ExpressionMustBeCollection");

        internal static string ExpressionMustBeNumericType =>
            EntityRes.GetString("ExpressionMustBeNumericType");

        internal static string ExpressionTypeMustBeBoolean =>
            EntityRes.GetString("ExpressionTypeMustBeBoolean");

        internal static string ExpressionTypeMustBeEqualComparable =>
            EntityRes.GetString("ExpressionTypeMustBeEqualComparable");

        internal static string ExpressionTypeMustNotBeCollection =>
            EntityRes.GetString("ExpressionTypeMustNotBeCollection");

        internal static string ExprIsNotValidEntitySetForCreateRef =>
            EntityRes.GetString("ExprIsNotValidEntitySetForCreateRef");

        internal static string FailedToDeclareCanonicalNamespace =>
            EntityRes.GetString("FailedToDeclareCanonicalNamespace");

        internal static string FailedToRetrieveProviderManifest =>
            EntityRes.GetString("FailedToRetrieveProviderManifest");

        internal static string GeneralQueryError =>
            EntityRes.GetString("GeneralQueryError");

        internal static string Generated_Views_Changed =>
            EntityRes.GetString("Generated_Views_Changed");

        internal static string GeneratorErrorSeverityError =>
            EntityRes.GetString("GeneratorErrorSeverityError");

        internal static string GeneratorErrorSeverityUnknown =>
            EntityRes.GetString("GeneratorErrorSeverityUnknown");

        internal static string GeneratorErrorSeverityWarning =>
            EntityRes.GetString("GeneratorErrorSeverityWarning");

        internal static string GenericSyntaxError =>
            EntityRes.GetString("GenericSyntaxError");

        internal static string GroupingKeysMustBeEqualComparable =>
            EntityRes.GetString("GroupingKeysMustBeEqualComparable");

        internal static string GroupVarNotFoundInScope =>
            EntityRes.GetString("GroupVarNotFoundInScope");

        internal static string HavingRequiresGroupClause =>
            EntityRes.GetString("HavingRequiresGroupClause");

        internal static string ImcompatibleCreateRefKeyElementType =>
            EntityRes.GetString("ImcompatibleCreateRefKeyElementType");

        internal static string ImcompatibleCreateRefKeyType =>
            EntityRes.GetString("ImcompatibleCreateRefKeyType");

        internal static string IncorrectProviderManifest =>
            EntityRes.GetString("IncorrectProviderManifest");

        internal static string InFromClause =>
            EntityRes.GetString("InFromClause");

        internal static string InGroupClause =>
            EntityRes.GetString("InGroupClause");

        internal static string InnerJoinMustHaveOnPredicate =>
            EntityRes.GetString("InnerJoinMustHaveOnPredicate");

        internal static string InRowConstructor =>
            EntityRes.GetString("InRowConstructor");

        internal static string InSelectProjectionList =>
            EntityRes.GetString("InSelectProjectionList");

        internal static string InvalidArgumentTypeForAggregateFunction =>
            EntityRes.GetString("InvalidArgumentTypeForAggregateFunction");

        internal static string InvalidBaseTypeLoop =>
            EntityRes.GetString("InvalidBaseTypeLoop");

        internal static string InvalidCaseElseType =>
            EntityRes.GetString("InvalidCaseElseType");

        internal static string InvalidCaseThenNullType =>
            EntityRes.GetString("InvalidCaseThenNullType");

        internal static string InvalidCaseThenTypes =>
            EntityRes.GetString("InvalidCaseThenTypes");

        internal static string InvalidCaseWhenThenNullType =>
            EntityRes.GetString("InvalidCaseWhenThenNullType");

        internal static string InvalidCastExpressionType =>
            EntityRes.GetString("InvalidCastExpressionType");

        internal static string InvalidCastType =>
            EntityRes.GetString("InvalidCastType");

        internal static string InvalidCreateRefKeyType =>
            EntityRes.GetString("InvalidCreateRefKeyType");

        internal static string InvalidDistinctArgumentInCtor =>
            EntityRes.GetString("InvalidDistinctArgumentInCtor");

        internal static string InvalidDistinctArgumentInNonAggFunction =>
            EntityRes.GetString("InvalidDistinctArgumentInNonAggFunction");

        internal static string InvalidDocumentationBothTextAndStructure =>
            EntityRes.GetString("InvalidDocumentationBothTextAndStructure");

        internal static string InvalidEmptyIdentifier =>
            EntityRes.GetString("InvalidEmptyIdentifier");

        internal static string InvalidEmptyQuery =>
            EntityRes.GetString("InvalidEmptyQuery");

        internal static string InvalidEmptyQueryTextArgument =>
            EntityRes.GetString("InvalidEmptyQueryTextArgument");

        internal static string InvalidEscapedIdentifierEOF =>
            EntityRes.GetString("InvalidEscapedIdentifierEOF");

        internal static string InvalidEscapedNamespaceAlias =>
            EntityRes.GetString("InvalidEscapedNamespaceAlias");

        internal static string InvalidFlattenArgument =>
            EntityRes.GetString("InvalidFlattenArgument");

        internal static string InvalidJoinLeftCorrelation =>
            EntityRes.GetString("InvalidJoinLeftCorrelation");

        internal static string InvalidMaxLengthSize =>
            EntityRes.GetString("InvalidMaxLengthSize");

        internal static string InvalidMetadataPath =>
            EntityRes.GetString("InvalidMetadataPath");

        internal static string InvalidModeForWithRelationshipClause =>
            EntityRes.GetString("InvalidModeForWithRelationshipClause");

        internal static string InvalidModeInParameterInFunction =>
            EntityRes.GetString("InvalidModeInParameterInFunction");

        internal static string InvalidModeInReturnParameterInFunction =>
            EntityRes.GetString("InvalidModeInReturnParameterInFunction");

        internal static string InvalidNamespace =>
            EntityRes.GetString("InvalidNamespace");

        internal static string InvalidNamespaceAlias =>
            EntityRes.GetString("InvalidNamespaceAlias");

        internal static string InvalidNestedGroupAggregateCall =>
            EntityRes.GetString("InvalidNestedGroupAggregateCall");

        internal static string InvalidNullArithmetic =>
            EntityRes.GetString("InvalidNullArithmetic");

        internal static string InvalidNullComparison =>
            EntityRes.GetString("InvalidNullComparison");

        internal static string InvalidOperationMultipleEndsInAssociation =>
            EntityRes.GetString("InvalidOperationMultipleEndsInAssociation");

        internal static string InvalidOperatorSymbol =>
            EntityRes.GetString("InvalidOperatorSymbol");

        internal static string InvalidPredicateForCrossJoin =>
            EntityRes.GetString("InvalidPredicateForCrossJoin");

        internal static string InvalidPunctuatorSymbol =>
            EntityRes.GetString("InvalidPunctuatorSymbol");

        internal static string InvalidRelationshipSourceType =>
            EntityRes.GetString("InvalidRelationshipSourceType");

        internal static string InvalidRelationTypeName =>
            EntityRes.GetString("InvalidRelationTypeName");

        internal static string InvalidSavePoint =>
            EntityRes.GetString("InvalidSavePoint");

        internal static string InvalidScopeIndex =>
            EntityRes.GetString("InvalidScopeIndex");

        internal static string InvalidSelectItem =>
            EntityRes.GetString("InvalidSelectItem");

        internal static string InvalidSelectValueAliasedExpression =>
            EntityRes.GetString("InvalidSelectValueAliasedExpression");

        internal static string InvalidSelectValueList =>
            EntityRes.GetString("InvalidSelectValueList");

        internal static string InvalidTypeName =>
            EntityRes.GetString("InvalidTypeName");

        internal static string InvalidTypeNameExpression =>
            EntityRes.GetString("InvalidTypeNameExpression");

        internal static string Iqt_CTGen_UnexpectedAggregate =>
            EntityRes.GetString("Iqt_CTGen_UnexpectedAggregate");

        internal static string Iqt_CTGen_UnexpectedVarDef =>
            EntityRes.GetString("Iqt_CTGen_UnexpectedVarDef");

        internal static string Iqt_CTGen_UnexpectedVarDefList =>
            EntityRes.GetString("Iqt_CTGen_UnexpectedVarDefList");

        internal static string IsNullInvalidType =>
            EntityRes.GetString("IsNullInvalidType");

        internal static string LeftSetExpressionArgsMustBeCollection =>
            EntityRes.GetString("LeftSetExpressionArgsMustBeCollection");

        internal static string LikeArgMustBeStringType =>
            EntityRes.GetString("LikeArgMustBeStringType");

        internal static string LimitExpressionMustBeParamOrLiteral =>
            EntityRes.GetString("LimitExpressionMustBeParamOrLiteral");

        internal static string LocalizedCollection =>
            EntityRes.GetString("LocalizedCollection");

        internal static string LocalizedColumn =>
            EntityRes.GetString("LocalizedColumn");

        internal static string LocalizedComplex =>
            EntityRes.GetString("LocalizedComplex");

        internal static string LocalizedEntity =>
            EntityRes.GetString("LocalizedEntity");

        internal static string LocalizedKeyword =>
            EntityRes.GetString("LocalizedKeyword");

        internal static string LocalizedLeft =>
            EntityRes.GetString("LocalizedLeft");

        internal static string LocalizedLine =>
            EntityRes.GetString("LocalizedLine");

        internal static string LocalizedNear =>
            EntityRes.GetString("LocalizedNear");

        internal static string LocalizedPrimitive =>
            EntityRes.GetString("LocalizedPrimitive");

        internal static string LocalizedReference =>
            EntityRes.GetString("LocalizedReference");

        internal static string LocalizedRight =>
            EntityRes.GetString("LocalizedRight");

        internal static string LocalizedRow =>
            EntityRes.GetString("LocalizedRow");

        internal static string LocalizedTerm =>
            EntityRes.GetString("LocalizedTerm");

        internal static string LocalizedType =>
            EntityRes.GetString("LocalizedType");

        internal static string MalformedSingleQuotePayload =>
            EntityRes.GetString("MalformedSingleQuotePayload");

        internal static string MalformedStringLiteralPayload =>
            EntityRes.GetString("MalformedStringLiteralPayload");

        internal static string Mapping_ConditionValueTypeMismatch_0 =>
            EntityRes.GetString("Mapping_ConditionValueTypeMismatch_0");

        internal static string Mapping_General_Error_0 =>
            EntityRes.GetString("Mapping_General_Error_0");

        internal static string Mapping_Invalid_CSRootElementMissing_0 =>
            EntityRes.GetString("Mapping_Invalid_CSRootElementMissing_0");

        internal static string Mapping_Invalid_Function_Mapping_In_Table_Context_0 =>
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_In_Table_Context_0");

        internal static string Mapping_Invalid_Function_Mapping_MissingVersion_0 =>
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_MissingVersion_0");

        internal static string Mapping_Invalid_Function_Mapping_Multiple_Types_0 =>
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_Multiple_Types_0");

        internal static string Mapping_Invalid_Function_Mapping_VersionMustBeCurrent_0 =>
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_VersionMustBeCurrent_0");

        internal static string Mapping_Invalid_Function_Mapping_VersionMustBeOriginal_0 =>
            EntityRes.GetString("Mapping_Invalid_Function_Mapping_VersionMustBeOriginal_0");

        internal static string Mapping_InvalidContent_Association_Type_Empty_0 =>
            EntityRes.GetString("Mapping_InvalidContent_Association_Type_Empty_0");

        internal static string Mapping_InvalidContent_ConditionMapping_Both_Members_0 =>
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_Both_Members_0");

        internal static string Mapping_InvalidContent_ConditionMapping_Both_Values_0 =>
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_Both_Values_0");

        internal static string Mapping_InvalidContent_ConditionMapping_Either_Members_0 =>
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_Either_Members_0");

        internal static string Mapping_InvalidContent_ConditionMapping_Either_Values_0 =>
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_Either_Values_0");

        internal static string Mapping_InvalidContent_ConditionMapping_NonScalar_0 =>
            EntityRes.GetString("Mapping_InvalidContent_ConditionMapping_NonScalar_0");

        internal static string Mapping_InvalidContent_General_0 =>
            EntityRes.GetString("Mapping_InvalidContent_General_0");

        internal static string Mapping_InvalidContent_IsTypeOfNotTerminated =>
            EntityRes.GetString("Mapping_InvalidContent_IsTypeOfNotTerminated");

        internal static string Mapping_InvalidContent_Set_Mapping_0 =>
            EntityRes.GetString("Mapping_InvalidContent_Set_Mapping_0");

        internal static string Mapping_InvalidContent_Table_Expected_0 =>
            EntityRes.GetString("Mapping_InvalidContent_Table_Expected_0");

        internal static string Mapping_InvalidContent_TypeMapping_QueryView =>
            EntityRes.GetString("Mapping_InvalidContent_TypeMapping_QueryView");

        internal static string Mapping_NoViewsCanBeGenerated =>
            EntityRes.GetString("Mapping_NoViewsCanBeGenerated");

        internal static string Mapping_TypeName_For_First_QueryView =>
            EntityRes.GetString("Mapping_TypeName_For_First_QueryView");

        internal static string Materializer_CannotReEnumerateQueryResults =>
            EntityRes.GetString("Materializer_CannotReEnumerateQueryResults");

        internal static string Materializer_PropertyIsNotNullable =>
            EntityRes.GetString("Materializer_PropertyIsNotNullable");

        internal static string Materializer_UnsupportedType =>
            EntityRes.GetString("Materializer_UnsupportedType");

        internal static string MemberAlreadyBelongsToType =>
            EntityRes.GetString("MemberAlreadyBelongsToType");

        internal static string Metadata_General_Error =>
            EntityRes.GetString("Metadata_General_Error");

        internal static string MethodInvocationNotSupported =>
            EntityRes.GetString("MethodInvocationNotSupported");

        internal static string MethodNotAllowedOnScalars =>
            EntityRes.GetString("MethodNotAllowedOnScalars");

        internal static string MismatchNumberOfPropertiesinRelationshipConstraint =>
            EntityRes.GetString("MismatchNumberOfPropertiesinRelationshipConstraint");

        internal static string MissingName =>
            EntityRes.GetString("MissingName");

        internal static string MultisetElemsAreNotTypeCompatible =>
            EntityRes.GetString("MultisetElemsAreNotTypeCompatible");

        internal static string NestedAggregatesCannotBeUsedInAggregateFunctions =>
            EntityRes.GetString("NestedAggregatesCannotBeUsedInAggregateFunctions");

        internal static string NiladicFunctionsCannotHaveParameters =>
            EntityRes.GetString("NiladicFunctionsCannotHaveParameters");

        internal static string NonComposableFunctionHasDisallowedAttribute =>
            EntityRes.GetString("NonComposableFunctionHasDisallowedAttribute");

        internal static string NonComposableFunctionMustNotDeclareReturnType =>
            EntityRes.GetString("NonComposableFunctionMustNotDeclareReturnType");

        internal static string NotBinaryTypeForTypeUsage =>
            EntityRes.GetString("NotBinaryTypeForTypeUsage");

        internal static string NotDateTimeOffsetTypeForTypeUsage =>
            EntityRes.GetString("NotDateTimeOffsetTypeForTypeUsage");

        internal static string NotDateTimeTypeForTypeUsage =>
            EntityRes.GetString("NotDateTimeTypeForTypeUsage");

        internal static string NotDecimalTypeForTypeUsage =>
            EntityRes.GetString("NotDecimalTypeForTypeUsage");

        internal static string NotStringTypeForTypeUsage =>
            EntityRes.GetString("NotStringTypeForTypeUsage");

        internal static string NotTimeTypeForTypeUsage =>
            EntityRes.GetString("NotTimeTypeForTypeUsage");

        internal static string NotValidInputPath =>
            EntityRes.GetString("NotValidInputPath");

        internal static string NullLiteralCannotBePromotedToCollectionOfNulls =>
            EntityRes.GetString("NullLiteralCannotBePromotedToCollectionOfNulls");

        internal static string ObjectContext_CannotAttachEntityWithoutKey =>
            EntityRes.GetString("ObjectContext_CannotAttachEntityWithoutKey");

        internal static string ObjectContext_CannotAttachEntityWithTemporaryKey =>
            EntityRes.GetString("ObjectContext_CannotAttachEntityWithTemporaryKey");

        internal static string ObjectContext_CannotDeleteEntityNotInObjectStateManager =>
            EntityRes.GetString("ObjectContext_CannotDeleteEntityNotInObjectStateManager");

        internal static string ObjectContext_CannotDetachEntityNotInObjectStateManager =>
            EntityRes.GetString("ObjectContext_CannotDetachEntityNotInObjectStateManager");

        internal static string ObjectContext_CannotSetDefaultContainerName =>
            EntityRes.GetString("ObjectContext_CannotSetDefaultContainerName");

        internal static string ObjectContext_ContainerQualifiedEntitySetNameRequired =>
            EntityRes.GetString("ObjectContext_ContainerQualifiedEntitySetNameRequired");

        internal static string ObjectContext_EntityAlreadyExistsInObjectStateManager =>
            EntityRes.GetString("ObjectContext_EntityAlreadyExistsInObjectStateManager");

        internal static string ObjectContext_EntitySetNameOrEntityKeyRequired =>
            EntityRes.GetString("ObjectContext_EntitySetNameOrEntityKeyRequired");

        internal static string ObjectContext_InvalidCommandTimeout =>
            EntityRes.GetString("ObjectContext_InvalidCommandTimeout");

        internal static string ObjectContext_InvalidConnection =>
            EntityRes.GetString("ObjectContext_InvalidConnection");

        internal static string ObjectContext_InvalidConnectionString =>
            EntityRes.GetString("ObjectContext_InvalidConnectionString");

        internal static string ObjectContext_InvalidDataAdapter =>
            EntityRes.GetString("ObjectContext_InvalidDataAdapter");

        internal static string ObjectContext_MetadataHasChanged =>
            EntityRes.GetString("ObjectContext_MetadataHasChanged");

        internal static string ObjectContext_ObjectDisposed =>
            EntityRes.GetString("ObjectContext_ObjectDisposed");

        internal static string ObjectContext_ObjectNotFound =>
            EntityRes.GetString("ObjectContext_ObjectNotFound");

        internal static string ObjectContext_QualfiedEntitySetName =>
            EntityRes.GetString("ObjectContext_QualfiedEntitySetName");

        internal static string ObjectContext_RequiredMetadataNotAvailble =>
            EntityRes.GetString("ObjectContext_RequiredMetadataNotAvailble");

        internal static string ObjectParameterCollection_ParametersLocked =>
            EntityRes.GetString("ObjectParameterCollection_ParametersLocked");

        internal static string ObjectQuery_InvalidConnection =>
            EntityRes.GetString("ObjectQuery_InvalidConnection");

        internal static string ObjectQuery_InvalidEmptyQuery =>
            EntityRes.GetString("ObjectQuery_InvalidEmptyQuery");

        internal static string ObjectQuery_QueryBuilder_InvalidFilterPredicate =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidFilterPredicate");

        internal static string ObjectQuery_QueryBuilder_InvalidGroupKeyList =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidGroupKeyList");

        internal static string ObjectQuery_QueryBuilder_InvalidProjectionList =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidProjectionList");

        internal static string ObjectQuery_QueryBuilder_InvalidQueryArgument =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidQueryArgument");

        internal static string ObjectQuery_QueryBuilder_InvalidSkipCount =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidSkipCount");

        internal static string ObjectQuery_QueryBuilder_InvalidSortKeyList =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidSortKeyList");

        internal static string ObjectQuery_QueryBuilder_InvalidTopCount =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_InvalidTopCount");

        internal static string ObjectQuery_QueryBuilder_NotSupportedLinqSource =>
            EntityRes.GetString("ObjectQuery_QueryBuilder_NotSupportedLinqSource");

        internal static string ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection =>
            EntityRes.GetString("ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection");

        internal static string ObjectQuery_Span_SpanPathSyntaxError =>
            EntityRes.GetString("ObjectQuery_Span_SpanPathSyntaxError");

        internal static string ObjectQuery_UnableToMapResultType =>
            EntityRes.GetString("ObjectQuery_UnableToMapResultType");

        internal static string ObjectStateEntry_CannotAccessKeyEntryValues =>
            EntityRes.GetString("ObjectStateEntry_CannotAccessKeyEntryValues");

        internal static string ObjectStateEntry_CannotDeleteOnKeyEntry =>
            EntityRes.GetString("ObjectStateEntry_CannotDeleteOnKeyEntry");

        internal static string ObjectStateEntry_CannotModifyKeyEntryState =>
            EntityRes.GetString("ObjectStateEntry_CannotModifyKeyEntryState");

        internal static string ObjectStateEntry_CantModifyDetachedDeletedEntries =>
            EntityRes.GetString("ObjectStateEntry_CantModifyDetachedDeletedEntries");

        internal static string ObjectStateEntry_CantModifyRelationState =>
            EntityRes.GetString("ObjectStateEntry_CantModifyRelationState");

        internal static string ObjectStateEntry_CantModifyRelationValues =>
            EntityRes.GetString("ObjectStateEntry_CantModifyRelationValues");

        internal static string ObjectStateEntry_CantSetEntityKey =>
            EntityRes.GetString("ObjectStateEntry_CantSetEntityKey");

        internal static string ObjectStateEntry_CurrentValuesDoesNotExist =>
            EntityRes.GetString("ObjectStateEntry_CurrentValuesDoesNotExist");

        internal static string ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging =>
            EntityRes.GetString("ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging");

        internal static string ObjectStateEntry_InvalidState =>
            EntityRes.GetString("ObjectStateEntry_InvalidState");

        internal static string ObjectStateEntry_OriginalValuesDoesNotExist =>
            EntityRes.GetString("ObjectStateEntry_OriginalValuesDoesNotExist");

        internal static string ObjectStateEntry_SetModifiedStates =>
            EntityRes.GetString("ObjectStateEntry_SetModifiedStates");

        internal static string ObjectStateEntryDbDataRecord_CantCreate =>
            EntityRes.GetString("ObjectStateEntryDbDataRecord_CantCreate");

        internal static string ObjectStateEntryDbUpdatableDataRecord_CantCreate =>
            EntityRes.GetString("ObjectStateEntryDbUpdatableDataRecord_CantCreate");

        internal static string ObjectStateEntryOriginalDbUpdatableDataRecord_CantCreate =>
            EntityRes.GetString("ObjectStateEntryOriginalDbUpdatableDataRecord_CantCreate");

        internal static string ObjectStateManager_AcceptChangesEntityKeyIsNotValid =>
            EntityRes.GetString("ObjectStateManager_AcceptChangesEntityKeyIsNotValid");

        internal static string ObjectStateManager_CannotAddEntityWithKeyAttached =>
            EntityRes.GetString("ObjectStateManager_CannotAddEntityWithKeyAttached");

        internal static string ObjectStateManager_CannotFixUpKeyToExistingValues =>
            EntityRes.GetString("ObjectStateManager_CannotFixUpKeyToExistingValues");

        internal static string ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager =>
            EntityRes.GetString("ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager");

        internal static string ObjectStateManager_EntityConflictsWithKeyEntry =>
            EntityRes.GetString("ObjectStateManager_EntityConflictsWithKeyEntry");

        internal static string ObjectStateManager_EntityTypeDoesntMatchEntitySetFromKey =>
            EntityRes.GetString("ObjectStateManager_EntityTypeDoesntMatchEntitySetFromKey");

        internal static string ObjectStateManager_InvalidKey =>
            EntityRes.GetString("ObjectStateManager_InvalidKey");

        internal static string ObjectStateManager_KeyPropertyDoesntMatchValueInKey =>
            EntityRes.GetString("ObjectStateManager_KeyPropertyDoesntMatchValueInKey");

        internal static string ObjectStateManager_NoEntryExistForEntityKey =>
            EntityRes.GetString("ObjectStateManager_NoEntryExistForEntityKey");

        internal static string ObjectStateManager_ObjectStateManagerContainsThisEntityKey =>
            EntityRes.GetString("ObjectStateManager_ObjectStateManagerContainsThisEntityKey");

        internal static string ObjectView_AddNewOperationNotAllowedOnAbstractBindingList =>
            EntityRes.GetString("ObjectView_AddNewOperationNotAllowedOnAbstractBindingList");

        internal static string ObjectView_CannotReplacetheEntityorRow =>
            EntityRes.GetString("ObjectView_CannotReplacetheEntityorRow");

        internal static string ObjectView_IncompatibleArgument =>
            EntityRes.GetString("ObjectView_IncompatibleArgument");

        internal static string ObjectView_IndexBasedInsertIsNotSupported =>
            EntityRes.GetString("ObjectView_IndexBasedInsertIsNotSupported");

        internal static string ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList =>
            EntityRes.GetString("ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList");

        internal static string OnlyStoreConnectionsSupported =>
            EntityRes.GetString("OnlyStoreConnectionsSupported");

        internal static string OperationActionNotSupported =>
            EntityRes.GetString("OperationActionNotSupported");

        internal static string OperationOnReadOnlyCollection =>
            EntityRes.GetString("OperationOnReadOnlyCollection");

        internal static string OperationOnReadOnlyItem =>
            EntityRes.GetString("OperationOnReadOnlyItem");

        internal static string OrderByKeyIsNotOrderComparable =>
            EntityRes.GetString("OrderByKeyIsNotOrderComparable");

        internal static string ParameterForLimitNotSupportedOnSql8 =>
            EntityRes.GetString("ParameterForLimitNotSupportedOnSql8");

        internal static string ParameterForSkipNotSupportedOnSql8 =>
            EntityRes.GetString("ParameterForSkipNotSupportedOnSql8");

        internal static string ParserFatalError =>
            EntityRes.GetString("ParserFatalError");

        internal static string ParserInputError =>
            EntityRes.GetString("ParserInputError");

        internal static string PlusLeftExpressionInvalidType =>
            EntityRes.GetString("PlusLeftExpressionInvalidType");

        internal static string PlusRightExpressionInvalidType =>
            EntityRes.GetString("PlusRightExpressionInvalidType");

        internal static string PropertyCannotBeChangedAtThisTime =>
            EntityRes.GetString("PropertyCannotBeChangedAtThisTime");

        internal static string ProviderDidNotReturnAProviderManifest =>
            EntityRes.GetString("ProviderDidNotReturnAProviderManifest");

        internal static string ProviderDidNotReturnAProviderManifestToken =>
            EntityRes.GetString("ProviderDidNotReturnAProviderManifestToken");

        internal static string ProviderManifestTokenNotFound =>
            EntityRes.GetString("ProviderManifestTokenNotFound");

        internal static string ProviderReturnedNullForCreateCommandDefinition =>
            EntityRes.GetString("ProviderReturnedNullForCreateCommandDefinition");

        internal static string RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts =>
            EntityRes.GetString("RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts");

        internal static string RelatedEnd_InvalidEntityContextForAttach =>
            EntityRes.GetString("RelatedEnd_InvalidEntityContextForAttach");

        internal static string RelatedEnd_InvalidEntityStateForAttach =>
            EntityRes.GetString("RelatedEnd_InvalidEntityStateForAttach");

        internal static string RelatedEnd_InvalidOwnerStateForAttach =>
            EntityRes.GetString("RelatedEnd_InvalidOwnerStateForAttach");

        internal static string RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd =>
            EntityRes.GetString("RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd");

        internal static string RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd =>
            EntityRes.GetString("RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd");

        internal static string RelatedEnd_OwnerIsNull =>
            EntityRes.GetString("RelatedEnd_OwnerIsNull");

        internal static string RelatedEnd_RelatedEndNotFound =>
            EntityRes.GetString("RelatedEnd_RelatedEndNotFound");

        internal static string RelatedEnd_UnableToAddEntity =>
            EntityRes.GetString("RelatedEnd_UnableToAddEntity");

        internal static string RelatedEnd_UnableToRemoveEntity =>
            EntityRes.GetString("RelatedEnd_UnableToRemoveEntity");

        internal static string RelationshipFromEndIsAmbiguos =>
            EntityRes.GetString("RelationshipFromEndIsAmbiguos");

        internal static string RelationshipManager_CircularRelationshipsWithReferentialConstraints =>
            EntityRes.GetString("RelationshipManager_CircularRelationshipsWithReferentialConstraints");

        internal static string RelationshipManager_CollectionInitializeIsForDeserialization =>
            EntityRes.GetString("RelationshipManager_CollectionInitializeIsForDeserialization");

        internal static string RelationshipManager_InconsistentReferentialConstraintProperties =>
            EntityRes.GetString("RelationshipManager_InconsistentReferentialConstraintProperties");

        internal static string RelationshipManager_InitializeIsForDeserialization =>
            EntityRes.GetString("RelationshipManager_InitializeIsForDeserialization");

        internal static string RelationshipManager_InvalidRelationshipManagerOwner =>
            EntityRes.GetString("RelationshipManager_InvalidRelationshipManagerOwner");

        internal static string RelationshipManager_UnableToRetrieveReferentialConstraintProperties =>
            EntityRes.GetString("RelationshipManager_UnableToRetrieveReferentialConstraintProperties");

        internal static string RelationshipManager_UnexpectedNull =>
            EntityRes.GetString("RelationshipManager_UnexpectedNull");

        internal static string RelationshipManager_UnexpectedNullContext =>
            EntityRes.GetString("RelationshipManager_UnexpectedNullContext");

        internal static string ResultingExpressionTypeCannotBeNull =>
            EntityRes.GetString("ResultingExpressionTypeCannotBeNull");

        internal static string RightSetExpressionArgsMustBeCollection =>
            EntityRes.GetString("RightSetExpressionArgsMustBeCollection");

        internal static string RowCtorElementCannotBeNull =>
            EntityRes.GetString("RowCtorElementCannotBeNull");

        internal static string RowTypeInvalidMembers =>
            EntityRes.GetString("RowTypeInvalidMembers");

        internal static string SelectDistinctMustBeEqualComparable =>
            EntityRes.GetString("SelectDistinctMustBeEqualComparable");

        internal static string SkipExpressionMustBeParamOrLiteral =>
            EntityRes.GetString("SkipExpressionMustBeParamOrLiteral");

        internal static string SourceUriUnknown =>
            EntityRes.GetString("SourceUriUnknown");

        internal static string StackOverflowInParser =>
            EntityRes.GetString("StackOverflowInParser");

        internal static string TooManyFunctionArguments =>
            EntityRes.GetString("TooManyFunctionArguments");

        internal static string TopAndLimitCannotCoexist =>
            EntityRes.GetString("TopAndLimitCannotCoexist");

        internal static string TopAndSkipCannotCoexist =>
            EntityRes.GetString("TopAndSkipCannotCoexist");

        internal static string TopExpressionMustBeParamOrLiteral =>
            EntityRes.GetString("TopExpressionMustBeParamOrLiteral");

        internal static string TypeIndentifierArgMustBeLiteral =>
            EntityRes.GetString("TypeIndentifierArgMustBeLiteral");

        internal static string TypeIndentifierMustHaveOneOrTwoArgs =>
            EntityRes.GetString("TypeIndentifierMustHaveOneOrTwoArgs");

        internal static string TypeMustBeInheritableType =>
            EntityRes.GetString("TypeMustBeInheritableType");

        internal static string TypeSpecIsNotValid =>
            EntityRes.GetString("TypeSpecIsNotValid");

        internal static string TypeUsageHasNoEdmType =>
            EntityRes.GetString("TypeUsageHasNoEdmType");

        internal static string UnableToDetermineApplicationContext =>
            EntityRes.GetString("UnableToDetermineApplicationContext");

        internal static string UnableToDetermineStoreVersion =>
            EntityRes.GetString("UnableToDetermineStoreVersion");

        internal static string UnableToLoadResource =>
            EntityRes.GetString("UnableToLoadResource");

        internal static string UnknownAstCommandExpression =>
            EntityRes.GetString("UnknownAstCommandExpression");

        internal static string UnknownAstExpressionType =>
            EntityRes.GetString("UnknownAstExpressionType");

        internal static string UnknownBuiltInAstExpressionType =>
            EntityRes.GetString("UnknownBuiltInAstExpressionType");

        internal static string Update_AmbiguousServerGenIdentifier =>
            EntityRes.GetString("Update_AmbiguousServerGenIdentifier");

        internal static string Update_CircularRelationships =>
            EntityRes.GetString("Update_CircularRelationships");

        internal static string Update_ConstraintCycle =>
            EntityRes.GetString("Update_ConstraintCycle");

        internal static string Update_ErrorLoadingRecord =>
            EntityRes.GetString("Update_ErrorLoadingRecord");

        internal static string Update_GeneralExecutionException =>
            EntityRes.GetString("Update_GeneralExecutionException");

        internal static string Update_ReferentialConstraintIntegrityViolation =>
            EntityRes.GetString("Update_ReferentialConstraintIntegrityViolation");

        internal static string Update_WorkspaceMismatch =>
            EntityRes.GetString("Update_WorkspaceMismatch");

        internal static string Validator_BaseTypeHasMemberOfSameName =>
            EntityRes.GetString("Validator_BaseTypeHasMemberOfSameName");

        internal static string Validator_CollectionHasNoTypeUsage =>
            EntityRes.GetString("Validator_CollectionHasNoTypeUsage");

        internal static string Validator_CollectionTypesCannotHaveBaseType =>
            EntityRes.GetString("Validator_CollectionTypesCannotHaveBaseType");

        internal static string Validator_EmptyIdentity =>
            EntityRes.GetString("Validator_EmptyIdentity");

        internal static string Validator_FacetHasNoName =>
            EntityRes.GetString("Validator_FacetHasNoName");

        internal static string Validator_FacetTypeIsNull =>
            EntityRes.GetString("Validator_FacetTypeIsNull");

        internal static string Validator_ItemAttributeHasNullTypeUsage =>
            EntityRes.GetString("Validator_ItemAttributeHasNullTypeUsage");

        internal static string Validator_MemberHasNoName =>
            EntityRes.GetString("Validator_MemberHasNoName");

        internal static string Validator_MemberHasNullDeclaringType =>
            EntityRes.GetString("Validator_MemberHasNullDeclaringType");

        internal static string Validator_MemberHasNullTypeUsage =>
            EntityRes.GetString("Validator_MemberHasNullTypeUsage");

        internal static string Validator_MetadataPropertyHasNoName =>
            EntityRes.GetString("Validator_MetadataPropertyHasNoName");

        internal static string Validator_NoKeyMembers =>
            EntityRes.GetString("Validator_NoKeyMembers");

        internal static string Validator_RefTypeHasNullEntityType =>
            EntityRes.GetString("Validator_RefTypeHasNullEntityType");

        internal static string Validator_RefTypesCannotHaveBaseType =>
            EntityRes.GetString("Validator_RefTypesCannotHaveBaseType");

        internal static string Validator_TypeHasNoName =>
            EntityRes.GetString("Validator_TypeHasNoName");

        internal static string Validator_TypeHasNoNamespace =>
            EntityRes.GetString("Validator_TypeHasNoNamespace");

        internal static string Validator_TypeUsageHasNullEdmType =>
            EntityRes.GetString("Validator_TypeUsageHasNullEdmType");

        internal static string ViewGen_AND =>
            EntityRes.GetString("ViewGen_AND");

        internal static string ViewGen_CellIdBooleans_Invalid =>
            EntityRes.GetString("ViewGen_CellIdBooleans_Invalid");

        internal static string ViewGen_CommaBlank =>
            EntityRes.GetString("ViewGen_CommaBlank");

        internal static string ViewGen_DomainConstraint_EntityTypes =>
            EntityRes.GetString("ViewGen_DomainConstraint_EntityTypes");

        internal static string ViewGen_Entities =>
            EntityRes.GetString("ViewGen_Entities");

        internal static string ViewGen_EntityInstanceToken =>
            EntityRes.GetString("ViewGen_EntityInstanceToken");

        internal static string ViewGen_Error =>
            EntityRes.GetString("ViewGen_Error");

        internal static string Viewgen_ErrorPattern_Partition_Disj_Eq =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Disj_Eq");

        internal static string Viewgen_ErrorPattern_Partition_Disj_Subs =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Disj_Subs");

        internal static string Viewgen_ErrorPattern_Partition_Disj_Subs_Ref =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Disj_Subs_Ref");

        internal static string Viewgen_ErrorPattern_Partition_Disj_Unk =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Disj_Unk");

        internal static string Viewgen_ErrorPattern_Partition_Eq_Disj =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Eq_Disj");

        internal static string Viewgen_ErrorPattern_Partition_Eq_Subs =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Eq_Subs");

        internal static string Viewgen_ErrorPattern_Partition_Eq_Subs_Ref =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Eq_Subs_Ref");

        internal static string Viewgen_ErrorPattern_Partition_Eq_Unk =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Eq_Unk");

        internal static string Viewgen_ErrorPattern_Partition_Eq_Unk_Ass =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Eq_Unk_Ass");

        internal static string Viewgen_ErrorPattern_Partition_Sub_Disj =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Sub_Disj");

        internal static string Viewgen_ErrorPattern_Partition_Sub_Eq =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Sub_Eq");

        internal static string Viewgen_ErrorPattern_Partition_Sub_Eq_Ref =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Sub_Eq_Ref");

        internal static string Viewgen_ErrorPattern_Partition_Sub_Unk =>
            EntityRes.GetString("Viewgen_ErrorPattern_Partition_Sub_Unk");

        internal static string ViewGen_Extent =>
            EntityRes.GetString("ViewGen_Extent");

        internal static string ViewGen_Failure =>
            EntityRes.GetString("ViewGen_Failure");

        internal static string ViewGen_Internal_Error =>
            EntityRes.GetString("ViewGen_Internal_Error");

        internal static string ViewGen_NotNull =>
            EntityRes.GetString("ViewGen_NotNull");

        internal static string ViewGen_Null =>
            EntityRes.GetString("ViewGen_Null");

        internal static string ViewGen_OR =>
            EntityRes.GetString("ViewGen_OR");

        internal static string ViewGen_Tuples =>
            EntityRes.GetString("ViewGen_Tuples");

        internal static string WildcardEnumeratorReturnedNull =>
            EntityRes.GetString("WildcardEnumeratorReturnedNull");
    }
}

