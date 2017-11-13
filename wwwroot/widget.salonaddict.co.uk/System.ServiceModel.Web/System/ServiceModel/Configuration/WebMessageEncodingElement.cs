namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Xml;

    public sealed class WebMessageEncodingElement : BindingElementExtensionElement
    {
        private const string ConfigurationStringsWebContentTypeMapperType = "webContentTypeMapperType";

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            WebMessageEncodingBindingElement element = (WebMessageEncodingBindingElement) bindingElement;
            element.WriteEncoding = this.WriteEncoding;
            element.MaxReadPoolSize = this.MaxReadPoolSize;
            element.MaxWritePoolSize = this.MaxWritePoolSize;
            if (!string.IsNullOrEmpty(this.WebContentTypeMapperType))
            {
                Type c = Type.GetType(this.WebContentTypeMapperType, true);
                if (!typeof(WebContentTypeMapper).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR2.GetString(SR2.ConfigInvalidWebContentTypeMapper, new object[] { c, "webContentTypeMapperType", typeof(WebMessageEncodingBindingElement), typeof(WebContentTypeMapper) })));
                }
                try
                {
                    element.ContentTypeMapper = (WebContentTypeMapper) Activator.CreateInstance(c);
                }
                catch (MissingMethodException exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR2.GetString(SR2.ConfigWebContentTypeMapperNoConstructor, new object[] { c, "webContentTypeMapperType", typeof(WebMessageEncodingBindingElement), typeof(WebContentTypeMapper) }), exception));
                }
            }
            this.ApplyConfiguration(this.ReaderQuotas, element.ReaderQuotas);
        }

        internal void ApplyConfiguration(XmlDictionaryReaderQuotasElement currentQuotas, XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("readerQuotas");
            }
            if (currentQuotas.MaxDepth != 0)
            {
                readerQuotas.MaxDepth = currentQuotas.MaxDepth;
            }
            if (currentQuotas.MaxStringContentLength != 0)
            {
                readerQuotas.MaxStringContentLength = currentQuotas.MaxStringContentLength;
            }
            if (currentQuotas.MaxArrayLength != 0)
            {
                readerQuotas.MaxArrayLength = currentQuotas.MaxArrayLength;
            }
            if (currentQuotas.MaxBytesPerRead != 0)
            {
                readerQuotas.MaxBytesPerRead = currentQuotas.MaxBytesPerRead;
            }
            if (currentQuotas.MaxNameTableCharCount != 0)
            {
                readerQuotas.MaxNameTableCharCount = currentQuotas.MaxNameTableCharCount;
            }
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            WebMessageEncodingElement element = (WebMessageEncodingElement) from;
            this.WriteEncoding = element.WriteEncoding;
            this.MaxReadPoolSize = element.MaxReadPoolSize;
            this.MaxWritePoolSize = element.MaxWritePoolSize;
            this.WebContentTypeMapperType = element.WebContentTypeMapperType;
            this.ReaderQuotas.MaxArrayLength = element.ReaderQuotas.MaxArrayLength;
            this.ReaderQuotas.MaxBytesPerRead = element.ReaderQuotas.MaxBytesPerRead;
            this.ReaderQuotas.MaxDepth = element.ReaderQuotas.MaxDepth;
            this.ReaderQuotas.MaxNameTableCharCount = element.ReaderQuotas.MaxNameTableCharCount;
            this.ReaderQuotas.MaxStringContentLength = element.ReaderQuotas.MaxStringContentLength;
        }

        protected internal override BindingElement CreateBindingElement()
        {
            WebMessageEncodingBindingElement bindingElement = new WebMessageEncodingBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        public override Type BindingElementType =>
            typeof(WebMessageEncodingBindingElement);

        [ConfigurationProperty("maxReadPoolSize", DefaultValue=0x40), IntegerValidator(MinValue=1)]
        public int MaxReadPoolSize
        {
            get => 
                ((int) base["maxReadPoolSize"]);
            set
            {
                base["maxReadPoolSize"] = value;
            }
        }

        [ConfigurationProperty("maxWritePoolSize", DefaultValue=0x10), IntegerValidator(MinValue=1)]
        public int MaxWritePoolSize
        {
            get => 
                ((int) base["maxWritePoolSize"]);
            set
            {
                base["maxWritePoolSize"] = value;
            }
        }

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas =>
            ((XmlDictionaryReaderQuotasElement) base["readerQuotas"]);

        [StringValidator(MinLength=0), ConfigurationProperty("webContentTypeMapperType", DefaultValue="")]
        public string WebContentTypeMapperType
        {
            get => 
                ((string) base["webContentTypeMapperType"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["webContentTypeMapperType"] = value;
            }
        }

        [WebEncodingValidator, TypeConverter(typeof(EncodingConverter)), ConfigurationProperty("writeEncoding", DefaultValue="utf-8")]
        public Encoding WriteEncoding
        {
            get => 
                ((Encoding) base["writeEncoding"]);
            set
            {
                base["writeEncoding"] = value;
            }
        }
    }
}

