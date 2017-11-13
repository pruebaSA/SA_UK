﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [ConfigurationCollection(typeof(OutputCacheProfile)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class OutputCacheProfileCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public OutputCacheProfileCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(OutputCacheProfile name)
        {
            this.BaseAdd(name);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new OutputCacheProfile();

        public OutputCacheProfile Get(int index) => 
            ((OutputCacheProfile) base.BaseGet(index));

        public OutputCacheProfile Get(string name) => 
            ((OutputCacheProfile) base.BaseGet(name));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((OutputCacheProfile) element).Name;

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

        public void Set(OutputCacheProfile user)
        {
            base.BaseAdd(user, false);
        }

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public OutputCacheProfile this[string name] =>
            ((OutputCacheProfile) base.BaseGet(name));

        public OutputCacheProfile this[int index]
        {
            get => 
                ((OutputCacheProfile) base.BaseGet(index));
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

