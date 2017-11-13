namespace System.Web.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private Dictionary<TKey, TValue> _dictionary;
        private List<TKey> _keys;
        private List<TValue> _values;

        public OrderedDictionary() : this(0)
        {
        }

        public OrderedDictionary(int capacity)
        {
            this._dictionary = new Dictionary<TKey, TValue>(capacity);
            this._keys = new List<TKey>(capacity);
            this._values = new List<TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            this._dictionary.Add(key, value);
            this._keys.Add(key);
            this._values.Add(value);
        }

        public void Clear()
        {
            this._dictionary.Clear();
            this._keys.Clear();
            this._values.Clear();
        }

        public bool ContainsKey(TKey key) => 
            this._dictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => 
            this._dictionary.ContainsValue(value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            int iteratorVariable0 = 0;
            foreach (TKey iteratorVariable1 in this._keys)
            {
                yield return new KeyValuePair<TKey, TValue>(iteratorVariable1, this._values[iteratorVariable0]);
                iteratorVariable0++;
            }
        }

        public bool Remove(TKey key)
        {
            this.RemoveFromLists(key);
            return this._dictionary.Remove(key);
        }

        private void RemoveFromLists(TKey key)
        {
            int index = this._keys.IndexOf(key);
            if (index != -1)
            {
                this._keys.RemoveAt(index);
                this._values.RemoveAt(index);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => 
            this._dictionary.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this._dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            bool flag = this._dictionary.Remove(item);
            if (flag)
            {
                this.RemoveFromLists(item.Key);
            }
            return flag;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value) => 
            this._dictionary.TryGetValue(key, out value);

        public int Count =>
            this._dictionary.Count;

        public TValue this[TKey key]
        {
            get => 
                this._dictionary[key];
            set
            {
                this.RemoveFromLists(key);
                this._dictionary[key] = value;
                this._keys.Add(key);
                this._values.Add(value);
            }
        }

        public ICollection<TKey> Keys =>
            this._keys.AsReadOnly();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            this._dictionary.IsReadOnly;

        public ICollection<TValue> Values =>
            ((ICollection<TValue>) this._values.AsReadOnly());

        [CompilerGenerated]
        private sealed class <GetEnumerator>d__0 : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private KeyValuePair<TKey, TValue> <>2__current;
            public OrderedDictionary<TKey, TValue> <>4__this;
            public List<TKey>.Enumerator <>7__wrap3;
            public int <i>5__1;
            public TKey <key>5__2;

            [DebuggerHidden]
            public <GetEnumerator>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private void <>m__Finally4()
            {
                this.<>1__state = -1;
                this.<>7__wrap3.Dispose();
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<i>5__1 = 0;
                            this.<>7__wrap3 = this.<>4__this._keys.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_00A3;

                        case 2:
                            this.<>1__state = 1;
                            this.<i>5__1++;
                            goto Label_00A3;

                        default:
                            goto Label_00B6;
                    }
                Label_004B:
                    this.<key>5__2 = this.<>7__wrap3.Current;
                    this.<>2__current = new KeyValuePair<TKey, TValue>(this.<key>5__2, this.<>4__this._values[this.<i>5__1]);
                    this.<>1__state = 2;
                    return true;
                Label_00A3:
                    if (this.<>7__wrap3.MoveNext())
                    {
                        goto Label_004B;
                    }
                    this.<>m__Finally4();
                Label_00B6:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally4();
                        }
                        return;
                }
            }

            KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

