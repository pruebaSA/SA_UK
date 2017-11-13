namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.Xml;

    public class HttpTransportBindingElement : TransportBindingElement, IWsdlExportExtension, IPolicyExportExtension, ITransportPolicyImport
    {
        private bool allowCookies;
        private AuthenticationSchemes authenticationScheme;
        private bool bypassProxyOnLocal;
        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy;
        private System.ServiceModel.HostNameComparisonMode hostNameComparisonMode;
        private bool inheritBaseAddressSettings;
        private bool keepAliveEnabled;
        private int maxBufferSize;
        private bool maxBufferSizeInitialized;
        private string method;
        private Uri proxyAddress;
        private AuthenticationSchemes proxyAuthenticationScheme;
        private string realm;
        private System.ServiceModel.TransferMode transferMode;
        private bool unsafeConnectionNtlmAuthentication;
        private bool useDefaultWebProxy;
        private IWebProxy webProxy;

        public HttpTransportBindingElement()
        {
            this.extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
            this.allowCookies = false;
            this.authenticationScheme = AuthenticationSchemes.Anonymous;
            this.bypassProxyOnLocal = false;
            this.hostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
            this.keepAliveEnabled = true;
            this.maxBufferSize = 0x10000;
            this.method = string.Empty;
            this.proxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
            this.proxyAddress = null;
            this.realm = "";
            this.transferMode = System.ServiceModel.TransferMode.Buffered;
            this.unsafeConnectionNtlmAuthentication = false;
            this.useDefaultWebProxy = true;
            this.webProxy = null;
        }

        protected HttpTransportBindingElement(HttpTransportBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            this.extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
            this.allowCookies = elementToBeCloned.allowCookies;
            this.authenticationScheme = elementToBeCloned.authenticationScheme;
            this.bypassProxyOnLocal = elementToBeCloned.bypassProxyOnLocal;
            this.hostNameComparisonMode = elementToBeCloned.hostNameComparisonMode;
            this.inheritBaseAddressSettings = elementToBeCloned.InheritBaseAddressSettings;
            this.keepAliveEnabled = elementToBeCloned.keepAliveEnabled;
            this.maxBufferSize = elementToBeCloned.maxBufferSize;
            this.maxBufferSizeInitialized = elementToBeCloned.maxBufferSizeInitialized;
            this.method = elementToBeCloned.method;
            this.proxyAddress = elementToBeCloned.proxyAddress;
            this.proxyAuthenticationScheme = elementToBeCloned.proxyAuthenticationScheme;
            this.realm = elementToBeCloned.realm;
            this.transferMode = elementToBeCloned.transferMode;
            this.unsafeConnectionNtlmAuthentication = elementToBeCloned.unsafeConnectionNtlmAuthentication;
            this.useDefaultWebProxy = elementToBeCloned.useDefaultWebProxy;
            this.webProxy = elementToBeCloned.webProxy;
            this.extendedProtectionPolicy = elementToBeCloned.ExtendedProtectionPolicy;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("CouldnTCreateChannelForChannelType2", new object[] { context.Binding.Name, typeof(TChannel) }));
            }
            return (IChannelFactory<TChannel>) new HttpChannelFactory(this, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (!this.CanBuildChannelListener<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("CouldnTCreateChannelForChannelType2", new object[] { context.Binding.Name, typeof(TChannel) }));
            }
            HttpChannelListener listener = new HttpChannelListener(this, context);
            VirtualPathExtension.ApplyHostedContext(listener, context);
            return (IChannelListener<TChannel>) listener;
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => 
            (typeof(TChannel) == typeof(IRequestChannel));

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            (typeof(TChannel) == typeof(IReplyChannel));

        public override BindingElement Clone() => 
            new HttpTransportBindingElement(this);

        private MessageEncodingBindingElement FindMessageEncodingBindingElement(BindingElementCollection bindingElements, out bool createdNew)
        {
            createdNew = false;
            MessageEncodingBindingElement element = bindingElements.Find<MessageEncodingBindingElement>();
            if (element == null)
            {
                createdNew = true;
                element = new TextMessageEncodingBindingElement();
            }
            return element;
        }

        private MessageEncodingBindingElement FindMessageEncodingBindingElement(WsdlEndpointConversionContext endpointContext, out bool createdNew)
        {
            BindingElementCollection bindingElements = endpointContext.Endpoint.Binding.CreateBindingElements();
            return this.FindMessageEncodingBindingElement(bindingElements, out createdNew);
        }

        public override T GetProperty<T>(BindingContext context) where T: class
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T) new SecurityCapabilities(this.SupportsClientAuthenticationImpl, this.AuthenticationScheme == AuthenticationSchemes.Negotiate, this.SupportsClientWindowsIdentityImpl, ProtectionLevel.None, ProtectionLevel.None);
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T) new BindingDeliveryCapabilitiesHelper();
            }
            if (typeof(T) == typeof(System.ServiceModel.TransferMode))
            {
                return (T) this.TransferMode;
            }
            if (typeof(T) == typeof(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy))
            {
                return (T) this.ExtendedProtectionPolicy;
            }
            if (context.BindingParameters.Find<MessageEncodingBindingElement>() == null)
            {
                context.BindingParameters.Add(new TextMessageEncodingBindingElement());
            }
            return base.GetProperty<T>(context);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }
            HttpTransportBindingElement element = b as HttpTransportBindingElement;
            if (element == null)
            {
                return false;
            }
            if (this.allowCookies != element.allowCookies)
            {
                return false;
            }
            if (this.authenticationScheme != element.authenticationScheme)
            {
                return false;
            }
            if (this.hostNameComparisonMode != element.hostNameComparisonMode)
            {
                return false;
            }
            if (this.inheritBaseAddressSettings != element.inheritBaseAddressSettings)
            {
                return false;
            }
            if (this.keepAliveEnabled != element.keepAliveEnabled)
            {
                return false;
            }
            if (this.maxBufferSize != element.maxBufferSize)
            {
                return false;
            }
            if (this.method != element.method)
            {
                return false;
            }
            if (this.proxyAddress != element.proxyAddress)
            {
                return false;
            }
            if (this.proxyAuthenticationScheme != element.proxyAuthenticationScheme)
            {
                return false;
            }
            if (this.realm != element.realm)
            {
                return false;
            }
            if (this.transferMode != element.transferMode)
            {
                return false;
            }
            if (this.unsafeConnectionNtlmAuthentication != element.unsafeConnectionNtlmAuthentication)
            {
                return false;
            }
            if (this.useDefaultWebProxy != element.useDefaultWebProxy)
            {
                return false;
            }
            if (this.webProxy != element.webProxy)
            {
                return false;
            }
            if (!ChannelBindingUtility.ValidatePolicies(this.ExtendedProtectionPolicy, element.ExtendedProtectionPolicy, false))
            {
                return false;
            }
            return true;
        }

        internal virtual void OnExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            string localName = null;
            switch (this.AuthenticationScheme)
            {
                case AuthenticationSchemes.Digest:
                    localName = "DigestAuthentication";
                    break;

                case AuthenticationSchemes.Negotiate:
                    localName = "NegotiateAuthentication";
                    break;

                case AuthenticationSchemes.Ntlm:
                    localName = "NtlmAuthentication";
                    break;

                case AuthenticationSchemes.Basic:
                    localName = "BasicAuthentication";
                    break;
            }
            if (localName != null)
            {
                policyContext.GetBindingAssertions().Add(new XmlDocument().CreateElement("http", localName, "http://schemas.microsoft.com/ws/06/2004/policy/http"));
            }
        }

        internal virtual void OnImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
        }

        void ITransportPolicyImport.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            ICollection<XmlElement> bindingAssertions = policyContext.GetBindingAssertions();
            List<XmlElement> list = new List<XmlElement>();
            bool flag = false;
            foreach (XmlElement element in bindingAssertions)
            {
                string str;
                if ((element.NamespaceURI == "http://schemas.microsoft.com/ws/06/2004/policy/http") && ((str = element.LocalName) != null))
                {
                    if (str == "BasicAuthentication")
                    {
                        this.AuthenticationScheme = AuthenticationSchemes.Basic;
                    }
                    else if (str == "DigestAuthentication")
                    {
                        this.AuthenticationScheme = AuthenticationSchemes.Digest;
                    }
                    else if (str == "NegotiateAuthentication")
                    {
                        this.AuthenticationScheme = AuthenticationSchemes.Negotiate;
                    }
                    else
                    {
                        if (str != "NtlmAuthentication")
                        {
                            continue;
                        }
                        this.AuthenticationScheme = AuthenticationSchemes.Ntlm;
                    }
                    if (flag)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("HttpTransportCannotHaveMultipleAuthenticationSchemes", new object[] { policyContext.Contract.Namespace, policyContext.Contract.Name })));
                    }
                    flag = true;
                    list.Add(element);
                }
            }
            list.ForEach(element => bindingAssertions.Remove(element));
            this.OnImportPolicy(importer, policyContext);
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            bool flag;
            if (exporter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("exporter");
            }
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            this.OnExportPolicy(exporter, context);
            MessageEncodingBindingElement element = this.FindMessageEncodingBindingElement(context.BindingElements, out flag);
            if (flag && (element is IPolicyExportExtension))
            {
                ((IPolicyExportExtension) element).ExportPolicy(exporter, context);
            }
            WsdlExporter.WSAddressingHelper.AddWSAddressingAssertion(exporter, context, element.MessageVersion.Addressing);
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext endpointContext)
        {
            bool flag;
            MessageEncodingBindingElement element = this.FindMessageEncodingBindingElement(endpointContext, out flag);
            TransportBindingElement.ExportWsdlEndpoint(exporter, endpointContext, this.WsdlTransportUri, element.MessageVersion.Addressing);
        }

        public bool AllowCookies
        {
            get => 
                this.allowCookies;
            set
            {
                this.allowCookies = value;
            }
        }

        public AuthenticationSchemes AuthenticationScheme
        {
            get => 
                this.authenticationScheme;
            set
            {
                if (!AuthenticationSchemesHelper.IsSingleton(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", System.ServiceModel.SR.GetString("HttpRequiresSingleAuthScheme", new object[] { value }));
                }
                this.authenticationScheme = value;
            }
        }

        public bool BypassProxyOnLocal
        {
            get => 
                this.bypassProxyOnLocal;
            set
            {
                this.bypassProxyOnLocal = value;
            }
        }

        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get => 
                this.extendedProtectionPolicy;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if ((value.PolicyEnforcement == PolicyEnforcement.Always) && !ChannelBindingUtility.OSSupportsExtendedProtection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PlatformNotSupportedException(System.ServiceModel.SR.GetString("ExtendedProtectionNotSupported")));
                }
                this.extendedProtectionPolicy = value;
            }
        }

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                this.hostNameComparisonMode;
            set
            {
                HostNameComparisonModeHelper.Validate(value);
                this.hostNameComparisonMode = value;
            }
        }

        internal bool InheritBaseAddressSettings
        {
            get => 
                this.inheritBaseAddressSettings;
            set
            {
                this.inheritBaseAddressSettings = value;
            }
        }

        public bool KeepAliveEnabled
        {
            get => 
                this.keepAliveEnabled;
            set
            {
                this.keepAliveEnabled = value;
            }
        }

        public int MaxBufferSize
        {
            get
            {
                if (this.maxBufferSizeInitialized || (this.TransferMode != System.ServiceModel.TransferMode.Buffered))
                {
                    return this.maxBufferSize;
                }
                long maxReceivedMessageSize = this.MaxReceivedMessageSize;
                if (maxReceivedMessageSize > 0x7fffffffL)
                {
                    return 0x7fffffff;
                }
                return (int) maxReceivedMessageSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.maxBufferSizeInitialized = true;
                this.maxBufferSize = value;
            }
        }

        internal string Method
        {
            get => 
                this.method;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.method = value;
            }
        }

        internal IWebProxy Proxy
        {
            get => 
                this.webProxy;
            set
            {
                this.webProxy = value;
            }
        }

        public Uri ProxyAddress
        {
            get => 
                this.proxyAddress;
            set
            {
                this.proxyAddress = value;
            }
        }

        public AuthenticationSchemes ProxyAuthenticationScheme
        {
            get => 
                this.proxyAuthenticationScheme;
            set
            {
                if (!AuthenticationSchemesHelper.IsSingleton(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", System.ServiceModel.SR.GetString("HttpProxyRequiresSingleAuthScheme", new object[] { value }));
                }
                this.proxyAuthenticationScheme = value;
            }
        }

        public string Realm
        {
            get => 
                this.realm;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.realm = value;
            }
        }

        public override string Scheme =>
            "http";

        internal virtual bool SupportsClientAuthenticationImpl =>
            (this.authenticationScheme != AuthenticationSchemes.Anonymous);

        internal virtual bool SupportsClientWindowsIdentityImpl =>
            (this.authenticationScheme != AuthenticationSchemes.Anonymous);

        public System.ServiceModel.TransferMode TransferMode
        {
            get => 
                this.transferMode;
            set
            {
                TransferModeHelper.Validate(value);
                this.transferMode = value;
            }
        }

        public bool UnsafeConnectionNtlmAuthentication
        {
            get => 
                this.unsafeConnectionNtlmAuthentication;
            set
            {
                this.unsafeConnectionNtlmAuthentication = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get => 
                this.useDefaultWebProxy;
            set
            {
                this.useDefaultWebProxy = value;
            }
        }

        internal virtual string WsdlTransportUri =>
            "http://schemas.xmlsoap.org/soap/http";

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            internal BindingDeliveryCapabilitiesHelper()
            {
            }

            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery =>
                false;

            bool IBindingDeliveryCapabilities.QueuedDelivery =>
                false;
        }
    }
}

