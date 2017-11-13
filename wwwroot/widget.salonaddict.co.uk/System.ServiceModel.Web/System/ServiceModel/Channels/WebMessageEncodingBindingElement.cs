namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Administration;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Xml;

    public sealed class WebMessageEncodingBindingElement : MessageEncodingBindingElement, IWsdlExportExtension, IWmiInstanceProvider
    {
        private WebContentTypeMapper contentTypeMapper;
        private int maxReadPoolSize;
        private int maxWritePoolSize;
        private XmlDictionaryReaderQuotas readerQuotas;
        private Encoding writeEncoding;

        public WebMessageEncodingBindingElement() : this(TextEncoderDefaults.Encoding)
        {
        }

        private WebMessageEncodingBindingElement(WebMessageEncodingBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            this.maxReadPoolSize = elementToBeCloned.maxReadPoolSize;
            this.maxWritePoolSize = elementToBeCloned.maxWritePoolSize;
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            elementToBeCloned.readerQuotas.CopyTo(this.readerQuotas);
            this.writeEncoding = elementToBeCloned.writeEncoding;
            this.contentTypeMapper = elementToBeCloned.contentTypeMapper;
        }

        public WebMessageEncodingBindingElement(Encoding writeEncoding)
        {
            if (writeEncoding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writeEncoding");
            }
            TextEncoderDefaults.ValidateEncoding(writeEncoding);
            this.maxReadPoolSize = 0x40;
            this.maxWritePoolSize = 0x10;
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            EncoderDefaults.ReaderQuotas.CopyTo(this.readerQuotas);
            this.writeEncoding = writeEncoding;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) => 
            base.InternalBuildChannelFactory<TChannel>(context);

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalBuildChannelListener<TChannel>(context);

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalCanBuildChannelListener<TChannel>(context);

        internal override bool CheckEncodingVersion(EnvelopeVersion version) => 
            (this.MessageVersion.Envelope == version);

        public override BindingElement Clone() => 
            new WebMessageEncodingBindingElement(this);

        public override MessageEncoderFactory CreateMessageEncoderFactory() => 
            new WebMessageEncoderFactory(this.WriteEncoding, this.MaxReadPoolSize, this.MaxWritePoolSize, this.ReaderQuotas, this.ContentTypeMapper);

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
            WebMessageEncodingBindingElement element = b as WebMessageEncodingBindingElement;
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
            if (this.WriteEncoding.EncodingName != element.WriteEncoding.EncodingName)
            {
                return false;
            }
            if (!this.MessageVersion.IsMatch(element.MessageVersion))
            {
                return false;
            }
            if (this.ContentTypeMapper != element.ContentTypeMapper)
            {
                return false;
            }
            return true;
        }

        void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
        {
            wmiInstance.SetProperty("MessageVersion", this.MessageVersion.ToString());
            wmiInstance.SetProperty("Encoding", this.writeEncoding.WebName);
            wmiInstance.SetProperty("MaxReadPoolSize", this.maxReadPoolSize);
            wmiInstance.SetProperty("MaxWritePoolSize", this.maxWritePoolSize);
            if (this.ReaderQuotas != null)
            {
                IWmiInstance instance = wmiInstance.NewInstance("XmlDictionaryReaderQuotas");
                instance.SetProperty("MaxArrayLength", this.readerQuotas.MaxArrayLength);
                instance.SetProperty("MaxBytesPerRead", this.readerQuotas.MaxBytesPerRead);
                instance.SetProperty("MaxDepth", this.readerQuotas.MaxDepth);
                instance.SetProperty("MaxNameTableCharCount", this.readerQuotas.MaxNameTableCharCount);
                instance.SetProperty("MaxStringContentLength", this.readerQuotas.MaxStringContentLength);
                wmiInstance.SetProperty("ReaderQuotas", instance);
            }
        }

        string IWmiInstanceProvider.GetInstanceType() => 
            typeof(WebMessageEncodingBindingElement).Name;

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            SoapHelper.SetSoapVersion(context, exporter, this.MessageVersion.Envelope);
        }

        public WebContentTypeMapper ContentTypeMapper
        {
            get => 
                this.contentTypeMapper;
            set
            {
                this.contentTypeMapper = value;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SR2.GetString(SR2.ValueMustBePositive, new object[0])));
                }
                this.maxReadPoolSize = value;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SR2.GetString(SR2.ValueMustBePositive, new object[0])));
                }
                this.maxWritePoolSize = value;
            }
        }

        public override System.ServiceModel.Channels.MessageVersion MessageVersion
        {
            get => 
                System.ServiceModel.Channels.MessageVersion.None;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value != System.ServiceModel.Channels.MessageVersion.None)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR2.GetString(SR2.JsonOnlySupportsMessageVersionNone, new object[0]));
                }
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas =>
            this.readerQuotas;

        public Encoding WriteEncoding
        {
            get => 
                this.writeEncoding;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                TextEncoderDefaults.ValidateEncoding(value);
                this.writeEncoding = value;
            }
        }
    }
}

