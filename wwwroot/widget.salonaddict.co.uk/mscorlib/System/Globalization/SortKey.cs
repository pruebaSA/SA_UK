namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public class SortKey
    {
        internal byte[] m_KeyData;
        internal string m_String;
        internal CompareOptions options;
        private const CompareOptions ValidSortkeyCtorMaskOffFlags = ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
        internal int win32LCID;

        internal SortKey(int win32LCID, string str, CompareOptions options)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if ((options & ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (CultureInfo.GetNativeSortKey(win32LCID, CompareInfo.GetNativeCompareFlags(options), str, str.Length, out this.m_KeyData) < 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "str");
            }
            this.win32LCID = win32LCID;
            this.options = options;
            this.m_String = str;
        }

        internal unsafe SortKey(void* pSortingFile, int win32LCID, string str, CompareOptions options)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if ((options & ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            this.win32LCID = win32LCID;
            this.options = options;
            this.m_String = str;
            this.m_KeyData = CompareInfo.nativeCreateSortKey(pSortingFile, str, (int) options, win32LCID);
        }

        public static int Compare(SortKey sortkey1, SortKey sortkey2)
        {
            if ((sortkey1 == null) || (sortkey2 == null))
            {
                throw new ArgumentNullException((sortkey1 == null) ? "sortkey1" : "sortkey2");
            }
            byte[] keyData = sortkey1.m_KeyData;
            byte[] buffer2 = sortkey2.m_KeyData;
            if (keyData.Length == 0)
            {
                if (buffer2.Length == 0)
                {
                    return 0;
                }
                return -1;
            }
            if (buffer2.Length == 0)
            {
                return 1;
            }
            int num = (keyData.Length < buffer2.Length) ? keyData.Length : buffer2.Length;
            for (int i = 0; i < num; i++)
            {
                if (keyData[i] > buffer2[i])
                {
                    return 1;
                }
                if (keyData[i] < buffer2[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public override bool Equals(object value)
        {
            SortKey key = value as SortKey;
            return ((key != null) && (Compare(this, key) == 0));
        }

        public override int GetHashCode() => 
            CompareInfo.GetCompareInfo(this.win32LCID).GetHashCodeOfString(this.m_String, this.options);

        public override string ToString() => 
            string.Concat(new object[] { "SortKey - ", this.win32LCID, ", ", this.options, ", ", this.m_String });

        public virtual byte[] KeyData =>
            ((byte[]) this.m_KeyData.Clone());

        public virtual string OriginalString =>
            this.m_String;
    }
}

