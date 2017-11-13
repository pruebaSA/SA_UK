namespace System.ServiceModel
{
    using System;

    internal static class WebHttpSecurityModeHelper
    {
        internal static bool IsDefined(WebHttpSecurityMode value)
        {
            if ((value != WebHttpSecurityMode.None) && (value != WebHttpSecurityMode.Transport))
            {
                return (value == WebHttpSecurityMode.TransportCredentialOnly);
            }
            return true;
        }
    }
}

