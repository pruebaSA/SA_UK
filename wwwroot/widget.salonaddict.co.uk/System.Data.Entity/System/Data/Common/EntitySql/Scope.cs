namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal sealed class Scope : IEnumerable<KeyValuePair<string, ScopeEntry>>, IEnumerable
    {
        private Dictionary<string, ScopeEntry> scopeEntries;

        internal Scope(IEqualityComparer<string> keyComparer)
        {
            this.scopeEntries = new Dictionary<string, ScopeEntry>(keyComparer);
        }

        internal Scope Add(string key, ScopeEntry value)
        {
            this.scopeEntries.Add(key, value);
            return this;
        }

        internal bool Contains(string key) => 
            this.scopeEntries.ContainsKey(key);

        public Dictionary<string, ScopeEntry>.Enumerator GetEnumerator() => 
            this.scopeEntries.GetEnumerator();

        internal void Remove(string key)
        {
            this.scopeEntries.Remove(key);
        }

        IEnumerator<KeyValuePair<string, ScopeEntry>> IEnumerable<KeyValuePair<string, ScopeEntry>>.GetEnumerator() => 
            this.scopeEntries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.scopeEntries.GetEnumerator();

        internal bool TryLookup(string key, out ScopeEntry value) => 
            this.scopeEntries.TryGetValue(key, out value);
    }
}

