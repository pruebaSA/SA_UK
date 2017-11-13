namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [ConfigurationCollection(typeof(SqlCacheDependencyDatabase)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SqlCacheDependencyDatabaseCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(SqlCacheDependencyDatabase name)
        {
            this.BaseAdd(name);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new SqlCacheDependencyDatabase();

        public SqlCacheDependencyDatabase Get(int index) => 
            ((SqlCacheDependencyDatabase) base.BaseGet(index));

        public SqlCacheDependencyDatabase Get(string name) => 
            ((SqlCacheDependencyDatabase) base.BaseGet(name));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((SqlCacheDependencyDatabase) element).Name;

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

        public void Set(SqlCacheDependencyDatabase user)
        {
            base.BaseAdd(user, false);
        }

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public SqlCacheDependencyDatabase this[string name] =>
            ((SqlCacheDependencyDatabase) base.BaseGet(name));

        public SqlCacheDependencyDatabase this[int index]
        {
            get => 
                ((SqlCacheDependencyDatabase) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }
    }
}

