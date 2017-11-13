namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class DefaultPortElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public DefaultPortElement()
        {
        }

        public DefaultPortElement(DefaultPortElement other)
        {
            this.Scheme = other.Scheme;
            this.Port = other.Port;
        }

        [ConfigurationProperty("port", Options=ConfigurationPropertyOptions.IsRequired), IntegerValidator(MinValue=0, MaxValue=0xffff)]
        public int Port
        {
            get => 
                ((int) base["port"]);
            set
            {
                base["port"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("scheme", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("port", typeof(int), null, null, new IntegerValidator(0, 0xffff, false), ConfigurationPropertyOptions.IsRequired)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("scheme", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Scheme
        {
            get => 
                ((string) base["scheme"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["scheme"] = value;
            }
        }
    }
}

