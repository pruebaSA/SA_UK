namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml.Xsl.Qil;

    internal sealed class CompilerScopeManager<V> : IEnumerable where V: class
    {
        private const int LastPredefRecord = 0;
        private int lastRecord;
        private int lastScopes;
        private ScopeRecord<V>[] records;

        public CompilerScopeManager()
        {
            this.records = new ScopeRecord<V>[0x20];
            this.Reset();
        }

        public void AddNamespace(string prefix, string uri)
        {
            this.AddRecord(prefix, uri, default(V));
        }

        private void AddRecord(string ncName, string uri, V value)
        {
            this.records[this.lastRecord].scopeCount = this.lastScopes;
            if (++this.lastRecord == this.records.Length)
            {
                ScopeRecord<V>[] destinationArray = new ScopeRecord<V>[this.lastRecord * 2];
                Array.Copy(this.records, 0, destinationArray, 0, this.lastRecord);
                this.records = destinationArray;
            }
            this.lastScopes = 0;
            this.records[this.lastRecord].ncName = ncName;
            this.records[this.lastRecord].nsUri = uri;
            this.records[this.lastRecord].value = value;
        }

        public void AddVariable(QilName varName, V value)
        {
            this.AddRecord(varName.LocalName, varName.NamespaceUri, value);
        }

        public bool IsExNamespace(string nsUri)
        {
            for (int i = this.lastRecord; 0 <= i; i--)
            {
                if (this.records[i].IsExNamespace && (this.records[i].nsUri == nsUri))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLocalVariable(string localName, string uri)
        {
            int index = this.SearchVariable(localName, uri);
            while (0 <= --index)
            {
                if (this.records[index].scopeCount != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public string LookupNamespace(string prefix) => 
            this.LookupNamespace(prefix, this.lastRecord, 0);

        private string LookupNamespace(string prefix, int from, int to)
        {
            for (int i = from; to <= i; i--)
            {
                if (this.records[i].IsNamespace && (this.records[i].ncName == prefix))
                {
                    return this.records[i].nsUri;
                }
            }
            return null;
        }

        public V LookupVariable(string localName, string uri)
        {
            int index = this.SearchVariable(localName, uri);
            if (index >= 0)
            {
                return this.records[index].value;
            }
            return default(V);
        }

        public void PopScope()
        {
            if (0 < this.lastScopes)
            {
                this.lastScopes--;
            }
            else
            {
                while (this.records[--this.lastRecord].scopeCount == 0)
                {
                }
                this.lastScopes = this.records[this.lastRecord].scopeCount;
                this.lastScopes--;
            }
        }

        public void PushScope()
        {
            this.lastScopes++;
        }

        private void Reset()
        {
            this.records[0].ncName = "xml";
            this.records[0].nsUri = "http://www.w3.org/XML/1998/namespace";
            this.lastRecord = 0;
        }

        private int SearchVariable(string localName, string uri)
        {
            for (int i = this.lastRecord; 0 <= i; i--)
            {
                if ((this.records[i].IsVariable && (this.records[i].ncName == localName)) && (this.records[i].nsUri == uri))
                {
                    return i;
                }
            }
            return -1;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new NamespaceEnumerator<V>((CompilerScopeManager<V>) this);

        private sealed class NamespaceEnumerator : IEnumerator
        {
            private int currentRecord;
            private int lastRecord;
            private CompilerScopeManager<V> scope;

            public NamespaceEnumerator(CompilerScopeManager<V> scope)
            {
                this.scope = scope;
                this.lastRecord = scope.lastRecord;
                this.Reset();
            }

            public bool MoveNext()
            {
                while (0 < --this.currentRecord)
                {
                    if (this.scope.records[this.currentRecord].IsNamespace && (this.scope.LookupNamespace(this.scope.records[this.currentRecord].ncName, this.lastRecord, this.currentRecord + 1) == null))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                this.currentRecord = this.lastRecord + 1;
            }

            public object Current =>
                this.scope.records[this.currentRecord];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScopeRecord
        {
            public int scopeCount;
            public string ncName;
            public string nsUri;
            public V value;
            public bool IsVariable =>
                (this.value != default(V));
            public bool IsNamespace =>
                ((this.value == default(V)) && (this.ncName != null));
            public bool IsExNamespace =>
                ((this.value == default(V)) && (this.ncName == null));
        }
    }
}

