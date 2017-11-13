namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(BufferModeSettings)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BufferModesCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(BufferModeSettings bufferModeSettings)
        {
            this.BaseAdd(bufferModeSettings);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new BufferModeSettings();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((BufferModeSettings) element).Name;

        public void Remove(string s)
        {
            base.BaseRemove(s);
        }

        public BufferModeSettings this[string key] =>
            ((BufferModeSettings) base.BaseGet(key));

        public BufferModeSettings this[int index]
        {
            get => 
                ((BufferModeSettings) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

