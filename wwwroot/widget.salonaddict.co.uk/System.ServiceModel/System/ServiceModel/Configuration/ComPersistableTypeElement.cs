﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ComPersistableTypeElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public ComPersistableTypeElement()
        {
        }

        public ComPersistableTypeElement(string ID) : this()
        {
            this.ID = ID;
        }

        [ConfigurationProperty("ID", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string ID
        {
            get => 
                ((string) base["ID"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["ID"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("name", DefaultValue="", Options=ConfigurationPropertyOptions.None)]
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
                        new ConfigurationProperty("ID", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

