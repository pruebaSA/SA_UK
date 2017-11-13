﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ComContractElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public ComContractElement()
        {
        }

        public ComContractElement(string contractType) : this()
        {
            this.Contract = contractType;
        }

        [ConfigurationProperty("contract", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Contract
        {
            get => 
                ((string) base["contract"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["contract"] = value;
            }
        }

        [ConfigurationProperty("exposedMethods", Options=ConfigurationPropertyOptions.None)]
        public ComMethodElementCollection ExposedMethods =>
            ((ComMethodElementCollection) base["exposedMethods"]);

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

        [ConfigurationProperty("namespace", DefaultValue="", Options=ConfigurationPropertyOptions.None), StringValidator(MinLength=0)]
        public string Namespace
        {
            get => 
                ((string) base["namespace"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["namespace"] = value;
            }
        }

        [ConfigurationProperty("persistableTypes")]
        public ComPersistableTypeElementCollection PersistableTypes =>
            ((ComPersistableTypeElementCollection) base["persistableTypes"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("contract", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("exposedMethods", typeof(ComMethodElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("name", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("namespace", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("persistableTypes", typeof(ComPersistableTypeElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("requiresSession", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("userDefinedTypes", typeof(ComUdtElementCollection), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("requiresSession", DefaultValue=true)]
        public bool RequiresSession
        {
            get => 
                ((bool) base["requiresSession"]);
            set
            {
                base["requiresSession"] = value;
            }
        }

        [ConfigurationProperty("userDefinedTypes")]
        public ComUdtElementCollection UserDefinedTypes =>
            ((ComUdtElementCollection) base["userDefinedTypes"]);
    }
}

