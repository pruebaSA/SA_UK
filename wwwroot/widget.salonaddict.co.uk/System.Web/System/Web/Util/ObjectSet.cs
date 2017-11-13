namespace System.Web.Util
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    internal class ObjectSet : ICollection, IEnumerable
    {
        private static EmptyEnumerator _emptyEnumerator = new EmptyEnumerator();
        private IDictionary _objects;

        internal ObjectSet()
        {
        }

        public void Add(object o)
        {
            if (this._objects == null)
            {
                this._objects = new HybridDictionary(this.CaseInsensitive);
            }
            this._objects[o] = null;
        }

        public void AddCollection(ICollection c)
        {
            foreach (object obj2 in c)
            {
                this.Add(obj2);
            }
        }

        public bool Contains(object o) => 
            this._objects?.Contains(o);

        public void CopyTo(Array array, int index)
        {
            if (this._objects != null)
            {
                this._objects.Keys.CopyTo(array, index);
            }
        }

        public void Remove(object o)
        {
            if (this._objects != null)
            {
                this._objects.Remove(o);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this._objects?.Keys.GetEnumerator();

        protected virtual bool CaseInsensitive =>
            false;

        public int Count =>
            this._objects?.Keys.Count;

        bool ICollection.IsSynchronized =>
            ((this._objects == null) || this._objects.Keys.IsSynchronized);

        object ICollection.SyncRoot =>
            this._objects?.Keys.SyncRoot;

        private class EmptyEnumerator : IEnumerator
        {
            public bool MoveNext() => 
                false;

            public void Reset()
            {
            }

            public object Current =>
                null;
        }
    }
}

