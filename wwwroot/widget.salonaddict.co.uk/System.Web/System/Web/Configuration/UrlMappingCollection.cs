﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [ConfigurationCollection(typeof(UrlMapping)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class UrlMappingCollection : ConfigurationElementCollection
    {
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public UrlMappingCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(UrlMapping urlMapping)
        {
            this.BaseAdd(urlMapping);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new UrlMapping();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((UrlMapping) element).Url;

        public string GetKey(int index) => 
            ((string) base.BaseGetKey(index));

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(UrlMapping urlMapping)
        {
            base.BaseRemove(this.GetElementKey(urlMapping));
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public UrlMapping this[string name] =>
            ((UrlMapping) base.BaseGet(name));

        public UrlMapping this[int index]
        {
            get => 
                ((UrlMapping) base.BaseGet(index));
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

