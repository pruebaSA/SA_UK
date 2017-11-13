namespace SA.BAL
{
    using System;

    public interface ICacheManager
    {
        void Add(string key, object obj);
        object Get(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);

        bool IsEnabled { get; }
    }
}

