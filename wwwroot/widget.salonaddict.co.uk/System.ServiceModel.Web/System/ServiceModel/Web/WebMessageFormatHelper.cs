namespace System.ServiceModel.Web
{
    using System;

    internal static class WebMessageFormatHelper
    {
        internal static bool IsDefined(WebMessageFormat format)
        {
            if (format != WebMessageFormat.Xml)
            {
                return (format == WebMessageFormat.Json);
            }
            return true;
        }
    }
}

