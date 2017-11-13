namespace System.Linq
{
    using System;

    internal static class Strings
    {
        internal static string ArgumentArrayHasTooManyElements(object p0) => 
            System.Linq.SR.GetString("ArgumentArrayHasTooManyElements", new object[] { p0 });

        internal static string ArgumentNotIEnumerableGeneric(object p0) => 
            System.Linq.SR.GetString("ArgumentNotIEnumerableGeneric", new object[] { p0 });

        internal static string ArgumentNotLambda(object p0) => 
            System.Linq.SR.GetString("ArgumentNotLambda", new object[] { p0 });

        internal static string ArgumentNotSequence(object p0) => 
            System.Linq.SR.GetString("ArgumentNotSequence", new object[] { p0 });

        internal static string ArgumentNotValid(object p0) => 
            System.Linq.SR.GetString("ArgumentNotValid", new object[] { p0 });

        internal static string NoArgumentMatchingMethodsInQueryable(object p0) => 
            System.Linq.SR.GetString("NoArgumentMatchingMethodsInQueryable", new object[] { p0 });

        internal static string NoMethodOnType(object p0, object p1) => 
            System.Linq.SR.GetString("NoMethodOnType", new object[] { p0, p1 });

        internal static string NoMethodOnTypeMatchingArguments(object p0, object p1) => 
            System.Linq.SR.GetString("NoMethodOnTypeMatchingArguments", new object[] { p0, p1 });

        internal static string NoNameMatchingMethodsInQueryable(object p0) => 
            System.Linq.SR.GetString("NoNameMatchingMethodsInQueryable", new object[] { p0 });

        internal static string EmptyEnumerable =>
            System.Linq.SR.GetString("EmptyEnumerable");

        internal static string IncompatibleElementTypes =>
            System.Linq.SR.GetString("IncompatibleElementTypes");

        internal static string MoreThanOneElement =>
            System.Linq.SR.GetString("MoreThanOneElement");

        internal static string MoreThanOneMatch =>
            System.Linq.SR.GetString("MoreThanOneMatch");

        internal static string NoElements =>
            System.Linq.SR.GetString("NoElements");

        internal static string NoMatch =>
            System.Linq.SR.GetString("NoMatch");

        internal static string OwningTeam =>
            System.Linq.SR.GetString("OwningTeam");
    }
}

