namespace System.Configuration
{
    using System;
    using System.Globalization;

    internal static class CommonConfigurationStrings
    {
        internal const string Enabled = "enabled";
        internal const string Idn = "idn";
        internal const string IriParsing = "iriParsing";
        internal const string UriSectionName = "uri";

        private static string GetSectionPath(string sectionName) => 
            string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { sectionName });

        private static string GetSectionPath(string sectionName, string subSectionName) => 
            string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { sectionName, subSectionName });

        internal static string UriSectionPath =>
            GetSectionPath("uri");
    }
}

