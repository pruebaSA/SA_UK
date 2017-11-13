namespace System.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    [Serializable, DebuggerDisplay("Count = {Count}"), ComVisible(false), DebuggerTypeProxy(typeof(System_DictionaryDebugView<,>))]
    public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private const int _defaultCapacity = 4;
        private int _size;
        [NonSerialized]
        private object _syncRoot;
        private IComparer<TKey> comparer;
        private static TKey[] emptyKeys;
        private static TValue[] emptyValues;
        private KeyList<TKey, TValue> keyList;
        private TKey[] keys;
        private ValueList<TKey, TValue> valueList;
        private TValue[] values;
        private int version;

        static SortedList()
        {
            SortedList<TKey, TValue>.emptyKeys = new TKey[0];
            SortedList<TKey, TValue>.emptyValues = new TValue[0];
        }

        public SortedList()
        {
            this.keys = SortedList<TKey, TValue>.emptyKeys;
            this.values = SortedList<TKey, TValue>.emptyValues;
            this._size = 0;
            this.comparer = Comparer<TKey>.Default;
        }

        public SortedList(IComparer<TKey> comparer) : this()
        {
            if (comparer != null)
            {
                this.comparer = comparer;
            }
        }

        public SortedList(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }

        public SortedList(int capacity)
        {
            if (capacity < 0)
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.capacity, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired);
            }
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            this.comparer = Comparer<TKey>.Default;
        }

        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.dictionary);
            }
            dictionary.Keys.CopyTo(this.keys, 0);
            dictionary.Values.CopyTo(this.values, 0);
            Array.Sort<TKey, TValue>(this.keys, this.values, comparer);
            this._size = dictionary.Count;
        }

        public SortedList(int capacity, IComparer<TKey> comparer) : this(comparer)
        {
            this.Capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num >= 0)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_AddingDuplicate);
            }
            this.Insert(~num, key, value);
        }

        public void Clear()
        {
            this.version++;
            Array.Clear(this.keys, 0, this._size);
            Array.Clear(this.values, 0, this._size);
            this._size = 0;
        }

        public bool ContainsKey(TKey key) => 
            (this.IndexOfKey(key) >= 0);

        public bool ContainsValue(TValue value) => 
            (this.IndexOfValue(value) >= 0);

        private void EnsureCapacity(int min)
        {
            int num = (this.keys.Length == 0) ? 4 : (this.keys.Length * 2);
            if (num < min)
            {
                num = min;
            }
            this.Capacity = num;
        }

        private TValue GetByIndex(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index, System.ExceptionResource.ArgumentOutOfRange_Index);
            }
            return this.values[index];
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedList<TKey, TValue>) this, 1);

        private TKey GetKey(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index, System.ExceptionResource.ArgumentOutOfRange_Index);
            }
            return this.keys[index];
        }

        private KeyList<TKey, TValue> GetKeyListHelper()
        {
            if (this.keyList == null)
            {
                this.keyList = new KeyList<TKey, TValue>((SortedList<TKey, TValue>) this);
            }
            return this.keyList;
        }

        private ValueList<TKey, TValue> GetValueListHelper()
        {
            if (this.valueList == null)
            {
                this.valueList = new ValueList<TKey, TValue>((SortedList<TKey, TValue>) this);
            }
            return this.valueList;
        }

        public int IndexOfKey(TKey key)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num < 0)
            {
                return -1;
            }
            return num;
        }

        public int IndexOfValue(TValue value) => 
            Array.IndexOf<TValue>(this.values, value, 0, this._size);

        private void Insert(int index, TKey key, TValue value)
        {
            if (this._size == this.keys.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }
            if (index < this._size)
            {
                Array.Copy(this.keys, index, this.keys, index + 1, this._size - index);
                Array.Copy(this.values, index, this.values, index + 1, this._size - index);
            }
            this.keys[index] = key;
            this.values[index] = value;
            this._size++;
            this.version++;
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            return (key is TKey);
        }

        public bool Remove(TKey key)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
            return (index >= 0);
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index, System.ExceptionResource.ArgumentOutOfRange_Index);
            }
            this._size--;
            if (index < this._size)
            {
                Array.Copy(this.keys, index + 1, this.keys, index, this._size - index);
                Array.Copy(this.values, index + 1, this.values, index, this._size - index);
            }
            this.keys[this._size] = default(TKey);
            this.values[this._size] = default(TValue);
            this.version++;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            return ((index >= 0) && EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.arrayIndex, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if ((array.Length - arrayIndex) < this.Count)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            for (int i = 0; i < this.Count; i++)
            {
                KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(this.keys[i], this.values[i]);
                array[arrayIndex + i] = pair;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            if ((index >= 0) && EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value))
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedList<TKey, TValue>) this, 1);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
            }
            if (array.Rank != 1)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_NonZeroLowerBound);
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.arrayIndex, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if ((array.Length - arrayIndex) < this.Count)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            KeyValuePair<TKey, TValue>[] pairArray = array as KeyValuePair<TKey, TValue>[];
            if (pairArray != null)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    pairArray[i + arrayIndex] = new KeyValuePair<TKey, TValue>(this.keys[i], this.values[i]);
                }
            }
            else
            {
                object[] objArray = array as object[];
                if (objArray == null)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
                try
                {
                    for (int j = 0; j < this.Count; j++)
                    {
                        objArray[j + arrayIndex] = new KeyValuePair<TKey, TValue>(this.keys[j], this.values[j]);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
            }
        }

        void IDictionary.Add(object key, object value)
        {
            SortedList<TKey, TValue>.VerifyKey(key);
            SortedList<TKey, TValue>.VerifyValueType(value);
            this.Add((TKey) key, (TValue) value);
        }

        bool IDictionary.Contains(object key) => 
            (SortedList<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey) key));

        IDictionaryEnumerator IDictionary.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedList<TKey, TValue>) this, 2);

        void IDictionary.Remove(object key)
        {
            if (SortedList<TKey, TValue>.IsCompatibleKey(key))
            {
                this.Remove((TKey) key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedList<TKey, TValue>) this, 1);

        public void TrimExcess()
        {
            int num = (int) (this.keys.Length * 0.9);
            if (this._size < num)
            {
                this.Capacity = this._size;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
            {
                value = this.values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        private static void VerifyKey(object key)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            if (!(key is TKey))
            {
                System.ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        private static void VerifyValueType(object value)
        {
            if (!(value is TValue) && ((value != null) || typeof(TValue).IsValueType))
            {
                System.ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
            }
        }

        public int Capacity
        {
            get => 
                this.keys.Length;
            set
            {
                if (value != this.keys.Length)
                {
                    if (value < this._size)
                    {
                        System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.value, System.ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                    }
                    if (value > 0)
                    {
                        TKey[] destinationArray = new TKey[value];
                        TValue[] localArray2 = new TValue[value];
                        if (this._size > 0)
                        {
                            Array.Copy(this.keys, 0, destinationArray, 0, this._size);
                            Array.Copy(this.values, 0, localArray2, 0, this._size);
                        }
                        this.keys = destinationArray;
                        this.values = localArray2;
                    }
                    else
                    {
                        this.keys = SortedList<TKey, TValue>.emptyKeys;
                        this.values = SortedList<TKey, TValue>.emptyValues;
                    }
                }
            }
        }

        public IComparer<TKey> Comparer =>
            this.comparer;

        public int Count =>
            this._size;

        public TValue this[TKey key]
        {
            get
            {
                int index = this.IndexOfKey(key);
                if (index >= 0)
                {
                    return this.values[index];
                }
                System.ThrowHelper.ThrowKeyNotFoundException();
                return default(TValue);
            }
            set
            {
                if (key == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
                }
                int index = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
                if (index >= 0)
                {
                    this.values[index] = value;
                    this.version++;
                }
                else
                {
                    this.Insert(~index, key, value);
                }
            }
        }

        public IList<TKey> Keys =>
            this.GetKeyListHelper();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            false;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys =>
            this.GetKeyListHelper();

        ICollection<TValue> IDictionary<TKey, TValue>.Values =>
            this.GetValueListHelper();

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        bool IDictionary.IsFixedSize =>
            false;

        bool IDictionary.IsReadOnly =>
            false;

        object IDictionary.this[object key]
        {
            get
            {
                if (SortedList<TKey, TValue>.IsCompatibleKey(key))
                {
                    int index = this.IndexOfKey((TKey) key);
                    if (index >= 0)
                    {
                        return this.values[index];
                    }
                }
                return null;
            }
            set
            {
                SortedList<TKey, TValue>.VerifyKey(key);
                SortedList<TKey, TValue>.VerifyValueType(value);
                this[(TKey) key] = (TValue) value;
            }
        }

        ICollection IDictionary.Keys =>
            this.GetKeyListHelper();

        ICollection IDictionary.Values =>
            this.GetValueListHelper();

        public IList<TValue> Values =>
            this.GetValueListHelper();

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;
            private SortedList<TKey, TValue> _sortedList;
            private TKey key;
            private TValue value;
            private int index;
            private int version;
            private int getEnumeratorRetType;
            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                this._sortedList = sortedList;
                this.index = 0;
                this.version = this._sortedList.version;
                this.getEnumeratorRetType = getEnumeratorRetType;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            public void Dispose()
            {
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.key;
                }
            }
            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (this.index < this._sortedList.Count)
                {
                    this.key = this._sortedList.keys[this.index];
                    this.value = this._sortedList.values[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._sortedList.Count + 1;
                this.key = default(TKey);
                this.value = default(TValue);
                return false;
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(this.key, this.value);
                }
            }
            public KeyValuePair<TKey, TValue> Current =>
                new KeyValuePair<TKey, TValue>(this.key, this.value);
            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (this.getEnumeratorRetType == 2)
                    {
                        return new DictionaryEntry(this.key, this.value);
                    }
                    return new KeyValuePair<TKey, TValue>(this.key, this.value);
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.value;
                }
            }
            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }
        }

        [Serializable, DebuggerTypeProxy(typeof(System_DictionaryKeyCollectionDebugView<,>)), DebuggerDisplay("Count = {Count}")]
        private sealed class KeyList : IList<TKey>, ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TKey key)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public void Clear()
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Contains(TKey key) => 
                this._dict.ContainsKey(key);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy(this._dict.keys, 0, array, arrayIndex, this._dict.Count);
            }

            public IEnumerator<TKey> GetEnumerator() => 
                new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);

            public int IndexOf(TKey key)
            {
                if (key == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
                }
                int num = Array.BinarySearch<TKey>(this._dict.keys, 0, this._dict.Count, key, this._dict.comparer);
                if (num >= 0)
                {
                    return num;
                }
                return -1;
            }

            public void Insert(int index, TKey value)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Remove(TKey key)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
                return false;
            }

            public void RemoveAt(int index)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                try
                {
                    Array.Copy(this._dict.keys, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);

            public int Count =>
                this._dict._size;

            public bool IsReadOnly =>
                true;

            public TKey this[int index]
            {
                get => 
                    this._dict.GetKey(index);
                set
                {
                    System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_KeyCollectionSet);
                }
            }

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                ((ICollection) this._dict).SyncRoot;
        }

        [Serializable]
        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private TKey currentKey;
            private int index;
            private int version;

            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (this.index < this._sortedList.Count)
                {
                    this.currentKey = this._sortedList.keys[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._sortedList.Count + 1;
                this.currentKey = default(TKey);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                this.index = 0;
                this.currentKey = default(TKey);
            }

            public TKey Current =>
                this.currentKey;

            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.currentKey;
                }
            }
        }

        [Serializable]
        private sealed class SortedListValueEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private TValue currentValue;
            private int index;
            private int version;

            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (this.index < this._sortedList.Count)
                {
                    this.currentValue = this._sortedList.values[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._sortedList.Count + 1;
                this.currentValue = default(TValue);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public TValue Current =>
                this.currentValue;

            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this._sortedList.Count + 1)))
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.currentValue;
                }
            }
        }

        [Serializable, DebuggerTypeProxy(typeof(System_DictionaryValueCollectionDebugView<,>)), DebuggerDisplay("Count = {Count}")]
        private sealed class ValueList : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TValue key)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public void Clear()
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Contains(TValue value) => 
                this._dict.ContainsValue(value);

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy(this._dict.values, 0, array, arrayIndex, this._dict.Count);
            }

            public IEnumerator<TValue> GetEnumerator() => 
                new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);

            public int IndexOf(TValue value) => 
                Array.IndexOf<TValue>(this._dict.values, value, 0, this._dict.Count);

            public void Insert(int index, TValue value)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            public bool Remove(TValue value)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
                return false;
            }

            public void RemoveAt(int index)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                try
                {
                    Array.Copy(this._dict.values, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);

            public int Count =>
                this._dict._size;

            public bool IsReadOnly =>
                true;

            public TValue this[int index]
            {
                get => 
                    this._dict.GetByIndex(index);
                set
                {
                    System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_SortedListNestedWrite);
                }
            }

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                ((ICollection) this._dict).SyncRoot;
        }
    }
}

