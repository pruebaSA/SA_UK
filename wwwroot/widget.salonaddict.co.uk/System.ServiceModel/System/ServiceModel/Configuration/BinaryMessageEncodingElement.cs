namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;

    public sealed class BinaryMessageEncodingElement : BindingElementExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            BinaryMessageEncodingBindingElement element = (BinaryMessageEncodingBindingElement) bindingElement;
            element.MaxSessionSize = this.MaxSessionSize;
            element.MaxReadPoolSize = this.MaxReadPoolSize;
            element.MaxWritePoolSize = this.MaxWritePoolSize;
            this.ReaderQuotas.ApplyConfiguration(element.ReaderQuotas);
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            BinaryMessageEncodingElement element = (BinaryMessageEncodingElement) from;
            this.MaxSessionSize = element.MaxSessionSize;
            this.MaxReadPoolSize = element.MaxReadPoolSize;
            this.MaxWritePoolSize = element.MaxWritePoolSize;
        }

        protected internal override BindingElement CreateBindingElement()
        {
            BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            BinaryMessageEncodingBindingElement element = (BinaryMessageEncodingBindingElement) bindingElement;
            this.MaxSessionSize = element.MaxSessionSize;
            this.MaxReadPoolSize = element.MaxReadPoolSize;
            this.MaxWritePoolSize = element.MaxWritePoolSize;
            this.ReaderQuotas.InitializeFrom(element.ReaderQuotas);
        }

        public override Type BindingElementType =>
            typeof(BinaryMessageEncodingBindingElement);

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

        [ConfigurationProperty("maxSessionSize", DefaultValue=0x800), IntegerValidator(MinValue=0)]
        public int MaxSessionSize
        {
            get => 
                ((int) base["maxSessionSize"]);
            set
            {
                base["maxSessionSize"] = value;
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

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("maxReadPoolSize", typeof(int), 0x40, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxWritePoolSize", typeof(int), 0x10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxSessionSize", typeof(int), 0x800, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas =>
            ((XmlDictionaryReaderQuotasElement) base["readerQuotas"]);
    }
}

