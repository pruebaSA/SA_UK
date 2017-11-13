namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class ReadOnlyMetadataCollection<T> : ReadOnlyCollection<T> where T: MetadataItem
    {
        internal ReadOnlyMetadataCollection(IList<T> collection) : base(collection)
        {
        }

        public virtual bool Contains(string identity) => 
            ((MetadataCollection<T>) base.Items).ContainsIdentity(identity);

        public Enumerator<T> GetEnumerator() => 
            new Enumerator<T>(base.Items);

        public virtual T GetValue(string identity, bool ignoreCase) => 
            ((MetadataCollection<T>) base.Items).GetValue(identity, ignoreCase);

        public virtual int IndexOf(T value) => 
            base.IndexOf(value);

        public virtual bool TryGetValue(string identity, bool ignoreCase, out T item) => 
            ((MetadataCollection<T>) base.Items).TryGetValue(identity, ignoreCase, out item);

        public bool IsReadOnly =>
            true;

        public virtual T this[string identity] =>
            ((MetadataCollection<T>) base.Items)[identity];

        internal MetadataCollection<T> Source =>
            ((MetadataCollection<T>) base.Items);

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private int _nextIndex;
            private IList<T> _parent;
            private T _current;
            internal Enumerator(IList<T> collection)
            {
                this._parent = collection;
                this._nextIndex = 0;
                this._current = default(T);
            }

            public T Current =>
                this._current;
            object IEnumerator.Current =>
                this.Current;
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this._nextIndex < this._parent.Count)
                {
                    this._current = this._parent[this._nextIndex];
                    this._nextIndex++;
                    return true;
                }
                this._current = default(T);
                return false;
            }

            public void Reset()
            {
                this._current = default(T);
                this._nextIndex = 0;
            }
        }
    }
}

