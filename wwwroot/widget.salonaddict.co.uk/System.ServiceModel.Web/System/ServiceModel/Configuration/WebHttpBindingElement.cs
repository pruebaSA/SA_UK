namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Xml;

    public class WebHttpBindingElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        public WebHttpBindingElement() : this(null)
        {
        }

        public WebHttpBindingElement(string name) : base(name)
        {
        }

        private void ApplyReaderQuotasConfiguration(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("readerQuotas");
            }
            if (this.ReaderQuotas.MaxDepth != 0)
            {
                readerQuotas.MaxDepth = this.ReaderQuotas.MaxDepth;
            }
            if (this.ReaderQuotas.MaxStringContentLength != 0)
            {
                readerQuotas.MaxStringContentLength = this.ReaderQuotas.MaxStringContentLength;
            }
            if (this.ReaderQuotas.MaxArrayLength != 0)
            {
                readerQuotas.MaxArrayLength = this.ReaderQuotas.MaxArrayLength;
            }
            if (this.ReaderQuotas.MaxBytesPerRead != 0)
            {
                readerQuotas.MaxBytesPerRead = this.ReaderQuotas.MaxBytesPerRead;
            }
            if (this.ReaderQuotas.MaxNameTableCharCount != 0)
            {
                readerQuotas.MaxNameTableCharCount = this.ReaderQuotas.MaxNameTableCharCount;
            }
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            WebHttpBinding binding2 = (WebHttpBinding) binding;
            this.BypassProxyOnLocal = binding2.BypassProxyOnLocal;
            this.HostNameComparisonMode = binding2.HostNameComparisonMode;
            this.MaxBufferSize = binding2.MaxBufferSize;
            this.MaxBufferPoolSize = binding2.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = binding2.MaxReceivedMessageSize;
            if (binding2.ProxyAddress != null)
            {
                this.ProxyAddress = binding2.ProxyAddress;
            }
            this.WriteEncoding = binding2.WriteEncoding;
            this.TransferMode = binding2.TransferMode;
            this.UseDefaultWebProxy = binding2.UseDefaultWebProxy;
            this.AllowCookies = binding2.AllowCookies;
            this.Security.InitializeFrom(binding2.Security);
            this.InitializeReaderQuotas(binding2.ReaderQuotas);
        }

        internal void InitializeReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("readerQuotas");
            }
            this.ReaderQuotas.MaxDepth = readerQuotas.MaxDepth;
            this.ReaderQuotas.MaxStringContentLength = readerQuotas.MaxStringContentLength;
            this.ReaderQuotas.MaxArrayLength = readerQuotas.MaxArrayLength;
            this.ReaderQuotas.MaxBytesPerRead = readerQuotas.MaxBytesPerRead;
            this.ReaderQuotas.MaxNameTableCharCount = readerQuotas.MaxNameTableCharCount;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            WebHttpBinding binding2 = (WebHttpBinding) binding;
            binding2.BypassProxyOnLocal = this.BypassProxyOnLocal;
            binding2.HostNameComparisonMode = this.HostNameComparisonMode;
            binding2.MaxBufferPoolSize = this.MaxBufferPoolSize;
            binding2.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            binding2.WriteEncoding = this.WriteEncoding;
            binding2.TransferMode = this.TransferMode;
            binding2.UseDefaultWebProxy = this.UseDefaultWebProxy;
            binding2.AllowCookies = this.AllowCookies;
            if (this.ProxyAddress != null)
            {
                binding2.ProxyAddress = this.ProxyAddress;
            }
            if (base.ElementInformation.Properties["maxBufferSize"].ValueOrigin != PropertyValueOrigin.Default)
            {
                binding2.MaxBufferSize = this.MaxBufferSize;
            }
            this.Security.ApplyConfiguration(binding2.Security);
            this.ApplyReaderQuotasConfiguration(binding2.ReaderQuotas);
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

        protected override Type BindingElementType =>
            typeof(WebHttpBinding);

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

        [LongValidator(MinValue=0L), ConfigurationProperty("maxBufferPoolSize", DefaultValue=0x80000L)]
        public long MaxBufferPoolSize
        {
            get => 
                ((long) base["maxBufferPoolSize"]);
            set
            {
                base["maxBufferPoolSize"] = value;
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

        [ConfigurationProperty("maxReceivedMessageSize", DefaultValue=0x10000L), LongValidator(MinValue=1L)]
        public long MaxReceivedMessageSize
        {
            get => 
                ((long) base["maxReceivedMessageSize"]);
            set
            {
                base["maxReceivedMessageSize"] = value;
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
                    properties.Add(new ConfigurationProperty("bypassProxyOnLocal", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferSize", typeof(int), 0x10000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferPoolSize", typeof(long), 0x80000L, null, new LongValidator(0L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("proxyAddress", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("security", typeof(WebHttpSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("writeEncoding", typeof(Encoding), "utf-8", new EncodingConverter(), null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transferMode", typeof(System.ServiceModel.TransferMode), System.ServiceModel.TransferMode.Buffered, null, new ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useDefaultWebProxy", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
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

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas =>
            ((XmlDictionaryReaderQuotasElement) base["readerQuotas"]);

        [ConfigurationProperty("security")]
        public WebHttpSecurityElement Security =>
            ((WebHttpSecurityElement) base["security"]);

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

        [ConfigurationProperty("writeEncoding", DefaultValue="utf-8"), TypeConverter(typeof(EncodingConverter)), WebEncodingValidator]
        public Encoding WriteEncoding
        {
            get => 
                ((Encoding) base["writeEncoding"]);
            set
            {
                base["writeEncoding"] = value;
            }
        }
    }
}

