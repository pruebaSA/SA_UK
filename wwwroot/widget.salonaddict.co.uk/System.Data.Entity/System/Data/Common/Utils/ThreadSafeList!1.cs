namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private List<T> _list;
        private readonly ReaderWriterLockSlim _lock;

        internal ThreadSafeList()
        {
            this._list = new List<T>();
            this._lock = new ReaderWriterLockSlim();
        }

        public void Add(T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Add(item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Clear();
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            bool flag;
            this._lock.EnterReadLock();
            try
            {
                flag = this._list.Contains(item);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
            return flag;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.CopyTo(array, arrayIndex);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            this._lock.EnterReadLock();
            foreach (T iteratorVariable0 in this._list)
            {
                yield return iteratorVariable0;
            }
        }

        public int IndexOf(T item)
        {
            int index;
            this._lock.EnterReadLock();
            try
            {
                index = this._list.IndexOf(item);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
            return index;
        }

        public void Insert(int index, T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Insert(index, item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            bool flag;
            this._lock.EnterWriteLock();
            try
            {
                flag = this._list.Remove(item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
            return flag;
        }

        public void RemoveAt(int index)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.RemoveAt(index);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count
        {
            get
            {
                int count;
                this._lock.EnterReadLock();
                try
                {
                    count = this._list.Count;
                }
                finally
                {
                    this._lock.ExitReadLock();
                }
                return count;
            }
        }

        public bool IsReadOnly =>
            false;

        public T this[int index]
        {
            get
            {
                T local;
                this._lock.EnterReadLock();
                try
                {
                    local = this._list[index];
                }
                finally
                {
                    this._lock.ExitReadLock();
                }
                return local;
            }
            set
            {
                this._lock.EnterWriteLock();
                try
                {
                    this._list[index] = value;
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetEnumerator>d__0 : IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public ThreadSafeList<T> <>4__this;
            public List<T>.Enumerator <>7__wrap3;
            public T <value>5__1;

            [DebuggerHidden]
            public <GetEnumerator>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private void <>m__Finally2()
            {
                this.<>1__state = -1;
                this.<>4__this._lock.ExitReadLock();
            }

            private void <>m__Finally4()
            {
                this.<>1__state = 1;
                this.<>7__wrap3.Dispose();
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>4__this._lock.EnterReadLock();
                            this.<>1__state = 1;
                            this.<>7__wrap3 = this.<>4__this._list.GetEnumerator();
                            this.<>1__state = 2;
                            while (this.<>7__wrap3.MoveNext())
                            {
                                this.<value>5__1 = this.<>7__wrap3.Current;
                                this.<>2__current = this.<value>5__1;
                                this.<>1__state = 3;
                                return true;
                            Label_0079:
                                this.<>1__state = 2;
                            }
                            this.<>m__Finally4();
                            this.<>m__Finally2();
                            break;

                        case 3:
                            goto Label_0079;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
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
                    case 3:
                        try
                        {
                            switch (this.<>1__state)
                            {
                                case 2:
                                case 3:
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
                        finally
                        {
                            this.<>m__Finally2();
                        }
                        break;

                    default:
                        return;
                }
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

