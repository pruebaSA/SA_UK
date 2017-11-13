namespace System.Data.Linq.SqlClient
{
    using System;

    internal static class Strings
    {
        internal static string ArgumentEmpty(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ArgumentEmpty", new object[] { p0 });

        internal static string ArgumentTypeMismatch(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ArgumentTypeMismatch", new object[] { p0 });

        internal static string ArgumentWrongType(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("ArgumentWrongType", new object[] { p0, p1, p2 });

        internal static string ArgumentWrongValue(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ArgumentWrongValue", new object[] { p0 });

        internal static string BadParameterType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("BadParameterType", new object[] { p0 });

        internal static string BinaryOperatorNotRecognized(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("BinaryOperatorNotRecognized", new object[] { p0 });

        internal static string CannotAggregateType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotAggregateType", new object[] { p0 });

        internal static string CannotAssignNull(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotAssignNull", new object[] { p0 });

        internal static string CannotAssignToMember(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotAssignToMember", new object[] { p0 });

        internal static string CannotConvertToEntityRef(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotConvertToEntityRef", new object[] { p0 });

        internal static string CannotDeleteTypesOf(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotDeleteTypesOf", new object[] { p0 });

        internal static string CannotMaterializeEntityType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CannotMaterializeEntityType", new object[] { p0 });

        internal static string ClassLiteralsNotAllowed(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ClassLiteralsNotAllowed", new object[] { p0 });

        internal static string ClientCaseShouldNotHold(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ClientCaseShouldNotHold", new object[] { p0 });

        internal static string ClrBoolDoesNotAgreeWithSqlType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ClrBoolDoesNotAgreeWithSqlType", new object[] { p0 });

        internal static string ColumnIsDefinedInMultiplePlaces(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ColumnIsDefinedInMultiplePlaces", new object[] { p0 });

        internal static string ColumnIsNotAccessibleThroughDistinct(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ColumnIsNotAccessibleThroughDistinct", new object[] { p0 });

        internal static string ColumnIsNotAccessibleThroughGroupBy(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ColumnIsNotAccessibleThroughGroupBy", new object[] { p0 });

        internal static string ColumnReferencedIsNotInScope(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ColumnReferencedIsNotInScope", new object[] { p0 });

        internal static string ComparisonNotSupportedForType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ComparisonNotSupportedForType", new object[] { p0 });

        internal static string CompiledQueryCannotReturnType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CompiledQueryCannotReturnType", new object[] { p0 });

        internal static string CouldNotAssignSequence(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotAssignSequence", new object[] { p0, p1 });

        internal static string CouldNotConvertToPropertyOrField(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotConvertToPropertyOrField", new object[] { p0 });

        internal static string CouldNotDetermineDbGeneratedSqlType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotDetermineDbGeneratedSqlType", new object[] { p0 });

        internal static string CouldNotDetermineSqlType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotDetermineSqlType", new object[] { p0 });

        internal static string CouldNotHandleAliasRef(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotHandleAliasRef", new object[] { p0 });

        internal static string CouldNotTranslateExpressionForReading(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CouldNotTranslateExpressionForReading", new object[] { p0 });

        internal static string CreateDatabaseFailedBecauseOfClassWithNoMembers(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CreateDatabaseFailedBecauseOfClassWithNoMembers", new object[] { p0 });

        internal static string CreateDatabaseFailedBecauseOfContextWithNoTables(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CreateDatabaseFailedBecauseOfContextWithNoTables", new object[] { p0 });

        internal static string CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists", new object[] { p0 });

        internal static string DidNotExpectAs(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("DidNotExpectAs", new object[] { p0 });

        internal static string DidNotExpectTypeChange(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("DidNotExpectTypeChange", new object[] { p0, p1 });

        internal static string ExpectedClrTypesToAgree(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("ExpectedClrTypesToAgree", new object[] { p0, p1 });

        internal static string ExpectedQueryableArgument(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("ExpectedQueryableArgument", new object[] { p0, p1, p2 });

        internal static string IifReturnTypesMustBeEqual(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("IifReturnTypesMustBeEqual", new object[] { p0, p1 });

        internal static string InvalidConnectionArgument(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidConnectionArgument", new object[] { p0 });

        internal static string InvalidDbGeneratedType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidDbGeneratedType", new object[] { p0 });

        internal static string InvalidFormatNode(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidFormatNode", new object[] { p0 });

        internal static string InvalidGroupByExpressionType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidGroupByExpressionType", new object[] { p0 });

        internal static string InvalidMethodExecution(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidMethodExecution", new object[] { p0 });

        internal static string InvalidOrderByExpression(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidOrderByExpression", new object[] { p0 });

        internal static string InvalidProviderType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidProviderType", new object[] { p0 });

        internal static string InvalidReturnFromSproc(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidReturnFromSproc", new object[] { p0 });

        internal static string InvalidSequenceOperatorCall(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("InvalidSequenceOperatorCall", new object[] { p0 });

        internal static string LenOfTextOrNTextNotSupported(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("LenOfTextOrNTextNotSupported", new object[] { p0 });

        internal static string LogAttemptingToDeleteDatabase(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("LogAttemptingToDeleteDatabase", new object[] { p0 });

        internal static string LogGeneralInfoMessage(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("LogGeneralInfoMessage", new object[] { p0, p1 });

        internal static string LogStoredProcedureExecution(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("LogStoredProcedureExecution", new object[] { p0, p1 });

        internal static string MappedTypeMustHaveDefaultConstructor(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("MappedTypeMustHaveDefaultConstructor", new object[] { p0 });

        internal static string MaxSizeNotSupported(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("MaxSizeNotSupported", new object[] { p0 });

        internal static string MemberAccessIllegal(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("MemberAccessIllegal", new object[] { p0, p1, p2 });

        internal static string MemberCannotBeTranslated(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("MemberCannotBeTranslated", new object[] { p0, p1 });

        internal static string MemberCouldNotBeTranslated(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("MemberCouldNotBeTranslated", new object[] { p0, p1 });

        internal static string MemberNotPartOfProjection(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("MemberNotPartOfProjection", new object[] { p0, p1 });

        internal static string MethodFormHasNoSupportConversionToSql(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("MethodFormHasNoSupportConversionToSql", new object[] { p0, p1 });

        internal static string MethodHasNoSupportConversionToSql(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("MethodHasNoSupportConversionToSql", new object[] { p0 });

        internal static string MethodNotMappedToStoredProcedure(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("MethodNotMappedToStoredProcedure", new object[] { p0 });

        internal static string NoMethodInTypeMatchingArguments(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("NoMethodInTypeMatchingArguments", new object[] { p0 });

        internal static string NonConstantExpressionsNotSupportedFor(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("NonConstantExpressionsNotSupportedFor", new object[] { p0 });

        internal static string NonCountAggregateFunctionsAreNotValidOnProjections(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("NonCountAggregateFunctionsAreNotValidOnProjections", new object[] { p0 });

        internal static string ParameterNotInScope(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ParameterNotInScope", new object[] { p0 });

        internal static string ProviderNotInstalled(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("ProviderNotInstalled", new object[] { p0, p1 });

        internal static string QueryOperatorNotSupported(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("QueryOperatorNotSupported", new object[] { p0 });

        internal static string QueryOperatorOverloadNotSupported(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("QueryOperatorOverloadNotSupported", new object[] { p0 });

        internal static string RequiredColumnDoesNotExist(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("RequiredColumnDoesNotExist", new object[] { p0 });

        internal static string ResultTypeNotMappedToFunction(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("ResultTypeNotMappedToFunction", new object[] { p0, p1 });

        internal static string SequenceOperatorsNotSupportedForType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("SequenceOperatorsNotSupportedForType", new object[] { p0 });

        internal static string SimpleCaseShouldNotHold(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("SimpleCaseShouldNotHold", new object[] { p0 });

        internal static string SourceExpressionAnnotation(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("SourceExpressionAnnotation", new object[] { p0 });

        internal static string SqlMethodOnlyForSql(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("SqlMethodOnlyForSql", new object[] { p0 });

        internal static string TextNTextAndImageCannotOccurInDistinct(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("TextNTextAndImageCannotOccurInDistinct", new object[] { p0 });

        internal static string TextNTextAndImageCannotOccurInUnion(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("TextNTextAndImageCannotOccurInUnion", new object[] { p0 });

        internal static string TypeCannotBeOrdered(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("TypeCannotBeOrdered", new object[] { p0 });

        internal static string UnableToBindUnmappedMember(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("UnableToBindUnmappedMember", new object[] { p0, p1, p2 });

        internal static string UnexpectedNode(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnexpectedNode", new object[] { p0 });

        internal static string UnexpectedTypeCode(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnexpectedTypeCode", new object[] { p0 });

        internal static string UnhandledBindingType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnhandledBindingType", new object[] { p0 });

        internal static string UnhandledExpressionType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnhandledExpressionType", new object[] { p0 });

        internal static string UnhandledMemberAccess(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("UnhandledMemberAccess", new object[] { p0, p1 });

        internal static string UnmappedDataMember(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("UnmappedDataMember", new object[] { p0, p1, p2 });

        internal static string UnrecognizedExpressionNode(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnrecognizedExpressionNode", new object[] { p0 });

        internal static string UnrecognizedProviderMode(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnrecognizedProviderMode", new object[] { p0 });

        internal static string UnsafeStringConversion(object p0, object p1) => 
            System.Data.Linq.SqlClient.SR.GetString("UnsafeStringConversion", new object[] { p0, p1 });

        internal static string UnsupportedNodeType(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedNodeType", new object[] { p0 });

        internal static string UnsupportedTypeConstructorForm(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedTypeConstructorForm", new object[] { p0 });

        internal static string ValueHasNoLiteralInSql(object p0) => 
            System.Data.Linq.SqlClient.SR.GetString("ValueHasNoLiteralInSql", new object[] { p0 });

        internal static string WrongNumberOfValuesInCollectionArgument(object p0, object p1, object p2) => 
            System.Data.Linq.SqlClient.SR.GetString("WrongNumberOfValuesInCollectionArgument", new object[] { p0, p1, p2 });

        internal static string BadProjectionInSelect =>
            System.Data.Linq.SqlClient.SR.GetString("BadProjectionInSelect");

        internal static string CannotCompareItemsAssociatedWithDifferentTable =>
            System.Data.Linq.SqlClient.SR.GetString("CannotCompareItemsAssociatedWithDifferentTable");

        internal static string CannotEnumerateResultsMoreThanOnce =>
            System.Data.Linq.SqlClient.SR.GetString("CannotEnumerateResultsMoreThanOnce");

        internal static string CannotTranslateExpressionToSql =>
            System.Data.Linq.SqlClient.SR.GetString("CannotTranslateExpressionToSql");

        internal static string CapturedValuesCannotBeSequences =>
            System.Data.Linq.SqlClient.SR.GetString("CapturedValuesCannotBeSequences");

        internal static string ColumnCannotReferToItself =>
            System.Data.Linq.SqlClient.SR.GetString("ColumnCannotReferToItself");

        internal static string ColumnClrTypeDoesNotAgreeWithExpressionsClrType =>
            System.Data.Linq.SqlClient.SR.GetString("ColumnClrTypeDoesNotAgreeWithExpressionsClrType");

        internal static string CompiledQueryAgainstMultipleShapesNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("CompiledQueryAgainstMultipleShapesNotSupported");

        internal static string ConstructedArraysNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("ConstructedArraysNotSupported");

        internal static string ContextNotInitialized =>
            System.Data.Linq.SqlClient.SR.GetString("ContextNotInitialized");

        internal static string ConvertToCharFromBoolNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("ConvertToCharFromBoolNotSupported");

        internal static string ConvertToDateTimeOnlyForDateTimeOrString =>
            System.Data.Linq.SqlClient.SR.GetString("ConvertToDateTimeOnlyForDateTimeOrString");

        internal static string CouldNotDetermineCatalogName =>
            System.Data.Linq.SqlClient.SR.GetString("CouldNotDetermineCatalogName");

        internal static string CouldNotGetClrType =>
            System.Data.Linq.SqlClient.SR.GetString("CouldNotGetClrType");

        internal static string CouldNotGetSqlType =>
            System.Data.Linq.SqlClient.SR.GetString("CouldNotGetSqlType");

        internal static string DatabaseDeleteThroughContext =>
            System.Data.Linq.SqlClient.SR.GetString("DatabaseDeleteThroughContext");

        internal static string DeferredMemberWrongType =>
            System.Data.Linq.SqlClient.SR.GetString("DeferredMemberWrongType");

        internal static string DidNotExpectTypeBinding =>
            System.Data.Linq.SqlClient.SR.GetString("DidNotExpectTypeBinding");

        internal static string DistributedTransactionsAreNotAllowed =>
            System.Data.Linq.SqlClient.SR.GetString("DistributedTransactionsAreNotAllowed");

        internal static string EmptyCaseNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("EmptyCaseNotSupported");

        internal static string ExceptNotSupportedForHierarchicalTypes =>
            System.Data.Linq.SqlClient.SR.GetString("ExceptNotSupportedForHierarchicalTypes");

        internal static string ExpectedBitFoundPredicate =>
            System.Data.Linq.SqlClient.SR.GetString("ExpectedBitFoundPredicate");

        internal static string ExpectedNoObjectType =>
            System.Data.Linq.SqlClient.SR.GetString("ExpectedNoObjectType");

        internal static string ExpectedPredicateFoundBit =>
            System.Data.Linq.SqlClient.SR.GetString("ExpectedPredicateFoundBit");

        internal static string ExpressionNotDeferredQuerySource =>
            System.Data.Linq.SqlClient.SR.GetString("ExpressionNotDeferredQuerySource");

        internal static string GeneralCollectionMaterializationNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("GeneralCollectionMaterializationNotSupported");

        internal static string GroupingNotSupportedAsOrderCriterion =>
            System.Data.Linq.SqlClient.SR.GetString("GroupingNotSupportedAsOrderCriterion");

        internal static string Impossible =>
            System.Data.Linq.SqlClient.SR.GetString("Impossible");

        internal static string IndexOfWithStringComparisonArgNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("IndexOfWithStringComparisonArgNotSupported");

        internal static string InfiniteDescent =>
            System.Data.Linq.SqlClient.SR.GetString("InfiniteDescent");

        internal static string InsertItemMustBeConstant =>
            System.Data.Linq.SqlClient.SR.GetString("InsertItemMustBeConstant");

        internal static string IntersectNotSupportedForHierarchicalTypes =>
            System.Data.Linq.SqlClient.SR.GetString("IntersectNotSupportedForHierarchicalTypes");

        internal static string InvalidGroupByExpression =>
            System.Data.Linq.SqlClient.SR.GetString("InvalidGroupByExpression");

        internal static string InvalidReferenceToRemovedAliasDuringDeflation =>
            System.Data.Linq.SqlClient.SR.GetString("InvalidReferenceToRemovedAliasDuringDeflation");

        internal static string LastIndexOfWithStringComparisonArgNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("LastIndexOfWithStringComparisonArgNotSupported");

        internal static string MathRoundNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("MathRoundNotSupported");

        internal static string NonConstantExpressionsNotSupportedForRounding =>
            System.Data.Linq.SqlClient.SR.GetString("NonConstantExpressionsNotSupportedForRounding");

        internal static string OwningTeam =>
            System.Data.Linq.SqlClient.SR.GetString("OwningTeam");

        internal static string ParametersCannotBeSequences =>
            System.Data.Linq.SqlClient.SR.GetString("ParametersCannotBeSequences");

        internal static string ProviderCannotBeUsedAfterDispose =>
            System.Data.Linq.SqlClient.SR.GetString("ProviderCannotBeUsedAfterDispose");

        internal static string QueryOnLocalCollectionNotSupported =>
            System.Data.Linq.SqlClient.SR.GetString("QueryOnLocalCollectionNotSupported");

        internal static string ReaderUsedAfterDispose =>
            System.Data.Linq.SqlClient.SR.GetString("ReaderUsedAfterDispose");

        internal static string SelectManyDoesNotSupportStrings =>
            System.Data.Linq.SqlClient.SR.GetString("SelectManyDoesNotSupportStrings");

        internal static string SkipIsValidOnlyOverOrderedQueries =>
            System.Data.Linq.SqlClient.SR.GetString("SkipIsValidOnlyOverOrderedQueries");

        internal static string SkipNotSupportedForSequenceTypes =>
            System.Data.Linq.SqlClient.SR.GetString("SkipNotSupportedForSequenceTypes");

        internal static string SkipRequiresSingleTableQueryWithPKs =>
            System.Data.Linq.SqlClient.SR.GetString("SkipRequiresSingleTableQueryWithPKs");

        internal static string SprocsCannotBeComposed =>
            System.Data.Linq.SqlClient.SR.GetString("SprocsCannotBeComposed");

        internal static string ToStringOnlySupportedForPrimitiveTypes =>
            System.Data.Linq.SqlClient.SR.GetString("ToStringOnlySupportedForPrimitiveTypes");

        internal static string TransactionDoesNotMatchConnection =>
            System.Data.Linq.SqlClient.SR.GetString("TransactionDoesNotMatchConnection");

        internal static string TypeBinaryOperatorNotRecognized =>
            System.Data.Linq.SqlClient.SR.GetString("TypeBinaryOperatorNotRecognized");

        internal static string TypeColumnWithUnhandledSource =>
            System.Data.Linq.SqlClient.SR.GetString("TypeColumnWithUnhandledSource");

        internal static string UnexpectedFloatingColumn =>
            System.Data.Linq.SqlClient.SR.GetString("UnexpectedFloatingColumn");

        internal static string UnexpectedSharedExpression =>
            System.Data.Linq.SqlClient.SR.GetString("UnexpectedSharedExpression");

        internal static string UnexpectedSharedExpressionReference =>
            System.Data.Linq.SqlClient.SR.GetString("UnexpectedSharedExpressionReference");

        internal static string UnhandledStringTypeComparison =>
            System.Data.Linq.SqlClient.SR.GetString("UnhandledStringTypeComparison");

        internal static string UnionDifferentMemberOrder =>
            System.Data.Linq.SqlClient.SR.GetString("UnionDifferentMemberOrder");

        internal static string UnionDifferentMembers =>
            System.Data.Linq.SqlClient.SR.GetString("UnionDifferentMembers");

        internal static string UnionIncompatibleConstruction =>
            System.Data.Linq.SqlClient.SR.GetString("UnionIncompatibleConstruction");

        internal static string UnionOfIncompatibleDynamicTypes =>
            System.Data.Linq.SqlClient.SR.GetString("UnionOfIncompatibleDynamicTypes");

        internal static string UnionWithHierarchy =>
            System.Data.Linq.SqlClient.SR.GetString("UnionWithHierarchy");

        internal static string UnsupportedDateTimeConstructorForm =>
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedDateTimeConstructorForm");

        internal static string UnsupportedDateTimeOffsetConstructorForm =>
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedDateTimeOffsetConstructorForm");

        internal static string UnsupportedStringConstructorForm =>
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedStringConstructorForm");

        internal static string UnsupportedTimeSpanConstructorForm =>
            System.Data.Linq.SqlClient.SR.GetString("UnsupportedTimeSpanConstructorForm");

        internal static string UpdateItemMustBeConstant =>
            System.Data.Linq.SqlClient.SR.GetString("UpdateItemMustBeConstant");

        internal static string VbLikeDoesNotSupportMultipleCharacterRanges =>
            System.Data.Linq.SqlClient.SR.GetString("VbLikeDoesNotSupportMultipleCharacterRanges");

        internal static string VbLikeUnclosedBracket =>
            System.Data.Linq.SqlClient.SR.GetString("VbLikeUnclosedBracket");

        internal static string WrongDataContext =>
            System.Data.Linq.SqlClient.SR.GetString("WrongDataContext");
    }
}

