namespace System.Web
{
    using System;
    using System.Collections.Specialized;

    internal class HttpDictionary : NameObjectCollectionBase
    {
        internal HttpDictionary() : base(Misc.CaseInsensitiveInvariantKeyComparer)
        {
        }

        internal string[] GetAllKeys() => 
            base.BaseGetAllKeys();

        internal string GetKey(int index) => 
            base.BaseGetKey(index);

        internal object GetValue(int index) => 
            base.BaseGet(index);

        internal object GetValue(string key) => 
            base.BaseGet(key);

        internal void SetValue(string key, object value)
        {
            base.BaseSet(key, value);
        }

        internal int Size =>
            this.Count;
    }
}

