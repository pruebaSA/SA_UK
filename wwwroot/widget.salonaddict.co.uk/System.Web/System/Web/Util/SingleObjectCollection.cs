namespace System.Web.Util
{
    using System;
    using System.Collections;

    internal class SingleObjectCollection : ICollection, IEnumerable
    {
        private object _object;

        public SingleObjectCollection(object o)
        {
            this._object = o;
        }

        public void CopyTo(Array array, int index)
        {
            array.SetValue(this._object, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new SingleObjectEnumerator(this._object);

        public int Count =>
            1;

        bool ICollection.IsSynchronized =>
            true;

        object ICollection.SyncRoot =>
            this;

        private class SingleObjectEnumerator : IEnumerator
        {
            private object _object;
            private bool done;

            public SingleObjectEnumerator(object o)
            {
                this._object = o;
            }

            public bool MoveNext()
            {
                if (!this.done)
                {
                    this.done = true;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                this.done = false;
            }

            public object Current =>
                this._object;
        }
    }
}

