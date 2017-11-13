namespace System.Configuration
{
    using System;
    using System.Reflection;

    [ConfigurationCollection(typeof(KeyValueConfigurationElement))]
    public class KeyValueConfigurationCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public KeyValueConfigurationCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
            base.internalAddToEnd = true;
        }

        public void Add(KeyValueConfigurationElement keyValue)
        {
            keyValue.Init();
            KeyValueConfigurationElement element = (KeyValueConfigurationElement) base.BaseGet(keyValue.Key);
            if (element == null)
            {
                this.BaseAdd(keyValue);
            }
            else
            {
                element.Value = element.Value + "," + keyValue.Value;
                int index = base.BaseIndexOf(element);
                base.BaseRemoveAt(index);
                this.BaseAdd(index, element);
            }
        }

        public void Add(string key, string value)
        {
            KeyValueConfigurationElement keyValue = new KeyValueConfigurationElement(key, value);
            this.Add(keyValue);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new KeyValueConfigurationElement();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((KeyValueConfigurationElement) element).Key;

        public void Remove(string key)
        {
            base.BaseRemove(key);
        }

        public string[] AllKeys =>
            StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public KeyValueConfigurationElement this[string key] =>
            ((KeyValueConfigurationElement) base.BaseGet(key));

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

