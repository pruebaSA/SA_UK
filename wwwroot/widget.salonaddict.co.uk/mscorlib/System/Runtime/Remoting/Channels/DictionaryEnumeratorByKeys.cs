namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;

    internal class DictionaryEnumeratorByKeys : IDictionaryEnumerator, IEnumerator
    {
        private IEnumerator _keyEnum;
        private IDictionary _properties;

        public DictionaryEnumeratorByKeys(IDictionary properties)
        {
            this._properties = properties;
            this._keyEnum = properties.Keys.GetEnumerator();
        }

        public bool MoveNext() => 
            this._keyEnum.MoveNext();

        public void Reset()
        {
            this._keyEnum.Reset();
        }

        public object Current =>
            this.Entry;

        public DictionaryEntry Entry =>
            new DictionaryEntry(this.Key, this.Value);

        public object Key =>
            this._keyEnum.Current;

        public object Value =>
            this._properties[this.Key];
    }
}

