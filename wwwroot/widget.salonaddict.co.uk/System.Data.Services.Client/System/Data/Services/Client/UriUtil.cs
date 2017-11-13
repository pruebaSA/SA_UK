namespace System.Data.Services.Client
{
    using System;

    internal static class UriUtil
    {
        internal static string GetNameFromAtomLinkRelationAttribute(string value)
        {
            string str = null;
            if (!string.IsNullOrEmpty(value))
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(value, UriKind.RelativeOrAbsolute);
                }
                catch (UriFormatException)
                {
                }
                if ((null != uri) && uri.IsAbsoluteUri)
                {
                    string components = uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
                    if (components.StartsWith("http://schemas.microsoft.com/ado/2007/08/dataservices/related/", StringComparison.Ordinal))
                    {
                        str = components.Substring("http://schemas.microsoft.com/ado/2007/08/dataservices/related/".Length);
                    }
                }
            }
            return str;
        }
    }
}

