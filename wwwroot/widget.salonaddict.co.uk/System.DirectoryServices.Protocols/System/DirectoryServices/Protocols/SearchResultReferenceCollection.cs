namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class SearchResultReferenceCollection : ReadOnlyCollectionBase
    {
        internal SearchResultReferenceCollection()
        {
        }

        internal int Add(SearchResultReference reference) => 
            base.InnerList.Add(reference);

        internal void Clear()
        {
            base.InnerList.Clear();
        }

        public bool Contains(SearchResultReference value) => 
            base.InnerList.Contains(value);

        public void CopyTo(SearchResultReference[] values, int index)
        {
            base.InnerList.CopyTo(values, index);
        }

        public int IndexOf(SearchResultReference value) => 
            base.InnerList.IndexOf(value);

        public SearchResultReference this[int index] =>
            ((SearchResultReference) base.InnerList[index]);
    }
}

