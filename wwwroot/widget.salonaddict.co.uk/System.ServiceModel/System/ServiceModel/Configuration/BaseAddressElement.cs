﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class BaseAddressElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [StringValidator(MinLength=1), ConfigurationProperty("baseAddress", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string BaseAddress
        {
            get => 
                ((string) base["baseAddress"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["baseAddress"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("baseAddress", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

