namespace System.Web.Services.Discovery
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class DiscoveryExceptionDictionary : DictionaryBase
    {
        public void Add(string url, Exception value)
        {
            base.Dictionary.Add(url, value);
        }

        public bool Contains(string url) => 
            base.Dictionary.Contains(url);

        public void Remove(string url)
        {
            base.Dictionary.Remove(url);
        }

        public Exception this[string url]
        {
            get => 
                ((Exception) base.Dictionary[url]);
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

