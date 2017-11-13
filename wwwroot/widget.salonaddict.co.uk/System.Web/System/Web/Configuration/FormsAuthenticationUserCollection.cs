namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [ConfigurationCollection(typeof(FormsAuthenticationUser), AddItemName="user", CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FormsAuthenticationUserCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(FormsAuthenticationUser user)
        {
            this.BaseAdd(user);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new FormsAuthenticationUser();

        public FormsAuthenticationUser Get(int index) => 
            ((FormsAuthenticationUser) base.BaseGet(index));

        public FormsAuthenticationUser Get(string name) => 
            ((FormsAuthenticationUser) base.BaseGet(name));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((FormsAuthenticationUser) element).Name;

        public string GetKey(int index) => 
            ((string) base.BaseGetKey(index));

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public void Set(FormsAuthenticationUser user)
        {
            base.BaseAdd(user, false);
        }

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "user";

        public FormsAuthenticationUser this[string name] =>
            ((FormsAuthenticationUser) base.BaseGet(name));

        public FormsAuthenticationUser this[int index]
        {
            get => 
                ((FormsAuthenticationUser) base.BaseGet(index));
            set
            {
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        protected override bool ThrowOnDuplicate =>
            true;
    }
}

