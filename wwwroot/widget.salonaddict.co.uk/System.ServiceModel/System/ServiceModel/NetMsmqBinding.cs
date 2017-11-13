namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.Xml;

    public class NetMsmqBinding : MsmqBindingBase
    {
        private BinaryMessageEncodingBindingElement encoding;
        private NetMsmqSecurity security;

        public NetMsmqBinding()
        {
            this.Initialize();
            this.security = new NetMsmqSecurity();
        }

        private NetMsmqBinding(NetMsmqSecurity security)
        {
            this.Initialize();
            this.security = security;
        }

        public NetMsmqBinding(NetMsmqSecurityMode securityMode)
        {
            if (!NetMsmqSecurityModeHelper.IsDefined(securityMode))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("mode", (int) securityMode, typeof(NetMsmqSecurityMode)));
            }
            this.Initialize();
            this.security = new NetMsmqSecurity(securityMode);
        }

        public NetMsmqBinding(string configurationName)
        {
            this.Initialize();
            this.security = new NetMsmqSecurity();
            this.ApplyConfiguration(configurationName);
        }

        private void ApplyConfiguration(string configurationName)
        {
            NetMsmqBindingElement element2 = NetMsmqBindingCollectionElement.GetBindingCollectionElement().Bindings[configurationName];
            if (element2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidBindingConfigurationName", new object[] { configurationName, "netMsmqBinding" })));
            }
            element2.ApplyConfiguration(this);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            SecurityBindingElement item = this.CreateMessageSecurity();
            if (item != null)
            {
                elements.Add(item);
            }
            elements.Add(this.encoding);
            elements.Add(this.GetTransport());
            return elements.Clone();
        }

        private SecurityBindingElement CreateMessageSecurity()
        {
            if ((this.security.Mode != NetMsmqSecurityMode.Message) && (this.security.Mode != NetMsmqSecurityMode.Both))
            {
                return null;
            }
            return this.security.CreateMessageSecurity();
        }

        private System.ServiceModel.Channels.MsmqBindingElementBase GetTransport()
        {
            this.security.ConfigureTransportSecurity(base.transport);
            return base.transport;
        }

        private void Initialize()
        {
            base.transport = new MsmqTransportBindingElement();
            this.encoding = new BinaryMessageEncodingBindingElement();
        }

        private void InitializeFrom(MsmqTransportBindingElement transport, BinaryMessageEncodingBindingElement encoding)
        {
            base.CustomDeadLetterQueue = transport.CustomDeadLetterQueue;
            base.DeadLetterQueue = transport.DeadLetterQueue;
            base.Durable = transport.Durable;
            base.ExactlyOnce = transport.ExactlyOnce;
            base.MaxReceivedMessageSize = transport.MaxReceivedMessageSize;
            base.ReceiveRetryCount = transport.ReceiveRetryCount;
            base.MaxRetryCycles = transport.MaxRetryCycles;
            base.ReceiveErrorHandling = transport.ReceiveErrorHandling;
            base.RetryCycleDelay = transport.RetryCycleDelay;
            base.TimeToLive = transport.TimeToLive;
            base.UseSourceJournal = transport.UseSourceJournal;
            base.UseMsmqTracing = transport.UseMsmqTracing;
            this.QueueTransferProtocol = transport.QueueTransferProtocol;
            this.MaxBufferPoolSize = transport.MaxBufferPoolSize;
            this.UseActiveDirectory = transport.UseActiveDirectory;
            this.ReaderQuotas = encoding.ReaderQuotas;
        }

        private bool IsBindingElementsMatch(MsmqTransportBindingElement transport, MessageEncodingBindingElement encoding)
        {
            if (!this.GetTransport().IsMatch(transport))
            {
                return false;
            }
            if (!this.encoding.IsMatch(encoding))
            {
                return false;
            }
            return true;
        }

        private static bool IsValidTransport(MsmqTransportBindingElement msmq, out UnifiedSecurityMode mode)
        {
            mode = 0;
            if (msmq == null)
            {
                return false;
            }
            return NetMsmqSecurity.IsConfiguredTransportSecurity(msmq, out mode);
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            UnifiedSecurityMode mode;
            NetMsmqSecurity security;
            binding = null;
            if (elements.Count > 3)
            {
                return false;
            }
            SecurityBindingElement sbe = null;
            BinaryMessageEncodingBindingElement encoding = null;
            MsmqTransportBindingElement msmq = null;
            foreach (BindingElement element4 in elements)
            {
                if (element4 is SecurityBindingElement)
                {
                    sbe = element4 as SecurityBindingElement;
                }
                else if (element4 is TransportBindingElement)
                {
                    msmq = element4 as MsmqTransportBindingElement;
                }
                else if (element4 is MessageEncodingBindingElement)
                {
                    encoding = element4 as BinaryMessageEncodingBindingElement;
                }
                else
                {
                    return false;
                }
            }
            if (!IsValidTransport(msmq, out mode))
            {
                return false;
            }
            if (encoding == null)
            {
                return false;
            }
            if (!TryCreateSecurity(sbe, mode, out security))
            {
                return false;
            }
            NetMsmqBinding binding2 = new NetMsmqBinding(security);
            binding2.InitializeFrom(msmq, encoding);
            if (!binding2.IsBindingElementsMatch(msmq, encoding))
            {
                return false;
            }
            binding = binding2;
            return true;
        }

        private static bool TryCreateSecurity(SecurityBindingElement sbe, UnifiedSecurityMode mode, out NetMsmqSecurity security)
        {
            if (sbe != null)
            {
                mode &= UnifiedSecurityMode.Both | UnifiedSecurityMode.Message;
            }
            else
            {
                mode &= ~(UnifiedSecurityMode.Both | UnifiedSecurityMode.Message);
            }
            NetMsmqSecurityMode mode2 = NetMsmqSecurityModeHelper.ToSecurityMode(mode);
            return NetMsmqSecurity.TryCreate(sbe, mode2, out security);
        }

        public System.ServiceModel.EnvelopeVersion EnvelopeVersion =>
            System.ServiceModel.EnvelopeVersion.Soap12;

        public long MaxBufferPoolSize
        {
            get => 
                base.transport.MaxBufferPoolSize;
            set
            {
                base.transport.MaxBufferPoolSize = value;
            }
        }

        internal int MaxPoolSize
        {
            get => 
                (base.transport as MsmqTransportBindingElement).MaxPoolSize;
            set
            {
                (base.transport as MsmqTransportBindingElement).MaxPoolSize = value;
            }
        }

        public System.ServiceModel.QueueTransferProtocol QueueTransferProtocol
        {
            get => 
                (base.transport as MsmqTransportBindingElement).QueueTransferProtocol;
            set
            {
                (base.transport as MsmqTransportBindingElement).QueueTransferProtocol = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get => 
                this.encoding.ReaderQuotas;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                value.CopyTo(this.encoding.ReaderQuotas);
            }
        }

        public NetMsmqSecurity Security =>
            this.security;

        public bool UseActiveDirectory
        {
            get => 
                (base.transport as MsmqTransportBindingElement).UseActiveDirectory;
            set
            {
                (base.transport as MsmqTransportBindingElement).UseActiveDirectory = value;
            }
        }
    }
}

