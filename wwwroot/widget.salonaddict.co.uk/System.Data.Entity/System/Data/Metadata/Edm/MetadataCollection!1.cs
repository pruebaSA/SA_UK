namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class MetadataCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T: MetadataItem
    {
        private CollectionData<T> _collectionData;
        private bool _readOnly;
        private const int UseSortedListCrossover = 0x19;

        internal MetadataCollection() : this(null)
        {
        }

        internal MetadataCollection(IEnumerable<T> items)
        {
            this._collectionData = new CollectionData<T>();
            if (items != null)
            {
                foreach (T local in items)
                {
                    if (local == null)
                    {
                        throw EntityUtil.CollectionParameterElementIsNull("items");
                    }
                    this.AddInternal(local);
                }
            }
        }

        public virtual void Add(T item)
        {
            this.AddInternal(item);
        }

        private void AddInternal(T item)
        {
            Util.CheckItemHasIdentity(item, "item");
            this.ThrowIfReadOnly();
            MetadataCollection<T>.AddInternalHelper(item, this._collectionData, false);
        }

        private static void AddInternalHelper(T item, CollectionData<T> collectionData, bool updateIfFound)
        {
            int num;
            Util.CheckItemHasIdentity(item, "item");
            int count = collectionData.OrderedList.Count;
            if (collectionData.IdentityDictionary != null)
            {
                num = MetadataCollection<T>.AddToDictionary(collectionData, item.Identity, count, updateIfFound);
            }
            else
            {
                num = MetadataCollection<T>.IndexOf(collectionData, item.Identity, false);
                if (0 <= num)
                {
                    if (!updateIfFound)
                    {
                        throw EntityUtil.ItemDuplicateIdentity(item.Identity, "item", null);
                    }
                }
                else if (0x19 <= count)
                {
                    collectionData.IdentityDictionary = new Dictionary<string, OrderedIndex<T>>(collectionData.OrderedList.Count + 1, StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < collectionData.OrderedList.Count; i++)
                    {
                        T local = collectionData.OrderedList[i];
                        MetadataCollection<T>.AddToDictionary(collectionData, local.Identity, i, false);
                    }
                    MetadataCollection<T>.AddToDictionary(collectionData, item.Identity, count, false);
                }
            }
            if ((0 <= num) && (num < count))
            {
                collectionData.OrderedList[num] = item;
            }
            else
            {
                collectionData.OrderedList.Add(item);
            }
        }

        private static int AddToDictionary(CollectionData<T> collectionData, string identity, int index, bool updateIfFound)
        {
            int[] array = null;
            OrderedIndex<T> index2;
            int exactIndex = index;
            if (collectionData.IdentityDictionary.TryGetValue(identity, out index2))
            {
                if (MetadataCollection<T>.EqualIdentity(collectionData.OrderedList, index2.ExactIndex, identity))
                {
                    if (!updateIfFound)
                    {
                        throw EntityUtil.ItemDuplicateIdentity(identity, "item", null);
                    }
                    return index2.ExactIndex;
                }
                if (index2.InexactIndexes != null)
                {
                    for (int i = 0; i < index2.InexactIndexes.Length; i++)
                    {
                        if (MetadataCollection<T>.EqualIdentity(collectionData.OrderedList, index2.InexactIndexes[i], identity))
                        {
                            if (!updateIfFound)
                            {
                                throw EntityUtil.ItemDuplicateIdentity(identity, "item", null);
                            }
                            return index2.InexactIndexes[i];
                        }
                    }
                    array = new int[index2.InexactIndexes.Length + 1];
                    index2.InexactIndexes.CopyTo(array, 0);
                    array[array.Length - 1] = index;
                }
                else
                {
                    array = new int[] { index };
                }
                exactIndex = index2.ExactIndex;
            }
            collectionData.IdentityDictionary[identity] = new OrderedIndex<T>(exactIndex, array);
            return index;
        }

        public virtual ReadOnlyMetadataCollection<T> AsReadOnlyMetadataCollection() => 
            new ReadOnlyMetadataCollection<T>(this);

        internal bool AtomicAddRange(List<T> items)
        {
            CollectionData<T> original = this._collectionData;
            CollectionData<T> collectionData = new CollectionData<T>(original, items.Count);
            foreach (T local in items)
            {
                MetadataCollection<T>.AddInternalHelper(local, collectionData, false);
            }
            if (Interlocked.CompareExchange<CollectionData<T>>(ref this._collectionData, collectionData, original) != original)
            {
                return false;
            }
            return true;
        }

        public bool Contains(T item)
        {
            Util.CheckItemHasIdentity(item, "item");
            return (-1 != this.IndexOf(item));
        }

        public virtual bool ContainsIdentity(string identity)
        {
            EntityUtil.CheckStringArgument(identity, "identity");
            return (0 <= MetadataCollection<T>.IndexOf(this._collectionData, identity, false));
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            EntityUtil.GenericCheckArgumentNull<T[]>(array, "array");
            if (arrayIndex < 0)
            {
                throw EntityUtil.ArgumentOutOfRange("arrayIndex");
            }
            if (this._collectionData.OrderedList.Count > (array.Length - arrayIndex))
            {
                throw EntityUtil.ArrayTooSmall("arrayIndex");
            }
            this._collectionData.OrderedList.CopyTo(array, arrayIndex);
        }

        private static bool EqualIdentity(List<T> orderedList, int index, string identity)
        {
            T local = orderedList[index];
            return (local.Identity == identity);
        }

        public ReadOnlyMetadataCollection<T>.Enumerator GetEnumerator() => 
            new ReadOnlyMetadataCollection<T>.Enumerator(this);

        public T GetValue(string identity, bool ignoreCase)
        {
            T local = this.InternalTryGetValue(identity, ignoreCase);
            if (local == null)
            {
                throw EntityUtil.ItemInvalidIdentity(identity, "identity");
            }
            return local;
        }

        public virtual int IndexOf(T item)
        {
            Util.CheckItemHasIdentity(item, "item");
            int num = MetadataCollection<T>.IndexOf(this._collectionData, item.Identity, false);
            if ((num != -1) && (this._collectionData.OrderedList[num] == item))
            {
                return num;
            }
            return -1;
        }

        private static int IndexOf(CollectionData<T> collectionData, string identity, bool ignoreCase)
        {
            int exactIndex = -1;
            if (collectionData.IdentityDictionary != null)
            {
                OrderedIndex<T> index;
                if (collectionData.IdentityDictionary.TryGetValue(identity, out index))
                {
                    if (ignoreCase)
                    {
                        exactIndex = index.ExactIndex;
                    }
                    else if (MetadataCollection<T>.EqualIdentity(collectionData.OrderedList, index.ExactIndex, identity))
                    {
                        return index.ExactIndex;
                    }
                    if (index.InexactIndexes != null)
                    {
                        if (ignoreCase)
                        {
                            throw EntityUtil.MoreThanOneItemMatchesIdentity(identity);
                        }
                        for (int j = 0; j < index.InexactIndexes.Length; j++)
                        {
                            if (MetadataCollection<T>.EqualIdentity(collectionData.OrderedList, index.InexactIndexes[j], identity))
                            {
                                return index.InexactIndexes[j];
                            }
                        }
                    }
                }
                return exactIndex;
            }
            if (ignoreCase)
            {
                for (int k = 0; k < collectionData.OrderedList.Count; k++)
                {
                    T local = collectionData.OrderedList[k];
                    if (string.Equals(local.Identity, identity, StringComparison.OrdinalIgnoreCase))
                    {
                        if (0 <= exactIndex)
                        {
                            throw EntityUtil.MoreThanOneItemMatchesIdentity(identity);
                        }
                        exactIndex = k;
                    }
                }
                return exactIndex;
            }
            for (int i = 0; i < collectionData.OrderedList.Count; i++)
            {
                if (MetadataCollection<T>.EqualIdentity(collectionData.OrderedList, i, identity))
                {
                    return i;
                }
            }
            return exactIndex;
        }

        private T InternalTryGetValue(string identity, bool ignoreCase)
        {
            int num = MetadataCollection<T>.IndexOf(this._collectionData, EntityUtil.GenericCheckArgumentNull<string>(identity, "identity"), ignoreCase);
            if (0 > num)
            {
                return default(T);
            }
            return this._collectionData.OrderedList[num];
        }

        public MetadataCollection<T> SetReadOnly()
        {
            for (int i = 0; i < this._collectionData.OrderedList.Count; i++)
            {
                this._collectionData.OrderedList[i].SetReadOnly();
            }
            this._collectionData.OrderedList.TrimExcess();
            this._readOnly = true;
            return (MetadataCollection<T>) this;
        }

        void ICollection<T>.Clear()
        {
            throw EntityUtil.OperationOnReadOnlyCollection();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw EntityUtil.OperationOnReadOnlyCollection();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            this.GetEnumerator();

        void IList<T>.Insert(int index, T item)
        {
            throw EntityUtil.OperationOnReadOnlyCollection();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw EntityUtil.OperationOnReadOnlyCollection();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        internal void ThrowIfReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }

        public virtual bool TryGetValue(string identity, bool ignoreCase, out T item)
        {
            item = this.InternalTryGetValue(identity, ignoreCase);
            return (null != ((T) item));
        }

        public virtual ReadOnlyCollection<T> AsReadOnly =>
            this._collectionData.OrderedList.AsReadOnly();

        public virtual int Count =>
            this._collectionData.OrderedList.Count;

        public bool IsReadOnly =>
            this._readOnly;

        public virtual T this[int index]
        {
            get => 
                this._collectionData.OrderedList[index];
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }

        public virtual T this[string identity]
        {
            get => 
                this.GetValue(identity, false);
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }

        private class CollectionData
        {
            internal Dictionary<string, MetadataCollection<T>.OrderedIndex> IdentityDictionary;
            internal List<T> OrderedList;

            internal CollectionData()
            {
                this.OrderedList = new List<T>();
            }

            internal CollectionData(MetadataCollection<T>.CollectionData original, int additionalCapacity)
            {
                this.OrderedList = new List<T>(original.OrderedList.Count + additionalCapacity);
                foreach (T local in original.OrderedList)
                {
                    this.OrderedList.Add(local);
                }
                if (0x19 <= this.OrderedList.Capacity)
                {
                    this.IdentityDictionary = new Dictionary<string, MetadataCollection<T>.OrderedIndex>(this.OrderedList.Capacity, StringComparer.OrdinalIgnoreCase);
                    if (original.IdentityDictionary != null)
                    {
                        foreach (KeyValuePair<string, MetadataCollection<T>.OrderedIndex> pair in original.IdentityDictionary)
                        {
                            this.IdentityDictionary.Add(pair.Key, pair.Value);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.OrderedList.Count; i++)
                        {
                            T local2 = this.OrderedList[i];
                            MetadataCollection<T>.AddToDictionary((MetadataCollection<T>.CollectionData) this, local2.Identity, i, false);
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct OrderedIndex
        {
            internal readonly int ExactIndex;
            internal readonly int[] InexactIndexes;
            internal OrderedIndex(int exactIndex, int[] inexactIndexes)
            {
                this.ExactIndex = exactIndex;
                this.InexactIndexes = inexactIndexes;
            }
        }
    }
}

