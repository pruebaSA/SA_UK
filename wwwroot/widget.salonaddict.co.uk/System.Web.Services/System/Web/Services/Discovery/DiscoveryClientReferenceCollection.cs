namespace System.Web.Services.Discovery
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class DiscoveryClientReferenceCollection : DictionaryBase
    {
        public void Add(DiscoveryReference value)
        {
            this.Add(value.Url, value);
        }

        public void Add(string url, DiscoveryReference value)
        {
            base.Dictionary.Add(url, value);
        }

        public bool Contains(string url) => 
            base.Dictionary.Contains(url);

        public void Remove(string url)
        {
            base.Dictionary.Remove(url);
        }

        public DiscoveryReference this[string url]
        {
            get => 
                ((DiscoveryReference) base.Dictionary[url]);
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

