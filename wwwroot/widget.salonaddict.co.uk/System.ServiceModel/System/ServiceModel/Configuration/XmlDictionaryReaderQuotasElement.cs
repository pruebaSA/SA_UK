namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.Xml;

    public sealed class XmlDictionaryReaderQuotasElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("readerQuotas");
            }
            if (this.MaxDepth != 0)
            {
                readerQuotas.MaxDepth = this.MaxDepth;
            }
            if (this.MaxStringContentLength != 0)
            {
                readerQuotas.MaxStringContentLength = this.MaxStringContentLength;
            }
            if (this.MaxArrayLength != 0)
            {
                readerQuotas.MaxArrayLength = this.MaxArrayLength;
            }
            if (this.MaxBytesPerRead != 0)
            {
                readerQuotas.MaxBytesPerRead = this.MaxBytesPerRead;
            }
            if (this.MaxNameTableCharCount != 0)
            {
                readerQuotas.MaxNameTableCharCount = this.MaxNameTableCharCount;
            }
        }

        internal void InitializeFrom(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("readerQuotas");
            }
            this.MaxDepth = readerQuotas.MaxDepth;
            this.MaxStringContentLength = readerQuotas.MaxStringContentLength;
            this.MaxArrayLength = readerQuotas.MaxArrayLength;
            this.MaxBytesPerRead = readerQuotas.MaxBytesPerRead;
            this.MaxNameTableCharCount = readerQuotas.MaxNameTableCharCount;
        }

        [ConfigurationProperty("maxArrayLength", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int MaxArrayLength
        {
            get => 
                ((int) base["maxArrayLength"]);
            set
            {
                base["maxArrayLength"] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxBytesPerRead", DefaultValue=0)]
        public int MaxBytesPerRead
        {
            get => 
                ((int) base["maxBytesPerRead"]);
            set
            {
                base["maxBytesPerRead"] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxDepth", DefaultValue=0)]
        public int MaxDepth
        {
            get => 
                ((int) base["maxDepth"]);
            set
            {
                base["maxDepth"] = value;
            }
        }

        [ConfigurationProperty("maxNameTableCharCount", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int MaxNameTableCharCount
        {
            get => 
                ((int) base["maxNameTableCharCount"]);
            set
            {
                base["maxNameTableCharCount"] = value;
            }
        }

        [ConfigurationProperty("maxStringContentLength", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int MaxStringContentLength
        {
            get => 
                ((int) base["maxStringContentLength"]);
            set
            {
                base["maxStringContentLength"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("maxDepth", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxStringContentLength", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxArrayLength", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxBytesPerRead", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxNameTableCharCount", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

