namespace System.Linq
{
    using System;

    internal static class Error
    {
        internal static Exception ArgumentArrayHasTooManyElements(object p0) => 
            new ArgumentException(Strings.ArgumentArrayHasTooManyElements(p0));

        internal static Exception ArgumentNotIEnumerableGeneric(object p0) => 
            new ArgumentException(Strings.ArgumentNotIEnumerableGeneric(p0));

        internal static Exception ArgumentNotLambda(object p0) => 
            new ArgumentException(Strings.ArgumentNotLambda(p0));

        internal static Exception ArgumentNotSequence(object p0) => 
            new ArgumentException(Strings.ArgumentNotSequence(p0));

        internal static Exception ArgumentNotValid(object p0) => 
            new ArgumentException(Strings.ArgumentNotValid(p0));

        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static Exception IncompatibleElementTypes() => 
            new ArgumentException(Strings.IncompatibleElementTypes);

        internal static Exception MoreThanOneElement() => 
            new InvalidOperationException(Strings.MoreThanOneElement);

        internal static Exception MoreThanOneMatch() => 
            new InvalidOperationException(Strings.MoreThanOneMatch);

        internal static Exception NoArgumentMatchingMethodsInQueryable(object p0) => 
            new InvalidOperationException(Strings.NoArgumentMatchingMethodsInQueryable(p0));

        internal static Exception NoElements() => 
            new InvalidOperationException(Strings.NoElements);

        internal static Exception NoMatch() => 
            new InvalidOperationException(Strings.NoMatch);

        internal static Exception NoMethodOnType(object p0, object p1) => 
            new InvalidOperationException(Strings.NoMethodOnType(p0, p1));

        internal static Exception NoMethodOnTypeMatchingArguments(object p0, object p1) => 
            new InvalidOperationException(Strings.NoMethodOnTypeMatchingArguments(p0, p1));

        internal static Exception NoNameMatchingMethodsInQueryable(object p0) => 
            new InvalidOperationException(Strings.NoNameMatchingMethodsInQueryable(p0));

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();
    }
}

