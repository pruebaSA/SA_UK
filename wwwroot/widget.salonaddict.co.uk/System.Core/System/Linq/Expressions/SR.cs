namespace System.Linq.Expressions
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        internal const string ArgumentCannotBeOfTypeVoid = "ArgumentCannotBeOfTypeVoid";
        internal const string ArgumentMemberNotDeclOnType = "ArgumentMemberNotDeclOnType";
        internal const string ArgumentMustBeArray = "ArgumentMustBeArray";
        internal const string ArgumentMustBeArrayIndexType = "ArgumentMustBeArrayIndexType";
        internal const string ArgumentMustBeBoolean = "ArgumentMustBeBoolean";
        internal const string ArgumentMustBeCheckable = "ArgumentMustBeCheckable";
        internal const string ArgumentMustBeComparable = "ArgumentMustBeComparable";
        internal const string ArgumentMustBeConvertible = "ArgumentMustBeConvertible";
        internal const string ArgumentMustBeFieldInfoOrPropertInfo = "ArgumentMustBeFieldInfoOrPropertInfo";
        internal const string ArgumentMustBeFieldInfoOrPropertInfoOrMethod = "ArgumentMustBeFieldInfoOrPropertInfoOrMethod";
        internal const string ArgumentMustBeInstanceMember = "ArgumentMustBeInstanceMember";
        internal const string ArgumentMustBeInt32 = "ArgumentMustBeInt32";
        internal const string ArgumentMustBeInteger = "ArgumentMustBeInteger";
        internal const string ArgumentMustBeIntegerOrBoolean = "ArgumentMustBeIntegerOrBoolean";
        internal const string ArgumentMustBeNumeric = "ArgumentMustBeNumeric";
        internal const string ArgumentMustBeSingleDimensionalArrayType = "ArgumentMustBeSingleDimensionalArrayType";
        internal const string ArgumentTypeDoesNotMatchMember = "ArgumentTypeDoesNotMatchMember";
        internal const string ArgumentTypesMustMatch = "ArgumentTypesMustMatch";
        internal const string BinaryOperatorNotDefined = "BinaryOperatorNotDefined";
        internal const string CannotAutoInitializeValueTypeElementThroughProperty = "CannotAutoInitializeValueTypeElementThroughProperty";
        internal const string CannotAutoInitializeValueTypeMemberThroughProperty = "CannotAutoInitializeValueTypeMemberThroughProperty";
        internal const string CannotCastTypeToType = "CannotCastTypeToType";
        internal const string CoalesceUsedOnNonNullType = "CoalesceUsedOnNonNullType";
        internal const string CoercionOperatorNotDefined = "CoercionOperatorNotDefined";
        internal const string ElementInitializerMethodNoRefOutParam = "ElementInitializerMethodNoRefOutParam";
        internal const string ElementInitializerMethodNotAdd = "ElementInitializerMethodNotAdd";
        internal const string ElementInitializerMethodStatic = "ElementInitializerMethodStatic";
        internal const string ElementInitializerMethodWithZeroArgs = "ElementInitializerMethodWithZeroArgs";
        internal const string ExpressionMayNotContainByrefParameters = "ExpressionMayNotContainByrefParameters";
        internal const string ExpressionTypeCannotInitializeArrayType = "ExpressionTypeCannotInitializeArrayType";
        internal const string ExpressionTypeCannotInitializeCollectionType = "ExpressionTypeCannotInitializeCollectionType";
        internal const string ExpressionTypeDoesNotMatchArrayType = "ExpressionTypeDoesNotMatchArrayType";
        internal const string ExpressionTypeDoesNotMatchConstructorParameter = "ExpressionTypeDoesNotMatchConstructorParameter";
        internal const string ExpressionTypeDoesNotMatchMethodParameter = "ExpressionTypeDoesNotMatchMethodParameter";
        internal const string ExpressionTypeDoesNotMatchParameter = "ExpressionTypeDoesNotMatchParameter";
        internal const string ExpressionTypeDoesNotMatchReturn = "ExpressionTypeDoesNotMatchReturn";
        internal const string ExpressionTypeNotInvocable = "ExpressionTypeNotInvocable";
        internal const string FieldNotDefinedForType = "FieldNotDefinedForType";
        internal const string IncorrectNumberOfArgumentsForMembers = "IncorrectNumberOfArgumentsForMembers";
        internal const string IncorrectNumberOfConstructorArguments = "IncorrectNumberOfConstructorArguments";
        internal const string IncorrectNumberOfIndexes = "IncorrectNumberOfIndexes";
        internal const string IncorrectNumberOfLambdaArguments = "IncorrectNumberOfLambdaArguments";
        internal const string IncorrectNumberOfLambdaDeclarationParameters = "IncorrectNumberOfLambdaDeclarationParameters";
        internal const string IncorrectNumberOfMembersForGivenConstructor = "IncorrectNumberOfMembersForGivenConstructor";
        internal const string IncorrectNumberOfMethodCallArguments = "IncorrectNumberOfMethodCallArguments";
        internal const string IncorrectNumberOfTypeArgsForAction = "IncorrectNumberOfTypeArgsForAction";
        internal const string IncorrectNumberOfTypeArgsForFunc = "IncorrectNumberOfTypeArgsForFunc";
        internal const string IncorrectTypeForTypeAs = "IncorrectTypeForTypeAs";
        internal const string InvalidCast = "InvalidCast";
        internal const string LambdaParameterNotInScope = "LambdaParameterNotInScope";
        internal const string LambdaTypeMustBeDerivedFromSystemDelegate = "LambdaTypeMustBeDerivedFromSystemDelegate";
        internal const string ListInitializerWithZeroMembers = "ListInitializerWithZeroMembers";
        private static System.Linq.Expressions.SR loader;
        internal const string LogicalOperatorMustHaveBooleanOperators = "LogicalOperatorMustHaveBooleanOperators";
        internal const string LogicalOperatorMustHaveConsistentTypes = "LogicalOperatorMustHaveConsistentTypes";
        internal const string MemberNotFieldOrProperty = "MemberNotFieldOrProperty";
        internal const string MethodContainsGenericParameters = "MethodContainsGenericParameters";
        internal const string MethodDoesNotExistOnType = "MethodDoesNotExistOnType";
        internal const string MethodIsGeneric = "MethodIsGeneric";
        internal const string MethodNotDefinedForType = "MethodNotDefinedForType";
        internal const string MethodNotPropertyAccessor = "MethodNotPropertyAccessor";
        internal const string MethodWithArgsDoesNotExistOnType = "MethodWithArgsDoesNotExistOnType";
        internal const string MethodWithMoreThanOneMatch = "MethodWithMoreThanOneMatch";
        internal const string NotAMemberOfType = "NotAMemberOfType";
        internal const string OperandTypesDoNotMatchParameters = "OperandTypesDoNotMatchParameters";
        internal const string OperatorNotImplementedForType = "OperatorNotImplementedForType";
        internal const string OwningTeam = "OwningTeam";
        internal const string ParameterExpressionNotValidAsDelegate = "ParameterExpressionNotValidAsDelegate";
        internal const string ParameterNotCaptured = "ParameterNotCaptured";
        internal const string PropertyDoesNotHaveGetter = "PropertyDoesNotHaveGetter";
        internal const string PropertyDoesNotHaveSetter = "PropertyDoesNotHaveSetter";
        internal const string PropertyNotDefinedForType = "PropertyNotDefinedForType";
        private ResourceManager resources;
        internal const string TypeContainsGenericParameters = "TypeContainsGenericParameters";
        internal const string TypeIsGeneric = "TypeIsGeneric";
        internal const string TypeMissingDefaultConstructor = "TypeMissingDefaultConstructor";
        internal const string TypeNotIEnumerable = "TypeNotIEnumerable";
        internal const string TypeParameterIsNotDelegate = "TypeParameterIsNotDelegate";
        internal const string UnaryOperatorNotDefined = "UnaryOperatorNotDefined";
        internal const string UnexpectedCoalesceOperator = "UnexpectedCoalesceOperator";
        internal const string UnhandledBinary = "UnhandledBinary";
        internal const string UnhandledBinding = "UnhandledBinding";
        internal const string UnhandledBindingType = "UnhandledBindingType";
        internal const string UnhandledCall = "UnhandledCall";
        internal const string UnhandledConvert = "UnhandledConvert";
        internal const string UnhandledConvertFromDecimal = "UnhandledConvertFromDecimal";
        internal const string UnhandledConvertToDecimal = "UnhandledConvertToDecimal";
        internal const string UnhandledExpressionType = "UnhandledExpressionType";
        internal const string UnhandledMemberAccess = "UnhandledMemberAccess";
        internal const string UnhandledUnary = "UnhandledUnary";
        internal const string UnknownBindingType = "UnknownBindingType";
        internal const string UserDefinedOperatorMustBeStatic = "UserDefinedOperatorMustBeStatic";
        internal const string UserDefinedOperatorMustNotBeVoid = "UserDefinedOperatorMustNotBeVoid";

        internal SR()
        {
            this.resources = new ResourceManager("System.Linq.Expressions", base.GetType().Assembly);
        }

        private static System.Linq.Expressions.SR GetLoader()
        {
            if (loader == null)
            {
                System.Linq.Expressions.SR sr = new System.Linq.Expressions.SR();
                Interlocked.CompareExchange<System.Linq.Expressions.SR>(ref loader, sr, null);
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            System.Linq.Expressions.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            System.Linq.Expressions.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            System.Linq.Expressions.SR loader = GetLoader();
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

        public static ResourceManager Resources =>
            GetLoader().resources;
    }
}

