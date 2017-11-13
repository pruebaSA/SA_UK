namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfilePropertySettings : ConfigurationElement
    {
        private static readonly ConfigurationProperty _propAllowAnonymous = new ConfigurationProperty("allowAnonymous", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCustomProviderData = new ConfigurationProperty("customProviderData", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultValue = new ConfigurationProperty("defaultValue", typeof(string), "", ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, ProfilePropertyNameValidator.SingletonInstance, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propProviderName = new ConfigurationProperty("provider", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propReadOnly = new ConfigurationProperty("readOnly", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSerializeAs = new ConfigurationProperty("serializeAs", typeof(SerializationMode), SerializationMode.ProviderSpecific, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), "string", ConfigurationPropertyOptions.None);
        private SettingsProvider _providerInternal;
        private System.Type _type;

        static ProfilePropertySettings()
        {
            _properties.Add(_propName);
            _properties.Add(_propReadOnly);
            _properties.Add(_propSerializeAs);
            _properties.Add(_propProviderName);
            _properties.Add(_propDefaultValue);
            _properties.Add(_propType);
            _properties.Add(_propAllowAnonymous);
            _properties.Add(_propCustomProviderData);
        }

        internal ProfilePropertySettings()
        {
        }

        public ProfilePropertySettings(string name)
        {
            this.Name = name;
        }

        public ProfilePropertySettings(string name, bool readOnly, SerializationMode serializeAs, string providerName, string defaultValue, string profileType, bool allowAnonymous, string customProviderData)
        {
            this.Name = name;
            this.ReadOnly = readOnly;
            this.SerializeAs = serializeAs;
            this.Provider = providerName;
            this.DefaultValue = defaultValue;
            this.Type = profileType;
            this.AllowAnonymous = allowAnonymous;
            this.CustomProviderData = customProviderData;
        }

        [ConfigurationProperty("allowAnonymous", DefaultValue=false)]
        public bool AllowAnonymous
        {
            get => 
                ((bool) base[_propAllowAnonymous]);
            set
            {
                base[_propAllowAnonymous] = value;
            }
        }

        [ConfigurationProperty("customProviderData", DefaultValue="")]
        public string CustomProviderData
        {
            get => 
                ((string) base[_propCustomProviderData]);
            set
            {
                base[_propCustomProviderData] = value;
            }
        }

        [ConfigurationProperty("defaultValue", DefaultValue="")]
        public string DefaultValue
        {
            get => 
                ((string) base[_propDefaultValue]);
            set
            {
                base[_propDefaultValue] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired=true, IsKey=true)]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                base[_propName] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("provider", DefaultValue="")]
        public string Provider
        {
            get => 
                ((string) base[_propProviderName]);
            set
            {
                base[_propProviderName] = value;
            }
        }

        internal SettingsProvider ProviderInternal
        {
            get => 
                this._providerInternal;
            set
            {
                this._providerInternal = value;
            }
        }

        [ConfigurationProperty("readOnly", DefaultValue=false)]
        public bool ReadOnly
        {
            get => 
                ((bool) base[_propReadOnly]);
            set
            {
                base[_propReadOnly] = value;
            }
        }

        [ConfigurationProperty("serializeAs", DefaultValue=3)]
        public SerializationMode SerializeAs
        {
            get => 
                ((SerializationMode) base[_propSerializeAs]);
            set
            {
                base[_propSerializeAs] = value;
            }
        }

        [ConfigurationProperty("type", DefaultValue="string")]
        public string Type
        {
            get => 
                ((string) base[_propType]);
            set
            {
                base[_propType] = value;
            }
        }

        internal System.Type TypeInternal
        {
            get => 
                this._type;
            set
            {
                this._type = value;
            }
        }
    }
}

