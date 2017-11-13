namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class SearchResultEntryCollection : ReadOnlyCollectionBase
    {
        internal SearchResultEntryCollection()
        {
        }

        internal int Add(SearchResultEntry entry) => 
            base.InnerList.Add(entry);

        internal void Clear()
        {
            base.InnerList.Clear();
        }

        public bool Contains(SearchResultEntry value) => 
            base.InnerList.Contains(value);

        public void CopyTo(SearchResultEntry[] values, int index)
        {
            base.InnerList.CopyTo(values, index);
        }

        public int IndexOf(SearchResultEntry value) => 
            base.InnerList.IndexOf(value);

        public SearchResultEntry this[int index] =>
            ((SearchResultEntry) base.InnerList[index]);
    }
}

