namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class ServiceMetadataPublishingElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceMetadataPublishingElement element = (ServiceMetadataPublishingElement) from;
            this.HttpGetEnabled = element.HttpGetEnabled;
            this.HttpGetUrl = element.HttpGetUrl;
            this.HttpsGetEnabled = element.HttpsGetEnabled;
            this.HttpsGetUrl = element.HttpsGetUrl;
            this.ExternalMetadataLocation = element.ExternalMetadataLocation;
            this.PolicyVersion = element.PolicyVersion;
            this.HttpGetBinding = element.HttpGetBinding;
            this.HttpGetBindingConfiguration = element.HttpGetBindingConfiguration;
            this.HttpsGetBinding = element.HttpsGetBinding;
            this.HttpsGetBindingConfiguration = element.HttpsGetBindingConfiguration;
        }

        protected internal override object CreateBehavior()
        {
            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior {
                HttpGetEnabled = this.HttpGetEnabled,
                HttpGetUrl = this.HttpGetUrl,
                HttpsGetEnabled = this.HttpsGetEnabled,
                HttpsGetUrl = this.HttpsGetUrl,
                ExternalMetadataLocation = this.ExternalMetadataLocation,
                MetadataExporter = { PolicyVersion = this.PolicyVersion }
            };
            if (!string.IsNullOrEmpty(this.HttpGetBinding))
            {
                behavior.HttpGetBinding = ConfigLoader.LookupBinding(this.HttpGetBinding, this.HttpGetBindingConfiguration);
            }
            if (!string.IsNullOrEmpty(this.HttpsGetBinding))
            {
                behavior.HttpsGetBinding = ConfigLoader.LookupBinding(this.HttpsGetBinding, this.HttpsGetBindingConfiguration);
            }
            return behavior;
        }

        public override Type BehaviorType =>
            typeof(ServiceMetadataBehavior);

        [ConfigurationProperty("externalMetadataLocation")]
        public Uri ExternalMetadataLocation
        {
            get => 
                ((Uri) base["externalMetadataLocation"]);
            set
            {
                base["externalMetadataLocation"] = value;
            }
        }

        [ConfigurationProperty("httpGetBinding", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpGetBinding
        {
            get => 
                ((string) base["httpGetBinding"]);
            set
            {
                base["httpGetBinding"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("httpGetBindingConfiguration", DefaultValue="")]
        public string HttpGetBindingConfiguration
        {
            get => 
                ((string) base["httpGetBindingConfiguration"]);
            set
            {
                base["httpGetBindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("httpGetEnabled", DefaultValue=false)]
        public bool HttpGetEnabled
        {
            get => 
                ((bool) base["httpGetEnabled"]);
            set
            {
                base["httpGetEnabled"] = value;
            }
        }

        [ConfigurationProperty("httpGetUrl")]
        public Uri HttpGetUrl
        {
            get => 
                ((Uri) base["httpGetUrl"]);
            set
            {
                base["httpGetUrl"] = value;
            }
        }

        [ConfigurationProperty("httpsGetBinding", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpsGetBinding
        {
            get => 
                ((string) base["httpsGetBinding"]);
            set
            {
                base["httpsGetBinding"] = value;
            }
        }

        [ConfigurationProperty("httpsGetBindingConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpsGetBindingConfiguration
        {
            get => 
                ((string) base["httpsGetBindingConfiguration"]);
            set
            {
                base["httpsGetBindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("httpsGetEnabled", DefaultValue=false)]
        public bool HttpsGetEnabled
        {
            get => 
                ((bool) base["httpsGetEnabled"]);
            set
            {
                base["httpsGetEnabled"] = value;
            }
        }

        [ConfigurationProperty("httpsGetUrl")]
        public Uri HttpsGetUrl
        {
            get => 
                ((Uri) base["httpsGetUrl"]);
            set
            {
                base["httpsGetUrl"] = value;
            }
        }

        [ConfigurationProperty("policyVersion", DefaultValue="Default"), TypeConverter(typeof(PolicyVersionConverter))]
        public System.ServiceModel.Description.PolicyVersion PolicyVersion
        {
            get => 
                ((System.ServiceModel.Description.PolicyVersion) base["policyVersion"]);
            set
            {
                base["policyVersion"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("externalMetadataLocation", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpGetEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpGetUrl", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsGetEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsGetUrl", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpGetBinding", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpGetBindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsGetBinding", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsGetBindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("policyVersion", typeof(System.ServiceModel.Description.PolicyVersion), "Default", new PolicyVersionConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

