namespace System.Data.Linq
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class SubqueryRules
    {
        private static bool IsSequenceOperatorCall(MethodInfo mi)
        {
            Type declaringType = mi.DeclaringType;
            if ((declaringType != typeof(Enumerable)) && (declaringType != typeof(Queryable)))
            {
                return false;
            }
            return true;
        }

        internal static bool IsSupportedTopLevelMethod(MethodInfo mi)
        {
            string str;
            if (!IsSequenceOperatorCall(mi))
            {
                return false;
            }
            if (((str = mi.Name) == null) || ((((str != "Where") && (str != "OrderBy")) && ((str != "OrderByDescending") && (str != "ThenBy"))) && ((str != "ThenByDescending") && (str != "Take"))))
            {
                return false;
            }
            return true;
        }
    }
}

