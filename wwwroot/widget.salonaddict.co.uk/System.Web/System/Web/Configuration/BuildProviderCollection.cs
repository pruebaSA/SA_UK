namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(BuildProvider)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BuildProviderCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public BuildProviderCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(BuildProvider buildProvider)
        {
            this.BaseAdd(buildProvider);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new BuildProvider();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((BuildProvider) element).Extension;

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public BuildProvider this[string name] =>
            ((BuildProvider) base.BaseGet(name));

        public BuildProvider this[int index]
        {
            get => 
                ((BuildProvider) base.BaseGet(index));
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

