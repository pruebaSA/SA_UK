namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class HttpTransportElement : TransportElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            HttpTransportBindingElement element = (HttpTransportBindingElement) bindingElement;
            element.AllowCookies = this.AllowCookies;
            element.AuthenticationScheme = this.AuthenticationScheme;
            element.BypassProxyOnLocal = this.BypassProxyOnLocal;
            element.KeepAliveEnabled = this.KeepAliveEnabled;
            element.HostNameComparisonMode = this.HostNameComparisonMode;
            if (base.ElementInformation.Properties["maxBufferSize"].ValueOrigin != PropertyValueOrigin.Default)
            {
                element.MaxBufferSize = this.MaxBufferSize;
            }
            element.ProxyAddress = this.ProxyAddress;
            element.ProxyAuthenticationScheme = this.ProxyAuthenticationScheme;
            element.Realm = this.Realm;
            element.TransferMode = this.TransferMode;
            element.UnsafeConnectionNtlmAuthentication = this.UnsafeConnectionNtlmAuthentication;
            element.UseDefaultWebProxy = this.UseDefaultWebProxy;
            element.ExtendedProtectionPolicy = ChannelBindingUtility.BuildPolicy(this.ExtendedProtectionPolicy);
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            HttpTransportElement element = (HttpTransportElement) from;
            this.AllowCookies = element.AllowCookies;
            this.AuthenticationScheme = element.AuthenticationScheme;
            this.BypassProxyOnLocal = element.BypassProxyOnLocal;
            this.KeepAliveEnabled = element.KeepAliveEnabled;
            this.HostNameComparisonMode = element.HostNameComparisonMode;
            this.MaxBufferSize = element.MaxBufferSize;
            this.ProxyAddress = element.ProxyAddress;
            this.ProxyAuthenticationScheme = element.ProxyAuthenticationScheme;
            this.Realm = element.Realm;
            this.TransferMode = element.TransferMode;
            this.UnsafeConnectionNtlmAuthentication = element.UnsafeConnectionNtlmAuthentication;
            this.UseDefaultWebProxy = element.UseDefaultWebProxy;
            ChannelBindingUtility.CopyFrom(element.ExtendedProtectionPolicy, this.ExtendedProtectionPolicy);
        }

        protected override TransportBindingElement CreateDefaultBindingElement() => 
            new HttpTransportBindingElement();

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            HttpTransportBindingElement element = (HttpTransportBindingElement) bindingElement;
            this.AllowCookies = element.AllowCookies;
            this.AuthenticationScheme = element.AuthenticationScheme;
            this.BypassProxyOnLocal = element.BypassProxyOnLocal;
            this.KeepAliveEnabled = element.KeepAliveEnabled;
            this.HostNameComparisonMode = element.HostNameComparisonMode;
            this.MaxBufferSize = element.MaxBufferSize;
            this.ProxyAddress = element.ProxyAddress;
            this.ProxyAuthenticationScheme = element.ProxyAuthenticationScheme;
            this.Realm = element.Realm;
            this.TransferMode = element.TransferMode;
            this.UnsafeConnectionNtlmAuthentication = element.UnsafeConnectionNtlmAuthentication;
            this.UseDefaultWebProxy = element.UseDefaultWebProxy;
            ChannelBindingUtility.InitializeFrom(element.ExtendedProtectionPolicy, this.ExtendedProtectionPolicy);
        }

        [ConfigurationProperty("allowCookies", DefaultValue=false)]
        public bool AllowCookies
        {
            get => 
                ((bool) base["allowCookies"]);
            set
            {
                base["allowCookies"] = value;
            }
        }

        [ConfigurationProperty("authenticationScheme", DefaultValue=0x8000), StandardRuntimeEnumValidator(typeof(AuthenticationSchemes))]
        public AuthenticationSchemes AuthenticationScheme
        {
            get => 
                ((AuthenticationSchemes) base["authenticationScheme"]);
            set
            {
                base["authenticationScheme"] = value;
            }
        }

        public override Type BindingElementType =>
            typeof(HttpTransportBindingElement);

        [ConfigurationProperty("bypassProxyOnLocal", DefaultValue=false)]
        public bool BypassProxyOnLocal
        {
            get => 
                ((bool) base["bypassProxyOnLocal"]);
            set
            {
                base["bypassProxyOnLocal"] = value;
            }
        }

        [ConfigurationProperty("extendedProtectionPolicy")]
        public ExtendedProtectionPolicyElement ExtendedProtectionPolicy =>
            ((ExtendedProtectionPolicyElement) base["extendedProtectionPolicy"]);

        [ConfigurationProperty("hostNameComparisonMode", DefaultValue=0), ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper))]
        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                ((System.ServiceModel.HostNameComparisonMode) base["hostNameComparisonMode"]);
            set
            {
                base["hostNameComparisonMode"] = value;
            }
        }

        [ConfigurationProperty("keepAliveEnabled", DefaultValue=true)]
        public bool KeepAliveEnabled
        {
            get => 
                ((bool) base["keepAliveEnabled"]);
            set
            {
                base["keepAliveEnabled"] = value;
            }
        }

        [ConfigurationProperty("maxBufferSize", DefaultValue=0x10000), IntegerValidator(MinValue=1)]
        public int MaxBufferSize
        {
            get => 
                ((int) base["maxBufferSize"]);
            set
            {
                base["maxBufferSize"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("allowCookies", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("authenticationScheme", typeof(AuthenticationSchemes), AuthenticationSchemes.Anonymous, null, new StandardRuntimeEnumValidator(typeof(AuthenticationSchemes)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("bypassProxyOnLocal", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("keepAliveEnabled", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferSize", typeof(int), 0x10000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("proxyAddress", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("proxyAuthenticationScheme", typeof(AuthenticationSchemes), AuthenticationSchemes.Anonymous, null, new StandardRuntimeEnumValidator(typeof(AuthenticationSchemes)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("realm", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transferMode", typeof(System.ServiceModel.TransferMode), System.ServiceModel.TransferMode.Buffered, null, new ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("unsafeConnectionNtlmAuthentication", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useDefaultWebProxy", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("extendedProtectionPolicy", typeof(ExtendedProtectionPolicyElement), null, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("proxyAddress", DefaultValue=null)]
        public Uri ProxyAddress
        {
            get => 
                ((Uri) base["proxyAddress"]);
            set
            {
                base["proxyAddress"] = value;
            }
        }

        [StandardRuntimeEnumValidator(typeof(AuthenticationSchemes)), ConfigurationProperty("proxyAuthenticationScheme", DefaultValue=0x8000)]
        public AuthenticationSchemes ProxyAuthenticationScheme
        {
            get => 
                ((AuthenticationSchemes) base["proxyAuthenticationScheme"]);
            set
            {
                base["proxyAuthenticationScheme"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("realm", DefaultValue="")]
        public string Realm
        {
            get => 
                ((string) base["realm"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["realm"] = value;
            }
        }

        [ConfigurationProperty("transferMode", DefaultValue=0), ServiceModelEnumValidator(typeof(TransferModeHelper))]
        public System.ServiceModel.TransferMode TransferMode
        {
            get => 
                ((System.ServiceModel.TransferMode) base["transferMode"]);
            set
            {
                base["transferMode"] = value;
            }
        }

        [ConfigurationProperty("unsafeConnectionNtlmAuthentication", DefaultValue=false)]
        public bool UnsafeConnectionNtlmAuthentication
        {
            get => 
                ((bool) base["unsafeConnectionNtlmAuthentication"]);
            set
            {
                base["unsafeConnectionNtlmAuthentication"] = value;
            }
        }

        [ConfigurationProperty("useDefaultWebProxy", DefaultValue=true)]
        public bool UseDefaultWebProxy
        {
            get => 
                ((bool) base["useDefaultWebProxy"]);
            set
            {
                base["useDefaultWebProxy"] = value;
            }
        }
    }
}

