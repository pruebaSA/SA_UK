﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(TrustLevel), AddItemName="trustLevel", CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TrustLevelCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(TrustLevel trustLevel)
        {
            this.BaseAdd(trustLevel);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new TrustLevel();

        public TrustLevel Get(int index) => 
            ((TrustLevel) base.BaseGet(index));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((TrustLevel) element).Name;

        protected override bool IsElementName(string elementname)
        {
            string str;
            bool flag = false;
            if (((str = elementname) != null) && (str == "trustLevel"))
            {
                flag = true;
            }
            return flag;
        }

        public void Remove(TrustLevel trustLevel)
        {
            base.BaseRemove(this.GetElementKey(trustLevel));
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public void Set(int index, TrustLevel trustLevel)
        {
            this.BaseAdd(index, trustLevel);
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "trustLevel";

        public TrustLevel this[int index]
        {
            get => 
                ((TrustLevel) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public TrustLevel this[string key] =>
            ((TrustLevel) base.BaseGet(key));

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        protected override bool ThrowOnDuplicate =>
            true;
    }
}

