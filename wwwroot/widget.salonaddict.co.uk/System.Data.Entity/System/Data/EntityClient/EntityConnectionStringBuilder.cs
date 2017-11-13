namespace System.Data.EntityClient
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class EntityConnectionStringBuilder : DbConnectionStringBuilder
    {
        private string _metadataLocations;
        private string _namedConnectionName;
        private string _providerName;
        private string _storeProviderConnectionString;
        internal const string MetadataParameterName = "metadata";
        internal const string NameParameterName = "name";
        internal const string ProviderConnectionStringParameterName = "provider connection string";
        internal const string ProviderParameterName = "provider";
        private static Hashtable s_synonyms;
        private static readonly string[] s_validKeywords = new string[] { "name", "metadata", "provider", "provider connection string" };

        public EntityConnectionStringBuilder()
        {
        }

        public EntityConnectionStringBuilder(string connectionString)
        {
            base.ConnectionString = connectionString;
        }

        public override void Clear()
        {
            base.Clear();
            this._namedConnectionName = null;
            this._providerName = null;
            this._metadataLocations = null;
            this._storeProviderConnectionString = null;
        }

        public override bool ContainsKey(string keyword)
        {
            EntityUtil.CheckArgumentNull<string>(keyword, "keyword");
            foreach (string str in s_validKeywords)
            {
                if (str.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Remove(string keyword)
        {
            EntityUtil.CheckArgumentNull<string>(keyword, "keyword");
            if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this._metadataLocations = null;
            }
            else if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this._storeProviderConnectionString = null;
            }
            else if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this._namedConnectionName = null;
            }
            else if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this._providerName = null;
            }
            return base.Remove(keyword);
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            EntityUtil.CheckArgumentNull<string>(keyword, "keyword");
            if (this.ContainsKey(keyword))
            {
                value = this[keyword];
                return true;
            }
            value = null;
            return false;
        }

        public override bool IsFixedSize =>
            true;

        public override object this[string keyword]
        {
            get
            {
                EntityUtil.CheckArgumentNull<string>(keyword, "keyword");
                if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return this.Metadata;
                }
                if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return this.ProviderConnectionString;
                }
                if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return this.Name;
                }
                if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw EntityUtil.KeywordNotSupported(keyword);
                }
                return this.Provider;
            }
            set
            {
                EntityUtil.CheckArgumentNull<string>(keyword, "keyword");
                if (value == null)
                {
                    this.Remove(keyword);
                }
                else
                {
                    string str = value as string;
                    if (str == null)
                    {
                        throw EntityUtil.Argument(Strings.EntityClient_ValueNotString, "value");
                    }
                    if (string.Compare(keyword, "metadata", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.Metadata = str;
                    }
                    else if (string.Compare(keyword, "provider connection string", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.ProviderConnectionString = str;
                    }
                    else if (string.Compare(keyword, "name", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.Name = str;
                    }
                    else
                    {
                        if (string.Compare(keyword, "provider", StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw EntityUtil.KeywordNotSupported(keyword);
                        }
                        this.Provider = str;
                    }
                }
            }
        }

        public override ICollection Keys =>
            new System.Collections.ObjectModel.ReadOnlyCollection<string>(s_validKeywords);

        [RefreshProperties(RefreshProperties.All), EntityResDescription("EntityConnectionString_Metadata"), EntityResCategory("EntityDataCategory_Context"), DisplayName("Metadata")]
        public string Metadata
        {
            get => 
                (this._metadataLocations ?? "");
            set
            {
                this._metadataLocations = value;
                base["metadata"] = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), EntityResDescription("EntityConnectionString_Name"), DisplayName("Name"), EntityResCategory("EntityDataCategory_NamedConnectionString")]
        public string Name
        {
            get => 
                (this._namedConnectionName ?? "");
            set
            {
                this._namedConnectionName = value;
                base["name"] = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), DisplayName("Provider"), EntityResCategory("EntityDataCategory_Source"), EntityResDescription("EntityConnectionString_Provider")]
        public string Provider
        {
            get => 
                (this._providerName ?? "");
            set
            {
                this._providerName = value;
                base["provider"] = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), EntityResDescription("EntityConnectionString_ProviderConnectionString"), DisplayName("Provider Connection String"), EntityResCategory("EntityDataCategory_Source")]
        public string ProviderConnectionString
        {
            get => 
                (this._storeProviderConnectionString ?? "");
            set
            {
                this._storeProviderConnectionString = value;
                base["provider connection string"] = value;
            }
        }

        internal static Hashtable Synonyms
        {
            get
            {
                if (s_synonyms == null)
                {
                    Hashtable hashtable = new Hashtable(s_validKeywords.Length);
                    foreach (string str in s_validKeywords)
                    {
                        hashtable.Add(str, str);
                    }
                    s_synonyms = hashtable;
                }
                return s_synonyms;
            }
        }
    }
}

