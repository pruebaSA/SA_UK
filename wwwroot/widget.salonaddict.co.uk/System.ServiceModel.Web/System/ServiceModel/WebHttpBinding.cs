namespace System.ServiceModel
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.Text;
    using System.Xml;

    public class WebHttpBinding : Binding, IBindingRuntimePreferences
    {
        private HttpsTransportBindingElement httpsTransportBindingElement;
        private HttpTransportBindingElement httpTransportBindingElement;
        private WebHttpSecurity security;
        private WebMessageEncodingBindingElement webMessageEncodingBindingElement;

        public WebHttpBinding() : this(WebHttpSecurityMode.None)
        {
        }

        public WebHttpBinding(WebHttpSecurityMode securityMode)
        {
            this.security = new WebHttpSecurity();
            this.Initialize();
            this.security.Mode = securityMode;
        }

        public WebHttpBinding(string configurationName) : this()
        {
            this.ApplyConfiguration(configurationName);
        }

        private void ApplyConfiguration(string configurationName)
        {
            WebHttpBindingElement element2 = WebHttpBindingCollectionElement.GetBindingCollectionElement().Bindings[configurationName];
            if (element2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR2.GetString(SR2.ConfigInvalidBindingConfigurationName, new object[] { configurationName, "webHttpBinding" })));
            }
            element2.ApplyConfiguration(this);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            elements.Add(this.webMessageEncodingBindingElement);
            elements.Add(this.GetTransport());
            return elements.Clone();
        }

        private TransportBindingElement GetTransport()
        {
            if (this.security.Mode == WebHttpSecurityMode.Transport)
            {
                this.security.EnableTransportSecurity(this.httpsTransportBindingElement);
                return this.httpsTransportBindingElement;
            }
            if (this.security.Mode == WebHttpSecurityMode.TransportCredentialOnly)
            {
                this.security.EnableTransportAuthentication(this.httpTransportBindingElement);
                return this.httpTransportBindingElement;
            }
            this.security.DisableTransportAuthentication(this.httpTransportBindingElement);
            return this.httpTransportBindingElement;
        }

        private void Initialize()
        {
            this.httpTransportBindingElement = new HttpTransportBindingElement();
            this.httpsTransportBindingElement = new HttpsTransportBindingElement();
            this.httpTransportBindingElement.ManualAddressing = true;
            this.httpsTransportBindingElement.ManualAddressing = true;
            this.webMessageEncodingBindingElement = new WebMessageEncodingBindingElement();
            this.webMessageEncodingBindingElement.MessageVersion = MessageVersion.None;
        }

        public bool AllowCookies
        {
            get => 
                this.httpTransportBindingElement.AllowCookies;
            set
            {
                this.httpTransportBindingElement.AllowCookies = value;
                this.httpsTransportBindingElement.AllowCookies = value;
            }
        }

        public bool BypassProxyOnLocal
        {
            get => 
                this.httpTransportBindingElement.BypassProxyOnLocal;
            set
            {
                this.httpTransportBindingElement.BypassProxyOnLocal = value;
                this.httpsTransportBindingElement.BypassProxyOnLocal = value;
            }
        }

        public System.ServiceModel.EnvelopeVersion EnvelopeVersion =>
            System.ServiceModel.EnvelopeVersion.None;

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                this.httpTransportBindingElement.HostNameComparisonMode;
            set
            {
                this.httpTransportBindingElement.HostNameComparisonMode = value;
                this.httpsTransportBindingElement.HostNameComparisonMode = value;
            }
        }

        public long MaxBufferPoolSize
        {
            get => 
                this.httpTransportBindingElement.MaxBufferPoolSize;
            set
            {
                this.httpTransportBindingElement.MaxBufferPoolSize = value;
                this.httpsTransportBindingElement.MaxBufferPoolSize = value;
            }
        }

        public int MaxBufferSize
        {
            get => 
                this.httpTransportBindingElement.MaxBufferSize;
            set
            {
                this.httpTransportBindingElement.MaxBufferSize = value;
                this.httpsTransportBindingElement.MaxBufferSize = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get => 
                this.httpTransportBindingElement.MaxReceivedMessageSize;
            set
            {
                this.httpTransportBindingElement.MaxReceivedMessageSize = value;
                this.httpsTransportBindingElement.MaxReceivedMessageSize = value;
            }
        }

        public Uri ProxyAddress
        {
            get => 
                this.httpTransportBindingElement.ProxyAddress;
            set
            {
                this.httpTransportBindingElement.ProxyAddress = value;
                this.httpsTransportBindingElement.ProxyAddress = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get => 
                this.webMessageEncodingBindingElement.ReaderQuotas;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                value.CopyTo(this.webMessageEncodingBindingElement.ReaderQuotas);
            }
        }

        public override string Scheme =>
            this.GetTransport().Scheme;

        public WebHttpSecurity Security =>
            this.security;

        bool IBindingRuntimePreferences.ReceiveSynchronously =>
            false;

        public System.ServiceModel.TransferMode TransferMode
        {
            get => 
                this.httpTransportBindingElement.TransferMode;
            set
            {
                this.httpTransportBindingElement.TransferMode = value;
                this.httpsTransportBindingElement.TransferMode = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get => 
                this.httpTransportBindingElement.UseDefaultWebProxy;
            set
            {
                this.httpTransportBindingElement.UseDefaultWebProxy = value;
                this.httpsTransportBindingElement.UseDefaultWebProxy = value;
            }
        }

        public Encoding WriteEncoding
        {
            get => 
                this.webMessageEncodingBindingElement.WriteEncoding;
            set
            {
                this.webMessageEncodingBindingElement.WriteEncoding = value;
            }
        }

        internal static class WebHttpBindingConfigurationStrings
        {
            internal const string WebHttpBindingCollectionElementName = "webHttpBinding";
        }
    }
}

