namespace System.Web.Caching
{
    using System;
    using System.Web;

    internal class CachedRawResponse
    {
        internal readonly string _kernelCacheUrl;
        internal readonly HttpRawResponse _rawResponse;
        internal readonly HttpCachePolicySettings _settings;

        internal CachedRawResponse(HttpRawResponse rawResponse, HttpCachePolicySettings settings, string kernelCacheUrl)
        {
            this._rawResponse = rawResponse;
            this._settings = settings;
            this._kernelCacheUrl = kernelCacheUrl;
        }
    }
}

