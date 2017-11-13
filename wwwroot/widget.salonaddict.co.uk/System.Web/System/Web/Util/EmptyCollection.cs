namespace System.Web.Util
{
    using System;
    using System.Collections;

    internal class EmptyCollection : ICollection, IEnumerable, IEnumerator
    {
        private static EmptyCollection s_theEmptyCollection = new EmptyCollection();

        private EmptyCollection()
        {
        }

        public void CopyTo(Array array, int index)
        {
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this;

        bool IEnumerator.MoveNext() => 
            false;

        void IEnumerator.Reset()
        {
        }

        public int Count =>
            0;

        internal static EmptyCollection Instance =>
            s_theEmptyCollection;

        bool ICollection.IsSynchronized =>
            true;

        object ICollection.SyncRoot =>
            this;

        object IEnumerator.Current =>
            null;
    }
}

