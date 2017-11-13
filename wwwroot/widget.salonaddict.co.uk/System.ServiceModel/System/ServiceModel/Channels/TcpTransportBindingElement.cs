namespace System.ServiceModel.Channels
{
    using System;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    public class TcpTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        private TcpConnectionPoolSettings connectionPoolSettings;
        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy;
        private int listenBacklog;
        private bool portSharingEnabled;
        private bool teredoEnabled;

        public TcpTransportBindingElement()
        {
            this.listenBacklog = 10;
            this.portSharingEnabled = false;
            this.teredoEnabled = false;
            this.connectionPoolSettings = new TcpConnectionPoolSettings();
            this.extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        }

        protected TcpTransportBindingElement(TcpTransportBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            this.listenBacklog = elementToBeCloned.listenBacklog;
            this.portSharingEnabled = elementToBeCloned.portSharingEnabled;
            this.teredoEnabled = elementToBeCloned.teredoEnabled;
            this.connectionPoolSettings = elementToBeCloned.connectionPoolSettings.Clone();
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { typeof(TChannel) }));
            }
            return (IChannelFactory<TChannel>) new TcpChannelFactory<TChannel>(this, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel
        {
            TcpChannelListener listener;
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (!this.CanBuildChannelListener<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { typeof(TChannel) }));
            }
            if (typeof(TChannel) == typeof(IReplyChannel))
            {
                listener = new TcpReplyChannelListener(this, context);
            }
            else
            {
                if (typeof(TChannel) != typeof(IDuplexSessionChannel))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { typeof(TChannel) }));
                }
                listener = new TcpDuplexChannelListener(this, context);
            }
            VirtualPathExtension.ApplyHostedContext(listener, context);
            return (IChannelListener<TChannel>) listener;
        }

        public override BindingElement Clone() => 
            new TcpTransportBindingElement(this);

        public override T GetProperty<T>(BindingContext context) where T: class
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T) new BindingDeliveryCapabilitiesHelper();
            }
            if (typeof(T) == typeof(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy))
            {
                return (T) this.ExtendedProtectionPolicy;
            }
            return base.GetProperty<T>(context);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }
            TcpTransportBindingElement element = b as TcpTransportBindingElement;
            if (element == null)
            {
                return false;
            }
            if (this.listenBacklog != element.listenBacklog)
            {
                return false;
            }
            if (this.portSharingEnabled != element.portSharingEnabled)
            {
                return false;
            }
            if (this.teredoEnabled != element.teredoEnabled)
            {
                return false;
            }
            if (!this.connectionPoolSettings.IsMatch(element.connectionPoolSettings))
            {
                return false;
            }
            if (!ChannelBindingUtility.ValidatePolicies(this.ExtendedProtectionPolicy, element.ExtendedProtectionPolicy, false))
            {
                return false;
            }
            return true;
        }

        public TcpConnectionPoolSettings ConnectionPoolSettings =>
            this.connectionPoolSettings;

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

        public int ListenBacklog
        {
            get => 
                this.listenBacklog;
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.listenBacklog = value;
            }
        }

        public bool PortSharingEnabled
        {
            get => 
                this.portSharingEnabled;
            set
            {
                this.portSharingEnabled = value;
            }
        }

        public override string Scheme =>
            "net.tcp";

        public bool TeredoEnabled
        {
            get => 
                this.teredoEnabled;
            set
            {
                this.teredoEnabled = value;
            }
        }

        internal override string WsdlTransportUri =>
            "http://schemas.microsoft.com/soap/tcp";

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            internal BindingDeliveryCapabilitiesHelper()
            {
            }

            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery =>
                true;

            bool IBindingDeliveryCapabilities.QueuedDelivery =>
                false;
        }
    }
}

