namespace System.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal abstract class ListBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        protected ListBase()
        {
        }

        public virtual void Add(T value)
        {
            this.Insert(this.Count, value);
        }

        public virtual void Clear()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                this.RemoveAt(i);
            }
        }

        public virtual bool Contains(T value) => 
            (this.IndexOf(value) != -1);

        public virtual void CopyTo(T[] array, int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[index + i] = this[i];
            }
        }

        public virtual IListEnumerator<T> GetEnumerator() => 
            new IListEnumerator<T>(this);

        public virtual int IndexOf(T value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (value.Equals(this[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual void Insert(int index, T value)
        {
            throw new NotSupportedException();
        }

        private static bool IsCompatibleType(object value)
        {
            if (((value != null) || typeof(T).IsValueType) && !(value is T))
            {
                return false;
            }
            return true;
        }

        public virtual bool Remove(T value)
        {
            int index = this.IndexOf(value);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            new IListEnumerator<T>(this);

        void ICollection.CopyTo(Array array, int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array.SetValue(this[i], index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new IListEnumerator<T>(this);

        int IList.Add(object value)
        {
            if (!ListBase<T>.IsCompatibleType(value.GetType()))
            {
                throw new ArgumentException();
            }
            this.Add((T) value);
            return (this.Count - 1);
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            if (!ListBase<T>.IsCompatibleType(value.GetType()))
            {
                return false;
            }
            return this.Contains((T) value);
        }

        int IList.IndexOf(object value)
        {
            if (!ListBase<T>.IsCompatibleType(value.GetType()))
            {
                return -1;
            }
            return this.IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            if (!ListBase<T>.IsCompatibleType(value.GetType()))
            {
                throw new ArgumentException();
            }
            this.Insert(index, (T) value);
        }

        void IList.Remove(object value)
        {
            if (!ListBase<T>.IsCompatibleType(value.GetType()))
            {
                throw new ArgumentException();
            }
            this.Remove((T) value);
        }

        public abstract int Count { get; }

        public virtual bool IsFixedSize =>
            true;

        public virtual bool IsReadOnly =>
            true;

        public abstract T this[int index] { get; set; }

        bool ICollection.IsSynchronized =>
            this.IsReadOnly;

        object ICollection.SyncRoot =>
            this;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                if (!ListBase<T>.IsCompatibleType(value.GetType()))
                {
                    throw new ArgumentException();
                }
                this[index] = (T) value;
            }
        }
    }
}

