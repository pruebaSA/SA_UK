﻿namespace System.Data.Odbc
{
    using System;
    using System.Collections;
    using System.Reflection;

    [Serializable]
    public sealed class OdbcErrorCollection : ICollection, IEnumerable
    {
        private ArrayList _items = new ArrayList();

        internal OdbcErrorCollection()
        {
        }

        internal void Add(OdbcError error)
        {
            this._items.Add(error);
        }

        public void CopyTo(Array array, int i)
        {
            this._items.CopyTo(array, i);
        }

        public void CopyTo(OdbcError[] array, int i)
        {
            this._items.CopyTo(array, i);
        }

        public IEnumerator GetEnumerator() => 
            this._items.GetEnumerator();

        internal void SetSource(string Source)
        {
            foreach (object obj2 in this._items)
            {
                ((OdbcError) obj2).SetSource(Source);
            }
        }

        public int Count =>
            this._items.Count;

        public OdbcError this[int i] =>
            ((OdbcError) this._items[i]);

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;
    }
}

