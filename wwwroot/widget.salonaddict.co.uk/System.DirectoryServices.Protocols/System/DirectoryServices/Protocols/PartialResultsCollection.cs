namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class PartialResultsCollection : ReadOnlyCollectionBase
    {
        internal PartialResultsCollection()
        {
        }

        internal int Add(object value) => 
            base.InnerList.Add(value);

        public bool Contains(object value) => 
            base.InnerList.Contains(value);

        public void CopyTo(object[] values, int index)
        {
            base.InnerList.CopyTo(values, index);
        }

        public int IndexOf(object value) => 
            base.InnerList.IndexOf(value);

        public object this[int index] =>
            base.InnerList[index];
    }
}

