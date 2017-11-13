namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;

    public class HttpsTransportElement : HttpTransportElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            HttpsTransportBindingElement element = (HttpsTransportBindingElement) bindingElement;
            element.RequireClientCertificate = this.RequireClientCertificate;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            HttpsTransportElement element = (HttpsTransportElement) from;
            this.RequireClientCertificate = element.RequireClientCertificate;
        }

        protected override TransportBindingElement CreateDefaultBindingElement() => 
            new HttpsTransportBindingElement();

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            HttpsTransportBindingElement element = (HttpsTransportBindingElement) bindingElement;
            this.RequireClientCertificate = element.RequireClientCertificate;
        }

        public override Type BindingElementType =>
            typeof(HttpsTransportBindingElement);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("requireClientCertificate", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("requireClientCertificate", DefaultValue=false)]
        public bool RequireClientCertificate
        {
            get => 
                ((bool) base["requireClientCertificate"]);
            set
            {
                base["requireClientCertificate"] = value;
            }
        }
    }
}

