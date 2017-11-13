namespace System.Net.Configuration
{
    using System;
    using System.Configuration;

    public sealed class AuthenticationModuleElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;
        private readonly ConfigurationProperty type;

        public AuthenticationModuleElement()
        {
            this.properties = new ConfigurationPropertyCollection();
            this.type = new ConfigurationProperty("type", typeof(string), null, ConfigurationPropertyOptions.IsKey);
            this.properties.Add(this.type);
        }

        public AuthenticationModuleElement(string typeName) : this()
        {
            if (typeName != ((string) this.type.DefaultValue))
            {
                this.Type = typeName;
            }
        }

        internal string Key =>
            this.Type;

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;

        [ConfigurationProperty("type", IsRequired=true, IsKey=true)]
        public string Type
        {
            get => 
                ((string) base[this.type]);
            set
            {
                base[this.type] = value;
            }
        }
    }
}

