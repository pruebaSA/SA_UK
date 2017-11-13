namespace System.Xml.XmlConfiguration
{
    using System;

    internal static class XmlConfigurationString
    {
        internal const string EnableMemberAccessForXslCompiledTransformName = "enableMemberAccessForXslCompiledTransform";
        internal const string LimitCharactersFromEntitiesName = "limitCharactersFromEntities";
        internal const string LimitXPathComplexityName = "limitXPathComplexity";
        internal const string ProhibitDefaultResolverName = "prohibitDefaultResolver";
        internal const string XmlConfigurationSectionName = "system.xml";
        internal const string XmlReaderSectionName = "xmlReader";
        internal static string XmlReaderSectionPath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { "system.xml", "xmlReader" });
        internal const string XmlTextReaderSectionName = "xmlTextReader";
        internal static string XmlTextReaderSectionPath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { "system.xml", "xmlTextReader" });
        internal const string XsltSectionName = "xslt";
        internal static string XsltSectionPath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { "system.xml", "xslt" });
    }
}

