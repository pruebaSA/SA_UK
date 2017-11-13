﻿namespace System.Web.Configuration
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Compilation;

    internal class HttpCapabilitiesEvaluator
    {
        internal string _cacheKeyPrefix = ("e" + Interlocked.Increment(ref _idCounter).ToString(CultureInfo.InvariantCulture));
        internal TimeSpan _cachetime;
        private static object _disableOptimisticCachingSingleton = new object();
        private static int _idCounter;
        private const string _isMobileDeviceCapKey = "isMobileDevice";
        internal Type _resultType;
        internal CapabilitiesRule _rule;
        private int _userAgentCacheKeyLength;
        internal Hashtable _variables;

        internal HttpCapabilitiesEvaluator(HttpCapabilitiesEvaluator parent)
        {
            if (parent == null)
            {
                this.ClearParent();
            }
            else
            {
                this._rule = parent._rule;
                if (parent._variables == null)
                {
                    this._variables = null;
                }
                else
                {
                    this._variables = new Hashtable(parent._variables);
                }
                this._cachetime = parent._cachetime;
                this._resultType = parent._resultType;
            }
            this.AddDependency(string.Empty);
        }

        internal virtual void AddDependency(string variable)
        {
            if (variable.Equals("HTTP_USER_AGENT"))
            {
                variable = string.Empty;
            }
            this._variables[variable] = true;
        }

        internal virtual void AddRuleList(ArrayList rulelist)
        {
            if (rulelist.Count != 0)
            {
                if (this._rule != null)
                {
                    rulelist.Insert(0, this._rule);
                }
                this._rule = new CapabilitiesSection(2, null, null, rulelist);
            }
        }

        private void CacheBrowserCapResult(ref HttpCapabilitiesBase result)
        {
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            if (result.Capabilities != null)
            {
                string key = "z";
                StringBuilder builder = new StringBuilder();
                foreach (string str2 in result.Capabilities.Keys)
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        string str3 = (string) result.Capabilities[str2];
                        if (str3 != null)
                        {
                            builder.Append(str2);
                            builder.Append("$");
                            builder.Append(str3);
                            builder.Append("$");
                        }
                    }
                }
                key = key + builder.ToString().GetHashCode().ToString(CultureInfo.InvariantCulture);
                HttpCapabilitiesBase base2 = cacheInternal.Get(key) as HttpCapabilitiesBase;
                if (base2 != null)
                {
                    result = base2;
                }
                else
                {
                    cacheInternal.UtcInsert(key, result, null, Cache.NoAbsoluteExpiration, this._cachetime);
                }
            }
        }

        internal virtual void ClearParent()
        {
            this._rule = null;
            this._cachetime = TimeSpan.FromSeconds(60.0);
            this._variables = new Hashtable();
            this._resultType = typeof(HttpCapabilitiesBase);
        }

        internal HttpCapabilitiesBase Evaluate(HttpRequest request)
        {
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            string userAgent = GetUserAgent(request);
            string str2 = userAgent;
            if ((str2 != null) && (str2.Length > this.UserAgentCacheKeyLength))
            {
                str2 = str2.Substring(0, this.UserAgentCacheKeyLength);
            }
            bool flag = false;
            string key = this._cacheKeyPrefix + str2;
            object obj2 = cacheInternal.Get(key);
            HttpCapabilitiesBase result = obj2 as HttpCapabilitiesBase;
            if (result == null)
            {
                if (obj2 == _disableOptimisticCachingSingleton)
                {
                    flag = true;
                }
                else
                {
                    result = this.EvaluateFinal(request, true);
                    if (result.UseOptimizedCacheKey)
                    {
                        this.CacheBrowserCapResult(ref result);
                        cacheInternal.UtcInsert(key, result, null, Cache.NoAbsoluteExpiration, this._cachetime);
                        return result;
                    }
                }
                IDictionaryEnumerator enumerator = this._variables.GetEnumerator();
                StringBuilder builder = new StringBuilder(this._cacheKeyPrefix);
                InternalSecurityPermissions.AspNetHostingPermissionLevelLow.Assert();
                while (enumerator.MoveNext())
                {
                    string str5;
                    string str4 = (string) enumerator.Key;
                    if (str4.Length == 0)
                    {
                        str5 = userAgent;
                    }
                    else
                    {
                        str5 = request.ServerVariables[str4];
                    }
                    if (str5 != null)
                    {
                        builder.Append(str5);
                    }
                }
                CodeAccessPermission.RevertAssert();
                builder.Append(BrowserCapabilitiesFactoryBase.GetBrowserCapKey(this.BrowserCapFactory.InternalGetMatchedHeaders(), request));
                string str6 = builder.ToString();
                if ((userAgent == null) || flag)
                {
                    result = cacheInternal.Get(str6) as HttpCapabilitiesBase;
                    if (result != null)
                    {
                        return result;
                    }
                }
                result = this.EvaluateFinal(request, false);
                this.CacheBrowserCapResult(ref result);
                cacheInternal.UtcInsert(str6, result, null, Cache.NoAbsoluteExpiration, this._cachetime);
                if (key != null)
                {
                    cacheInternal.UtcInsert(key, _disableOptimisticCachingSingleton, null, Cache.NoAbsoluteExpiration, this._cachetime);
                }
            }
            return result;
        }

        internal HttpCapabilitiesBase EvaluateFinal(HttpRequest request, bool onlyEvaluateUserAgent)
        {
            HttpBrowserCapabilities httpBrowserCapabilities = this.BrowserCapFactory.GetHttpBrowserCapabilities(request);
            CapabilitiesState state = new CapabilitiesState(request, httpBrowserCapabilities.Capabilities);
            if (onlyEvaluateUserAgent)
            {
                state.EvaluateOnlyUserAgent = true;
            }
            if (this._rule != null)
            {
                string str = httpBrowserCapabilities["isMobileDevice"];
                httpBrowserCapabilities.Capabilities["isMobileDevice"] = null;
                this._rule.Evaluate(state);
                string str2 = httpBrowserCapabilities["isMobileDevice"];
                if (str2 == null)
                {
                    httpBrowserCapabilities.Capabilities["isMobileDevice"] = str;
                }
                else if (str2.Equals("true"))
                {
                    httpBrowserCapabilities.DisableOptimizedCacheKey();
                }
            }
            HttpCapabilitiesBase base2 = (HttpCapabilitiesBase) HttpRuntime.CreateNonPublicInstance(this._resultType);
            base2.InitInternal(httpBrowserCapabilities);
            return base2;
        }

        internal static string GetUserAgent(HttpRequest request)
        {
            string userAgentFromClientTarget;
            if (request.ClientTarget.Length > 0)
            {
                userAgentFromClientTarget = GetUserAgentFromClientTarget(request.Context.ConfigurationPath, request.ClientTarget);
            }
            else
            {
                userAgentFromClientTarget = request.UserAgent;
            }
            if ((userAgentFromClientTarget != null) && (userAgentFromClientTarget.Length > 0x200))
            {
                userAgentFromClientTarget = userAgentFromClientTarget.Substring(0, 0x200);
            }
            return userAgentFromClientTarget;
        }

        internal static string GetUserAgentFromClientTarget(VirtualPath configPath, string clientTarget)
        {
            ClientTargetSection section = RuntimeConfig.GetConfig(configPath).ClientTarget;
            string userAgent = null;
            if (section.ClientTargets[clientTarget] != null)
            {
                userAgent = section.ClientTargets[clientTarget].UserAgent;
            }
            if (userAgent == null)
            {
                throw new HttpException(System.Web.SR.GetString("Invalid_client_target", new object[] { clientTarget }));
            }
            return userAgent;
        }

        internal virtual void SetCacheTime(int sec)
        {
            this._cachetime = TimeSpan.FromSeconds((double) sec);
        }

        internal BrowserCapabilitiesFactoryBase BrowserCapFactory =>
            BrowserCapabilitiesCompiler.BrowserCapabilitiesFactory;

        internal int UserAgentCacheKeyLength
        {
            get => 
                this._userAgentCacheKeyLength;
            set
            {
                this._userAgentCacheKeyLength = value;
            }
        }
    }
}

