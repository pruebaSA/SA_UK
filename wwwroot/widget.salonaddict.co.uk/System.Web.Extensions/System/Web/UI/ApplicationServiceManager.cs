namespace System.Web.UI
{
    using System;
    using System.Web.Resources;

    internal static class ApplicationServiceManager
    {
        public const int StringBuilderCapacity = 0x80;

        public static string MergeServiceUrls(string serviceUrl, string existingUrl, Control urlBase)
        {
            serviceUrl = serviceUrl.Trim();
            if (serviceUrl.Length > 0)
            {
                serviceUrl = urlBase.ResolveClientUrl(serviceUrl);
                if (string.IsNullOrEmpty(existingUrl))
                {
                    existingUrl = serviceUrl;
                    return existingUrl;
                }
                if (!string.Equals(serviceUrl, existingUrl, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(AtlasWeb.AppService_MultiplePaths);
                }
            }
            return existingUrl;
        }
    }
}

