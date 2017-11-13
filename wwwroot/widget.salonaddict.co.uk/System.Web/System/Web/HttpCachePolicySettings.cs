namespace System.Web
{
    using System;

    internal sealed class HttpCachePolicySettings
    {
        internal readonly int _allowInHistory;
        internal readonly HttpCacheability _cacheability;
        internal readonly string _cacheExtension;
        internal readonly string _etag;
        internal readonly bool _generateEtagFromFiles;
        internal readonly bool _generateLastModifiedFromFiles;
        internal readonly bool _hasSetCookieHeader;
        internal readonly bool _hasUserProvidedDependencies;
        internal readonly HttpResponseHeader _headerCacheControl;
        internal readonly HttpResponseHeader _headerEtag;
        internal readonly HttpResponseHeader _headerExpires;
        internal readonly HttpResponseHeader _headerLastModified;
        internal readonly HttpResponseHeader _headerPragma;
        internal readonly HttpResponseHeader _headerVaryBy;
        internal readonly bool _ignoreRangeRequests;
        internal readonly bool _isExpiresSet;
        internal readonly bool _isLastModifiedSet;
        internal readonly bool _isMaxAgeSet;
        internal readonly bool _isModified;
        internal readonly bool _isProxyMaxAgeSet;
        internal readonly TimeSpan _maxAge;
        internal readonly string[] _noCacheFields;
        internal readonly bool _noServerCaching;
        internal readonly bool _noStore;
        internal readonly bool _noTransforms;
        internal readonly int _omitVaryStar;
        internal readonly string[] _privateFields;
        internal readonly TimeSpan _proxyMaxAge;
        internal readonly HttpCacheRevalidation _revalidation;
        internal readonly TimeSpan _slidingDelta;
        internal readonly int _slidingExpiration;
        internal readonly DateTime _utcExpires;
        internal readonly DateTime _utcLastModified;
        internal readonly DateTime _utcTimestampCreated;
        internal readonly System.Web.ValidationCallbackInfo[] _validationCallbackInfo;
        internal readonly int _validUntilExpires;
        internal readonly string[] _varyByContentEncodings;
        internal readonly string _varyByCustom;
        internal readonly string[] _varyByHeaderValues;
        internal readonly string[] _varyByParamValues;

        internal HttpCachePolicySettings(bool isModified, System.Web.ValidationCallbackInfo[] validationCallbackInfo, bool hasSetCookieHeader, bool noServerCaching, string cacheExtension, bool noTransforms, bool ignoreRangeRequests, string[] varyByContentEncodings, string[] varyByHeaderValues, string[] varyByParamValues, string varyByCustom, HttpCacheability cacheability, bool noStore, string[] privateFields, string[] noCacheFields, DateTime utcExpires, bool isExpiresSet, TimeSpan maxAge, bool isMaxAgeSet, TimeSpan proxyMaxAge, bool isProxyMaxAgeSet, int slidingExpiration, TimeSpan slidingDelta, DateTime utcTimestampCreated, int validUntilExpires, int allowInHistory, HttpCacheRevalidation revalidation, DateTime utcLastModified, bool isLastModifiedSet, string etag, bool generateLastModifiedFromFiles, bool generateEtagFromFiles, int omitVaryStar, HttpResponseHeader headerCacheControl, HttpResponseHeader headerPragma, HttpResponseHeader headerExpires, HttpResponseHeader headerLastModified, HttpResponseHeader headerEtag, HttpResponseHeader headerVaryBy, bool hasUserProvidedDependencies)
        {
            this._isModified = isModified;
            this._validationCallbackInfo = validationCallbackInfo;
            this._hasSetCookieHeader = hasSetCookieHeader;
            this._noServerCaching = noServerCaching;
            this._cacheExtension = cacheExtension;
            this._noTransforms = noTransforms;
            this._ignoreRangeRequests = ignoreRangeRequests;
            this._varyByContentEncodings = varyByContentEncodings;
            this._varyByHeaderValues = varyByHeaderValues;
            this._varyByParamValues = varyByParamValues;
            this._varyByCustom = varyByCustom;
            this._cacheability = cacheability;
            this._noStore = noStore;
            this._privateFields = privateFields;
            this._noCacheFields = noCacheFields;
            this._utcExpires = utcExpires;
            this._isExpiresSet = isExpiresSet;
            this._maxAge = maxAge;
            this._isMaxAgeSet = isMaxAgeSet;
            this._proxyMaxAge = proxyMaxAge;
            this._isProxyMaxAgeSet = isProxyMaxAgeSet;
            this._slidingExpiration = slidingExpiration;
            this._slidingDelta = slidingDelta;
            this._utcTimestampCreated = utcTimestampCreated;
            this._validUntilExpires = validUntilExpires;
            this._allowInHistory = allowInHistory;
            this._revalidation = revalidation;
            this._utcLastModified = utcLastModified;
            this._isLastModifiedSet = isLastModifiedSet;
            this._etag = etag;
            this._generateLastModifiedFromFiles = generateLastModifiedFromFiles;
            this._generateEtagFromFiles = generateEtagFromFiles;
            this._omitVaryStar = omitVaryStar;
            this._headerCacheControl = headerCacheControl;
            this._headerPragma = headerPragma;
            this._headerExpires = headerExpires;
            this._headerLastModified = headerLastModified;
            this._headerEtag = headerEtag;
            this._headerVaryBy = headerVaryBy;
            this._hasUserProvidedDependencies = hasUserProvidedDependencies;
        }

        internal bool HasValidationPolicy()
        {
            if ((!this.ValidUntilExpires && !this.GenerateLastModifiedFromFiles) && !this.GenerateEtagFromFiles)
            {
                return (this.ValidationCallbackInfo != null);
            }
            return true;
        }

        internal int AllowInHistoryInternal =>
            this._allowInHistory;

        internal HttpCacheability CacheabilityInternal =>
            this._cacheability;

        internal string CacheExtension =>
            this._cacheExtension;

        internal string ETag =>
            this._etag;

        internal bool GenerateEtagFromFiles =>
            this._generateEtagFromFiles;

        internal bool GenerateLastModifiedFromFiles =>
            this._generateLastModifiedFromFiles;

        internal bool hasSetCookieHeader =>
            this._hasSetCookieHeader;

        internal bool HasUserProvidedDependencies =>
            this._hasUserProvidedDependencies;

        internal HttpResponseHeader HeaderCacheControl =>
            this._headerCacheControl;

        internal HttpResponseHeader HeaderEtag =>
            this._headerEtag;

        internal HttpResponseHeader HeaderExpires =>
            this._headerExpires;

        internal HttpResponseHeader HeaderLastModified =>
            this._headerLastModified;

        internal HttpResponseHeader HeaderPragma =>
            this._headerPragma;

        internal HttpResponseHeader HeaderVaryBy =>
            this._headerVaryBy;

        internal bool IgnoreParams =>
            ((this._varyByParamValues != null) && (this._varyByParamValues[0].Length == 0));

        internal bool IgnoreRangeRequests =>
            this._ignoreRangeRequests;

        internal bool IsExpiresSet =>
            this._isExpiresSet;

        internal bool IsLastModifiedSet =>
            this._isLastModifiedSet;

        internal bool IsMaxAgeSet =>
            this._isMaxAgeSet;

        internal bool IsModified =>
            this._isModified;

        internal bool IsProxyMaxAgeSet =>
            this._isProxyMaxAgeSet;

        internal TimeSpan MaxAge =>
            this._maxAge;

        internal string[] NoCacheFields
        {
            get
            {
                if (this._noCacheFields != null)
                {
                    return (string[]) this._noCacheFields.Clone();
                }
                return null;
            }
        }

        internal bool NoServerCaching =>
            this._noServerCaching;

        internal bool NoStore =>
            this._noStore;

        internal bool NoTransforms =>
            this._noTransforms;

        internal int OmitVaryStarInternal =>
            this._omitVaryStar;

        internal string[] PrivateFields
        {
            get
            {
                if (this._privateFields != null)
                {
                    return (string[]) this._privateFields.Clone();
                }
                return null;
            }
        }

        internal TimeSpan ProxyMaxAge =>
            this._proxyMaxAge;

        internal HttpCacheRevalidation Revalidation =>
            this._revalidation;

        internal TimeSpan SlidingDelta =>
            this._slidingDelta;

        internal bool SlidingExpiration =>
            (this._slidingExpiration == 1);

        internal int SlidingExpirationInternal =>
            this._slidingExpiration;

        internal DateTime UtcExpires =>
            this._utcExpires;

        internal DateTime UtcLastModified =>
            this._utcLastModified;

        internal DateTime UtcTimestampCreated =>
            this._utcTimestampCreated;

        internal System.Web.ValidationCallbackInfo[] ValidationCallbackInfo =>
            this._validationCallbackInfo;

        internal bool ValidUntilExpires =>
            ((((this._validUntilExpires == 1) && !this.SlidingExpiration) && (!this.GenerateLastModifiedFromFiles && !this.GenerateEtagFromFiles)) && (this.ValidationCallbackInfo == null));

        internal int ValidUntilExpiresInternal =>
            this._validUntilExpires;

        internal string[] VaryByContentEncodings
        {
            get
            {
                if (this._varyByContentEncodings != null)
                {
                    return (string[]) this._varyByContentEncodings.Clone();
                }
                return null;
            }
        }

        internal string VaryByCustom =>
            this._varyByCustom;

        internal string[] VaryByHeaders
        {
            get
            {
                if (this._varyByHeaderValues != null)
                {
                    return (string[]) this._varyByHeaderValues.Clone();
                }
                return null;
            }
        }

        internal string[] VaryByParams
        {
            get
            {
                if (this._varyByParamValues != null)
                {
                    return (string[]) this._varyByParamValues.Clone();
                }
                return null;
            }
        }
    }
}

