namespace System.Configuration
{
    using System;

    public class KeyValueConfigurationElement : ConfigurationElement
    {
        private string _initKey;
        private string _initValue;
        private bool _needsInit;
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propKey = new ConfigurationProperty("key", typeof(string), string.Empty, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propValue = new ConfigurationProperty("value", typeof(string), string.Empty, ConfigurationPropertyOptions.None);

        static KeyValueConfigurationElement()
        {
            _properties.Add(_propKey);
            _properties.Add(_propValue);
        }

        internal KeyValueConfigurationElement()
        {
        }

        public KeyValueConfigurationElement(string key, string value)
        {
            this._needsInit = true;
            this._initKey = key;
            this._initValue = value;
        }

        protected internal override void Init()
        {
            base.Init();
            if (this._needsInit)
            {
                this._needsInit = false;
                base[_propKey] = this._initKey;
                this.Value = this._initValue;
            }
        }

        [ConfigurationProperty("key", Options=ConfigurationPropertyOptions.IsKey, DefaultValue="")]
        public string Key =>
            ((string) base[_propKey]);

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("value", DefaultValue="")]
        public string Value
        {
            get => 
                ((string) base[_propValue]);
            set
            {
                base[_propValue] = value;
            }
        }
    }
}

