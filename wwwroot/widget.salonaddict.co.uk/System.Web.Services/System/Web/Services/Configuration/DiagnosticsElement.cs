namespace System.Web.Services.Configuration
{
    using System;
    using System.Configuration;

    public sealed class DiagnosticsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private readonly ConfigurationProperty suppressReturningExceptions = new ConfigurationProperty("suppressReturningExceptions", typeof(bool), false);

        public DiagnosticsElement()
        {
            this.properties.Add(this.suppressReturningExceptions);
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;

        [ConfigurationProperty("suppressReturningExceptions", DefaultValue=false)]
        public bool SuppressReturningExceptions
        {
            get => 
                ((bool) base[this.suppressReturningExceptions]);
            set
            {
                base[this.suppressReturningExceptions] = value;
            }
        }
    }
}

