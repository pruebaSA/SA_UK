namespace System.Web.Services.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ProtocolElement : ConfigurationElement
    {
        private readonly ConfigurationProperty name;
        private ConfigurationPropertyCollection properties;

        public ProtocolElement()
        {
            this.properties = new ConfigurationPropertyCollection();
            this.name = new ConfigurationProperty("name", typeof(WebServiceProtocols), WebServiceProtocols.Unknown, ConfigurationPropertyOptions.IsKey);
            this.properties.Add(this.name);
        }

        public ProtocolElement(WebServiceProtocols protocol) : this()
        {
            this.Name = protocol;
        }

        private bool IsValidProtocolsValue(WebServiceProtocols value) => 
            Enum.IsDefined(typeof(WebServiceProtocols), value);

        [ConfigurationProperty("name", IsKey=true, DefaultValue=0)]
        public WebServiceProtocols Name
        {
            get => 
                ((WebServiceProtocols) base[this.name]);
            set
            {
                if (!this.IsValidProtocolsValue(value))
                {
                    value = WebServiceProtocols.Unknown;
                }
                base[this.name] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

