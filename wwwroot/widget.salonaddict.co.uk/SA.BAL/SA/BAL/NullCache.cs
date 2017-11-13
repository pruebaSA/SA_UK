namespace SA.BAL
{
    using System;

    public sealed class NullCache : ICacheManager
    {
        public void Add(string key, object obj)
        {
        }

        public object Get(string key) => 
            null;

        public void Remove(string key)
        {
        }

        public void RemoveByPattern(string pattern)
        {
        }

        public bool IsEnabled =>
            true;
    }
}

