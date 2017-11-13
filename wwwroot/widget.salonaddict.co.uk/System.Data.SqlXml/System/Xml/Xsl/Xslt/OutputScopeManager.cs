namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Runtime.InteropServices;

    internal class OutputScopeManager
    {
        private int lastRecord;
        private int lastScopes;
        private ScopeReord[] records = new ScopeReord[0x20];

        public OutputScopeManager()
        {
            this.Reset();
        }

        public void AddNamespace(string prefix, string uri)
        {
            this.AddRecord(prefix, uri);
        }

        private void AddRecord(string prefix, string uri)
        {
            this.records[this.lastRecord].scopeCount = this.lastScopes;
            this.lastRecord++;
            if (this.lastRecord == this.records.Length)
            {
                ScopeReord[] destinationArray = new ScopeReord[this.lastRecord * 2];
                Array.Copy(this.records, 0, destinationArray, 0, this.lastRecord);
                this.records = destinationArray;
            }
            this.lastScopes = 0;
            this.records[this.lastRecord].prefix = prefix;
            this.records[this.lastRecord].nsUri = uri;
        }

        public void InvalidateAllPrefixes()
        {
            if (this.records[this.lastRecord].prefix != null)
            {
                this.AddRecord(null, null);
            }
        }

        public void InvalidateNonDefaultPrefixes()
        {
            string uri = this.LookupNamespace(string.Empty);
            if (uri == null)
            {
                this.InvalidateAllPrefixes();
            }
            else if ((this.records[this.lastRecord].prefix.Length != 0) || (this.records[this.lastRecord - 1].prefix != null))
            {
                this.AddRecord(null, null);
                this.AddRecord(string.Empty, uri);
            }
        }

        public string LookupNamespace(string prefix)
        {
            for (int i = this.lastRecord; this.records[i].prefix != null; i--)
            {
                if (this.records[i].prefix == prefix)
                {
                    return this.records[i].nsUri;
                }
            }
            return null;
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

        public void Reset()
        {
            this.records[0].prefix = null;
            this.records[0].nsUri = null;
            this.PushScope();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScopeReord
        {
            public int scopeCount;
            public string prefix;
            public string nsUri;
        }
    }
}

