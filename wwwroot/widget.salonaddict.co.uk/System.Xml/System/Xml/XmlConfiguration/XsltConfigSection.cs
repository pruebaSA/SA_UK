namespace System.Xml.XmlConfiguration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Xml;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class XsltConfigSection : ConfigurationSection
    {
        internal static XmlResolver CreateDefaultResolver()
        {
            if (s_ProhibitDefaultUrlResolver)
            {
                return XmlNullResolver.Singleton;
            }
            return new XmlUrlResolver();
        }

        private bool _EnableMemberAccessForXslCompiledTransform
        {
            get
            {
                string enableMemberAccessForXslCompiledTransformString = this.EnableMemberAccessForXslCompiledTransformString;
                bool result = false;
                XmlConvert.TryToBoolean(enableMemberAccessForXslCompiledTransformString, out result);
                return result;
            }
        }

        private bool _LimitXPathComplexity
        {
            get
            {
                string limitXPathComplexityString = this.LimitXPathComplexityString;
                bool result = true;
                XmlConvert.TryToBoolean(limitXPathComplexityString, out result);
                return result;
            }
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

        internal static bool EnableMemberAccessForXslCompiledTransform
        {
            get
            {
                XsltConfigSection section = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
                return section?._EnableMemberAccessForXslCompiledTransform;
            }
        }

        [ConfigurationProperty("enableMemberAccessForXslCompiledTransform", DefaultValue="False")]
        internal string EnableMemberAccessForXslCompiledTransformString
        {
            get => 
                ((string) base["enableMemberAccessForXslCompiledTransform"]);
            set
            {
                base["enableMemberAccessForXslCompiledTransform"] = value;
            }
        }

        public static bool LimitXPathComplexity
        {
            get
            {
                XsltConfigSection section = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
                return ((section == null) || section._LimitXPathComplexity);
            }
        }

        [ConfigurationProperty("limitXPathComplexity", DefaultValue="true")]
        internal string LimitXPathComplexityString
        {
            get => 
                ((string) base["limitXPathComplexity"]);
            set
            {
                base["limitXPathComplexity"] = value;
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

        private static bool s_ProhibitDefaultUrlResolver
        {
            get
            {
                XsltConfigSection section = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
                return section?._ProhibitDefaultResolver;
            }
        }
    }
}

