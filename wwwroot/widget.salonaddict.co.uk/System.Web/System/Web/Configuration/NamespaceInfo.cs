namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class NamespaceInfo : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propNamespace = new ConfigurationProperty("namespace", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

        static NamespaceInfo()
        {
            _properties.Add(_propNamespace);
        }

        internal NamespaceInfo()
        {
        }

        public NamespaceInfo(string name) : this()
        {
            this.Namespace = name;
        }

        public override bool Equals(object namespaceInformation)
        {
            NamespaceInfo info = namespaceInformation as NamespaceInfo;
            return ((info != null) && (this.Namespace == info.Namespace));
        }

        public override int GetHashCode() => 
            this.Namespace.GetHashCode();

        [StringValidator(MinLength=1), ConfigurationProperty("namespace", IsRequired=true, IsKey=true, DefaultValue="")]
        public string Namespace
        {
            get => 
                ((string) base[_propNamespace]);
            set
            {
                base[_propNamespace] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

