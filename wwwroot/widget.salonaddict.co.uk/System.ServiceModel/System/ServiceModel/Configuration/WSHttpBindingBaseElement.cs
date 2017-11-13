namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;

    public abstract class WSHttpBindingBaseElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        protected WSHttpBindingBaseElement() : this(null)
        {
        }

        protected WSHttpBindingBaseElement(string name) : base(name)
        {
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            WSHttpBindingBase base2 = (WSHttpBindingBase) binding;
            this.BypassProxyOnLocal = base2.BypassProxyOnLocal;
            this.TransactionFlow = base2.TransactionFlow;
            this.HostNameComparisonMode = base2.HostNameComparisonMode;
            this.MaxBufferPoolSize = base2.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = base2.MaxReceivedMessageSize;
            this.MessageEncoding = base2.MessageEncoding;
            if (base2.ProxyAddress != null)
            {
                this.ProxyAddress = base2.ProxyAddress;
            }
            this.TextEncoding = base2.TextEncoding;
            this.UseDefaultWebProxy = base2.UseDefaultWebProxy;
            this.ReaderQuotas.InitializeFrom(base2.ReaderQuotas);
            this.ReliableSession.InitializeFrom(base2.ReliableSession);
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            WSHttpBindingBase base2 = (WSHttpBindingBase) binding;
            base2.BypassProxyOnLocal = this.BypassProxyOnLocal;
            base2.TransactionFlow = this.TransactionFlow;
            base2.HostNameComparisonMode = this.HostNameComparisonMode;
            base2.MaxBufferPoolSize = this.MaxBufferPoolSize;
            base2.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            base2.MessageEncoding = this.MessageEncoding;
            if (this.ProxyAddress != null)
            {
                base2.ProxyAddress = this.ProxyAddress;
            }
            base2.TextEncoding = this.TextEncoding;
            base2.UseDefaultWebProxy = this.UseDefaultWebProxy;
            this.ReaderQuotas.ApplyConfiguration(base2.ReaderQuotas);
            this.ReliableSession.ApplyConfiguration(base2.ReliableSession);
        }

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

        [ConfigurationProperty("maxBufferPoolSize", DefaultValue=0x80000L), LongValidator(MinValue=0L)]
        public long MaxBufferPoolSize
        {
            get => 
                ((long) base["maxBufferPoolSize"]);
            set
            {
                base["maxBufferPoolSize"] = value;
            }
        }

        [LongValidator(MinValue=1L), ConfigurationProperty("maxReceivedMessageSize", DefaultValue=0x10000L)]
        public long MaxReceivedMessageSize
        {
            get => 
                ((long) base["maxReceivedMessageSize"]);
            set
            {
                base["maxReceivedMessageSize"] = value;
            }
        }

        [ServiceModelEnumValidator(typeof(WSMessageEncodingHelper)), ConfigurationProperty("messageEncoding", DefaultValue=0)]
        public WSMessageEncoding MessageEncoding
        {
            get => 
                ((WSMessageEncoding) base["messageEncoding"]);
            set
            {
                base["messageEncoding"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("bypassProxyOnLocal", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transactionFlow", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferPoolSize", typeof(long), 0x80000L, null, new LongValidator(0L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("messageEncoding", typeof(WSMessageEncoding), WSMessageEncoding.Text, null, new ServiceModelEnumValidator(typeof(WSMessageEncodingHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("proxyAddress", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("reliableSession", typeof(StandardBindingOptionalReliableSessionElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("textEncoding", typeof(Encoding), "utf-8", new EncodingConverter(), null, ConfigurationPropertyOptions.None));
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

        [ConfigurationProperty("reliableSession")]
        public StandardBindingOptionalReliableSessionElement ReliableSession =>
            ((StandardBindingOptionalReliableSessionElement) base["reliableSession"]);

        [TypeConverter(typeof(EncodingConverter)), ConfigurationProperty("textEncoding", DefaultValue="utf-8")]
        public Encoding TextEncoding
        {
            get => 
                ((Encoding) base["textEncoding"]);
            set
            {
                base["textEncoding"] = value;
            }
        }

        [ConfigurationProperty("transactionFlow", DefaultValue=false)]
        public bool TransactionFlow
        {
            get => 
                ((bool) base["transactionFlow"]);
            set
            {
                base["transactionFlow"] = value;
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

