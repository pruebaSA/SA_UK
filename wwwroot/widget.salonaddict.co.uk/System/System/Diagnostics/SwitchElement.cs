namespace System.Diagnostics
{
    using System;
    using System.Collections;
    using System.Configuration;

    internal class SwitchElement : ConfigurationElement
    {
        private Hashtable _attributes;
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), "", ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propValue = new ConfigurationProperty("value", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        static SwitchElement()
        {
            _properties.Add(_propName);
            _properties.Add(_propValue);
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);
            _properties.Add(property);
            base[property] = value;
            this.Attributes.Add(name, value);
            return true;
        }

        internal void ResetProperties()
        {
            if (this._attributes != null)
            {
                this._attributes.Clear();
                _properties.Clear();
                _properties.Add(_propName);
                _properties.Add(_propValue);
            }
        }

        public Hashtable Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new Hashtable(StringComparer.OrdinalIgnoreCase);
                }
                return this._attributes;
            }
        }

        [ConfigurationProperty("name", DefaultValue="", IsRequired=true, IsKey=true)]
        public string Name =>
            ((string) base[_propName]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("value", IsRequired=true)]
        public string Value =>
            ((string) base[_propValue]);
    }
}

