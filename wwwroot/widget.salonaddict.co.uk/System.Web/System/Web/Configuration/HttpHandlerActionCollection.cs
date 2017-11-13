namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(HttpHandlerAction), CollectionType=ConfigurationElementCollectionType.AddRemoveClearMapAlternate), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpHandlerActionCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public HttpHandlerActionCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(HttpHandlerAction httpHandlerAction)
        {
            base.BaseAdd(httpHandlerAction, false);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new HttpHandlerAction();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((HttpHandlerAction) element).Key;

        public int IndexOf(HttpHandlerAction action) => 
            base.BaseIndexOf(action);

        public void Remove(HttpHandlerAction action)
        {
            base.BaseRemove(action.Key);
        }

        public void Remove(string verb, string path)
        {
            base.BaseRemove("verb=" + verb + " | path=" + path);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.AddRemoveClearMapAlternate;

        public HttpHandlerAction this[int index]
        {
            get => 
                ((HttpHandlerAction) base.BaseGet(index));
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

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

