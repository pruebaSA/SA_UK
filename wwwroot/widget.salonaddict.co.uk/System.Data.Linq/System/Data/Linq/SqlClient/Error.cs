namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Transactions;

    internal static class Error
    {
        internal static Exception ArgumentEmpty(object p0) => 
            new ArgumentException(Strings.ArgumentEmpty(p0));

        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static Exception ArgumentTypeMismatch(object p0) => 
            new ArgumentException(Strings.ArgumentTypeMismatch(p0));

        internal static Exception ArgumentWrongType(object p0, object p1, object p2) => 
            new ArgumentException(Strings.ArgumentWrongType(p0, p1, p2));

        internal static Exception ArgumentWrongValue(object p0) => 
            new ArgumentException(Strings.ArgumentWrongValue(p0));

        internal static Exception BadParameterType(object p0) => 
            new NotSupportedException(Strings.BadParameterType(p0));

        internal static Exception BadProjectionInSelect() => 
            new InvalidOperationException(Strings.BadProjectionInSelect);

        internal static Exception BinaryOperatorNotRecognized(object p0) => 
            new InvalidOperationException(Strings.BinaryOperatorNotRecognized(p0));

        internal static Exception CannotAggregateType(object p0) => 
            new NotSupportedException(Strings.CannotAggregateType(p0));

        internal static Exception CannotAssignNull(object p0) => 
            new InvalidOperationException(Strings.CannotAssignNull(p0));

        internal static Exception CannotAssignToMember(object p0) => 
            new InvalidOperationException(Strings.CannotAssignToMember(p0));

        internal static Exception CannotCompareItemsAssociatedWithDifferentTable() => 
            new InvalidOperationException(Strings.CannotCompareItemsAssociatedWithDifferentTable);

        internal static Exception CannotConvertToEntityRef(object p0) => 
            new InvalidOperationException(Strings.CannotConvertToEntityRef(p0));

        internal static Exception CannotDeleteTypesOf(object p0) => 
            new InvalidOperationException(Strings.CannotDeleteTypesOf(p0));

        internal static Exception CannotEnumerateResultsMoreThanOnce() => 
            new InvalidOperationException(Strings.CannotEnumerateResultsMoreThanOnce);

        internal static Exception CannotMaterializeEntityType(object p0) => 
            new NotSupportedException(Strings.CannotMaterializeEntityType(p0));

        internal static Exception CapturedValuesCannotBeSequences() => 
            new NotSupportedException(Strings.CapturedValuesCannotBeSequences);

        internal static Exception ClassLiteralsNotAllowed(object p0) => 
            new InvalidOperationException(Strings.ClassLiteralsNotAllowed(p0));

        internal static Exception ClientCaseShouldNotHold(object p0) => 
            new InvalidOperationException(Strings.ClientCaseShouldNotHold(p0));

        internal static Exception ClrBoolDoesNotAgreeWithSqlType(object p0) => 
            new InvalidOperationException(Strings.ClrBoolDoesNotAgreeWithSqlType(p0));

        internal static Exception ColumnCannotReferToItself() => 
            new InvalidOperationException(Strings.ColumnCannotReferToItself);

        internal static Exception ColumnClrTypeDoesNotAgreeWithExpressionsClrType() => 
            new InvalidOperationException(Strings.ColumnClrTypeDoesNotAgreeWithExpressionsClrType);

        internal static Exception ColumnIsDefinedInMultiplePlaces(object p0) => 
            new InvalidOperationException(Strings.ColumnIsDefinedInMultiplePlaces(p0));

        internal static Exception ColumnIsNotAccessibleThroughDistinct(object p0) => 
            new InvalidOperationException(Strings.ColumnIsNotAccessibleThroughDistinct(p0));

        internal static Exception ColumnIsNotAccessibleThroughGroupBy(object p0) => 
            new InvalidOperationException(Strings.ColumnIsNotAccessibleThroughGroupBy(p0));

        internal static Exception ColumnReferencedIsNotInScope(object p0) => 
            new InvalidOperationException(Strings.ColumnReferencedIsNotInScope(p0));

        internal static Exception ComparisonNotSupportedForType(object p0) => 
            new NotSupportedException(Strings.ComparisonNotSupportedForType(p0));

        internal static Exception CompiledQueryAgainstMultipleShapesNotSupported() => 
            new NotSupportedException(Strings.CompiledQueryAgainstMultipleShapesNotSupported);

        internal static Exception CompiledQueryCannotReturnType(object p0) => 
            new InvalidOperationException(Strings.CompiledQueryCannotReturnType(p0));

        internal static Exception ConstructedArraysNotSupported() => 
            new NotSupportedException(Strings.ConstructedArraysNotSupported);

        internal static Exception ContextNotInitialized() => 
            new InvalidOperationException(Strings.ContextNotInitialized);

        internal static Exception ConvertToCharFromBoolNotSupported() => 
            new NotSupportedException(Strings.ConvertToCharFromBoolNotSupported);

        internal static Exception ConvertToDateTimeOnlyForDateTimeOrString() => 
            new NotSupportedException(Strings.ConvertToDateTimeOnlyForDateTimeOrString);

        internal static Exception CouldNotAssignSequence(object p0, object p1) => 
            new InvalidOperationException(Strings.CouldNotAssignSequence(p0, p1));

        internal static Exception CouldNotConvertToPropertyOrField(object p0) => 
            new InvalidOperationException(Strings.CouldNotConvertToPropertyOrField(p0));

        internal static Exception CouldNotDetermineCatalogName() => 
            new InvalidOperationException(Strings.CouldNotDetermineCatalogName);

        internal static Exception CouldNotDetermineDbGeneratedSqlType(object p0) => 
            new InvalidOperationException(Strings.CouldNotDetermineDbGeneratedSqlType(p0));

        internal static Exception CouldNotDetermineSqlType(object p0) => 
            new InvalidOperationException(Strings.CouldNotDetermineSqlType(p0));

        internal static Exception CouldNotGetClrType() => 
            new InvalidOperationException(Strings.CouldNotGetClrType);

        internal static Exception CouldNotGetSqlType() => 
            new InvalidOperationException(Strings.CouldNotGetSqlType);

        internal static Exception CouldNotHandleAliasRef(object p0) => 
            new InvalidOperationException(Strings.CouldNotHandleAliasRef(p0));

        internal static Exception CouldNotTranslateExpressionForReading(object p0) => 
            new InvalidOperationException(Strings.CouldNotTranslateExpressionForReading(p0));

        internal static Exception CreateDatabaseFailedBecauseOfClassWithNoMembers(object p0) => 
            new InvalidOperationException(Strings.CreateDatabaseFailedBecauseOfClassWithNoMembers(p0));

        internal static Exception CreateDatabaseFailedBecauseOfContextWithNoTables(object p0) => 
            new InvalidOperationException(Strings.CreateDatabaseFailedBecauseOfContextWithNoTables(p0));

        internal static Exception CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists(object p0) => 
            new InvalidOperationException(Strings.CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists(p0));

        internal static Exception DatabaseDeleteThroughContext() => 
            new InvalidOperationException(Strings.DatabaseDeleteThroughContext);

        internal static Exception DeferredMemberWrongType() => 
            new InvalidOperationException(Strings.DeferredMemberWrongType);

        internal static Exception DidNotExpectAs(object p0) => 
            new InvalidOperationException(Strings.DidNotExpectAs(p0));

        internal static Exception DidNotExpectTypeBinding() => 
            new InvalidOperationException(Strings.DidNotExpectTypeBinding);

        internal static Exception DidNotExpectTypeChange(object p0, object p1) => 
            new InvalidOperationException(Strings.DidNotExpectTypeChange(p0, p1));

        internal static Exception DistributedTransactionsAreNotAllowed() => 
            new TransactionPromotionException(Strings.DistributedTransactionsAreNotAllowed);

        internal static Exception EmptyCaseNotSupported() => 
            new InvalidOperationException(Strings.EmptyCaseNotSupported);

        internal static Exception ExceptNotSupportedForHierarchicalTypes() => 
            new NotSupportedException(Strings.ExceptNotSupportedForHierarchicalTypes);

        internal static Exception ExpectedBitFoundPredicate() => 
            new ArgumentException(Strings.ExpectedBitFoundPredicate);

        internal static Exception ExpectedClrTypesToAgree(object p0, object p1) => 
            new InvalidOperationException(Strings.ExpectedClrTypesToAgree(p0, p1));

        internal static Exception ExpectedNoObjectType() => 
            new InvalidOperationException(Strings.ExpectedNoObjectType);

        internal static Exception ExpectedPredicateFoundBit() => 
            new ArgumentException(Strings.ExpectedPredicateFoundBit);

        internal static Exception ExpectedQueryableArgument(object p0, object p1, object p2) => 
            new ArgumentException(Strings.ExpectedQueryableArgument(p0, p1, p2));

        internal static Exception ExpressionNotDeferredQuerySource() => 
            new InvalidOperationException(Strings.ExpressionNotDeferredQuerySource);

        internal static Exception ExpressionNotSupportedForSqlServerVersion(Collection<string> reasons)
        {
            StringBuilder builder = new StringBuilder(Strings.CannotTranslateExpressionToSql);
            foreach (string str in reasons)
            {
                builder.AppendLine(str);
            }
            return new NotSupportedException(builder.ToString());
        }

        internal static Exception GeneralCollectionMaterializationNotSupported() => 
            new NotSupportedException(Strings.GeneralCollectionMaterializationNotSupported);

        internal static Exception GroupingNotSupportedAsOrderCriterion() => 
            new NotSupportedException(Strings.GroupingNotSupportedAsOrderCriterion);

        internal static Exception IifReturnTypesMustBeEqual(object p0, object p1) => 
            new NotSupportedException(Strings.IifReturnTypesMustBeEqual(p0, p1));

        internal static Exception Impossible() => 
            new InvalidOperationException(Strings.Impossible);

        internal static Exception IndexOfWithStringComparisonArgNotSupported() => 
            new NotSupportedException(Strings.IndexOfWithStringComparisonArgNotSupported);

        internal static Exception InfiniteDescent() => 
            new InvalidOperationException(Strings.InfiniteDescent);

        internal static Exception InsertItemMustBeConstant() => 
            new NotSupportedException(Strings.InsertItemMustBeConstant);

        internal static Exception IntersectNotSupportedForHierarchicalTypes() => 
            new NotSupportedException(Strings.IntersectNotSupportedForHierarchicalTypes);

        internal static Exception InvalidConnectionArgument(object p0) => 
            new ArgumentException(Strings.InvalidConnectionArgument(p0));

        internal static Exception InvalidDbGeneratedType(object p0) => 
            new NotSupportedException(Strings.InvalidDbGeneratedType(p0));

        internal static Exception InvalidFormatNode(object p0) => 
            new InvalidOperationException(Strings.InvalidFormatNode(p0));

        internal static Exception InvalidGroupByExpression() => 
            new NotSupportedException(Strings.InvalidGroupByExpression);

        internal static Exception InvalidGroupByExpressionType(object p0) => 
            new NotSupportedException(Strings.InvalidGroupByExpressionType(p0));

        internal static Exception InvalidMethodExecution(object p0) => 
            new InvalidOperationException(Strings.InvalidMethodExecution(p0));

        internal static Exception InvalidOrderByExpression(object p0) => 
            new NotSupportedException(Strings.InvalidOrderByExpression(p0));

        internal static Exception InvalidProviderType(object p0) => 
            new NotSupportedException(Strings.InvalidProviderType(p0));

        internal static Exception InvalidReferenceToRemovedAliasDuringDeflation() => 
            new InvalidOperationException(Strings.InvalidReferenceToRemovedAliasDuringDeflation);

        internal static Exception InvalidReturnFromSproc(object p0) => 
            new InvalidOperationException(Strings.InvalidReturnFromSproc(p0));

        internal static Exception InvalidSequenceOperatorCall(object p0) => 
            new InvalidOperationException(Strings.InvalidSequenceOperatorCall(p0));

        internal static Exception LastIndexOfWithStringComparisonArgNotSupported() => 
            new NotSupportedException(Strings.LastIndexOfWithStringComparisonArgNotSupported);

        internal static Exception MappedTypeMustHaveDefaultConstructor(object p0) => 
            new InvalidOperationException(Strings.MappedTypeMustHaveDefaultConstructor(p0));

        internal static Exception MathRoundNotSupported() => 
            new NotSupportedException(Strings.MathRoundNotSupported);

        internal static Exception MemberAccessIllegal(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.MemberAccessIllegal(p0, p1, p2));

        internal static Exception MemberCannotBeTranslated(object p0, object p1) => 
            new NotSupportedException(Strings.MemberCannotBeTranslated(p0, p1));

        internal static Exception MemberCouldNotBeTranslated(object p0, object p1) => 
            new InvalidOperationException(Strings.MemberCouldNotBeTranslated(p0, p1));

        internal static Exception MemberNotPartOfProjection(object p0, object p1) => 
            new InvalidOperationException(Strings.MemberNotPartOfProjection(p0, p1));

        internal static Exception MethodFormHasNoSupportConversionToSql(object p0, object p1) => 
            new NotSupportedException(Strings.MethodFormHasNoSupportConversionToSql(p0, p1));

        internal static Exception MethodHasNoSupportConversionToSql(object p0) => 
            new NotSupportedException(Strings.MethodHasNoSupportConversionToSql(p0));

        internal static Exception MethodNotMappedToStoredProcedure(object p0) => 
            new InvalidOperationException(Strings.MethodNotMappedToStoredProcedure(p0));

        internal static Exception NoMethodInTypeMatchingArguments(object p0) => 
            new InvalidOperationException(Strings.NoMethodInTypeMatchingArguments(p0));

        internal static Exception NonConstantExpressionsNotSupportedFor(object p0) => 
            new NotSupportedException(Strings.NonConstantExpressionsNotSupportedFor(p0));

        internal static Exception NonConstantExpressionsNotSupportedForRounding() => 
            new NotSupportedException(Strings.NonConstantExpressionsNotSupportedForRounding);

        internal static Exception NonCountAggregateFunctionsAreNotValidOnProjections(object p0) => 
            new NotSupportedException(Strings.NonCountAggregateFunctionsAreNotValidOnProjections(p0));

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();

        internal static Exception ParameterNotInScope(object p0) => 
            new InvalidOperationException(Strings.ParameterNotInScope(p0));

        internal static Exception ParametersCannotBeSequences() => 
            new NotSupportedException(Strings.ParametersCannotBeSequences);

        internal static Exception ProviderCannotBeUsedAfterDispose() => 
            new ObjectDisposedException(Strings.ProviderCannotBeUsedAfterDispose);

        internal static Exception ProviderNotInstalled(object p0, object p1) => 
            new InvalidOperationException(Strings.ProviderNotInstalled(p0, p1));

        internal static Exception QueryOnLocalCollectionNotSupported() => 
            new NotSupportedException(Strings.QueryOnLocalCollectionNotSupported);

        internal static Exception QueryOperatorNotSupported(object p0) => 
            new NotSupportedException(Strings.QueryOperatorNotSupported(p0));

        internal static Exception QueryOperatorOverloadNotSupported(object p0) => 
            new NotSupportedException(Strings.QueryOperatorOverloadNotSupported(p0));

        internal static Exception ReaderUsedAfterDispose() => 
            new InvalidOperationException(Strings.ReaderUsedAfterDispose);

        internal static Exception RequiredColumnDoesNotExist(object p0) => 
            new InvalidOperationException(Strings.RequiredColumnDoesNotExist(p0));

        internal static Exception ResultTypeNotMappedToFunction(object p0, object p1) => 
            new InvalidOperationException(Strings.ResultTypeNotMappedToFunction(p0, p1));

        internal static Exception SelectManyDoesNotSupportStrings() => 
            new ArgumentException(Strings.SelectManyDoesNotSupportStrings);

        internal static Exception SequenceOperatorsNotSupportedForType(object p0) => 
            new NotSupportedException(Strings.SequenceOperatorsNotSupportedForType(p0));

        internal static Exception SimpleCaseShouldNotHold(object p0) => 
            new InvalidOperationException(Strings.SimpleCaseShouldNotHold(p0));

        internal static Exception SkipIsValidOnlyOverOrderedQueries() => 
            new InvalidOperationException(Strings.SkipIsValidOnlyOverOrderedQueries);

        internal static Exception SkipNotSupportedForSequenceTypes() => 
            new NotSupportedException(Strings.SkipNotSupportedForSequenceTypes);

        internal static Exception SkipRequiresSingleTableQueryWithPKs() => 
            new NotSupportedException(Strings.SkipRequiresSingleTableQueryWithPKs);

        internal static Exception SprocsCannotBeComposed() => 
            new InvalidOperationException(Strings.SprocsCannotBeComposed);

        internal static Exception SqlMethodOnlyForSql(object p0) => 
            new NotSupportedException(Strings.SqlMethodOnlyForSql(p0));

        internal static Exception ToStringOnlySupportedForPrimitiveTypes() => 
            new NotSupportedException(Strings.ToStringOnlySupportedForPrimitiveTypes);

        internal static Exception TransactionDoesNotMatchConnection() => 
            new InvalidOperationException(Strings.TransactionDoesNotMatchConnection);

        internal static Exception TypeBinaryOperatorNotRecognized() => 
            new InvalidOperationException(Strings.TypeBinaryOperatorNotRecognized);

        internal static Exception TypeCannotBeOrdered(object p0) => 
            new InvalidOperationException(Strings.TypeCannotBeOrdered(p0));

        internal static Exception TypeColumnWithUnhandledSource() => 
            new InvalidOperationException(Strings.TypeColumnWithUnhandledSource);

        internal static Exception UnableToBindUnmappedMember(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.UnableToBindUnmappedMember(p0, p1, p2));

        internal static Exception UnexpectedFloatingColumn() => 
            new InvalidOperationException(Strings.UnexpectedFloatingColumn);

        internal static Exception UnexpectedNode(object p0) => 
            new InvalidOperationException(Strings.UnexpectedNode(p0));

        internal static Exception UnexpectedSharedExpression() => 
            new InvalidOperationException(Strings.UnexpectedSharedExpression);

        internal static Exception UnexpectedSharedExpressionReference() => 
            new InvalidOperationException(Strings.UnexpectedSharedExpressionReference);

        internal static Exception UnexpectedTypeCode(object p0) => 
            new InvalidOperationException(Strings.UnexpectedTypeCode(p0));

        internal static Exception UnhandledBindingType(object p0) => 
            new InvalidOperationException(Strings.UnhandledBindingType(p0));

        internal static Exception UnhandledExpressionType(object p0) => 
            new ArgumentException(Strings.UnhandledExpressionType(p0));

        internal static Exception UnhandledMemberAccess(object p0, object p1) => 
            new InvalidOperationException(Strings.UnhandledMemberAccess(p0, p1));

        internal static Exception UnhandledStringTypeComparison() => 
            new NotSupportedException(Strings.UnhandledStringTypeComparison);

        internal static Exception UnionDifferentMemberOrder() => 
            new NotSupportedException(Strings.UnionDifferentMemberOrder);

        internal static Exception UnionDifferentMembers() => 
            new NotSupportedException(Strings.UnionDifferentMembers);

        internal static Exception UnionIncompatibleConstruction() => 
            new NotSupportedException(Strings.UnionIncompatibleConstruction);

        internal static Exception UnionOfIncompatibleDynamicTypes() => 
            new NotSupportedException(Strings.UnionOfIncompatibleDynamicTypes);

        internal static Exception UnionWithHierarchy() => 
            new NotSupportedException(Strings.UnionWithHierarchy);

        internal static Exception UnmappedDataMember(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.UnmappedDataMember(p0, p1, p2));

        internal static Exception UnrecognizedExpressionNode(object p0) => 
            new InvalidOperationException(Strings.UnrecognizedExpressionNode(p0));

        internal static Exception UnrecognizedProviderMode(object p0) => 
            new InvalidOperationException(Strings.UnrecognizedProviderMode(p0));

        internal static Exception UnsafeStringConversion(object p0, object p1) => 
            new FormatException(Strings.UnsafeStringConversion(p0, p1));

        internal static Exception UnsupportedDateTimeConstructorForm() => 
            new NotSupportedException(Strings.UnsupportedDateTimeConstructorForm);

        internal static Exception UnsupportedDateTimeOffsetConstructorForm() => 
            new NotSupportedException(Strings.UnsupportedDateTimeOffsetConstructorForm);

        internal static Exception UnsupportedNodeType(object p0) => 
            new NotSupportedException(Strings.UnsupportedNodeType(p0));

        internal static Exception UnsupportedStringConstructorForm() => 
            new NotSupportedException(Strings.UnsupportedStringConstructorForm);

        internal static Exception UnsupportedTimeSpanConstructorForm() => 
            new NotSupportedException(Strings.UnsupportedTimeSpanConstructorForm);

        internal static Exception UnsupportedTypeConstructorForm(object p0) => 
            new NotSupportedException(Strings.UnsupportedTypeConstructorForm(p0));

        internal static Exception UpdateItemMustBeConstant() => 
            new NotSupportedException(Strings.UpdateItemMustBeConstant);

        internal static Exception ValueHasNoLiteralInSql(object p0) => 
            new InvalidOperationException(Strings.ValueHasNoLiteralInSql(p0));

        internal static Exception VbLikeDoesNotSupportMultipleCharacterRanges() => 
            new ArgumentException(Strings.VbLikeDoesNotSupportMultipleCharacterRanges);

        internal static Exception VbLikeUnclosedBracket() => 
            new ArgumentException(Strings.VbLikeUnclosedBracket);

        internal static Exception WrongDataContext() => 
            new InvalidOperationException(Strings.WrongDataContext);

        internal static Exception WrongNumberOfValuesInCollectionArgument(object p0, object p1, object p2) => 
            new ArgumentException(Strings.WrongNumberOfValuesInCollectionArgument(p0, p1, p2));
    }
}

