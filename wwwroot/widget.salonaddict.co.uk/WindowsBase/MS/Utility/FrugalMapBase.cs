namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    internal abstract class FrugalMapBase
    {
        protected const int INVALIDKEY = 0x7fffffff;

        protected FrugalMapBase()
        {
        }

        public abstract void GetKeyValuePair(int index, out int key, out object value);
        public abstract FrugalMapStoreState InsertEntry(int key, object value);
        public abstract void Iterate(ArrayList list, FrugalMapIterationCallback callback);
        public abstract void Promote(FrugalMapBase newMap);
        public abstract void RemoveEntry(int key);
        public abstract object Search(int key);
        public abstract void Sort();

        public abstract int Count { get; }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Entry
        {
            public int Key;
            public object Value;
        }
    }
}

