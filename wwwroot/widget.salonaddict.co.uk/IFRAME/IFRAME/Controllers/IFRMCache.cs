namespace IFRAME.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    public class IFRMCache
    {
        private static readonly System.Web.Caching.Cache _cache;

        static IFRMCache()
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                _cache = current.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

        public void Add(string key, object obj)
        {
            this.Add(key, obj, DateTime.Now.AddMinutes(20.0));
        }

        public void Add(string key, object obj, DateTime absoluteExpiration)
        {
            if (this.IsEnabled)
            {
                _cache.Insert(key, obj, null, absoluteExpiration, TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
        }

        public void Clear()
        {
            List<string> list = new List<string>();
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Key.ToString());
            }
            list.ForEach((Action<string>) (key => _cache.Remove(key)));
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            if (!this.IsEnabled)
            {
                return null;
            }
            return _cache[key];
        }

        public T Get<T>(string key) where T: class => 
            (this.Get(key) as T);

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> list = new List<string>();
            while (enumerator.MoveNext())
            {
                if (regex.IsMatch(enumerator.Key.ToString()))
                {
                    list.Add(enumerator.Key.ToString());
                }
            }
            list.ForEach((Action<string>) (key => _cache.Remove(key)));
        }

        public System.Web.Caching.Cache Cache =>
            _cache;

        public static IFRMCache Current =>
            new IFRMCache();

        public bool IsEnabled =>
            IFRAME.Controllers.Settings.IFRMCACHE_ISENABLED;
    }
}

