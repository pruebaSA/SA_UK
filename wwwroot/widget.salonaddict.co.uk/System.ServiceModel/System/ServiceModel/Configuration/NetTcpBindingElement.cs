namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class NetTcpBindingElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        public NetTcpBindingElement() : this(null)
        {
        }

        public NetTcpBindingElement(string name) : base(name)
        {
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            NetTcpBinding binding2 = (NetTcpBinding) binding;
            this.TransactionFlow = binding2.TransactionFlow;
            this.TransferMode = binding2.TransferMode;
            this.TransactionProtocol = binding2.TransactionProtocol;
            this.HostNameComparisonMode = binding2.HostNameComparisonMode;
            this.MaxBufferPoolSize = binding2.MaxBufferPoolSize;
            this.MaxBufferSize = binding2.MaxBufferSize;
            this.MaxConnections = binding2.MaxConnections;
            this.MaxReceivedMessageSize = binding2.MaxReceivedMessageSize;
            this.ListenBacklog = binding2.ListenBacklog;
            this.ReliableSession.InitializeFrom(binding2.ReliableSession);
            this.Security.InitializeFrom(binding2.Security);
            this.ReaderQuotas.InitializeFrom(binding2.ReaderQuotas);
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            NetTcpBinding binding2 = (NetTcpBinding) binding;
            binding2.TransactionFlow = this.TransactionFlow;
            binding2.TransferMode = this.TransferMode;
            binding2.TransactionProtocol = this.TransactionProtocol;
            binding2.HostNameComparisonMode = this.HostNameComparisonMode;
            binding2.ListenBacklog = this.ListenBacklog;
            binding2.MaxBufferPoolSize = this.MaxBufferPoolSize;
            if (base.ElementInformation.Properties["maxBufferSize"].ValueOrigin != PropertyValueOrigin.Default)
            {
                binding2.MaxBufferSize = this.MaxBufferSize;
            }
            binding2.MaxConnections = this.MaxConnections;
            binding2.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            binding2.PortSharingEnabled = this.PortSharingEnabled;
            this.ReliableSession.ApplyConfiguration(binding2.ReliableSession);
            this.Security.ApplyConfiguration(binding2.Security);
            this.ReaderQuotas.ApplyConfiguration(binding2.ReaderQuotas);
        }

        protected override Type BindingElementType =>
            typeof(NetTcpBinding);

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

        [IntegerValidator(MinValue=1), ConfigurationProperty("listenBacklog", DefaultValue=10)]
        public int ListenBacklog
        {
            get => 
                ((int) base["listenBacklog"]);
            set
            {
                base["listenBacklog"] = value;
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

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxBufferSize", DefaultValue=0x10000)]
        public int MaxBufferSize
        {
            get => 
                ((int) base["maxBufferSize"]);
            set
            {
                base["maxBufferSize"] = value;
            }
        }

        [ConfigurationProperty("maxConnections", DefaultValue=10), IntegerValidator(MinValue=1)]
        public int MaxConnections
        {
            get => 
                ((int) base["maxConnections"]);
            set
            {
                base["maxConnections"] = value;
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

        [ConfigurationProperty("portSharingEnabled", DefaultValue=false)]
        public bool PortSharingEnabled
        {
            get => 
                ((bool) base["portSharingEnabled"]);
            set
            {
                base["portSharingEnabled"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("transactionFlow", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transferMode", typeof(System.ServiceModel.TransferMode), System.ServiceModel.TransferMode.Buffered, null, new ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transactionProtocol", typeof(System.ServiceModel.TransactionProtocol), "OleTransactions", new TransactionProtocolConverter(), null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("listenBacklog", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferPoolSize", typeof(long), 0x80000L, null, new LongValidator(0L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferSize", typeof(int), 0x10000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxConnections", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("portSharingEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("reliableSession", typeof(StandardBindingOptionalReliableSessionElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("security", typeof(NetTcpSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas =>
            ((XmlDictionaryReaderQuotasElement) base["readerQuotas"]);

        [ConfigurationProperty("reliableSession")]
        public StandardBindingOptionalReliableSessionElement ReliableSession =>
            ((StandardBindingOptionalReliableSessionElement) base["reliableSession"]);

        [ConfigurationProperty("security")]
        public NetTcpSecurityElement Security =>
            ((NetTcpSecurityElement) base["security"]);

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

        [ConfigurationProperty("transactionProtocol", DefaultValue="OleTransactions"), TypeConverter(typeof(TransactionProtocolConverter))]
        public System.ServiceModel.TransactionProtocol TransactionProtocol
        {
            get => 
                ((System.ServiceModel.TransactionProtocol) base["transactionProtocol"]);
            set
            {
                base["transactionProtocol"] = value;
            }
        }

        [ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationProperty("transferMode", DefaultValue=0)]
        public System.ServiceModel.TransferMode TransferMode
        {
            get => 
                ((System.ServiceModel.TransferMode) base["transferMode"]);
            set
            {
                base["transferMode"] = value;
            }
        }
    }
}

