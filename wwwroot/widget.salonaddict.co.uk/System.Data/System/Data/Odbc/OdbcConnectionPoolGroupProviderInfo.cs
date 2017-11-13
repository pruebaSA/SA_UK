namespace System.Data.Odbc
{
    using System;
    using System.Data.ProviderBase;

    internal sealed class OdbcConnectionPoolGroupProviderInfo : DbConnectionPoolGroupProviderInfo
    {
        private string _driverName;
        private string _driverVersion;
        private char _escapeChar;
        private bool _hasEscapeChar;
        private bool _hasQuoteChar;
        private bool _isV3Driver;
        private bool _noConnectionDead;
        private bool _noCurrentCatalog;
        private bool _noQueryTimeout;
        private bool _noSqlCASSColumnKey;
        private bool _noSqlPrimaryKeys;
        private bool _noSqlSoptSSHiddenColumns;
        private bool _noSqlSoptSSNoBrowseTable;
        private string _quoteChar;
        private int _restrictedSQLBindTypes;
        private int _supportedSQLTypes;
        private int _testedSQLTypes;

        internal string DriverName
        {
            get => 
                this._driverName;
            set
            {
                this._driverName = value;
            }
        }

        internal string DriverVersion
        {
            get => 
                this._driverVersion;
            set
            {
                this._driverVersion = value;
            }
        }

        internal char EscapeChar
        {
            get => 
                this._escapeChar;
            set
            {
                this._escapeChar = value;
                this._hasEscapeChar = true;
            }
        }

        internal bool HasEscapeChar =>
            this._hasEscapeChar;

        internal bool HasQuoteChar =>
            this._hasQuoteChar;

        internal bool IsV3Driver
        {
            get => 
                this._isV3Driver;
            set
            {
                this._isV3Driver = value;
            }
        }

        internal bool NoConnectionDead
        {
            get => 
                this._noConnectionDead;
            set
            {
                this._noConnectionDead = value;
            }
        }

        internal bool NoCurrentCatalog
        {
            get => 
                this._noCurrentCatalog;
            set
            {
                this._noCurrentCatalog = value;
            }
        }

        internal bool NoQueryTimeout
        {
            get => 
                this._noQueryTimeout;
            set
            {
                this._noQueryTimeout = value;
            }
        }

        internal bool NoSqlCASSColumnKey
        {
            get => 
                this._noSqlCASSColumnKey;
            set
            {
                this._noSqlCASSColumnKey = value;
            }
        }

        internal bool NoSqlPrimaryKeys
        {
            get => 
                this._noSqlPrimaryKeys;
            set
            {
                this._noSqlPrimaryKeys = value;
            }
        }

        internal bool NoSqlSoptSSHiddenColumns
        {
            get => 
                this._noSqlSoptSSHiddenColumns;
            set
            {
                this._noSqlSoptSSHiddenColumns = value;
            }
        }

        internal bool NoSqlSoptSSNoBrowseTable
        {
            get => 
                this._noSqlSoptSSNoBrowseTable;
            set
            {
                this._noSqlSoptSSNoBrowseTable = value;
            }
        }

        internal string QuoteChar
        {
            get => 
                this._quoteChar;
            set
            {
                this._quoteChar = value;
                this._hasQuoteChar = true;
            }
        }

        internal int RestrictedSQLBindTypes
        {
            get => 
                this._restrictedSQLBindTypes;
            set
            {
                this._restrictedSQLBindTypes = value;
            }
        }

        internal int SupportedSQLTypes
        {
            get => 
                this._supportedSQLTypes;
            set
            {
                this._supportedSQLTypes = value;
            }
        }

        internal int TestedSQLTypes
        {
            get => 
                this._testedSQLTypes;
            set
            {
                this._testedSQLTypes = value;
            }
        }
    }
}

