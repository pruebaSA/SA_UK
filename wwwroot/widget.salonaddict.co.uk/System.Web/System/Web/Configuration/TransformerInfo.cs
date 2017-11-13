﻿namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TransformerInfo : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);

        static TransformerInfo()
        {
            _properties.Add(_propName);
            _properties.Add(_propType);
        }

        internal TransformerInfo()
        {
        }

        public TransformerInfo(string name, string type) : this()
        {
            this.Name = name;
            this.Type = type;
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            TransformerInfo info = o as TransformerInfo;
            return (System.Web.Util.StringUtil.Equals(this.Name, info.Name) && System.Web.Util.StringUtil.Equals(this.Type, info.Type));
        }

        public override int GetHashCode() => 
            (this.Name.GetHashCode() ^ this.Type.GetHashCode());

        [ConfigurationProperty("name", IsRequired=true, DefaultValue="", IsKey=true), StringValidator(MinLength=1)]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                base[_propName] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("type", IsRequired=true, DefaultValue=""), StringValidator(MinLength=1)]
        public string Type
        {
            get => 
                ((string) base[_propType]);
            set
            {
                base[_propType] = value;
            }
        }
    }
}

