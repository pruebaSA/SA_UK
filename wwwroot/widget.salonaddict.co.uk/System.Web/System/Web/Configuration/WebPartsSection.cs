namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebPartsSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propEnableExport = new ConfigurationProperty("enableExport", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propPersonalization = new ConfigurationProperty("personalization", typeof(WebPartsPersonalization), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propTransformers = new ConfigurationProperty("transformers", typeof(TransformerInfoCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        static WebPartsSection()
        {
            _properties.Add(_propEnableExport);
            _properties.Add(_propPersonalization);
            _properties.Add(_propTransformers);
        }

        protected override object GetRuntimeObject()
        {
            this.Personalization.ValidateAuthorization();
            return base.GetRuntimeObject();
        }

        [ConfigurationProperty("enableExport", DefaultValue=false)]
        public bool EnableExport
        {
            get => 
                ((bool) base[_propEnableExport]);
            set
            {
                base[_propEnableExport] = value;
            }
        }

        [ConfigurationProperty("personalization")]
        public WebPartsPersonalization Personalization =>
            ((WebPartsPersonalization) base[_propPersonalization]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("transformers")]
        public TransformerInfoCollection Transformers =>
            ((TransformerInfoCollection) base[_propTransformers]);
    }
}

