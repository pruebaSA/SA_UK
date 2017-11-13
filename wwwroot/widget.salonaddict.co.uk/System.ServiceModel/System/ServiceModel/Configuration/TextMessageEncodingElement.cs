﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.Text;

    public sealed class TextMessageEncodingElement : BindingElementExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            TextMessageEncodingBindingElement element = (TextMessageEncodingBindingElement) bindingElement;
            element.MessageVersion = this.MessageVersion;
            element.WriteEncoding = this.WriteEncoding;
            element.MaxReadPoolSize = this.MaxReadPoolSize;
            element.MaxWritePoolSize = this.MaxWritePoolSize;
            this.ReaderQuotas.ApplyConfiguration(element.ReaderQuotas);
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            TextMessageEncodingElement element = (TextMessageEncodingElement) from;
            this.MessageVersion = element.MessageVersion;
            this.WriteEncoding = element.WriteEncoding;
            this.MaxReadPoolSize = element.MaxReadPoolSize;
            this.MaxWritePoolSize = element.MaxWritePoolSize;
        }

        protected internal override BindingElement CreateBindingElement()
        {
            TextMessageEncodingBindingElement bindingElement = new TextMessageEncodingBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            TextMessageEncodingBindingElement element = (TextMessageEncodingBindingElement) bindingElement;
            this.MessageVersion = element.MessageVersion;
            this.WriteEncoding = element.WriteEncoding;
            this.MaxReadPoolSize = element.MaxReadPoolSize;
            this.MaxWritePoolSize = element.MaxWritePoolSize;
            this.ReaderQuotas.InitializeFrom(element.ReaderQuotas);
        }

        public override Type BindingElementType =>
            typeof(TextMessageEncodingBindingElement);

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxReadPoolSize", DefaultValue=0x40)]
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

        [TypeConverter(typeof(MessageVersionConverter)), ConfigurationProperty("messageVersion", DefaultValue="Soap12WSAddressing10")]
        public System.ServiceModel.Channels.MessageVersion MessageVersion
        {
            get => 
                ((System.ServiceModel.Channels.MessageVersion) base["messageVersion"]);
            set
            {
                base["messageVersion"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("maxReadPoolSize", typeof(int), 0x40, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxWritePoolSize", typeof(int), 0x10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("messageVersion", typeof(System.ServiceModel.Channels.MessageVersion), "Soap12WSAddressing10", new MessageVersionConverter(), null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("writeEncoding", typeof(Encoding), "utf-8", new EncodingConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas =>
            ((XmlDictionaryReaderQuotasElement) base["readerQuotas"]);

        [ConfigurationProperty("writeEncoding", DefaultValue="utf-8"), TypeConverter(typeof(EncodingConverter))]
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

