namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;
    using System.Xml;

    [ConfigurationCollection(typeof(ProfilePropertySettings)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProfilePropertySettingsCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(ProfilePropertySettings propertySettings)
        {
            this.BaseAdd(propertySettings);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new ProfilePropertySettings();

        public ProfilePropertySettings Get(int index) => 
            ((ProfilePropertySettings) base.BaseGet(index));

        public ProfilePropertySettings Get(string name) => 
            ((ProfilePropertySettings) base.BaseGet(name));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((ProfilePropertySettings) element).Name;

        public string GetKey(int index) => 
            ((string) base.BaseGetKey(index));

        public int IndexOf(ProfilePropertySettings propertySettings) => 
            base.BaseIndexOf(propertySettings);

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (!this.AllowClear && (elementName == "clear"))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Clear_not_valid"), reader);
            }
            if (elementName == "group")
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Nested_group_not_valid"), reader);
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public void Set(ProfilePropertySettings propertySettings)
        {
            base.BaseAdd(propertySettings, false);
        }

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        protected virtual bool AllowClear =>
            false;

        public ProfilePropertySettings this[string name] =>
            ((ProfilePropertySettings) base.BaseGet(name));

        public ProfilePropertySettings this[int index]
        {
            get => 
                ((ProfilePropertySettings) base.BaseGet(index));
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
            true;
    }
}

