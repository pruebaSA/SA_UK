namespace System.Configuration
{
    using System;

    public sealed class UriSection : ConfigurationSection
    {
        private readonly ConfigurationProperty idn = new ConfigurationProperty("idn", typeof(IdnElement), null, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty iriParsing = new ConfigurationProperty("iriParsing", typeof(IriParsingElement), null, ConfigurationPropertyOptions.None);
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        public UriSection()
        {
            this.properties.Add(this.idn);
            this.properties.Add(this.iriParsing);
        }

        [ConfigurationProperty("idn")]
        public IdnElement Idn =>
            ((IdnElement) base[this.idn]);

        [ConfigurationProperty("iriParsing")]
        public IriParsingElement IriParsing =>
            ((IriParsingElement) base[this.iriParsing]);

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

