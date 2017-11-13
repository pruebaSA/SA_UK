namespace MS.Internal.Utility
{
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal class ReadOnlyDictionary<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IDictionary, ICollection, IEnumerable
    {
        private Dictionary<K, V> _dict;

        internal ReadOnlyDictionary(Dictionary<K, V> dict)
        {
            Invariant.Assert(dict != null);
            this._dict = dict;
        }

        public void Add(KeyValuePair<K, V> pair)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public void Add(K key, V value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public void Add(object key, object value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public void Clear()
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public bool Contains(KeyValuePair<K, V> pair) => 
            this._dict.Contains(pair);

        public bool Contains(object key) => 
            this._dict.Contains(key);

        public bool ContainsKey(K key) => 
            this._dict.ContainsKey(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            this._dict.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            this._dict.CopyTo(array, index);
        }

        public bool Remove(KeyValuePair<K, V> pair)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public bool Remove(K key)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        public void Remove(object key)
        {
            throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() => 
            this._dict.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => 
            this._dict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._dict.GetEnumerator();

        public bool TryGetValue(K key, out V value) => 
            this._dict.TryGetValue(key, out value);

        public int Count =>
            this._dict.Count;

        public bool IsFixedSize =>
            true;

        public bool IsReadOnly =>
            true;

        public bool IsSynchronized =>
            ((ICollection) this._dict).IsSynchronized;

        public V this[K key]
        {
            get => 
                this._dict[key];
            set
            {
                throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
            }
        }

        public object this[object key]
        {
            get => 
                ((IDictionary) this._dict)[key];
            set
            {
                throw new NotSupportedException(System.Windows.SR.Get("DictionaryIsReadOnly"));
            }
        }

        public ICollection<K> Keys =>
            this._dict.Keys;

        public object SyncRoot =>
            ((ICollection) this._dict).SyncRoot;

        ICollection IDictionary.Keys =>
            ((IDictionary) this._dict).Keys;

        ICollection IDictionary.Values =>
            ((IDictionary) this._dict).Values;

        public ICollection<V> Values =>
            this._dict.Values;
    }
}

