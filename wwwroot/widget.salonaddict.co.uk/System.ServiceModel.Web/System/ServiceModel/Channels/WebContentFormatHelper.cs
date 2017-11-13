namespace System.ServiceModel.Channels
{
    using System;

    internal static class WebContentFormatHelper
    {
        internal static bool IsDefined(WebContentFormat format)
        {
            if (((format != WebContentFormat.Default) && (format != WebContentFormat.Xml)) && (format != WebContentFormat.Json))
            {
                return (format == WebContentFormat.Raw);
            }
            return true;
        }
    }
}

