namespace System.Net
{
    using System;
    using System.Collections;

    internal class PrefixLookup
    {
        private Hashtable m_Store = new Hashtable();

        internal void Add(string prefix, object value)
        {
            lock (this.m_Store)
            {
                this.m_Store[prefix] = value;
            }
        }

        internal object Lookup(string lookupKey)
        {
            if (lookupKey == null)
            {
                return null;
            }
            object obj2 = null;
            int num = 0;
            lock (this.m_Store)
            {
                foreach (DictionaryEntry entry in this.m_Store)
                {
                    string key = (string) entry.Key;
                    if (lookupKey.StartsWith(key))
                    {
                        int length = key.Length;
                        if (length > num)
                        {
                            num = length;
                            obj2 = entry.Value;
                        }
                    }
                }
            }
            return obj2;
        }
    }
}

