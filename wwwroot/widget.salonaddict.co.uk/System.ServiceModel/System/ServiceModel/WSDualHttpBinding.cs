namespace System.ServiceModel
{
    using System;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.Text;
    using System.Xml;

    public class WSDualHttpBinding : Binding, IBindingRuntimePreferences
    {
        private CompositeDuplexBindingElement compositeDuplex;
        private HttpTransportBindingElement httpTransport;
        private WSMessageEncoding messageEncoding;
        private MtomMessageEncodingBindingElement mtomEncoding;
        private OneWayBindingElement oneWay;
        private System.ServiceModel.ReliableSession reliableSession;
        private WSDualHttpSecurity security;
        private ReliableSessionBindingElement session;
        private TextMessageEncodingBindingElement textEncoding;
        private TransactionFlowBindingElement txFlow;

        public WSDualHttpBinding()
        {
            this.security = new WSDualHttpSecurity();
            this.Initialize();
        }

        public WSDualHttpBinding(WSDualHttpSecurityMode securityMode) : this()
        {
            this.security.Mode = securityMode;
        }

        public WSDualHttpBinding(string configName) : this()
        {
            this.ApplyConfiguration(configName);
        }

        private WSDualHttpBinding(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, TransactionFlowBindingElement txFlow, ReliableSessionBindingElement session, CompositeDuplexBindingElement compositeDuplex, OneWayBindingElement oneWay, WSDualHttpSecurity security) : this()
        {
            this.security = security;
            this.InitializeFrom(transport, encoding, txFlow, session, compositeDuplex, oneWay);
        }

        private void ApplyConfiguration(string configurationName)
        {
            WSDualHttpBindingElement element2 = WSDualHttpBindingCollectionElement.GetBindingCollectionElement().Bindings[configurationName];
            if (element2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidBindingConfigurationName", new object[] { configurationName, "wsDualHttpBinding" })));
            }
            element2.ApplyConfiguration(this);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            elements.Add(this.txFlow);
            elements.Add(this.session);
            SecurityBindingElement item = this.CreateMessageSecurity();
            if (item != null)
            {
                elements.Add(item);
            }
            elements.Add(this.compositeDuplex);
            elements.Add(this.oneWay);
            WSMessageEncodingHelper.SyncUpEncodingBindingElementProperties(this.textEncoding, this.mtomEncoding);
            if (this.MessageEncoding == WSMessageEncoding.Text)
            {
                elements.Add(this.textEncoding);
            }
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
            {
                elements.Add(this.mtomEncoding);
            }
            elements.Add(this.httpTransport);
            return elements.Clone();
        }

        private SecurityBindingElement CreateMessageSecurity() => 
            this.Security.CreateMessageSecurity();

        private static TransactionFlowBindingElement GetDefaultTransactionFlowBindingElement() => 
            new TransactionFlowBindingElement(false) { TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004 };

        private void Initialize()
        {
            this.httpTransport = new HttpTransportBindingElement();
            this.messageEncoding = WSMessageEncoding.Text;
            this.txFlow = GetDefaultTransactionFlowBindingElement();
            this.session = new ReliableSessionBindingElement(true);
            this.textEncoding = new TextMessageEncodingBindingElement();
            this.textEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            this.mtomEncoding = new MtomMessageEncodingBindingElement();
            this.mtomEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            this.compositeDuplex = new CompositeDuplexBindingElement();
            this.reliableSession = new System.ServiceModel.ReliableSession(this.session);
            this.oneWay = new OneWayBindingElement();
        }

        private void InitializeFrom(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, TransactionFlowBindingElement txFlow, ReliableSessionBindingElement session, CompositeDuplexBindingElement compositeDuplex, OneWayBindingElement oneWay)
        {
            this.BypassProxyOnLocal = transport.BypassProxyOnLocal;
            this.HostNameComparisonMode = transport.HostNameComparisonMode;
            this.MaxBufferPoolSize = transport.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = transport.MaxReceivedMessageSize;
            this.ProxyAddress = transport.ProxyAddress;
            this.UseDefaultWebProxy = transport.UseDefaultWebProxy;
            if (encoding is TextMessageEncodingBindingElement)
            {
                this.MessageEncoding = WSMessageEncoding.Text;
                TextMessageEncodingBindingElement element = (TextMessageEncodingBindingElement) encoding;
                this.TextEncoding = element.WriteEncoding;
                this.ReaderQuotas = element.ReaderQuotas;
            }
            else if (encoding is MtomMessageEncodingBindingElement)
            {
                this.messageEncoding = WSMessageEncoding.Mtom;
                MtomMessageEncodingBindingElement element2 = (MtomMessageEncodingBindingElement) encoding;
                this.TextEncoding = element2.WriteEncoding;
                this.ReaderQuotas = element2.ReaderQuotas;
            }
            this.TransactionFlow = txFlow.Transactions;
            this.ClientBaseAddress = compositeDuplex.ClientBaseAddress;
            if (session != null)
            {
                this.session.InactivityTimeout = session.InactivityTimeout;
                this.session.Ordered = session.Ordered;
            }
        }

        private bool IsBindingElementsMatch(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, TransactionFlowBindingElement txFlow, ReliableSessionBindingElement session, CompositeDuplexBindingElement compositeDuplex, OneWayBindingElement oneWay)
        {
            if (!this.httpTransport.IsMatch(transport))
            {
                return false;
            }
            if (this.MessageEncoding == WSMessageEncoding.Text)
            {
                if (!this.textEncoding.IsMatch(encoding))
                {
                    return false;
                }
            }
            else if ((this.MessageEncoding == WSMessageEncoding.Mtom) && !this.mtomEncoding.IsMatch(encoding))
            {
                return false;
            }
            if (!this.txFlow.IsMatch(txFlow))
            {
                return false;
            }
            if (!this.session.IsMatch(session))
            {
                return false;
            }
            if (!this.compositeDuplex.IsMatch(compositeDuplex))
            {
                return false;
            }
            if (!this.oneWay.IsMatch(oneWay))
            {
                return false;
            }
            return true;
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            WSDualHttpSecurity security;
            binding = null;
            if (elements.Count > 7)
            {
                return false;
            }
            SecurityBindingElement securityElement = null;
            HttpTransportBindingElement transport = null;
            MessageEncodingBindingElement encoding = null;
            TransactionFlowBindingElement txFlow = null;
            ReliableSessionBindingElement session = null;
            CompositeDuplexBindingElement compositeDuplex = null;
            OneWayBindingElement oneWay = null;
            foreach (BindingElement element8 in elements)
            {
                if (element8 is SecurityBindingElement)
                {
                    securityElement = element8 as SecurityBindingElement;
                }
                else if (element8 is TransportBindingElement)
                {
                    transport = element8 as HttpTransportBindingElement;
                }
                else if (element8 is MessageEncodingBindingElement)
                {
                    encoding = element8 as MessageEncodingBindingElement;
                }
                else if (element8 is TransactionFlowBindingElement)
                {
                    txFlow = element8 as TransactionFlowBindingElement;
                }
                else if (element8 is ReliableSessionBindingElement)
                {
                    session = element8 as ReliableSessionBindingElement;
                }
                else if (element8 is CompositeDuplexBindingElement)
                {
                    compositeDuplex = element8 as CompositeDuplexBindingElement;
                }
                else if (element8 is OneWayBindingElement)
                {
                    oneWay = element8 as OneWayBindingElement;
                }
                else
                {
                    return false;
                }
            }
            if (transport == null)
            {
                return false;
            }
            if (encoding == null)
            {
                return false;
            }
            if (!encoding.CheckEncodingVersion(System.ServiceModel.EnvelopeVersion.Soap12))
            {
                return false;
            }
            if (compositeDuplex == null)
            {
                return false;
            }
            if (oneWay == null)
            {
                return false;
            }
            if (session == null)
            {
                return false;
            }
            if (txFlow == null)
            {
                txFlow = GetDefaultTransactionFlowBindingElement();
            }
            if (!TryCreateSecurity(securityElement, out security))
            {
                return false;
            }
            WSDualHttpBinding binding2 = new WSDualHttpBinding(transport, encoding, txFlow, session, compositeDuplex, oneWay, security);
            if (!binding2.IsBindingElementsMatch(transport, encoding, txFlow, session, compositeDuplex, oneWay))
            {
                return false;
            }
            binding = binding2;
            return true;
        }

        private static bool TryCreateSecurity(SecurityBindingElement securityElement, out WSDualHttpSecurity security) => 
            WSDualHttpSecurity.TryCreate(securityElement, out security);

        public bool BypassProxyOnLocal
        {
            get => 
                this.httpTransport.BypassProxyOnLocal;
            set
            {
                this.httpTransport.BypassProxyOnLocal = value;
            }
        }

        public Uri ClientBaseAddress
        {
            get => 
                this.compositeDuplex.ClientBaseAddress;
            set
            {
                this.compositeDuplex.ClientBaseAddress = value;
            }
        }

        public System.ServiceModel.EnvelopeVersion EnvelopeVersion =>
            System.ServiceModel.EnvelopeVersion.Soap12;

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                this.httpTransport.HostNameComparisonMode;
            set
            {
                this.httpTransport.HostNameComparisonMode = value;
            }
        }

        public long MaxBufferPoolSize
        {
            get => 
                this.httpTransport.MaxBufferPoolSize;
            set
            {
                this.httpTransport.MaxBufferPoolSize = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get => 
                this.httpTransport.MaxReceivedMessageSize;
            set
            {
                if (value > 0x7fffffffL)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value.MaxReceivedMessageSize", System.ServiceModel.SR.GetString("MaxReceivedMessageSizeMustBeInIntegerRange")));
                }
                this.httpTransport.MaxReceivedMessageSize = value;
                this.mtomEncoding.MaxBufferSize = (int) value;
            }
        }

        public WSMessageEncoding MessageEncoding
        {
            get => 
                this.messageEncoding;
            set
            {
                this.messageEncoding = value;
            }
        }

        public Uri ProxyAddress
        {
            get => 
                this.httpTransport.ProxyAddress;
            set
            {
                this.httpTransport.ProxyAddress = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get => 
                this.textEncoding.ReaderQuotas;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                value.CopyTo(this.textEncoding.ReaderQuotas);
                value.CopyTo(this.mtomEncoding.ReaderQuotas);
            }
        }

        public System.ServiceModel.ReliableSession ReliableSession =>
            this.reliableSession;

        public override string Scheme =>
            this.httpTransport.Scheme;

        public WSDualHttpSecurity Security =>
            this.security;

        bool IBindingRuntimePreferences.ReceiveSynchronously =>
            false;

        public Encoding TextEncoding
        {
            get => 
                this.textEncoding.WriteEncoding;
            set
            {
                this.textEncoding.WriteEncoding = value;
                this.mtomEncoding.WriteEncoding = value;
            }
        }

        public bool TransactionFlow
        {
            get => 
                this.txFlow.Transactions;
            set
            {
                this.txFlow.Transactions = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get => 
                this.httpTransport.UseDefaultWebProxy;
            set
            {
                this.httpTransport.UseDefaultWebProxy = value;
            }
        }
    }
}

