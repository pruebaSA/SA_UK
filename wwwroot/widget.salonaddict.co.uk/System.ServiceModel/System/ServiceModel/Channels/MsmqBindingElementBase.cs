namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Xml;

    public abstract class MsmqBindingElementBase : TransportBindingElement, ITransactedBindingElement, IWsdlExportExtension, IPolicyExportExtension, ITransportPolicyImport
    {
        private Uri customDeadLetterQueue;
        private System.ServiceModel.DeadLetterQueue deadLetterQueue;
        private bool durable;
        private bool exactlyOnce;
        private int maxRetryCycles;
        private System.ServiceModel.MsmqTransportSecurity msmqTransportSecurity;
        private System.ServiceModel.ReceiveErrorHandling receiveErrorHandling;
        private int receiveRetryCount;
        private TimeSpan retryCycleDelay;
        private TimeSpan timeToLive;
        private bool useMsmqTracing;
        private bool useSourceJournal;

        internal MsmqBindingElementBase()
        {
            this.customDeadLetterQueue = null;
            this.deadLetterQueue = System.ServiceModel.DeadLetterQueue.System;
            this.durable = true;
            this.exactlyOnce = true;
            this.maxRetryCycles = 2;
            this.receiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Fault;
            this.receiveRetryCount = 5;
            this.retryCycleDelay = MsmqDefaults.RetryCycleDelay;
            this.timeToLive = MsmqDefaults.TimeToLive;
            this.msmqTransportSecurity = new System.ServiceModel.MsmqTransportSecurity();
            this.useMsmqTracing = false;
            this.useSourceJournal = false;
        }

        internal MsmqBindingElementBase(MsmqBindingElementBase elementToBeCloned) : base(elementToBeCloned)
        {
            this.customDeadLetterQueue = elementToBeCloned.customDeadLetterQueue;
            this.deadLetterQueue = elementToBeCloned.deadLetterQueue;
            this.durable = elementToBeCloned.durable;
            this.exactlyOnce = elementToBeCloned.exactlyOnce;
            this.maxRetryCycles = elementToBeCloned.maxRetryCycles;
            this.msmqTransportSecurity = new System.ServiceModel.MsmqTransportSecurity(elementToBeCloned.MsmqTransportSecurity);
            this.receiveErrorHandling = elementToBeCloned.receiveErrorHandling;
            this.receiveRetryCount = elementToBeCloned.receiveRetryCount;
            this.retryCycleDelay = elementToBeCloned.retryCycleDelay;
            this.timeToLive = elementToBeCloned.timeToLive;
            this.useMsmqTracing = elementToBeCloned.useMsmqTracing;
            this.useSourceJournal = elementToBeCloned.useSourceJournal;
        }

        private static bool FindAssertion(ICollection<XmlElement> assertions, string name) => 
            (PolicyConversionContext.FindAssertion(assertions, name, "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq", true) != null);

        private MessageEncodingBindingElement FindMessageEncodingBindingElement(BindingElementCollection bindingElements, out bool createdNew)
        {
            createdNew = false;
            MessageEncodingBindingElement element = bindingElements.Find<MessageEncodingBindingElement>();
            if (element == null)
            {
                createdNew = true;
                element = new BinaryMessageEncodingBindingElement();
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
                return default(T);
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T) new BindingDeliveryCapabilitiesHelper();
            }
            return base.GetProperty<T>(context);
        }

        void ITransportPolicyImport.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            ICollection<XmlElement> bindingAssertions = policyContext.GetBindingAssertions();
            if (FindAssertion(bindingAssertions, "MsmqVolatile"))
            {
                this.Durable = false;
            }
            if (FindAssertion(bindingAssertions, "MsmqBestEffort"))
            {
                this.ExactlyOnce = false;
            }
            if (FindAssertion(bindingAssertions, "MsmqSession"))
            {
                policyContext.Contract.SessionMode = SessionMode.Required;
            }
            if (FindAssertion(bindingAssertions, "Authenticated"))
            {
                this.MsmqTransportSecurity.MsmqProtectionLevel = ProtectionLevel.Sign;
                if (FindAssertion(bindingAssertions, "WindowsDomain"))
                {
                    this.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.WindowsDomain;
                }
                else
                {
                    this.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.Certificate;
                }
            }
            else
            {
                this.MsmqTransportSecurity.MsmqProtectionLevel = ProtectionLevel.None;
                this.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.None;
            }
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
            XmlDocument document = new XmlDocument();
            ICollection<XmlElement> bindingAssertions = context.GetBindingAssertions();
            if (!this.Durable)
            {
                bindingAssertions.Add(document.CreateElement("msmq", "MsmqVolatile", "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq"));
            }
            if (!this.ExactlyOnce)
            {
                bindingAssertions.Add(document.CreateElement("msmq", "MsmqBestEffort", "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq"));
            }
            if (context.Contract.SessionMode == SessionMode.Required)
            {
                bindingAssertions.Add(document.CreateElement("msmq", "MsmqSession", "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq"));
            }
            if (this.MsmqTransportSecurity.MsmqProtectionLevel != ProtectionLevel.None)
            {
                bindingAssertions.Add(document.CreateElement("msmq", "Authenticated", "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq"));
                if (this.MsmqTransportSecurity.MsmqAuthenticationMode == MsmqAuthenticationMode.WindowsDomain)
                {
                    bindingAssertions.Add(document.CreateElement("msmq", "WindowsDomain", "http://schemas.microsoft.com/ws/06/2004/mspolicy/msmq"));
                }
            }
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

        internal abstract MsmqUri.IAddressTranslator AddressTranslator { get; }

        public Uri CustomDeadLetterQueue
        {
            get => 
                this.customDeadLetterQueue;
            set
            {
                this.customDeadLetterQueue = value;
            }
        }

        public System.ServiceModel.DeadLetterQueue DeadLetterQueue
        {
            get => 
                this.deadLetterQueue;
            set
            {
                if (!DeadLetterQueueHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.deadLetterQueue = value;
            }
        }

        public bool Durable
        {
            get => 
                this.durable;
            set
            {
                this.durable = value;
            }
        }

        public bool ExactlyOnce
        {
            get => 
                this.exactlyOnce;
            set
            {
                this.exactlyOnce = value;
            }
        }

        public int MaxRetryCycles
        {
            get => 
                this.maxRetryCycles;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("MsmqNonNegativeArgumentExpected")));
                }
                this.maxRetryCycles = value;
            }
        }

        public System.ServiceModel.MsmqTransportSecurity MsmqTransportSecurity
        {
            get => 
                this.msmqTransportSecurity;
            internal set
            {
                this.msmqTransportSecurity = value;
            }
        }

        public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling
        {
            get => 
                this.receiveErrorHandling;
            set
            {
                if (!ReceiveErrorHandlingHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.receiveErrorHandling = value;
            }
        }

        public int ReceiveRetryCount
        {
            get => 
                this.receiveRetryCount;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("MsmqNonNegativeArgumentExpected")));
                }
                this.receiveRetryCount = value;
            }
        }

        public TimeSpan RetryCycleDelay
        {
            get => 
                this.retryCycleDelay;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
                }
                this.retryCycleDelay = value;
            }
        }

        public TimeSpan TimeToLive
        {
            get => 
                this.timeToLive;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
                }
                this.timeToLive = value;
            }
        }

        public bool TransactedReceiveEnabled =>
            this.exactlyOnce;

        public bool UseMsmqTracing
        {
            get => 
                this.useMsmqTracing;
            set
            {
                this.useMsmqTracing = value;
            }
        }

        public bool UseSourceJournal
        {
            get => 
                this.useSourceJournal;
            set
            {
                this.useSourceJournal = value;
            }
        }

        internal virtual string WsdlTransportUri =>
            null;

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            internal BindingDeliveryCapabilitiesHelper()
            {
            }

            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery =>
                false;

            bool IBindingDeliveryCapabilities.QueuedDelivery =>
                true;
        }
    }
}

