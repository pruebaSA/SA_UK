namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;

    internal class TrailingSpaceStringComparer : IEqualityComparer<string>
    {
        internal static readonly TrailingSpaceStringComparer Instance = new TrailingSpaceStringComparer();

        private TrailingSpaceStringComparer()
        {
        }

        public bool Equals(string x, string y) => 
            StringComparer.OrdinalIgnoreCase.Equals(NormalizeString(x), NormalizeString(y));

        public int GetHashCode(string obj) => 
            StringComparer.OrdinalIgnoreCase.GetHashCode(NormalizeString(obj));

        internal static string NormalizeString(string value)
        {
            if ((value == null) || !value.EndsWith(" ", StringComparison.Ordinal))
            {
                return value;
            }
            return value.TrimEnd(new char[] { ' ' });
        }
    }
}

