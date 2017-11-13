namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections;

    internal class CaseInsensitiveOrdinalStringComparer : IEqualityComparer, IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Invariant.Assert((x is string) && (y is string));
            return string.CompareOrdinal(((string) x).ToUpperInvariant(), ((string) y).ToUpperInvariant());
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            Invariant.Assert((x is string) && (y is string));
            return (string.CompareOrdinal(((string) x).ToUpperInvariant(), ((string) y).ToUpperInvariant()) == 0);
        }

        int IEqualityComparer.GetHashCode(object str)
        {
            Invariant.Assert(str is string);
            return ((string) str).ToUpperInvariant().GetHashCode();
        }
    }
}

