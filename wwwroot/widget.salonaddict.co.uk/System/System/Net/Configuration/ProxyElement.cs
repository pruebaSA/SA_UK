namespace System.Net.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ProxyElement : ConfigurationElement
    {
        private readonly ConfigurationProperty autoDetect = new ConfigurationProperty("autoDetect", typeof(AutoDetectValues), AutoDetectValues.Unspecified, new EnumConverter(typeof(AutoDetectValues)), null, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty bypassonlocal = new ConfigurationProperty("bypassonlocal", typeof(BypassOnLocalValues), BypassOnLocalValues.Unspecified, new EnumConverter(typeof(BypassOnLocalValues)), null, ConfigurationPropertyOptions.None);
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private readonly ConfigurationProperty proxyaddress = new ConfigurationProperty("proxyaddress", typeof(Uri), null, new UriTypeConverter(UriKind.Absolute), null, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty scriptLocation = new ConfigurationProperty("scriptLocation", typeof(Uri), null, new UriTypeConverter(UriKind.Absolute), null, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty usesystemdefault = new ConfigurationProperty("usesystemdefault", typeof(UseSystemDefaultValues), UseSystemDefaultValues.Unspecified, new EnumConverter(typeof(UseSystemDefaultValues)), null, ConfigurationPropertyOptions.None);

        public ProxyElement()
        {
            this.properties.Add(this.autoDetect);
            this.properties.Add(this.scriptLocation);
            this.properties.Add(this.bypassonlocal);
            this.properties.Add(this.proxyaddress);
            this.properties.Add(this.usesystemdefault);
        }

        [ConfigurationProperty("autoDetect", DefaultValue=-1)]
        public AutoDetectValues AutoDetect
        {
            get => 
                ((AutoDetectValues) base[this.autoDetect]);
            set
            {
                base[this.autoDetect] = value;
            }
        }

        [ConfigurationProperty("bypassonlocal", DefaultValue=-1)]
        public BypassOnLocalValues BypassOnLocal
        {
            get => 
                ((BypassOnLocalValues) base[this.bypassonlocal]);
            set
            {
                base[this.bypassonlocal] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;

        [ConfigurationProperty("proxyaddress")]
        public Uri ProxyAddress
        {
            get => 
                ((Uri) base[this.proxyaddress]);
            set
            {
                base[this.proxyaddress] = value;
            }
        }

        [ConfigurationProperty("scriptLocation")]
        public Uri ScriptLocation
        {
            get => 
                ((Uri) base[this.scriptLocation]);
            set
            {
                base[this.scriptLocation] = value;
            }
        }

        [ConfigurationProperty("usesystemdefault", DefaultValue=-1)]
        public UseSystemDefaultValues UseSystemDefault
        {
            get => 
                ((UseSystemDefaultValues) base[this.usesystemdefault]);
            set
            {
                base[this.usesystemdefault] = value;
            }
        }

        public enum AutoDetectValues
        {
            False = 0,
            True = 1,
            Unspecified = -1
        }

        public enum BypassOnLocalValues
        {
            False = 0,
            True = 1,
            Unspecified = -1
        }

        public enum UseSystemDefaultValues
        {
            False = 0,
            True = 1,
            Unspecified = -1
        }
    }
}

