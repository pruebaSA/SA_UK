namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(TagPrefixInfo), AddItemName="add", CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TagPrefixCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public TagPrefixCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(TagPrefixInfo tagPrefixInformation)
        {
            this.BaseAdd(tagPrefixInformation);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new TagPrefixInfo();

        protected override object GetElementKey(ConfigurationElement element)
        {
            TagPrefixInfo info = (TagPrefixInfo) element;
            if (string.IsNullOrEmpty(info.TagName))
            {
                return (info.TagPrefix + ":" + info.Namespace + ":" + (string.IsNullOrEmpty(info.Assembly) ? string.Empty : info.Assembly));
            }
            return (info.TagPrefix + ":" + info.TagName);
        }

        public void Remove(TagPrefixInfo tagPrefixInformation)
        {
            base.BaseRemove(this.GetElementKey(tagPrefixInformation));
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "add";

        public TagPrefixInfo this[int index]
        {
            get => 
                ((TagPrefixInfo) base.BaseGet(index));
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

