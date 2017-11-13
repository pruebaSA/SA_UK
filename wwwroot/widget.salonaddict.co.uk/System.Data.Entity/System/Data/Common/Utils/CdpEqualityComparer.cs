namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;

    internal sealed class CdpEqualityComparer : IEqualityComparer
    {
        private static readonly IEqualityComparer s_defaultEqualityComparer = new CdpEqualityComparer();

        private CdpEqualityComparer()
        {
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null))
            {
                return false;
            }
            string str = x as string;
            if (str != null)
            {
                string str2 = y as string;
                if (str2 != null)
                {
                    return StringComparer.Ordinal.Equals(str, str2);
                }
                return (0 == str.CompareTo(y));
            }
            IComparable comparable = x as IComparable;
            if (comparable != null)
            {
                return (0 == comparable.CompareTo(y));
            }
            return x.Equals(y);
        }

        int IEqualityComparer.GetHashCode(object obj) => 
            obj.GetHashCode();

        internal static IEqualityComparer DefaultEqualityComparer =>
            s_defaultEqualityComparer;
    }
}

