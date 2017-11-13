namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ComUdtElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public ComUdtElement()
        {
        }

        public ComUdtElement(string typeDefID) : this()
        {
            this.TypeDefID = typeDefID;
        }

        [ConfigurationProperty("name", DefaultValue="", Options=ConfigurationPropertyOptions.None), StringValidator(MinLength=0)]
        public string Name
        {
            get => 
                ((string) base["name"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["name"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("name", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("typeLibID", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("typeLibVersion", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("typeDefID", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("typeDefID", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string TypeDefID
        {
            get => 
                ((string) base["typeDefID"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["typeDefID"] = value;
            }
        }

        [ConfigurationProperty("typeLibID", Options=ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string TypeLibID
        {
            get => 
                ((string) base["typeLibID"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["typeLibID"] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("typeLibVersion", Options=ConfigurationPropertyOptions.IsRequired)]
        public string TypeLibVersion
        {
            get => 
                ((string) base["typeLibVersion"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["typeLibVersion"] = value;
            }
        }
    }
}

