namespace System.Diagnostics
{
    using System;
    using System.Collections;

    internal class OrdinalCaseInsensitiveComparer : IComparer
    {
        internal static readonly OrdinalCaseInsensitiveComparer Default = new OrdinalCaseInsensitiveComparer();

        public int Compare(object a, object b)
        {
            string str = a as string;
            string str2 = b as string;
            if ((str != null) && (str2 != null))
            {
                return string.CompareOrdinal(str.ToUpperInvariant(), str2.ToUpperInvariant());
            }
            return Comparer.Default.Compare(a, b);
        }
    }
}

