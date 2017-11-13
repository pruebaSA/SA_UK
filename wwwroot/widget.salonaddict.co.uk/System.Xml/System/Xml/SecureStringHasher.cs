namespace System.Xml
{
    using System;
    using System.Collections.Generic;

    internal class SecureStringHasher : IEqualityComparer<string>
    {
        private int hashCodeRandomizer;

        public SecureStringHasher()
        {
            this.hashCodeRandomizer = Environment.TickCount;
        }

        public SecureStringHasher(int hashCodeRandomizer)
        {
            this.hashCodeRandomizer = hashCodeRandomizer;
        }

        public int Compare(string x, string y) => 
            string.Compare(x, y, StringComparison.Ordinal);

        public bool Equals(string x, string y) => 
            string.Equals(x, y, StringComparison.Ordinal);

        public int GetHashCode(string key)
        {
            int hashCodeRandomizer = this.hashCodeRandomizer;
            for (int i = 0; i < key.Length; i++)
            {
                hashCodeRandomizer += (hashCodeRandomizer << 7) ^ key[i];
            }
            hashCodeRandomizer -= hashCodeRandomizer >> 0x11;
            hashCodeRandomizer -= hashCodeRandomizer >> 11;
            return (hashCodeRandomizer - (hashCodeRandomizer >> 5));
        }
    }
}

