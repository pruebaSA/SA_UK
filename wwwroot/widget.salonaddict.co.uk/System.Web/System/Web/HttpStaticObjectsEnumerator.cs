namespace System.Web
{
    using System;
    using System.Collections;

    internal class HttpStaticObjectsEnumerator : IDictionaryEnumerator, IEnumerator
    {
        private IDictionaryEnumerator _enum;

        internal HttpStaticObjectsEnumerator(IDictionaryEnumerator e)
        {
            this._enum = e;
        }

        public bool MoveNext() => 
            this._enum.MoveNext();

        public void Reset()
        {
            this._enum.Reset();
        }

        public object Current =>
            this.Entry;

        public DictionaryEntry Entry =>
            new DictionaryEntry(this._enum.Key, this.Value);

        public object Key =>
            this._enum.Key;

        public object Value
        {
            get
            {
                HttpStaticObjectsEntry entry = (HttpStaticObjectsEntry) this._enum.Value;
                return entry?.Instance;
            }
        }
    }
}

