namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Xml;

    public sealed class BinaryMessageEncodingBindingElement : MessageEncodingBindingElement, IWsdlExportExtension, IPolicyExportExtension
    {
        private System.ServiceModel.Channels.BinaryVersion binaryVersion;
        private int maxReadPoolSize;
        private int maxSessionSize;
        private int maxWritePoolSize;
        private System.ServiceModel.Channels.MessageVersion messageVersion;
        private XmlDictionaryReaderQuotas readerQuotas;

        public BinaryMessageEncodingBindingElement()
        {
            this.maxReadPoolSize = 0x40;
            this.maxWritePoolSize = 0x10;
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            EncoderDefaults.ReaderQuotas.CopyTo(this.readerQuotas);
            this.maxSessionSize = 0x800;
            this.binaryVersion = BinaryEncoderDefaults.BinaryVersion;
            this.messageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(BinaryEncoderDefaults.EnvelopeVersion);
        }

        private BinaryMessageEncodingBindingElement(BinaryMessageEncodingBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            this.maxReadPoolSize = elementToBeCloned.maxReadPoolSize;
            this.maxWritePoolSize = elementToBeCloned.maxWritePoolSize;
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            elementToBeCloned.readerQuotas.CopyTo(this.readerQuotas);
            this.MaxSessionSize = elementToBeCloned.MaxSessionSize;
            this.BinaryVersion = elementToBeCloned.BinaryVersion;
            this.messageVersion = elementToBeCloned.messageVersion;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) => 
            base.InternalBuildChannelFactory<TChannel>(context);

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalBuildChannelListener<TChannel>(context);

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalCanBuildChannelListener<TChannel>(context);

        public override BindingElement Clone() => 
            new BinaryMessageEncodingBindingElement(this);

        public override MessageEncoderFactory CreateMessageEncoderFactory() => 
            new BinaryMessageEncoderFactory(this.MessageVersion, this.MaxReadPoolSize, this.MaxWritePoolSize, this.MaxSessionSize, this.ReaderQuotas, this.BinaryVersion);

        public override T GetProperty<T>(BindingContext context) where T: class
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T) this.readerQuotas;
            }
            return base.GetProperty<T>(context);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }
            BinaryMessageEncodingBindingElement element = b as BinaryMessageEncodingBindingElement;
            if (element == null)
            {
                return false;
            }
            if (this.maxReadPoolSize != element.MaxReadPoolSize)
            {
                return false;
            }
            if (this.maxWritePoolSize != element.MaxWritePoolSize)
            {
                return false;
            }
            if (this.readerQuotas.MaxStringContentLength != element.ReaderQuotas.MaxStringContentLength)
            {
                return false;
            }
            if (this.readerQuotas.MaxArrayLength != element.ReaderQuotas.MaxArrayLength)
            {
                return false;
            }
            if (this.readerQuotas.MaxBytesPerRead != element.ReaderQuotas.MaxBytesPerRead)
            {
                return false;
            }
            if (this.readerQuotas.MaxDepth != element.ReaderQuotas.MaxDepth)
            {
                return false;
            }
            if (this.readerQuotas.MaxNameTableCharCount != element.ReaderQuotas.MaxNameTableCharCount)
            {
                return false;
            }
            if (this.MaxSessionSize != element.MaxSessionSize)
            {
                return false;
            }
            return true;
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("policyContext");
            }
            XmlDocument document = new XmlDocument();
            policyContext.GetBindingAssertions().Add(document.CreateElement("msb", "BinaryEncoding", "http://schemas.microsoft.com/ws/06/2004/mspolicy/netbinary1"));
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            SoapHelper.SetSoapVersion(context, exporter, System.ServiceModel.Channels.MessageVersion.Soap12WSAddressing10.Envelope);
        }

        private System.ServiceModel.Channels.BinaryVersion BinaryVersion
        {
            get => 
                this.binaryVersion;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                this.binaryVersion = value;
            }
        }

        public int MaxReadPoolSize
        {
            get => 
                this.maxReadPoolSize;
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.maxReadPoolSize = value;
            }
        }

        public int MaxSessionSize
        {
            get => 
                this.maxSessionSize;
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.maxSessionSize = value;
            }
        }

        public int MaxWritePoolSize
        {
            get => 
                this.maxWritePoolSize;
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.maxWritePoolSize = value;
            }
        }

        public override System.ServiceModel.Channels.MessageVersion MessageVersion
        {
            get => 
                this.messageVersion;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value.Envelope != BinaryEncoderDefaults.EnvelopeVersion)
                {
                    string message = System.ServiceModel.SR.GetString("UnsupportedEnvelopeVersion", new object[] { base.GetType().FullName, BinaryEncoderDefaults.EnvelopeVersion, value.Envelope });
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(message));
                }
                this.messageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(BinaryEncoderDefaults.EnvelopeVersion, value.Addressing);
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas =>
            this.readerQuotas;
    }
}

