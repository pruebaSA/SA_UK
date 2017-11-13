namespace System.Configuration
{
    using System;

    public class ConfigurationLocation
    {
        private System.Configuration.Configuration _config;
        private string _locationSubPath;

        internal ConfigurationLocation(System.Configuration.Configuration config, string locationSubPath)
        {
            this._config = config;
            this._locationSubPath = locationSubPath;
        }

        public System.Configuration.Configuration OpenConfiguration() => 
            this._config.OpenLocationConfiguration(this._locationSubPath);

        public string Path =>
            this._locationSubPath;
    }
}

