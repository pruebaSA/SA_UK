namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class ServiceEndpointElement : ConfigurationElement, IConfigurationContextProviderInternal
    {
        private ConfigurationPropertyCollection properties;

        public ServiceEndpointElement()
        {
        }

        public ServiceEndpointElement(Uri address, string contractType) : this()
        {
            this.Address = address;
            this.Contract = contractType;
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            null;

        [ConfigurationProperty("address", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey)]
        public Uri Address
        {
            get => 
                ((Uri) base["address"]);
            set
            {
                base["address"] = value;
            }
        }

        [ConfigurationProperty("behaviorConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string BehaviorConfiguration
        {
            get => 
                ((string) base["behaviorConfiguration"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["behaviorConfiguration"] = value;
            }
        }

        [ConfigurationProperty("binding", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Binding
        {
            get => 
                ((string) base["binding"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["binding"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("bindingConfiguration", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey)]
        public string BindingConfiguration
        {
            get => 
                ((string) base["bindingConfiguration"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["bindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("bindingName", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey), StringValidator(MinLength=0)]
        public string BindingName
        {
            get => 
                ((string) base["bindingName"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["bindingName"] = value;
            }
        }

        [ConfigurationProperty("bindingNamespace", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey), StringValidator(MinLength=0)]
        public string BindingNamespace
        {
            get => 
                ((string) base["bindingNamespace"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["bindingNamespace"] = value;
            }
        }

        [ConfigurationProperty("contract", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey), StringValidator(MinLength=0)]
        public string Contract
        {
            get => 
                ((string) base["contract"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["contract"] = value;
            }
        }

        [ConfigurationProperty("headers")]
        public AddressHeaderCollectionElement Headers =>
            ((AddressHeaderCollectionElement) base["headers"]);

        [ConfigurationProperty("identity")]
        public IdentityElement Identity =>
            ((IdentityElement) base["identity"]);

        [ConfigurationProperty("listenUri", DefaultValue=null)]
        public Uri ListenUri
        {
            get => 
                ((Uri) base["listenUri"]);
            set
            {
                base["listenUri"] = value;
            }
        }

        [ServiceModelEnumValidator(typeof(ListenUriModeHelper)), ConfigurationProperty("listenUriMode", DefaultValue=0)]
        public System.ServiceModel.Description.ListenUriMode ListenUriMode
        {
            get => 
                ((System.ServiceModel.Description.ListenUriMode) base["listenUriMode"]);
            set
            {
                base["listenUriMode"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("name", DefaultValue="")]
        public string Name
        {
            get => 
                ((string) base["name"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["name"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("address", typeof(Uri), "", null, null, ConfigurationPropertyOptions.IsKey),
                        new ConfigurationProperty("behaviorConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("binding", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("bindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey),
                        new ConfigurationProperty("name", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("bindingName", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey),
                        new ConfigurationProperty("bindingNamespace", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey),
                        new ConfigurationProperty("contract", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey),
                        new ConfigurationProperty("headers", typeof(AddressHeaderCollectionElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("identity", typeof(IdentityElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("listenUriMode", typeof(System.ServiceModel.Description.ListenUriMode), System.ServiceModel.Description.ListenUriMode.Explicit, null, new ServiceModelEnumValidator(typeof(ListenUriModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("listenUri", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

