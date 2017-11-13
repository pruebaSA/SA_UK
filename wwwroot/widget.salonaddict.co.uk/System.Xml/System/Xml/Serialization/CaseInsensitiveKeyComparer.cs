namespace System.Xml.Serialization
{
    using System;
    using System.Collections;
    using System.Globalization;

    internal class CaseInsensitiveKeyComparer : CaseInsensitiveComparer, IEqualityComparer
    {
        public CaseInsensitiveKeyComparer() : base(CultureInfo.CurrentCulture)
        {
        }

        bool IEqualityComparer.Equals(object x, object y) => 
            (base.Compare(x, y) == 0);

        int IEqualityComparer.GetHashCode(object obj)
        {
            string str = obj as string;
            return str?.ToUpper(CultureInfo.CurrentCulture).GetHashCode();
        }
    }
}

