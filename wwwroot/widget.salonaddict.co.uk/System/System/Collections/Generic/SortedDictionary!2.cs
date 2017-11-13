namespace System.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, DebuggerTypeProxy(typeof(System_DictionaryDebugView<,>)), DebuggerDisplay("Count = {Count}")]
    public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private TreeSet<KeyValuePair<TKey, TValue>> _set;
        [NonSerialized]
        private KeyCollection<TKey, TValue> keys;
        [NonSerialized]
        private ValueCollection<TKey, TValue> values;

        public SortedDictionary() : this((IComparer<TKey>) null)
        {
        }

        public SortedDictionary(IComparer<TKey> comparer)
        {
            this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer<TKey, TValue>(comparer));
        }

        public SortedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }

        public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            if (dictionary == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.dictionary);
            }
            this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer<TKey, TValue>(comparer));
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                this._set.Add(pair);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Clear()
        {
            this._set.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            return this._set.Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)));
        }

        public bool ContainsValue(TValue value)
        {
            TreeWalkAction<KeyValuePair<TKey, TValue>> action = null;
            bool found = false;
            if (value == null)
            {
                if (action == null)
                {
                    action = delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                        if (node.Item.Value == null)
                        {
                            found = true;
                            return false;
                        }
                        return true;
                    };
                }
                this._set.InOrderTreeWalk(action);
            }
            else
            {
                EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
                this._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                    if (valueComparer.Equals(node.Item.Value, value))
                    {
                        found = true;
                        return false;
                    }
                    return true;
                });
            }
            return found;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            this._set.CopyTo(array, index);
        }

        public Enumerator<TKey, TValue> GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedDictionary<TKey, TValue>) this, 1);

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
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            return this._set.Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this._set.Add(keyValuePair);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
            if (node == null)
            {
                return false;
            }
            if (keyValuePair.Value == null)
            {
                return (node.Item.Value == null);
            }
            return EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
            if ((node != null) && EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value))
            {
                this._set.Remove(keyValuePair);
                return true;
            }
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedDictionary<TKey, TValue>) this, 1);

        void ICollection.CopyTo(Array array, int index)
        {
            this._set.CopyTo(array, index);
        }

        void IDictionary.Add(object key, object value)
        {
            SortedDictionary<TKey, TValue>.VerifyKey(key);
            SortedDictionary<TKey, TValue>.VerifyValueType(value);
            this.Add((TKey) key, (TValue) value);
        }

        bool IDictionary.Contains(object key) => 
            (SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey) key));

        IDictionaryEnumerator IDictionary.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedDictionary<TKey, TValue>) this, 2);

        void IDictionary.Remove(object key)
        {
            if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key))
            {
                this.Remove((TKey) key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new Enumerator<TKey, TValue>((SortedDictionary<TKey, TValue>) this, 1);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
            }
            TreeSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
            if (node == null)
            {
                value = default(TValue);
                return false;
            }
            value = node.Item.Value;
            return true;
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

        public IComparer<TKey> Comparer =>
            ((KeyValuePairComparer<TKey, TValue>) this._set.Comparer).keyComparer;

        public int Count =>
            this._set.Count;

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
                }
                TreeSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
                return node?.Item.Value;
            }
            set
            {
                if (key == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.key);
                }
                TreeSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
                if (node == null)
                {
                    this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
                else
                {
                    node.Item = new KeyValuePair<TKey, TValue>(node.Item.Key, value);
                    this._set.UpdateVersion();
                }
            }
        }

        public KeyCollection<TKey, TValue> Keys
        {
            get
            {
                if (this.keys == null)
                {
                    this.keys = new KeyCollection<TKey, TValue>((SortedDictionary<TKey, TValue>) this);
                }
                return this.keys;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            false;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys =>
            this.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values =>
            this.Values;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            ((ICollection) this._set).SyncRoot;

        bool IDictionary.IsFixedSize =>
            false;

        bool IDictionary.IsReadOnly =>
            false;

        object IDictionary.this[object key]
        {
            get
            {
                TValue local;
                if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.TryGetValue((TKey) key, out local))
                {
                    return local;
                }
                return null;
            }
            set
            {
                SortedDictionary<TKey, TValue>.VerifyKey(key);
                SortedDictionary<TKey, TValue>.VerifyValueType(value);
                this[(TKey) key] = (TValue) value;
            }
        }

        ICollection IDictionary.Keys =>
            this.Keys;

        ICollection IDictionary.Values =>
            this.Values;

        public ValueCollection<TKey, TValue> Values
        {
            get
            {
                if (this.values == null)
                {
                    this.values = new ValueCollection<TKey, TValue>((SortedDictionary<TKey, TValue>) this);
                }
                return this.values;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;
            private TreeSet<KeyValuePair<TKey, TValue>>.Enumerator treeEnum;
            private int getEnumeratorRetType;
            internal Enumerator(SortedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.treeEnum = dictionary._set.GetEnumerator();
                this.getEnumeratorRetType = getEnumeratorRetType;
            }

            public bool MoveNext() => 
                this.treeEnum.MoveNext();

            public void Dispose()
            {
                this.treeEnum.Dispose();
            }

            public KeyValuePair<TKey, TValue> Current =>
                this.treeEnum.Current;
            internal bool NotStartedOrEnded =>
                this.treeEnum.NotStartedOrEnded;
            internal void Reset()
            {
                this.treeEnum.Reset();
            }

            void IEnumerator.Reset()
            {
                this.treeEnum.Reset();
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.NotStartedOrEnded)
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (this.getEnumeratorRetType == 2)
                    {
                        return new DictionaryEntry(this.Current.Key, this.Current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(this.Current.Key, this.Current.Value);
                }
            }
            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (this.NotStartedOrEnded)
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.Current.Key;
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (this.NotStartedOrEnded)
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return this.Current.Value;
                }
            }
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (this.NotStartedOrEnded)
                    {
                        System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(this.Current.Key, this.Current.Value);
                }
            }
        }

        [Serializable, DebuggerTypeProxy(typeof(System_DictionaryKeyCollectionDebugView<,>)), DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private SortedDictionary<TKey, TValue> dictionary;

            public KeyCollection(SortedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
                }
                if (index < 0)
                {
                    System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index);
                }
                if ((array.Length - index) < this.Count)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                this.dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                    array[index++] = node.Item.Key;
                    return true;
                });
            }

            public Enumerator<TKey, TValue> GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            void ICollection<TKey>.Add(TKey item)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_KeyCollectionSet);
            }

            void ICollection<TKey>.Clear()
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_KeyCollectionSet);
            }

            bool ICollection<TKey>.Contains(TKey item) => 
                this.dictionary.ContainsKey(item);

            bool ICollection<TKey>.Remove(TKey item)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            void ICollection.CopyTo(Array array, int index)
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
                if (index < 0)
                {
                    System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.arrayIndex, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if ((array.Length - index) < this.dictionary.Count)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TKey[] localArray = array as TKey[];
                if (localArray != null)
                {
                    this.CopyTo(localArray, index);
                }
                else
                {
                    TreeWalkAction<KeyValuePair<TKey, TValue>> action = null;
                    object[] objects = (object[]) array;
                    if (objects == null)
                    {
                        System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                    }
                    try
                    {
                        if (action == null)
                        {
                            action = delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                                objects[index++] = node.Item.Key;
                                return true;
                            };
                        }
                        this.dictionary._set.InOrderTreeWalk(action);
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            public int Count =>
                this.dictionary.Count;

            bool ICollection<TKey>.IsReadOnly =>
                true;

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                ((ICollection) this.dictionary).SyncRoot;

            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
            {
                private SortedDictionary<TKey, TValue>.Enumerator dictEnum;
                internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
                {
                    this.dictEnum = dictionary.GetEnumerator();
                }

                public void Dispose()
                {
                    this.dictEnum.Dispose();
                }

                public bool MoveNext() => 
                    this.dictEnum.MoveNext();

                public TKey Current =>
                    this.dictEnum.Current.Key;
                object IEnumerator.Current
                {
                    get
                    {
                        if (this.dictEnum.NotStartedOrEnded)
                        {
                            System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return this.Current;
                    }
                }
                void IEnumerator.Reset()
                {
                    this.dictEnum.Reset();
                }
            }
        }

        [Serializable]
        internal class KeyValuePairComparer : Comparer<KeyValuePair<TKey, TValue>>
        {
            internal IComparer<TKey> keyComparer;

            public KeyValuePairComparer(IComparer<TKey> keyComparer)
            {
                if (keyComparer == null)
                {
                    this.keyComparer = Comparer<TKey>.Default;
                }
                else
                {
                    this.keyComparer = keyComparer;
                }
            }

            public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => 
                this.keyComparer.Compare(x.Key, y.Key);
        }

        [Serializable, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(System_DictionaryValueCollectionDebugView<,>))]
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
        {
            private SortedDictionary<TKey, TValue> dictionary;

            public ValueCollection(SortedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
                }
                if (index < 0)
                {
                    System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index);
                }
                if ((array.Length - index) < this.Count)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                this.dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                    array[index++] = node.Item.Value;
                    return true;
                });
            }

            public Enumerator<TKey, TValue> GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            void ICollection<TValue>.Add(TValue item)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_ValueCollectionSet);
            }

            void ICollection<TValue>.Clear()
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Contains(TValue item) => 
                this.dictionary.ContainsValue(item);

            bool ICollection<TValue>.Remove(TValue item)
            {
                System.ThrowHelper.ThrowNotSupportedException(System.ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            void ICollection.CopyTo(Array array, int index)
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
                if (index < 0)
                {
                    System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.arrayIndex, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if ((array.Length - index) < this.dictionary.Count)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                TValue[] localArray = array as TValue[];
                if (localArray != null)
                {
                    this.CopyTo(localArray, index);
                }
                else
                {
                    TreeWalkAction<KeyValuePair<TKey, TValue>> action = null;
                    object[] objects = (object[]) array;
                    if (objects == null)
                    {
                        System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                    }
                    try
                    {
                        if (action == null)
                        {
                            action = delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) {
                                objects[index++] = node.Item.Value;
                                return true;
                            };
                        }
                        this.dictionary._set.InOrderTreeWalk(action);
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                new Enumerator<TKey, TValue>(this.dictionary);

            public int Count =>
                this.dictionary.Count;

            bool ICollection<TValue>.IsReadOnly =>
                true;

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                ((ICollection) this.dictionary).SyncRoot;

            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
            {
                private SortedDictionary<TKey, TValue>.Enumerator dictEnum;
                internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
                {
                    this.dictEnum = dictionary.GetEnumerator();
                }

                public void Dispose()
                {
                    this.dictEnum.Dispose();
                }

                public bool MoveNext() => 
                    this.dictEnum.MoveNext();

                public TValue Current =>
                    this.dictEnum.Current.Value;
                object IEnumerator.Current
                {
                    get
                    {
                        if (this.dictEnum.NotStartedOrEnded)
                        {
                            System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return this.Current;
                    }
                }
                void IEnumerator.Reset()
                {
                    this.dictEnum.Reset();
                }
            }
        }
    }
}

