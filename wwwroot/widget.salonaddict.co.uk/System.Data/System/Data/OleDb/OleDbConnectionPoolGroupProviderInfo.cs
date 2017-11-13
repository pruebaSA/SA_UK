namespace System.Data.OleDb
{
    using System;
    using System.Data.ProviderBase;

    internal sealed class OleDbConnectionPoolGroupProviderInfo : DbConnectionPoolGroupProviderInfo
    {
        private bool _hasQuoteFix;
        private string _quotePrefix;
        private string _quoteSuffix;

        internal OleDbConnectionPoolGroupProviderInfo()
        {
        }

        internal void SetQuoteFix(string prefix, string suffix)
        {
            this._quotePrefix = prefix;
            this._quoteSuffix = suffix;
            this._hasQuoteFix = true;
        }

        internal bool HasQuoteFix =>
            this._hasQuoteFix;

        internal string QuotePrefix =>
            this._quotePrefix;

        internal string QuoteSuffix =>
            this._quoteSuffix;
    }
}

