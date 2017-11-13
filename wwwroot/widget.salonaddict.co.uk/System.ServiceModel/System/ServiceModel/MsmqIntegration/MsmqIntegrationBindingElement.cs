namespace System.ServiceModel.MsmqIntegration
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class MsmqIntegrationBindingElement : MsmqBindingElementBase
    {
        private MsmqMessageSerializationFormat serializationFormat;
        private Type[] targetSerializationTypes;

        public MsmqIntegrationBindingElement()
        {
            this.serializationFormat = MsmqMessageSerializationFormat.Xml;
        }

        private MsmqIntegrationBindingElement(MsmqIntegrationBindingElement other) : base(other)
        {
            this.serializationFormat = other.serializationFormat;
            if (other.targetSerializationTypes != null)
            {
                this.targetSerializationTypes = other.targetSerializationTypes.Clone() as Type[];
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(TChannel) != typeof(IOutputChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { typeof(TChannel) }));
            }
            MsmqChannelFactoryBase<IOutputChannel> factory = new MsmqIntegrationChannelFactory(this, context);
            MsmqVerifier.VerifySender<IOutputChannel>(factory);
            return (IChannelFactory<TChannel>) factory;
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(TChannel) != typeof(IInputChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { typeof(TChannel) }));
            }
            MsmqIntegrationReceiveParameters receiveParameters = new MsmqIntegrationReceiveParameters(this);
            MsmqIntegrationChannelListener listener = new MsmqIntegrationChannelListener(this, context, receiveParameters);
            MsmqVerifier.VerifyReceiver(receiveParameters, listener.Uri);
            return (IChannelListener<TChannel>) listener;
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => 
            (typeof(TChannel) == typeof(IOutputChannel));

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            (typeof(TChannel) == typeof(IInputChannel));

        public override BindingElement Clone() => 
            new MsmqIntegrationBindingElement(this);

        public override T GetProperty<T>(BindingContext context) where T: class
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T) MessageVersion.None;
            }
            return base.GetProperty<T>(context);
        }

        internal override MsmqUri.IAddressTranslator AddressTranslator =>
            MsmqUri.FormatNameAddressTranslator;

        public override string Scheme =>
            "msmq.formatname";

        public MsmqMessageSerializationFormat SerializationFormat
        {
            get => 
                this.serializationFormat;
            set
            {
                if (!MsmqMessageSerializationFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.serializationFormat = value;
            }
        }

        public Type[] TargetSerializationTypes
        {
            get
            {
                if (this.targetSerializationTypes == null)
                {
                    return null;
                }
                return (this.targetSerializationTypes.Clone() as Type[]);
            }
            set
            {
                if (value == null)
                {
                    this.targetSerializationTypes = null;
                }
                else
                {
                    this.targetSerializationTypes = value.Clone() as Type[];
                }
            }
        }
    }
}

