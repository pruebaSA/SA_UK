namespace System.Linq.Expressions
{
    using System;

    internal static class Error
    {
        internal static Exception ArgumentCannotBeOfTypeVoid() => 
            new ArgumentException(Strings.ArgumentCannotBeOfTypeVoid);

        internal static Exception ArgumentMemberNotDeclOnType(object p0, object p1) => 
            new ArgumentException(Strings.ArgumentMemberNotDeclOnType(p0, p1));

        internal static Exception ArgumentMustBeArray() => 
            new ArgumentException(Strings.ArgumentMustBeArray);

        internal static Exception ArgumentMustBeArrayIndexType() => 
            new ArgumentException(Strings.ArgumentMustBeArrayIndexType);

        internal static Exception ArgumentMustBeBoolean() => 
            new ArgumentException(Strings.ArgumentMustBeBoolean);

        internal static Exception ArgumentMustBeCheckable() => 
            new ArgumentException(Strings.ArgumentMustBeCheckable);

        internal static Exception ArgumentMustBeComparable() => 
            new ArgumentException(Strings.ArgumentMustBeComparable);

        internal static Exception ArgumentMustBeConvertible() => 
            new ArgumentException(Strings.ArgumentMustBeConvertible);

        internal static Exception ArgumentMustBeFieldInfoOrPropertInfo() => 
            new ArgumentException(Strings.ArgumentMustBeFieldInfoOrPropertInfo);

        internal static Exception ArgumentMustBeFieldInfoOrPropertInfoOrMethod() => 
            new ArgumentException(Strings.ArgumentMustBeFieldInfoOrPropertInfoOrMethod);

        internal static Exception ArgumentMustBeInstanceMember() => 
            new ArgumentException(Strings.ArgumentMustBeInstanceMember);

        internal static Exception ArgumentMustBeInt32() => 
            new ArgumentException(Strings.ArgumentMustBeInt32);

        internal static Exception ArgumentMustBeInteger() => 
            new ArgumentException(Strings.ArgumentMustBeInteger);

        internal static Exception ArgumentMustBeIntegerOrBoolean() => 
            new ArgumentException(Strings.ArgumentMustBeIntegerOrBoolean);

        internal static Exception ArgumentMustBeNumeric() => 
            new ArgumentException(Strings.ArgumentMustBeNumeric);

        internal static Exception ArgumentMustBeSingleDimensionalArrayType() => 
            new ArgumentException(Strings.ArgumentMustBeSingleDimensionalArrayType);

        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static Exception ArgumentTypeDoesNotMatchMember(object p0, object p1) => 
            new ArgumentException(Strings.ArgumentTypeDoesNotMatchMember(p0, p1));

        internal static Exception ArgumentTypesMustMatch() => 
            new ArgumentException(Strings.ArgumentTypesMustMatch);

        internal static Exception BinaryOperatorNotDefined(object p0, object p1, object p2) => 
            new InvalidOperationException(Strings.BinaryOperatorNotDefined(p0, p1, p2));

        internal static Exception CannotAutoInitializeValueTypeElementThroughProperty(object p0) => 
            new InvalidOperationException(Strings.CannotAutoInitializeValueTypeElementThroughProperty(p0));

        internal static Exception CannotAutoInitializeValueTypeMemberThroughProperty(object p0) => 
            new InvalidOperationException(Strings.CannotAutoInitializeValueTypeMemberThroughProperty(p0));

        internal static Exception CannotCastTypeToType(object p0, object p1) => 
            new ArgumentException(Strings.CannotCastTypeToType(p0, p1));

        internal static Exception CoalesceUsedOnNonNullType() => 
            new InvalidOperationException(Strings.CoalesceUsedOnNonNullType);

        internal static Exception CoercionOperatorNotDefined(object p0, object p1) => 
            new InvalidOperationException(Strings.CoercionOperatorNotDefined(p0, p1));

        internal static Exception ElementInitializerMethodNoRefOutParam(object p0, object p1) => 
            new ArgumentException(Strings.ElementInitializerMethodNoRefOutParam(p0, p1));

        internal static Exception ElementInitializerMethodNotAdd() => 
            new ArgumentException(Strings.ElementInitializerMethodNotAdd);

        internal static Exception ElementInitializerMethodStatic() => 
            new ArgumentException(Strings.ElementInitializerMethodStatic);

        internal static Exception ElementInitializerMethodWithZeroArgs() => 
            new ArgumentException(Strings.ElementInitializerMethodWithZeroArgs);

        internal static Exception ExpressionMayNotContainByrefParameters() => 
            new ArgumentException(Strings.ExpressionMayNotContainByrefParameters);

        internal static Exception ExpressionTypeCannotInitializeArrayType(object p0, object p1) => 
            new InvalidOperationException(Strings.ExpressionTypeCannotInitializeArrayType(p0, p1));

        internal static Exception ExpressionTypeCannotInitializeCollectionType(object p0, object p1) => 
            new InvalidOperationException(Strings.ExpressionTypeCannotInitializeCollectionType(p0, p1));

        internal static Exception ExpressionTypeDoesNotMatchArrayType(object p0, object p1) => 
            new InvalidOperationException(Strings.ExpressionTypeDoesNotMatchArrayType(p0, p1));

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1) => 
            new ArgumentException(Strings.ExpressionTypeDoesNotMatchConstructorParameter(p0, p1));

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2) => 
            new ArgumentException(Strings.ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2));

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1) => 
            new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1));

        internal static Exception ExpressionTypeDoesNotMatchReturn(object p0, object p1) => 
            new ArgumentException(Strings.ExpressionTypeDoesNotMatchReturn(p0, p1));

        internal static Exception ExpressionTypeNotInvocable(object p0) => 
            new ArgumentException(Strings.ExpressionTypeNotInvocable(p0));

        internal static Exception FieldNotDefinedForType(object p0, object p1) => 
            new ArgumentException(Strings.FieldNotDefinedForType(p0, p1));

        internal static Exception IncorrectNumberOfArgumentsForMembers() => 
            new ArgumentException(Strings.IncorrectNumberOfArgumentsForMembers);

        internal static Exception IncorrectNumberOfConstructorArguments() => 
            new ArgumentException(Strings.IncorrectNumberOfConstructorArguments);

        internal static Exception IncorrectNumberOfIndexes() => 
            new ArgumentException(Strings.IncorrectNumberOfIndexes);

        internal static Exception IncorrectNumberOfLambdaArguments() => 
            new InvalidOperationException(Strings.IncorrectNumberOfLambdaArguments);

        internal static Exception IncorrectNumberOfLambdaDeclarationParameters() => 
            new ArgumentException(Strings.IncorrectNumberOfLambdaDeclarationParameters);

        internal static Exception IncorrectNumberOfMembersForGivenConstructor() => 
            new ArgumentException(Strings.IncorrectNumberOfMembersForGivenConstructor);

        internal static Exception IncorrectNumberOfMethodCallArguments(object p0) => 
            new ArgumentException(Strings.IncorrectNumberOfMethodCallArguments(p0));

        internal static Exception IncorrectNumberOfTypeArgsForAction() => 
            new ArgumentException(Strings.IncorrectNumberOfTypeArgsForAction);

        internal static Exception IncorrectNumberOfTypeArgsForFunc() => 
            new ArgumentException(Strings.IncorrectNumberOfTypeArgsForFunc);

        internal static Exception IncorrectTypeForTypeAs(object p0) => 
            new ArgumentException(Strings.IncorrectTypeForTypeAs(p0));

        internal static Exception InvalidCast(object p0, object p1) => 
            new InvalidOperationException(Strings.InvalidCast(p0, p1));

        internal static Exception LambdaParameterNotInScope() => 
            new InvalidOperationException(Strings.LambdaParameterNotInScope);

        internal static Exception LambdaTypeMustBeDerivedFromSystemDelegate() => 
            new ArgumentException(Strings.LambdaTypeMustBeDerivedFromSystemDelegate);

        internal static Exception ListInitializerWithZeroMembers() => 
            new ArgumentException(Strings.ListInitializerWithZeroMembers);

        internal static Exception LogicalOperatorMustHaveBooleanOperators(object p0, object p1) => 
            new ArgumentException(Strings.LogicalOperatorMustHaveBooleanOperators(p0, p1));

        internal static Exception LogicalOperatorMustHaveConsistentTypes(object p0, object p1) => 
            new ArgumentException(Strings.LogicalOperatorMustHaveConsistentTypes(p0, p1));

        internal static Exception MemberNotFieldOrProperty(object p0) => 
            new ArgumentException(Strings.MemberNotFieldOrProperty(p0));

        internal static Exception MethodContainsGenericParameters(object p0) => 
            new ArgumentException(Strings.MethodContainsGenericParameters(p0));

        internal static Exception MethodDoesNotExistOnType(object p0, object p1) => 
            new InvalidOperationException(Strings.MethodDoesNotExistOnType(p0, p1));

        internal static Exception MethodIsGeneric(object p0) => 
            new ArgumentException(Strings.MethodIsGeneric(p0));

        internal static Exception MethodNotDefinedForType(object p0, object p1) => 
            new ArgumentException(Strings.MethodNotDefinedForType(p0, p1));

        internal static Exception MethodNotPropertyAccessor(object p0, object p1) => 
            new ArgumentException(Strings.MethodNotPropertyAccessor(p0, p1));

        internal static Exception MethodWithArgsDoesNotExistOnType(object p0, object p1) => 
            new InvalidOperationException(Strings.MethodWithArgsDoesNotExistOnType(p0, p1));

        internal static Exception MethodWithMoreThanOneMatch(object p0, object p1) => 
            new InvalidOperationException(Strings.MethodWithMoreThanOneMatch(p0, p1));

        internal static Exception NotAMemberOfType(object p0, object p1) => 
            new ArgumentException(Strings.NotAMemberOfType(p0, p1));

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();

        internal static Exception OperandTypesDoNotMatchParameters(object p0, object p1) => 
            new InvalidOperationException(Strings.OperandTypesDoNotMatchParameters(p0, p1));

        internal static Exception OperatorNotImplementedForType(object p0, object p1) => 
            new NotImplementedException(Strings.OperatorNotImplementedForType(p0, p1));

        internal static Exception ParameterExpressionNotValidAsDelegate(object p0, object p1) => 
            new ArgumentException(Strings.ParameterExpressionNotValidAsDelegate(p0, p1));

        internal static Exception ParameterNotCaptured() => 
            new ArgumentException(Strings.ParameterNotCaptured);

        internal static Exception PropertyDoesNotHaveGetter(object p0) => 
            new ArgumentException(Strings.PropertyDoesNotHaveGetter(p0));

        internal static Exception PropertyDoesNotHaveSetter(object p0) => 
            new ArgumentException(Strings.PropertyDoesNotHaveSetter(p0));

        internal static Exception PropertyNotDefinedForType(object p0, object p1) => 
            new ArgumentException(Strings.PropertyNotDefinedForType(p0, p1));

        internal static Exception TypeContainsGenericParameters(object p0) => 
            new ArgumentException(Strings.TypeContainsGenericParameters(p0));

        internal static Exception TypeIsGeneric(object p0) => 
            new ArgumentException(Strings.TypeIsGeneric(p0));

        internal static Exception TypeMissingDefaultConstructor(object p0) => 
            new ArgumentException(Strings.TypeMissingDefaultConstructor(p0));

        internal static Exception TypeNotIEnumerable(object p0) => 
            new ArgumentException(Strings.TypeNotIEnumerable(p0));

        internal static Exception TypeParameterIsNotDelegate(object p0) => 
            new InvalidOperationException(Strings.TypeParameterIsNotDelegate(p0));

        internal static Exception UnaryOperatorNotDefined(object p0, object p1) => 
            new InvalidOperationException(Strings.UnaryOperatorNotDefined(p0, p1));

        internal static Exception UnexpectedCoalesceOperator() => 
            new InvalidOperationException(Strings.UnexpectedCoalesceOperator);

        internal static Exception UnhandledBinary(object p0) => 
            new ArgumentException(Strings.UnhandledBinary(p0));

        internal static Exception UnhandledBinding() => 
            new ArgumentException(Strings.UnhandledBinding);

        internal static Exception UnhandledBindingType(object p0) => 
            new ArgumentException(Strings.UnhandledBindingType(p0));

        internal static Exception UnhandledCall(object p0) => 
            new ArgumentException(Strings.UnhandledCall(p0));

        internal static Exception UnhandledConvert(object p0) => 
            new ArgumentException(Strings.UnhandledConvert(p0));

        internal static Exception UnhandledConvertFromDecimal(object p0) => 
            new ArgumentException(Strings.UnhandledConvertFromDecimal(p0));

        internal static Exception UnhandledConvertToDecimal(object p0) => 
            new ArgumentException(Strings.UnhandledConvertToDecimal(p0));

        internal static Exception UnhandledExpressionType(object p0) => 
            new ArgumentException(Strings.UnhandledExpressionType(p0));

        internal static Exception UnhandledMemberAccess(object p0) => 
            new ArgumentException(Strings.UnhandledMemberAccess(p0));

        internal static Exception UnhandledUnary(object p0) => 
            new ArgumentException(Strings.UnhandledUnary(p0));

        internal static Exception UnknownBindingType() => 
            new ArgumentException(Strings.UnknownBindingType);

        internal static Exception UserDefinedOperatorMustBeStatic(object p0) => 
            new ArgumentException(Strings.UserDefinedOperatorMustBeStatic(p0));

        internal static Exception UserDefinedOperatorMustNotBeVoid(object p0) => 
            new ArgumentException(Strings.UserDefinedOperatorMustNotBeVoid(p0));
    }
}

