namespace SA.BAL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;

    public class RequestCache : ICacheManager
    {
        public virtual void Add(string key, object obj)
        {
            IDictionary items = this.Items;
            if ((items != null) && (this.IsEnabled && (obj != null)))
            {
                items.Add(key, obj);
            }
        }

        public virtual object Get(string key)
        {
            IDictionary items = this.Items;
            if ((items != null) && items.Contains(key))
            {
                return items[key];
            }
            return null;
        }

        public virtual void Remove(string key)
        {
            IDictionary items = this.Items;
            if (items != null)
            {
                items.Remove(key);
            }
        }

        public virtual void RemoveByPattern(string pattern)
        {
            IDictionary items = this.Items;
            if (items != null)
            {
                IDictionaryEnumerator enumerator = items.GetEnumerator();
                Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                List<string> list = new List<string>();
                while (enumerator.MoveNext())
                {
                    if (regex.IsMatch(enumerator.Key.ToString()))
                    {
                        list.Add(enumerator.Key.ToString());
                    }
                }
                list.ForEach(key => items.Remove(key));
            }
        }

        public virtual bool IsEnabled =>
            true;

        protected IDictionary Items
        {
            get
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    return current.Items;
                }
                return null;
            }
        }
    }
}

