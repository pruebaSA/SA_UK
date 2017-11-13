namespace System.ServiceModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class ReadOnlyDictionary<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        private IDictionary<K, V> dictionary;
        private bool isFixedSize;

        internal ReadOnlyDictionary(IDictionary<K, V> dictionary) : this(dictionary, true)
        {
        }

        internal ReadOnlyDictionary(IDictionary<K, V> dictionary, bool makeCopy)
        {
            if (makeCopy)
            {
                this.dictionary = new Dictionary<K, V>(dictionary);
            }
            else
            {
                this.dictionary = dictionary;
            }
            this.isFixedSize = makeCopy;
        }

        public void Add(K key, V value)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
        }

        public void Clear()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
        }

        public bool ContainsKey(K key) => 
            this.dictionary.ContainsKey(key);

        public bool Remove(K key)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> keyValuePair)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> keyValuePair)
        {
            if (this.ContainsKey(keyValuePair.Key))
            {
                V local = this[keyValuePair.Key];
                return local.Equals(keyValuePair.Value);
            }
            return false;
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            this.dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> keyValuePair)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() => 
            this.dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable<KeyValuePair<K, V>>) this).GetEnumerator();

        public bool TryGetValue(K key, out V value) => 
            this.dictionary.TryGetValue(key, out value);

        public int Count =>
            this.dictionary.Count;

        public bool IsFixedSize =>
            this.isFixedSize;

        public bool IsReadOnly =>
            true;

        public V this[K key]
        {
            get => 
                this.dictionary[key];
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("ObjectIsReadOnly")));
            }
        }

        public ICollection<K> Keys =>
            this.dictionary.Keys;

        public ICollection<V> Values =>
            this.dictionary.Values;
    }
}

