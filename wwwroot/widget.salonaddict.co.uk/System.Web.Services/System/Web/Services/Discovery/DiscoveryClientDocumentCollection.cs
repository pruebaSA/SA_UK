namespace System.Web.Services.Discovery
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class DiscoveryClientDocumentCollection : DictionaryBase
    {
        public void Add(string url, object value)
        {
            base.Dictionary.Add(url, value);
        }

        public bool Contains(string url) => 
            base.Dictionary.Contains(url);

        public void Remove(string url)
        {
            base.Dictionary.Remove(url);
        }

        public object this[string url]
        {
            get => 
                base.Dictionary[url];
            set
            {
                base.Dictionary[url] = value;
            }
        }

        public ICollection Keys =>
            base.Dictionary.Keys;

        public ICollection Values =>
            base.Dictionary.Values;
    }
}

