namespace System.Web.Services.Configuration
{
    using System;
    using System.Configuration;
    using System.Web.Services;

    public sealed class WsiProfilesElement : ConfigurationElement
    {
        private readonly ConfigurationProperty name;
        private ConfigurationPropertyCollection properties;

        public WsiProfilesElement()
        {
            this.properties = new ConfigurationPropertyCollection();
            this.name = new ConfigurationProperty("name", typeof(WsiProfiles), WsiProfiles.None, ConfigurationPropertyOptions.IsKey);
            this.properties.Add(this.name);
        }

        public WsiProfilesElement(WsiProfiles name) : this()
        {
            this.Name = name;
        }

        private bool IsValidWsiProfilesValue(WsiProfiles value) => 
            Enum.IsDefined(typeof(WsiProfiles), value);

        [ConfigurationProperty("name", IsKey=true, DefaultValue=0)]
        public WsiProfiles Name
        {
            get => 
                ((WsiProfiles) base[this.name]);
            set
            {
                if (!this.IsValidWsiProfilesValue(value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                base[this.name] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

