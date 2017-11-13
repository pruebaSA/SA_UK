namespace System.Collections.Specialized
{
    using System;
    using System.Collections;

    public class CollectionsUtil
    {
        public static Hashtable CreateCaseInsensitiveHashtable() => 
            new Hashtable(StringComparer.CurrentCultureIgnoreCase);

        public static Hashtable CreateCaseInsensitiveHashtable(IDictionary d) => 
            new Hashtable(d, StringComparer.CurrentCultureIgnoreCase);

        public static Hashtable CreateCaseInsensitiveHashtable(int capacity) => 
            new Hashtable(capacity, StringComparer.CurrentCultureIgnoreCase);

        public static SortedList CreateCaseInsensitiveSortedList() => 
            new SortedList(CaseInsensitiveComparer.Default);
    }
}

