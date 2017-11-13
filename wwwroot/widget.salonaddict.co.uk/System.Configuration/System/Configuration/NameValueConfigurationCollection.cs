namespace System.Configuration
{
    using System;
    using System.Reflection;

    [ConfigurationCollection(typeof(NameValueConfigurationElement))]
    public sealed class NameValueConfigurationCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(NameValueConfigurationElement nameValue)
        {
            this.BaseAdd(nameValue);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new NameValueConfigurationElement();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((NameValueConfigurationElement) element).Name;

        public void Remove(NameValueConfigurationElement nameValue)
        {
            if (base.BaseIndexOf(nameValue) >= 0)
            {
                base.BaseRemove(nameValue.Name);
            }
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public string[] AllKeys =>
            StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public NameValueConfigurationElement this[string name]
        {
            get => 
                ((NameValueConfigurationElement) base.BaseGet(name));
            set
            {
                int index = -1;
                NameValueConfigurationElement element = (NameValueConfigurationElement) base.BaseGet(name);
                if (element != null)
                {
                    index = base.BaseIndexOf(element);
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

