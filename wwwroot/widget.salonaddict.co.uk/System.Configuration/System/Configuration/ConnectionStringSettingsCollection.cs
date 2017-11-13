namespace System.Configuration
{
    using System;
    using System.Reflection;

    [ConfigurationCollection(typeof(ConnectionStringSettings))]
    public sealed class ConnectionStringSettingsCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public ConnectionStringSettingsCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(ConnectionStringSettings settings)
        {
            this.BaseAdd(settings);
        }

        protected override void BaseAdd(int index, ConfigurationElement element)
        {
            if (index == -1)
            {
                base.BaseAdd(element, false);
            }
            else
            {
                base.BaseAdd(index, element);
            }
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new ConnectionStringSettings();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((ConnectionStringSettings) element).Key;

        public int IndexOf(ConnectionStringSettings settings) => 
            base.BaseIndexOf(settings);

        public void Remove(ConnectionStringSettings settings)
        {
            if (base.BaseIndexOf(settings) >= 0)
            {
                base.BaseRemove(settings.Key);
            }
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public ConnectionStringSettings this[int index]
        {
            get => 
                ((ConnectionStringSettings) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public ConnectionStringSettings this[string name] =>
            ((ConnectionStringSettings) base.BaseGet(name));

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

