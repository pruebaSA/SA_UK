namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BuildProvider : ConfigurationElement
    {
        private BuildProviderAppliesTo _appliesToInternal;
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propExtension = new ConfigurationProperty("extension", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private System.Type _type;

        static BuildProvider()
        {
            _properties.Add(_propExtension);
            _properties.Add(_propType);
        }

        internal BuildProvider()
        {
        }

        public BuildProvider(string extension, string type) : this()
        {
            this.Extension = extension;
            this.Type = type;
        }

        public override bool Equals(object provider)
        {
            System.Web.Configuration.BuildProvider provider2 = provider as System.Web.Configuration.BuildProvider;
            return (((provider2 != null) && System.Web.Util.StringUtil.EqualsIgnoreCase(this.Extension, provider2.Extension)) && (this.Type == provider2.Type));
        }

        public override int GetHashCode() => 
            HashCodeCombiner.CombineHashCodes(this.Extension.ToLower(CultureInfo.InvariantCulture).GetHashCode(), this.Type.GetHashCode());

        internal BuildProviderAppliesTo AppliesToInternal
        {
            get
            {
                if (this._appliesToInternal == 0)
                {
                    object[] customAttributes = this.TypeInternal.GetCustomAttributes(typeof(BuildProviderAppliesToAttribute), true);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        this._appliesToInternal = ((BuildProviderAppliesToAttribute) customAttributes[0]).AppliesTo;
                    }
                    else
                    {
                        this._appliesToInternal = BuildProviderAppliesTo.All;
                    }
                }
                return this._appliesToInternal;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("extension", IsRequired=true, IsKey=true, DefaultValue="")]
        public string Extension
        {
            get => 
                ((string) base[_propExtension]);
            set
            {
                base[_propExtension] = value;
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

        internal System.Type TypeInternal
        {
            get
            {
                if (this._type == null)
                {
                    lock (this)
                    {
                        if (this._type == null)
                        {
                            this._type = CompilationUtil.LoadTypeWithChecks(this.Type, typeof(System.Web.Compilation.BuildProvider), null, this, "type");
                        }
                    }
                }
                return this._type;
            }
        }
    }
}

