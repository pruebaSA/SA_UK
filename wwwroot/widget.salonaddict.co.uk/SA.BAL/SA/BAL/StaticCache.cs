namespace SA.BAL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    public class StaticCache : ICacheManager
    {
        private readonly System.Web.Caching.Cache _cache;

        public StaticCache()
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                this._cache = current.Cache;
            }
            else
            {
                this._cache = HttpRuntime.Cache;
            }
        }

        public virtual void Add(string key, object obj)
        {
            this.Add(key, obj, this.DefaultExpiration, null);
        }

        public virtual void Add(string key, object obj, DateTime absoluteExpiration)
        {
            this.Add(key, obj, absoluteExpiration, null);
        }

        public virtual void Add(string key, object obj, DateTime absoluteExpiration, CacheDependency dep)
        {
            if (this.IsEnabled && (obj != null))
            {
                this._cache.Insert(key, obj, dep, absoluteExpiration, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }

        public virtual void Clear()
        {
            List<string> list = new List<string>();
            IDictionaryEnumerator enumerator = this._cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Key.ToString());
            }
            list.ForEach((Action<string>) (key => this._cache.Remove(key)));
        }

        public virtual object Get(string key) => 
            this._cache[key];

        public virtual T Get<T>(string key) where T: class
        {
            object obj2 = this._cache[key];
            return (obj2 as T);
        }

        public virtual void Remove(string key)
        {
            this._cache.Remove(key);
        }

        public virtual void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator enumerator = this._cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> list = new List<string>();
            while (enumerator.MoveNext())
            {
                if (regex.IsMatch(enumerator.Key.ToString()))
                {
                    list.Add(enumerator.Key.ToString());
                }
            }
            list.ForEach((Action<string>) (key => this._cache.Remove(key)));
        }

        protected System.Web.Caching.Cache Cache =>
            this._cache;

        public virtual DateTime DefaultExpiration =>
            DateTime.Now.AddMinutes(10.0);

        public virtual bool IsEnabled =>
            true;
    }
}

