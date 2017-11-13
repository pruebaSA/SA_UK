﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(ProfileSettings)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileSettingsCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(ProfileSettings profilesSettings)
        {
            this.BaseAdd(profilesSettings);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public bool Contains(string name) => 
            (this.IndexOf(name) != -1);

        protected override ConfigurationElement CreateNewElement() => 
            new ProfileSettings();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((ProfileSettings) element).Name;

        public int IndexOf(string name)
        {
            ConfigurationElement element = base.BaseGet(name);
            if (element == null)
            {
                return -1;
            }
            return base.BaseIndexOf(element);
        }

        public void Insert(int index, ProfileSettings authorizationSettings)
        {
            this.BaseAdd(index, authorizationSettings);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public ProfileSettings this[int index]
        {
            get => 
                ((ProfileSettings) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public ProfileSettings this[string key] =>
            ((ProfileSettings) base.BaseGet(key));

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

