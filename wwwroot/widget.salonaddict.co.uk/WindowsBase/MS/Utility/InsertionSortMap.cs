namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    internal struct InsertionSortMap
    {
        internal LargeSortedObjectMap _mapStore;
        public object this[int key]
        {
            get
            {
                if (this._mapStore != null)
                {
                    return this._mapStore.Search(key);
                }
                return DependencyProperty.UnsetValue;
            }
            set
            {
                if (value != DependencyProperty.UnsetValue)
                {
                    if (this._mapStore == null)
                    {
                        this._mapStore = new LargeSortedObjectMap();
                    }
                    FrugalMapStoreState state = this._mapStore.InsertEntry(key, value);
                    if (state != FrugalMapStoreState.Success)
                    {
                        if (FrugalMapStoreState.SortedArray != state)
                        {
                            throw new InvalidOperationException(System.Windows.SR.Get("FrugalMap_CannotPromoteBeyondHashtable"));
                        }
                        LargeSortedObjectMap newMap = new LargeSortedObjectMap();
                        this._mapStore.Promote(newMap);
                        this._mapStore = newMap;
                        this._mapStore.InsertEntry(key, value);
                    }
                }
                else if (this._mapStore != null)
                {
                    this._mapStore.RemoveEntry(key);
                    if (this._mapStore.Count == 0)
                    {
                        this._mapStore = null;
                    }
                }
            }
        }
        public void Sort()
        {
            if (this._mapStore != null)
            {
                this._mapStore.Sort();
            }
        }

        public void GetKeyValuePair(int index, out int key, out object value)
        {
            if (this._mapStore == null)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this._mapStore.GetKeyValuePair(index, out key, out value);
        }

        public void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (this._mapStore != null)
            {
                this._mapStore.Iterate(list, callback);
            }
        }

        public int Count
        {
            get
            {
                if (this._mapStore != null)
                {
                    return this._mapStore.Count;
                }
                return 0;
            }
        }
    }
}

