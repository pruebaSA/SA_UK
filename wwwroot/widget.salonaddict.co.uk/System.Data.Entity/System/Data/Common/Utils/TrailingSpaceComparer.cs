namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;

    internal class TrailingSpaceComparer : IEqualityComparer<object>
    {
        internal static readonly TrailingSpaceComparer Instance = new TrailingSpaceComparer();
        private static readonly IEqualityComparer<object> s_template = EqualityComparer<object>.Default;

        private TrailingSpaceComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            string str = x as string;
            if (str != null)
            {
                string str2 = y as string;
                if (str2 != null)
                {
                    return TrailingSpaceStringComparer.Instance.Equals(str, str2);
                }
            }
            return s_template.Equals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            string str = obj as string;
            if (str != null)
            {
                return TrailingSpaceStringComparer.Instance.GetHashCode(str);
            }
            return s_template.GetHashCode(obj);
        }
    }
}

