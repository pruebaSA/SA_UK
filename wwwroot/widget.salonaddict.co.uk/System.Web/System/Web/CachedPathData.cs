namespace System.Web
{
    using System;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Threading;
    using System.Web.Caching;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Util;

    internal class CachedPathData
    {
        private string _configPath;
        private System.Web.Util.SafeBitVector32 _flags;
        private HandlerMappingMemo _handlerMemo;
        private string _physicalPath;
        private System.Web.Configuration.RuntimeConfig _runtimeConfig = System.Web.Configuration.RuntimeConfig.GetErrorRuntimeConfig();
        private VirtualPath _virtualPath;
        internal const int FClosed = 0x20;
        internal const int FCloseNeeded = 0x40;
        internal const int FCompletedFirstRequest = 2;
        internal const int FExists = 4;
        internal const int FInited = 1;
        internal const int FOwnsConfigRecord = 0x10;
        private static CacheItemRemovedCallback s_callback = new CacheItemRemovedCallback(CachedPathData.OnCacheItemRemoved);

        internal CachedPathData(string configPath, VirtualPath virtualPath, string physicalPath, bool exists)
        {
            this._configPath = configPath;
            this._virtualPath = virtualPath;
            this._physicalPath = physicalPath;
            this._flags[4] = exists;
            string schemeDelimiter = Uri.SchemeDelimiter;
        }

        private void Close()
        {
            if ((this._flags[1] && this._flags.ChangeValue(0x20, true)) && this._flags[0x10])
            {
                this.ConfigRecord.Remove();
            }
        }

        private static string CreateKey(string configPath) => 
            ("d" + configPath);

        internal static CachedPathData GetApplicationPathData()
        {
            if (!HostingEnvironment.IsHosted)
            {
                return GetRootWebPathData();
            }
            return GetConfigPathData(HostingEnvironment.AppConfigPath);
        }

        private static CachedPathData GetConfigPathData(string configPath)
        {
            string key = CreateKey(configPath);
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            CachedPathData data = (CachedPathData) cacheInternal.Get(key);
            if (data != null)
            {
                data.WaitForInit();
                return data;
            }
            CachedPathData parentData = null;
            CacheDependency dependencies = null;
            VirtualPath vpath = null;
            string physicalPath = null;
            bool exists = false;
            string[] filenames = null;
            string[] cachekeys = null;
            string siteID = null;
            bool flag2 = false;
            if (WebConfigurationHost.IsMachineConfigPath(configPath))
            {
                exists = true;
                flag2 = true;
            }
            else
            {
                string parent = System.Configuration.ConfigPathUtility.GetParent(configPath);
                parentData = GetConfigPathData(parent);
                string str5 = CreateKey(parent);
                cachekeys = new string[] { str5 };
                if (!WebConfigurationHost.IsVirtualPathConfigPath(configPath))
                {
                    exists = true;
                    flag2 = true;
                }
                else
                {
                    WebConfigurationHost.GetSiteIDAndVPathFromConfigPath(configPath, out siteID, out vpath);
                    try
                    {
                        physicalPath = vpath.MapPathInternal(true);
                    }
                    catch (HttpException exception)
                    {
                        if (exception.GetHttpCode() == 500)
                        {
                            throw new HttpException(0x194, string.Empty);
                        }
                        throw;
                    }
                    System.Web.Util.FileUtil.CheckSuspiciousPhysicalPath(physicalPath);
                    bool isDirectory = false;
                    if (string.IsNullOrEmpty(physicalPath))
                    {
                        exists = false;
                    }
                    else
                    {
                        System.Web.Util.FileUtil.PhysicalPathStatus(physicalPath, false, false, out exists, out isDirectory);
                    }
                    if (exists && !isDirectory)
                    {
                        filenames = new string[] { physicalPath };
                    }
                }
                try
                {
                    dependencies = new CacheDependency(0, filenames, cachekeys);
                }
                catch
                {
                }
            }
            CachedPathData data3 = null;
            bool flag4 = false;
            bool flag5 = false;
            CacheItemPriority priority = flag2 ? CacheItemPriority.NotRemovable : CacheItemPriority.Normal;
            try
            {
                using (dependencies)
                {
                    data3 = new CachedPathData(configPath, vpath, physicalPath, exists);
                    try
                    {
                    }
                    finally
                    {
                        data = (CachedPathData) cacheInternal.UtcAdd(key, data3, dependencies, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, s_callback);
                        if (data == null)
                        {
                            flag4 = true;
                        }
                    }
                }
                if (!flag4)
                {
                    data.WaitForInit();
                    return data;
                }
                lock (data3)
                {
                    try
                    {
                        data3.Init(parentData);
                        flag5 = true;
                    }
                    finally
                    {
                        data3._flags[1] = true;
                        Monitor.PulseAll(data3);
                        if (data3._flags[0x40])
                        {
                            data3.Close();
                        }
                    }
                    return data3;
                }
            }
            finally
            {
                if (flag4)
                {
                    if (!data3._flags[1])
                    {
                        lock (data3)
                        {
                            data3._flags[1] = true;
                            Monitor.PulseAll(data3);
                            if (data3._flags[0x40])
                            {
                                data3.Close();
                            }
                        }
                    }
                    if (!flag5 || ((data3.ConfigRecord != null) && data3.ConfigRecord.HasInitErrors))
                    {
                        if (dependencies != null)
                        {
                            if (!flag5)
                            {
                                dependencies = new CacheDependency(0, null, cachekeys);
                            }
                            else
                            {
                                dependencies = new CacheDependency(0, filenames, cachekeys);
                            }
                        }
                        using (dependencies)
                        {
                            cacheInternal.UtcInsert(key, data3, dependencies, DateTime.UtcNow.AddSeconds(5.0), Cache.NoSlidingExpiration, CacheItemPriority.Normal, s_callback);
                        }
                    }
                }
            }
            return data3;
        }

        internal static CachedPathData GetMachinePathData() => 
            GetConfigPathData("machine");

        internal static CachedPathData GetRootWebPathData() => 
            GetConfigPathData("machine/webroot");

        internal static CachedPathData GetVirtualPathData(VirtualPath virtualPath, bool permitPathsOutsideApp)
        {
            if (!HostingEnvironment.IsHosted)
            {
                return GetRootWebPathData();
            }
            if (virtualPath != null)
            {
                virtualPath.FailIfRelativePath();
            }
            if ((virtualPath != null) && virtualPath.IsWithinAppRoot)
            {
                return GetConfigPathData(WebConfigurationHost.GetConfigPathFromSiteIDAndVPath(HostingEnvironment.SiteID, virtualPath));
            }
            if (!permitPathsOutsideApp)
            {
                throw new ArgumentException(System.Web.SR.GetString("Cross_app_not_allowed", new object[] { (virtualPath != null) ? virtualPath.VirtualPathString : "null" }));
            }
            return GetApplicationPathData();
        }

        private void Init(CachedPathData parentData)
        {
            if (!HttpConfigurationSystem.UseHttpConfigurationSystem)
            {
                this._runtimeConfig = null;
            }
            else
            {
                IInternalConfigRecord uniqueConfigRecord = HttpConfigurationSystem.GetUniqueConfigRecord(this._configPath);
                if (uniqueConfigRecord.ConfigPath.Length == this._configPath.Length)
                {
                    this._flags[0x10] = true;
                    this._runtimeConfig = new System.Web.Configuration.RuntimeConfig(uniqueConfigRecord);
                }
                else
                {
                    this._runtimeConfig = parentData._runtimeConfig;
                }
            }
        }

        internal static void MarkCompleted(CachedPathData pathData)
        {
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            string configPath = pathData._configPath;
        Label_000D:
            pathData.CompletedFirstRequest = true;
            configPath = System.Configuration.ConfigPathUtility.GetParent(configPath);
            if (configPath != null)
            {
                string key = CreateKey(configPath);
                pathData = (CachedPathData) cacheInternal.Get(key);
                if ((pathData != null) && !pathData.CompletedFirstRequest)
                {
                    goto Label_000D;
                }
            }
        }

        private static void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            CachedPathData data = (CachedPathData) value;
            data._flags[0x40] = true;
            data.Close();
        }

        internal static void RemoveBadPathData(CachedPathData pathData)
        {
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            string configPath = pathData._configPath;
            string key = CreateKey(configPath);
            while (((pathData != null) && !pathData.CompletedFirstRequest) && !pathData.Exists)
            {
                cacheInternal.Remove(key);
                configPath = System.Configuration.ConfigPathUtility.GetParent(configPath);
                if (configPath == null)
                {
                    return;
                }
                key = CreateKey(configPath);
                pathData = (CachedPathData) cacheInternal.Get(key);
            }
        }

        private void WaitForInit()
        {
            if (!this._flags[1])
            {
                lock (this)
                {
                    if (!this._flags[1])
                    {
                        Monitor.Wait(this);
                    }
                }
            }
        }

        internal HandlerMappingMemo CachedHandler
        {
            get => 
                this._handlerMemo;
            set
            {
                this._handlerMemo = value;
            }
        }

        internal bool CompletedFirstRequest
        {
            get => 
                this._flags[2];
            set
            {
                this._flags[2] = value;
            }
        }

        internal IInternalConfigRecord ConfigRecord =>
            this._runtimeConfig?.ConfigRecord;

        internal bool Exists =>
            this._flags[4];

        internal VirtualPath Path =>
            this._virtualPath;

        internal string PhysicalPath =>
            this._physicalPath;

        internal System.Web.Configuration.RuntimeConfig RuntimeConfig =>
            this._runtimeConfig;
    }
}

