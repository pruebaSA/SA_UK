namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;

    internal sealed class AliasGenerator
    {
        private string[] _cache;
        private int _counter;
        private static readonly string[] _counterNames = new string[250];
        private readonly string _prefix;
        private static Dictionary<string, string[]> _prefixCounter;
        private const int CacheSize = 250;
        private const int MaxPrefixCount = 500;

        internal AliasGenerator(string prefix) : this(prefix, 250)
        {
        }

        internal AliasGenerator(string prefix, int cacheSize)
        {
            this._prefix = prefix ?? string.Empty;
            if (0 < cacheSize)
            {
                Dictionary<string, string[]> dictionary2;
                string[] strArray = null;
                while (((dictionary2 = _prefixCounter) == null) || !dictionary2.TryGetValue(prefix, out this._cache))
                {
                    if (strArray == null)
                    {
                        strArray = new string[cacheSize];
                    }
                    int capacity = 1 + ((dictionary2 != null) ? dictionary2.Count : 0);
                    Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>(capacity, StringComparer.InvariantCultureIgnoreCase);
                    if ((dictionary2 != null) && (capacity < 500))
                    {
                        foreach (KeyValuePair<string, string[]> pair in dictionary2)
                        {
                            dictionary.Add(pair.Key, pair.Value);
                        }
                    }
                    dictionary.Add(prefix, strArray);
                    Interlocked.CompareExchange<Dictionary<string, string[]>>(ref _prefixCounter, dictionary, dictionary2);
                }
            }
        }

        internal string GetName(int index)
        {
            if ((this._cache == null) || (this._cache.Length <= index))
            {
                return (this._prefix + index.ToString(CultureInfo.InvariantCulture));
            }
            string str = this._cache[index];
            if (str == null)
            {
                if (_counterNames.Length <= index)
                {
                    str = index.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    str = _counterNames[index];
                    if (str == null)
                    {
                        _counterNames[index] = str = index.ToString(CultureInfo.InvariantCulture);
                    }
                }
                this._cache[index] = str = this._prefix + str;
            }
            return str;
        }

        internal string Next()
        {
            this._counter = Math.Max(1 + this._counter, 0);
            return this.GetName(this._counter);
        }
    }
}

