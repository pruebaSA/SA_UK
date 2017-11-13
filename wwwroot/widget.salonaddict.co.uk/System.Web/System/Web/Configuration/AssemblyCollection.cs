namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(AssemblyInfo)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AssemblyCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(AssemblyInfo assemblyInformation)
        {
            this.BaseAdd(assemblyInformation);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new AssemblyInfo();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((AssemblyInfo) element).Assembly;

        internal bool IsRemoved(string key) => 
            base.BaseIsRemoved(key);

        public void Remove(string key)
        {
            base.BaseRemove(key);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public AssemblyInfo this[int index]
        {
            get => 
                ((AssemblyInfo) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public AssemblyInfo this[string assemblyName] =>
            ((AssemblyInfo) base.BaseGet(assemblyName));

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

