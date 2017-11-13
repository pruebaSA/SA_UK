namespace MS.Internal
{
    using System;
    using System.Globalization;

    internal static class ResourceIDHelper
    {
        internal static string GetResourceIDFromRelativePath(string relPath)
        {
            Uri baseUri = new Uri("http://foo/");
            Uri sourceUri = new Uri(baseUri, relPath.Replace("#", "%23"));
            return GetResourceIDFromUri(baseUri, sourceUri);
        }

        private static string GetResourceIDFromUri(Uri baseUri, Uri sourceUri)
        {
            string str = string.Empty;
            if ((baseUri.IsAbsoluteUri && sourceUri.IsAbsoluteUri) && ((baseUri.Scheme == sourceUri.Scheme) && (baseUri.Host == sourceUri.Host)))
            {
                string components = baseUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                string str3 = sourceUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                components = components.ToLower(CultureInfo.InvariantCulture);
                str3 = str3.ToLower(CultureInfo.InvariantCulture);
                if (str3.StartsWith(components, StringComparison.OrdinalIgnoreCase))
                {
                    str = str3.Substring(components.Length);
                }
            }
            return str;
        }
    }
}

