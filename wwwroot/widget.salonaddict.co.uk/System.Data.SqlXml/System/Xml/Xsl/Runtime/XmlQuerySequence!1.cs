namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class XmlQuerySequence<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private const int DefaultCacheSize = 0x10;
        public static readonly XmlQuerySequence<T> Empty;
        private T[] items;
        private int size;
        private static readonly Type XPathItemType;

        static XmlQuerySequence()
        {
            XmlQuerySequence<T>.Empty = new XmlQuerySequence<T>();
            XmlQuerySequence<T>.XPathItemType = typeof(XPathItem);
        }

        public XmlQuerySequence()
        {
            this.items = new T[0x10];
        }

        public XmlQuerySequence(int capacity)
        {
            this.items = new T[capacity];
        }

        public XmlQuerySequence(T value)
        {
            this.items = new T[] { value };
            this.size = 1;
        }

        public XmlQuerySequence(T[] array, int size)
        {
            this.items = array;
            this.size = size;
        }

        public void Add(T value)
        {
            this.EnsureCache();
            this.items[this.size++] = value;
            this.OnItemsChanged();
        }

        public void Clear()
        {
            this.size = 0;
            this.OnItemsChanged();
        }

        public bool Contains(T value) => 
            (this.IndexOf(value) != -1);

        public void CopyTo(T[] array, int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[index + i] = this[i];
            }
        }

        public static XmlQuerySequence<T> CreateOrReuse(XmlQuerySequence<T> seq)
        {
            if (seq != null)
            {
                seq.Clear();
                return seq;
            }
            return new XmlQuerySequence<T>();
        }

        public static XmlQuerySequence<T> CreateOrReuse(XmlQuerySequence<T> seq, T item)
        {
            if (seq != null)
            {
                seq.Clear();
                seq.Add(item);
                return seq;
            }
            return new XmlQuerySequence<T>(item);
        }

        private void EnsureCache()
        {
            if (this.size >= this.items.Length)
            {
                T[] array = new T[this.size * 2];
                this.CopyTo(array, 0);
                this.items = array;
            }
        }

        public IEnumerator<T> GetEnumerator() => 
            new IListEnumerator<T>(this);

        public int IndexOf(T value)
        {
            int index = Array.IndexOf<T>(this.items, value);
            if (index >= this.size)
            {
                return -1;
            }
            return index;
        }

        protected virtual void OnItemsChanged()
        {
        }

        public void SortByKeys(Array keys)
        {
            if (this.size > 1)
            {
                Array.Sort(keys, this.items, 0, this.size);
                this.OnItemsChanged();
            }
        }

        void ICollection<T>.Add(T value)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T value)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T value)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (this.size != 0)
            {
                Array.Copy(this.items, 0, array, index, this.size);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new IListEnumerator<T>(this);

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) => 
            this.Contains((T) value);

        int IList.IndexOf(object value) => 
            this.IndexOf((T) value);

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int Count =>
            this.size;

        public T this[int index]
        {
            get
            {
                if (index >= this.size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this.items[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        bool ICollection<T>.IsReadOnly =>
            true;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            true;

        bool IList.IsReadOnly =>
            true;

        object IList.this[int index]
        {
            get
            {
                if (index >= this.size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this.items[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

