namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LifetimeContainer : ILifetimeContainer, IEnumerable<object>, IEnumerable, IDisposable
    {
        private readonly List<object> items = new List<object>();

        public void Add(object item)
        {
            lock (this.items)
            {
                this.items.Add(item);
            }
        }

        public bool Contains(object item)
        {
            lock (this.items)
            {
                return this.items.Contains(item);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.items)
                {
                    List<object> list = new List<object>(this.items);
                    list.Reverse();
                    foreach (object obj2 in list)
                    {
                        IDisposable disposable = obj2 as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    this.items.Clear();
                }
            }
        }

        public IEnumerator<object> GetEnumerator() => 
            this.items.GetEnumerator();

        public void Remove(object item)
        {
            lock (this.items)
            {
                if (this.items.Contains(item))
                {
                    this.items.Remove(item);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count
        {
            get
            {
                lock (this.items)
                {
                    return this.items.Count;
                }
            }
        }
    }
}

