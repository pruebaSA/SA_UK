namespace System.Collections.ObjectModel
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;

    [Serializable, DebuggerDisplay("Count = {Count}"), FriendAccessAllowed, ComVisible(false)]
    internal class WeakReadOnlyCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        [NonSerialized]
        private object _syncRoot;
        private IList<WeakReference> list;

        public WeakReadOnlyCollection(IList<WeakReference> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            this.list = list;
        }

        public bool Contains(T value) => 
            this.CreateDereferencedList().Contains(value);

        public void CopyTo(T[] array, int index)
        {
            this.CreateDereferencedList().CopyTo(array, index);
        }

        private IList<T> CreateDereferencedList()
        {
            int count = this.list.Count;
            List<T> list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add((T) this.list[i].Target);
            }
            return list;
        }

        public IEnumerator<T> GetEnumerator() => 
            new WeakEnumerator<T>(this.list.GetEnumerator());

        public int IndexOf(T value) => 
            this.CreateDereferencedList().IndexOf(value);

        private static bool IsCompatibleObject(object value) => 
            ((value is T) || ((value == null) && (default(T) == null)));

        void ICollection<T>.Add(T value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        bool ICollection<T>.Remove(T value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void IList<T>.Insert(int index, T value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException(System.Windows.SR.Get("Arg_RankMultiDimNotSupported"));
            }
            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("Arg_NonZeroLowerBound"));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", System.Windows.SR.Get("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((array.Length - index) < this.Count)
            {
                throw new ArgumentException(System.Windows.SR.Get("Arg_ArrayPlusOffTooSmall"));
            }
            IList<T> list = this.CreateDereferencedList();
            T[] localArray = array as T[];
            if (localArray != null)
            {
                list.CopyTo(localArray, index);
            }
            else
            {
                Type elementType = array.GetType().GetElementType();
                Type c = typeof(T);
                if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
                {
                    throw new ArgumentException(System.Windows.SR.Get("Argument_InvalidArrayType"));
                }
                object[] objArray = array as object[];
                if (objArray == null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("Argument_InvalidArrayType"));
                }
                int count = list.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objArray[index++] = list[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(System.Windows.SR.Get("Argument_InvalidArrayType"));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new WeakEnumerator<T>(this.list.GetEnumerator());

        int IList.Add(object value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void IList.Clear()
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        bool IList.Contains(object value) => 
            (WeakReadOnlyCollection<T>.IsCompatibleObject(value) && this.Contains((T) value));

        int IList.IndexOf(object value)
        {
            if (WeakReadOnlyCollection<T>.IsCompatibleObject(value))
            {
                return this.IndexOf((T) value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
        }

        public int Count =>
            this.list.Count;

        public T this[int index] =>
            ((T) this.list[index].Target);

        bool ICollection<T>.IsReadOnly =>
            true;

        T IList<T>.this[int index]
        {
            get => 
                ((T) this.list[index].Target);
            set
            {
                throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
            }
        }

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    ICollection list = this.list as ICollection;
                    if (list != null)
                    {
                        this._syncRoot = list.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
                    }
                }
                return this._syncRoot;
            }
        }

        bool IList.IsFixedSize =>
            true;

        bool IList.IsReadOnly =>
            true;

        object IList.this[int index]
        {
            get => 
                ((T) this.list[index].Target);
            set
            {
                throw new NotSupportedException(System.Windows.SR.Get("NotSupported_ReadOnlyCollection"));
            }
        }

        private class WeakEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private IEnumerator ie;

            public WeakEnumerator(IEnumerator ie)
            {
                this.ie = ie;
            }

            public void Dispose()
            {
            }

            public bool MoveNext() => 
                this.ie.MoveNext();

            void IEnumerator.Reset()
            {
                this.ie.Reset();
            }

            public T Current
            {
                get
                {
                    WeakReference current = this.ie.Current as WeakReference;
                    if (current != null)
                    {
                        return (T) current.Target;
                    }
                    return default(T);
                }
            }

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

