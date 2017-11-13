namespace System.Xml.XmlConfiguration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Xml;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class XmlReaderSection : ConfigurationSection
    {
        internal static XmlResolver CreateDefaultResolver()
        {
            if (ProhibitDefaultUrlResolver)
            {
                return null;
            }
            return new XmlUrlResolver();
        }

        private bool _ProhibitDefaultResolver
        {
            get
            {
                bool flag;
                XmlConvert.TryToBoolean(this.ProhibitDefaultResolverString, out flag);
                return flag;
            }
        }

        [ConfigurationProperty("prohibitDefaultResolver", DefaultValue="false")]
        internal string ProhibitDefaultResolverString
        {
            get => 
                ((string) base["prohibitDefaultResolver"]);
            set
            {
                base["prohibitDefaultResolver"] = value;
            }
        }

        internal static bool ProhibitDefaultUrlResolver
        {
            get
            {
                XmlReaderSection section = ConfigurationManager.GetSection(XmlConfigurationString.XmlReaderSectionPath) as XmlReaderSection;
                return section?._ProhibitDefaultResolver;
            }
        }
    }
}

