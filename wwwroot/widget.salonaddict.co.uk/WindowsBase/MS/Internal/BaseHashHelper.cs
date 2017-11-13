namespace MS.Internal
{
    using MS.Internal.Hashing.WindowsBase;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Specialized;
    using System.Reflection;

    [FriendAccessAllowed]
    internal static class BaseHashHelper
    {
        private static HybridDictionary _table = new HybridDictionary(3);

        static BaseHashHelper()
        {
            HashHelper.Initialize();
        }

        private static HybridDictionary DictionaryFromList(Type[] types)
        {
            HybridDictionary dictionary = new HybridDictionary(types.Length);
            for (int i = 0; i < types.Length; i++)
            {
                dictionary.Add(types[i], null);
            }
            return dictionary;
        }

        [FriendAccessAllowed]
        internal static bool HasReliableHashCode(object item)
        {
            HybridDictionary dictionary;
            if (item == null)
            {
                return false;
            }
            Type key = item.GetType();
            Assembly assembly = key.Assembly;
            lock (_table)
            {
                dictionary = (HybridDictionary) _table[assembly];
            }
            if (dictionary == null)
            {
                dictionary = new HybridDictionary();
                lock (_table)
                {
                    _table[assembly] = dictionary;
                }
            }
            return !dictionary.Contains(key);
        }

        [FriendAccessAllowed]
        internal static void RegisterTypes(Assembly assembly, Type[] types)
        {
            HybridDictionary dictionary = DictionaryFromList(types);
            lock (_table)
            {
                _table[assembly] = dictionary;
            }
        }
    }
}

