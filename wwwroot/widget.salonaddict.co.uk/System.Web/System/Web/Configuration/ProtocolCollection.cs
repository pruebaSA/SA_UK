﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(ProtocolElement)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProtocolCollection : ConfigurationElementCollection
    {
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(ProtocolElement protocolElement)
        {
            this.BaseAdd(protocolElement);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new ProtocolElement();

        protected override object GetElementKey(ConfigurationElement element)
        {
            string name = ((ProtocolElement) element).Name;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(System.Web.SR.GetString("Config_collection_add_element_without_key"));
            }
            return name;
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(ProtocolElement protocolElement)
        {
            base.BaseRemove(this.GetElementKey(protocolElement));
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public string[] AllKeys =>
            ((string[]) base.BaseGetAllKeys());

        public ProtocolElement this[string name] =>
            ((ProtocolElement) base.BaseGet(name));

        public ProtocolElement this[int index]
        {
            get => 
                ((ProtocolElement) base.BaseGet(index));
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

