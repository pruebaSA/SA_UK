﻿namespace System.Web.Compilation
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Util;

    internal static class BrowserCapabilitiesCompiler
    {
        private static BrowserCapabilitiesFactoryBase _browserCapabilitiesFactoryBaseInstance;
        private static Type _browserCapabilitiesFactoryBaseType;
        private static Type _browserCapabilitiesFactoryBaseTypeForCompilation;
        private static Type _browserCapabilitiesFactoryType;
        private static object _lockObject = new object();
        internal static readonly VirtualPath AppBrowsersVirtualDir = HttpRuntime.AppDomainAppVirtualPathObject.SimpleCombineWithDir("App_Browsers");
        private const string browerCapabilitiesCacheKey = "__browserCapabilitiesCompiler";
        private const string browerCapabilitiesTypeName = "BrowserCapabilities";

        static BrowserCapabilitiesCompiler()
        {
            Assembly assembly = null;
            string browserCapAssemblyPublicKeyToken = BrowserCapabilitiesCodeGenerator.BrowserCapAssemblyPublicKeyToken;
            if (browserCapAssemblyPublicKeyToken != null)
            {
                try
                {
                    assembly = Assembly.Load("ASP.BrowserCapsFactory, Version=2.0.0.0, Culture=neutral, PublicKeyToken=" + browserCapAssemblyPublicKeyToken);
                }
                catch (FileNotFoundException)
                {
                }
            }
            if ((assembly == null) || !assembly.GlobalAssemblyCache)
            {
                _browserCapabilitiesFactoryBaseTypeForCompilation = typeof(System.Web.Configuration.BrowserCapabilitiesFactory);
                if (AppSettings.UseLegacyBrowserCaps)
                {
                    _browserCapabilitiesFactoryBaseType = _browserCapabilitiesFactoryBaseTypeForCompilation;
                }
                else
                {
                    _browserCapabilitiesFactoryBaseType = typeof(BrowserCapabilitiesFactory2);
                }
            }
            else
            {
                _browserCapabilitiesFactoryBaseType = assembly.GetType("ASP.BrowserCapabilitiesFactory", true);
                _browserCapabilitiesFactoryBaseTypeForCompilation = _browserCapabilitiesFactoryBaseType;
            }
        }

        private static bool AddBrowserFilesToList(VirtualDirectory directory, IList list, bool doRecurse)
        {
            bool flag = false;
            foreach (VirtualFileBase base2 in directory.Children)
            {
                if (base2.IsDirectory)
                {
                    if (doRecurse)
                    {
                        AddBrowserFilesToList((VirtualDirectory) base2, list, true);
                    }
                    flag = true;
                }
                else if (StringUtil.EqualsIgnoreCase(Path.GetExtension(base2.Name), ".browser"))
                {
                    list.Add(base2.VirtualPath);
                }
            }
            return flag;
        }

        internal static Type GetBrowserCapabilitiesFactoryBaseType() => 
            _browserCapabilitiesFactoryBaseTypeForCompilation;

        internal static Type GetBrowserCapabilitiesType()
        {
            InternalSecurityPermissions.Unrestricted.Assert();
            BuildResult buildResultFromCache = null;
            try
            {
                buildResultFromCache = BuildManager.GetBuildResultFromCache("__browserCapabilitiesCompiler");
                if (buildResultFromCache == null)
                {
                    DateTime utcNow = DateTime.UtcNow;
                    VirtualDirectory directory = AppBrowsersVirtualDir.GetDirectory();
                    string path = HostingEnvironment.MapPathInternal(AppBrowsersVirtualDir);
                    if ((directory != null) && Directory.Exists(path))
                    {
                        ArrayList list = new ArrayList();
                        ArrayList list2 = new ArrayList();
                        if (AddBrowserFilesToList(directory, list, false))
                        {
                            AddBrowserFilesToList(directory, list2, true);
                        }
                        else
                        {
                            list2 = list;
                        }
                        if (list2.Count > 0)
                        {
                            ApplicationBrowserCapabilitiesBuildProvider o = new ApplicationBrowserCapabilitiesBuildProvider();
                            foreach (string str2 in list)
                            {
                                o.AddFile(str2);
                            }
                            BuildProvidersCompiler compiler = new BuildProvidersCompiler(null, BuildManager.GenerateRandomAssemblyName("App_Browsers"));
                            compiler.SetBuildProviders(new SingleObjectCollection(o));
                            buildResultFromCache = new BuildResultCompiledType(compiler.PerformBuild().CompiledAssembly.GetType("ASP.ApplicationBrowserCapabilitiesFactory")) {
                                VirtualPath = AppBrowsersVirtualDir
                            };
                            buildResultFromCache.AddVirtualPathDependencies(list2);
                            BuildManager.CacheBuildResult("__browserCapabilitiesCompiler", buildResultFromCache, utcNow);
                        }
                    }
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            if (buildResultFromCache == null)
            {
                return _browserCapabilitiesFactoryBaseType;
            }
            return ((BuildResultCompiledType) buildResultFromCache).ResultType;
        }

        internal static BrowserCapabilitiesFactoryBase BrowserCapabilitiesFactory
        {
            get
            {
                if (_browserCapabilitiesFactoryBaseInstance == null)
                {
                    Type browserCapabilitiesType = GetBrowserCapabilitiesType();
                    lock (_lockObject)
                    {
                        if ((_browserCapabilitiesFactoryBaseInstance == null) && (browserCapabilitiesType != null))
                        {
                            _browserCapabilitiesFactoryBaseInstance = (BrowserCapabilitiesFactoryBase) Activator.CreateInstance(browserCapabilitiesType);
                        }
                    }
                }
                return _browserCapabilitiesFactoryBaseInstance;
            }
        }
    }
}

