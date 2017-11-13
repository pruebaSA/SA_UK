namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class LinqDataSourceHelper
    {
        public static bool EnumerableContentEquals(IEnumerable enumerableA, IEnumerable enumerableB)
        {
            IEnumerator enumerator = enumerableA.GetEnumerator();
            IEnumerator enumerator2 = enumerableB.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator2.MoveNext())
                {
                    return false;
                }
                object current = enumerator.Current;
                object obj3 = enumerator2.Current;
                if (current == null)
                {
                    if (obj3 != null)
                    {
                        return false;
                    }
                }
                else if (!current.Equals(obj3))
                {
                    return false;
                }
            }
            if (enumerator2.MoveNext())
            {
                return false;
            }
            return true;
        }

        public static Type FindGenericEnumerableType(Type type)
        {
            while (((type != null) && (type != typeof(object))) && (type != typeof(string)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    return type;
                }
                foreach (Type type2 in type.GetInterfaces())
                {
                    Type type3 = FindGenericEnumerableType(type2);
                    if (type3 != null)
                    {
                        return type3;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}

