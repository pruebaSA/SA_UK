namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal sealed class WebScriptMetadataMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private XmlDictionaryReaderQuotas readerQuotas;

        public WebScriptMetadataMessageEncodingBindingElement()
        {
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            EncoderDefaults.ReaderQuotas.CopyTo(this.readerQuotas);
        }

        private WebScriptMetadataMessageEncodingBindingElement(WebScriptMetadataMessageEncodingBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            elementToBeCloned.readerQuotas.CopyTo(this.readerQuotas);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) => 
            base.InternalBuildChannelFactory<TChannel>(context);

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalBuildChannelListener<TChannel>(context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => 
            base.InternalCanBuildChannelFactory<TChannel>(context);

        public override bool CanBuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            base.InternalCanBuildChannelListener<TChannel>(context);

        public override BindingElement Clone() => 
            new WebScriptMetadataMessageEncodingBindingElement(this);

        public override MessageEncoderFactory CreateMessageEncoderFactory() => 
            new WebScriptMetadataMessageEncoderFactory(this.ReaderQuotas);

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
    }
}

