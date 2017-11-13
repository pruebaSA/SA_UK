namespace System.Diagnostics
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Xml;

    internal class SourceElement : ConfigurationElement
    {
        private Hashtable _attributes;
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propListeners = new ConfigurationProperty("listeners", typeof(ListenerElementsCollection), new ListenerElementsCollection(), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propSwitchName = new ConfigurationProperty("switchName", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSwitchType = new ConfigurationProperty("switchType", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSwitchValue = new ConfigurationProperty("switchValue", typeof(string), null, ConfigurationPropertyOptions.None);

        static SourceElement()
        {
            _properties.Add(_propName);
            _properties.Add(_propSwitchName);
            _properties.Add(_propSwitchValue);
            _properties.Add(_propSwitchType);
            _properties.Add(_propListeners);
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
            if (!string.IsNullOrEmpty(this.SwitchName) && !string.IsNullOrEmpty(this.SwitchValue))
            {
                throw new ConfigurationErrorsException(System.SR.GetString("Only_specify_one", new object[] { this.Name }));
            }
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
                _properties.Add(_propSwitchName);
                _properties.Add(_propSwitchValue);
                _properties.Add(_propSwitchType);
                _properties.Add(_propListeners);
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

        [ConfigurationProperty("listeners")]
        public ListenerElementsCollection Listeners =>
            ((ListenerElementsCollection) base[_propListeners]);

        [ConfigurationProperty("name", IsRequired=true, DefaultValue="")]
        public string Name =>
            ((string) base[_propName]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("switchName")]
        public string SwitchName =>
            ((string) base[_propSwitchName]);

        [ConfigurationProperty("switchType")]
        public string SwitchType =>
            ((string) base[_propSwitchType]);

        [ConfigurationProperty("switchValue")]
        public string SwitchValue =>
            ((string) base[_propSwitchValue]);
    }
}

