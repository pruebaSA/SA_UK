namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class ServiceDebugElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceDebugElement element = (ServiceDebugElement) from;
            this.HttpHelpPageEnabled = element.HttpHelpPageEnabled;
            this.HttpHelpPageUrl = element.HttpHelpPageUrl;
            this.HttpsHelpPageEnabled = element.HttpsHelpPageEnabled;
            this.HttpsHelpPageUrl = element.HttpsHelpPageUrl;
            this.IncludeExceptionDetailInFaults = element.IncludeExceptionDetailInFaults;
            this.HttpHelpPageBinding = element.HttpHelpPageBinding;
            this.HttpHelpPageBindingConfiguration = element.HttpHelpPageBindingConfiguration;
            this.HttpsHelpPageBinding = element.HttpsHelpPageBinding;
            this.HttpsHelpPageBindingConfiguration = element.HttpsHelpPageBindingConfiguration;
        }

        protected internal override object CreateBehavior()
        {
            ServiceDebugBehavior behavior = new ServiceDebugBehavior {
                HttpHelpPageEnabled = this.HttpHelpPageEnabled,
                HttpHelpPageUrl = this.HttpHelpPageUrl,
                HttpsHelpPageEnabled = this.HttpsHelpPageEnabled,
                HttpsHelpPageUrl = this.HttpsHelpPageUrl,
                IncludeExceptionDetailInFaults = this.IncludeExceptionDetailInFaults
            };
            if (!string.IsNullOrEmpty(this.HttpHelpPageBinding))
            {
                behavior.HttpHelpPageBinding = ConfigLoader.LookupBinding(this.HttpHelpPageBinding, this.HttpHelpPageBindingConfiguration);
            }
            if (!string.IsNullOrEmpty(this.HttpsHelpPageBinding))
            {
                behavior.HttpsHelpPageBinding = ConfigLoader.LookupBinding(this.HttpsHelpPageBinding, this.HttpsHelpPageBindingConfiguration);
            }
            return behavior;
        }

        public override Type BehaviorType =>
            typeof(ServiceDebugBehavior);

        [StringValidator(MinLength=0), ConfigurationProperty("httpHelpPageBinding", DefaultValue="")]
        public string HttpHelpPageBinding
        {
            get => 
                ((string) base["httpHelpPageBinding"]);
            set
            {
                base["httpHelpPageBinding"] = value;
            }
        }

        [ConfigurationProperty("httpHelpPageBindingConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpHelpPageBindingConfiguration
        {
            get => 
                ((string) base["httpHelpPageBindingConfiguration"]);
            set
            {
                base["httpHelpPageBindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("httpHelpPageEnabled", DefaultValue=true)]
        public bool HttpHelpPageEnabled
        {
            get => 
                ((bool) base["httpHelpPageEnabled"]);
            set
            {
                base["httpHelpPageEnabled"] = value;
            }
        }

        [ConfigurationProperty("httpHelpPageUrl")]
        public Uri HttpHelpPageUrl
        {
            get => 
                ((Uri) base["httpHelpPageUrl"]);
            set
            {
                base["httpHelpPageUrl"] = value;
            }
        }

        [ConfigurationProperty("httpsHelpPageBinding", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpsHelpPageBinding
        {
            get => 
                ((string) base["httpsHelpPageBinding"]);
            set
            {
                base["httpsHelpPageBinding"] = value;
            }
        }

        [ConfigurationProperty("httpsHelpPageBindingConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string HttpsHelpPageBindingConfiguration
        {
            get => 
                ((string) base["httpsHelpPageBindingConfiguration"]);
            set
            {
                base["httpsHelpPageBindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("httpsHelpPageEnabled", DefaultValue=true)]
        public bool HttpsHelpPageEnabled
        {
            get => 
                ((bool) base["httpsHelpPageEnabled"]);
            set
            {
                base["httpsHelpPageEnabled"] = value;
            }
        }

        [ConfigurationProperty("httpsHelpPageUrl")]
        public Uri HttpsHelpPageUrl
        {
            get => 
                ((Uri) base["httpsHelpPageUrl"]);
            set
            {
                base["httpsHelpPageUrl"] = value;
            }
        }

        [ConfigurationProperty("includeExceptionDetailInFaults", DefaultValue=false)]
        public bool IncludeExceptionDetailInFaults
        {
            get => 
                ((bool) base["includeExceptionDetailInFaults"]);
            set
            {
                base["includeExceptionDetailInFaults"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("httpHelpPageEnabled", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpHelpPageUrl", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsHelpPageEnabled", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsHelpPageUrl", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpHelpPageBinding", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpHelpPageBindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsHelpPageBinding", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("httpsHelpPageBindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("includeExceptionDetailInFaults", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

