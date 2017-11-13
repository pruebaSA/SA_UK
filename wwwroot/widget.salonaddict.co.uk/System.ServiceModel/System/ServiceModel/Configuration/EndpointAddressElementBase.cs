namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public class EndpointAddressElementBase : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        protected EndpointAddressElementBase()
        {
        }

        protected internal void Copy(EndpointAddressElementBase source)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.Address = source.Address;
            this.Headers.Headers = source.Headers.Headers;
            if (source.ElementInformation.Properties["identity"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.Identity.Copy(source.Identity);
            }
        }

        public void InitializeFrom(EndpointAddress endpointAddress)
        {
            if (null == endpointAddress)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointAddress");
            }
            this.Address = endpointAddress.Uri;
            this.Headers.Headers = endpointAddress.Headers;
            if (endpointAddress.Identity != null)
            {
                this.Identity.InitializeFrom(endpointAddress.Identity);
            }
        }

        [ConfigurationProperty("address", DefaultValue=null, Options=ConfigurationPropertyOptions.IsRequired)]
        public Uri Address
        {
            get => 
                ((Uri) base["address"]);
            set
            {
                base["address"] = value;
            }
        }

        [ConfigurationProperty("headers")]
        public AddressHeaderCollectionElement Headers =>
            ((AddressHeaderCollectionElement) base["headers"]);

        [ConfigurationProperty("identity")]
        public IdentityElement Identity =>
            ((IdentityElement) base["identity"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("address", typeof(Uri), null, null, null, ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("headers", typeof(AddressHeaderCollectionElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("identity", typeof(IdentityElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

