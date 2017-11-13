namespace System.Web.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Extensions.Util;
    using System.Web.Hosting;
    using System.Web.Resources;
    using System.Web.UI;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptResourceHandler : IHttpHandler
    {
        private static readonly IDictionary _assemblyInfoCache = Hashtable.Synchronized(new Hashtable());
        private static bool _bypassVirtualPathResolution = false;
        private static readonly IDictionary _cultureCache = Hashtable.Synchronized(new Hashtable());
        private static readonly object _getMethodLock = new object();
        private static int _maximumResourceUrlLength = 0x400;
        private static string _scriptResourceAbsolutePath;
        private static IScriptResourceHandler _scriptResourceHandler = new RuntimeScriptResourceHandler();
        private const string _scriptResourceUrl = "~/ScriptResource.axd";

        private static Exception Create404(Exception innerException) => 
            new HttpException(0x194, AtlasWeb.ScriptResourceHandler_InvalidRequest, innerException);

        private static string DecryptParameter(NameValueCollection queryString)
        {
            string str2;
            string str = queryString["d"];
            if (string.IsNullOrEmpty(str))
            {
                Throw404();
            }
            try
            {
                str2 = Page.DecryptString(str);
            }
            catch (CryptographicException exception)
            {
                throw Create404(exception);
            }
            return str2;
        }

        internal static CultureInfo DetermineNearestAvailableCulture(Assembly assembly, string scriptResourceName, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(scriptResourceName))
            {
                return CultureInfo.InvariantCulture;
            }
            Tuple tuple = new Tuple(new object[] { assembly, scriptResourceName, culture });
            CultureInfo info = (CultureInfo) _cultureCache[tuple];
            if (info == null)
            {
                string resourceName = scriptResourceName.EndsWith(".debug.js", StringComparison.OrdinalIgnoreCase) ? (scriptResourceName.Substring(0, scriptResourceName.Length - 9) + ".js") : null;
                ScriptResourceInfo instance = ScriptResourceInfo.GetInstance(assembly, scriptResourceName);
                ScriptResourceInfo info3 = (resourceName != null) ? ScriptResourceInfo.GetInstance(assembly, resourceName) : null;
                if (!string.IsNullOrEmpty(instance.ScriptResourceName) || ((info3 != null) && !string.IsNullOrEmpty(info3.ScriptResourceName)))
                {
                    ResourceManager resourceManager = ScriptResourceAttribute.GetResourceManager(instance.ScriptResourceName, assembly);
                    ResourceManager manager2 = (info3 != null) ? ScriptResourceAttribute.GetResourceManager(info3.ScriptResourceName, assembly) : null;
                    ResourceSet set = null;
                    ResourceSet set2 = null;
                    if (resourceManager != null)
                    {
                        resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                        set = resourceManager.GetResourceSet(culture, true, false);
                    }
                    if (manager2 != null)
                    {
                        manager2.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                        set2 = manager2.GetResourceSet(culture, true, false);
                    }
                    if ((resourceManager != null) || (manager2 != null))
                    {
                        while ((set == null) && (set2 == null))
                        {
                            culture = culture.Parent;
                            if (culture.Equals(CultureInfo.InvariantCulture))
                            {
                                break;
                            }
                            set = resourceManager.GetResourceSet(culture, true, false);
                            set2 = manager2?.GetResourceSet(culture, true, false);
                        }
                    }
                    else
                    {
                        culture = CultureInfo.InvariantCulture;
                    }
                }
                else
                {
                    culture = CultureInfo.InvariantCulture;
                }
                CultureInfo assemblyNeutralCulture = GetAssemblyNeutralCulture(assembly);
                if ((assemblyNeutralCulture != null) && assemblyNeutralCulture.Equals(culture))
                {
                    culture = CultureInfo.InvariantCulture;
                }
                info = culture;
                _cultureCache[tuple] = info;
            }
            return info;
        }

        private static void EnsureScriptResourceRequest(string path)
        {
            if (!IsScriptResourceRequest(path))
            {
                Throw404();
            }
        }

        private static Assembly GetAssembly(string assemblyName)
        {
            string[] strArray = assemblyName.Split(new char[] { ',' });
            if ((strArray.Length != 1) && (strArray.Length != 4))
            {
                Throw404();
            }
            AssemblyName assemblyRef = new AssemblyName {
                Name = strArray[0]
            };
            if (strArray.Length == 4)
            {
                assemblyRef.Version = new Version(strArray[1]);
                string name = strArray[2];
                assemblyRef.CultureInfo = (name.Length > 0) ? new CultureInfo(name) : CultureInfo.InvariantCulture;
                assemblyRef.SetPublicKeyToken(HexParser.Parse(strArray[3]));
            }
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyRef);
            }
            catch (FileNotFoundException exception)
            {
                Throw404(exception);
            }
            catch (FileLoadException exception2)
            {
                Throw404(exception2);
            }
            catch (BadImageFormatException exception3)
            {
                Throw404(exception3);
            }
            return assembly;
        }

        private static Pair<AssemblyName, string> GetAssemblyInfo(Assembly assembly)
        {
            Pair<AssemblyName, string> assemblyInfoInternal = (Pair<AssemblyName, string>) _assemblyInfoCache[assembly];
            if (assemblyInfoInternal == null)
            {
                assemblyInfoInternal = GetAssemblyInfoInternal(assembly);
                _assemblyInfoCache[assembly] = assemblyInfoInternal;
            }
            return assemblyInfoInternal;
        }

        private static Pair<AssemblyName, string> GetAssemblyInfoInternal(Assembly assembly)
        {
            AssemblyName first = new AssemblyName(assembly.FullName);
            return new Pair<AssemblyName, string>(first, Convert.ToBase64String(assembly.ManifestModule.ModuleVersionId.ToByteArray()));
        }

        private static CultureInfo GetAssemblyNeutralCulture(Assembly assembly)
        {
            CultureInfo cultureInfo = (CultureInfo) _cultureCache[assembly];
            if (cultureInfo == null)
            {
                object[] customAttributes = assembly.GetCustomAttributes(typeof(NeutralResourcesLanguageAttribute), false);
                if ((customAttributes != null) && (customAttributes.Length != 0))
                {
                    cultureInfo = CultureInfo.GetCultureInfo(((NeutralResourcesLanguageAttribute) customAttributes[0]).CultureName);
                    _cultureCache[assembly] = cultureInfo;
                }
            }
            return cultureInfo;
        }

        internal static string GetEmptyPageUrl(string title) => 
            GetScriptResourceHandler().GetEmptyPageUrl(title);

        private static IScriptResourceHandler GetScriptResourceHandler()
        {
            if (_scriptResourceHandler == null)
            {
                _scriptResourceHandler = new RuntimeScriptResourceHandler();
            }
            return _scriptResourceHandler;
        }

        internal static string GetScriptResourceUrl(List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists, bool zip, bool notifyScriptLoaded) => 
            GetScriptResourceHandler().GetScriptResourceUrl(assemblyResourceLists, zip, notifyScriptLoaded);

        internal static string GetScriptResourceUrl(Assembly assembly, string resourceName, CultureInfo culture, bool zip, bool notifyScriptLoaded) => 
            GetScriptResourceHandler().GetScriptResourceUrl(assembly, resourceName, culture, zip, notifyScriptLoaded);

        [SecurityTreatAsSafe, SecurityCritical, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), ReflectionPermission(SecurityAction.Assert, Unrestricted=true), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
        internal static string GetWebResourceUrl(Assembly assembly, string resourceName) => 
            AssemblyResourceLoader.GetWebResourceUrlInternal(assembly, resourceName, false);

        private static bool IsCompressionEnabled(HttpContext context)
        {
            if (!ScriptingScriptResourceHandlerSection.ApplicationSettings.EnableCompression)
            {
                return false;
            }
            if ((context != null) && context.Request.Browser.IsBrowser("IE"))
            {
                return (context.Request.Browser.MajorVersion > 6);
            }
            return true;
        }

        internal static bool IsScriptResourceRequest(string path) => 
            (!string.IsNullOrEmpty(path) && string.Equals(path, ScriptResourceAbsolutePath, StringComparison.OrdinalIgnoreCase));

        private static void OutputEmptyPage(HttpResponse response, string title)
        {
            PrepareResponseCache(response);
            response.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><script type=\"text/javascript\">parent.Sys.Application._onIFrameLoad();</script><title>" + HttpUtility.HtmlEncode(title) + "</title></head><body></body></html>");
        }

        private static void PrepareResponseCache(HttpResponse response)
        {
            HttpCachePolicy cache = response.Cache;
            DateTime now = DateTime.Now;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["d"] = true;
            cache.SetOmitVaryStar(true);
            cache.SetExpires(now + TimeSpan.FromDays(365.0));
            cache.SetValidUntilExpires(true);
            cache.SetLastModified(now);
        }

        private static void PrepareResponseNoCache(HttpResponse response)
        {
            HttpCachePolicy cache = response.Cache;
            DateTime now = DateTime.Now;
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(now + TimeSpan.FromDays(365.0));
            cache.SetValidUntilExpires(true);
            cache.SetLastModified(now);
            cache.SetNoServerCaching();
        }

        protected virtual void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpResponse response = context.Response;
                response.Clear();
                EnsureScriptResourceRequest(context.Request.Path);
                ProcessRequestInternal(response, context.Request.QueryString, delegate (string virtualPath, out Encoding encoding) {
                    string str;
                    VirtualPathProvider virtualPathProvider = HostingEnvironment.VirtualPathProvider;
                    if (!virtualPathProvider.FileExists(virtualPath))
                    {
                        Throw404();
                    }
                    VirtualFile file = virtualPathProvider.GetFile(virtualPath);
                    if (!System.Web.Extensions.Util.AppSettings.ScriptResourceAllowNonJsFiles && !file.Name.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                    {
                        Throw404();
                    }
                    using (Stream stream = file.Open())
                    {
                        using (StreamReader reader = new StreamReader(stream, true))
                        {
                            encoding = reader.CurrentEncoding;
                            str = reader.ReadToEnd();
                        }
                    }
                    return str;
                });
            }
            catch
            {
                Throw404();
            }
        }

        private static void ProcessRequestInternal(HttpResponse response, NameValueCollection queryString, VirtualFileReader fileReader)
        {
            bool flag;
            bool flag2;
            bool flag3;
            string str = DecryptParameter(queryString);
            if (string.IsNullOrEmpty(str))
            {
                Throw404();
            }
            switch (str[0])
            {
                case 'q':
                    flag3 = false;
                    flag = true;
                    flag2 = false;
                    break;

                case 'r':
                    flag3 = false;
                    flag = false;
                    flag2 = false;
                    break;

                case 'u':
                    flag3 = true;
                    flag = false;
                    flag2 = false;
                    break;

                case 'z':
                    flag3 = true;
                    flag = true;
                    flag2 = false;
                    break;

                case 'Q':
                    flag3 = false;
                    flag = true;
                    flag2 = true;
                    break;

                case 'R':
                    flag3 = false;
                    flag = false;
                    flag2 = true;
                    break;

                case 'T':
                    OutputEmptyPage(response, str.Substring(1));
                    return;

                case 'U':
                    flag3 = true;
                    flag = false;
                    flag2 = true;
                    break;

                case 'Z':
                    flag3 = true;
                    flag = true;
                    flag2 = true;
                    break;

                default:
                    Throw404();
                    return;
            }
            str = str.Substring(1);
            if (string.IsNullOrEmpty(str))
            {
                Throw404();
            }
            string[] strArray = str.Split(new char[] { '|' });
            if (flag3)
            {
                if ((strArray.Length != 3) && (strArray.Length != 5))
                {
                    Throw404();
                }
            }
            else if ((strArray.Length % 2) != 0)
            {
                Throw404();
            }
            StringBuilder builder = new StringBuilder();
            string contentType = null;
            if (flag3)
            {
                string assemblyName = strArray[0];
                string resourceName = strArray[1];
                string str5 = strArray[2];
                Assembly assembly = GetAssembly(assemblyName);
                if (assembly == null)
                {
                    Throw404();
                }
                builder.Append(ScriptResourceAttribute.GetScriptFromWebResourceInternal(assembly, resourceName, string.IsNullOrEmpty(str5) ? CultureInfo.InvariantCulture : new CultureInfo(str5), flag, false, out contentType));
            }
            else
            {
                bool flag4 = false;
                for (int i = 0; i < strArray.Length; i += 2)
                {
                    string str6 = strArray[i];
                    bool flag5 = !string.IsNullOrEmpty(str6);
                    if (!flag5 || (str6[0] != '#'))
                    {
                        string[] strArray2 = strArray[i + 1].Split(new char[] { ',' });
                        if (strArray2.Length == 0)
                        {
                            Throw404();
                        }
                        Assembly assembly2 = flag5 ? GetAssembly(str6) : null;
                        if (assembly2 == null)
                        {
                            if (contentType == null)
                            {
                                contentType = "text/javascript";
                            }
                            for (int j = 0; j < strArray2.Length; j++)
                            {
                                Encoding encoding;
                                string virtualPath = _bypassVirtualPathResolution ? strArray2[j] : VirtualPathUtility.ToAbsolute(strArray2[j]);
                                string str8 = fileReader(virtualPath, out encoding);
                                if (flag4)
                                {
                                    builder.Append('\n');
                                }
                                flag4 = true;
                                builder.Append(str8);
                            }
                        }
                        else
                        {
                            for (int k = 0; k < strArray2.Length; k += 2)
                            {
                                try
                                {
                                    string str9;
                                    string str10 = strArray2[k];
                                    string str11 = strArray2[k + 1];
                                    if (flag4)
                                    {
                                        builder.Append('\n');
                                    }
                                    flag4 = true;
                                    builder.Append(ScriptResourceAttribute.GetScriptFromWebResourceInternal(assembly2, str10, string.IsNullOrEmpty(str11) ? CultureInfo.InvariantCulture : new CultureInfo(str11), flag, false, out str9));
                                    if (contentType == null)
                                    {
                                        contentType = str9;
                                    }
                                }
                                catch (MissingManifestResourceException exception)
                                {
                                    throw Create404(exception);
                                }
                                catch (HttpException exception2)
                                {
                                    throw Create404(exception2);
                                }
                            }
                        }
                    }
                }
            }
            ScriptResourceAttribute.WriteNotificationToStringBuilder(flag2, builder);
            if (ScriptingScriptResourceHandlerSection.ApplicationSettings.EnableCaching)
            {
                PrepareResponseCache(response);
            }
            else
            {
                PrepareResponseNoCache(response);
            }
            response.ContentType = contentType;
            if (flag)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (Stream stream2 = new GZipStream(stream, CompressionMode.Compress))
                    {
                        using (StreamWriter writer = new StreamWriter(stream2, Encoding.UTF8))
                        {
                            writer.Write(builder.ToString());
                        }
                    }
                    byte[] buffer = stream.ToArray();
                    response.AddHeader("Content-encoding", "gzip");
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    return;
                }
            }
            response.Write(builder.ToString());
        }

        internal static void SetScriptResourceHandler(IScriptResourceHandler scriptResourceHandler)
        {
            _scriptResourceHandler = scriptResourceHandler;
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            this.ProcessRequest(context);
        }

        private static void Throw404()
        {
            throw Create404(null);
        }

        private static void Throw404(Exception innerException)
        {
            throw Create404(innerException);
        }

        protected virtual bool IsReusable =>
            true;

        private static string ScriptResourceAbsolutePath
        {
            get
            {
                if (_scriptResourceAbsolutePath == null)
                {
                    _scriptResourceAbsolutePath = VirtualPathUtility.ToAbsolute("~/ScriptResource.axd");
                }
                return _scriptResourceAbsolutePath;
            }
        }

        bool IHttpHandler.IsReusable =>
            this.IsReusable;

        private class RuntimeScriptResourceHandler : IScriptResourceHandler
        {
            private static string _absoluteScriptResourceUrl;
            private static readonly IDictionary _cultureCache = Hashtable.Synchronized(new Hashtable());
            private static readonly IDictionary _urlCache = Hashtable.Synchronized(new Hashtable());

            private static void EnsureAbsoluteScriptResourceUrl()
            {
                if (_absoluteScriptResourceUrl == null)
                {
                    _absoluteScriptResourceUrl = ScriptResourceHandler._bypassVirtualPathResolution ? "~/ScriptResource.axd?d=" : (VirtualPathUtility.ToAbsolute("~/ScriptResource.axd") + "?d=");
                }
            }

            private static string GetScriptResourceUrlImpl(List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists, bool zip, bool notifyScriptLoaded)
            {
                string str;
                string str5;
                EnsureAbsoluteScriptResourceUrl();
                bool flag = false;
                if (assemblyResourceLists.Count == 1)
                {
                    Pair<Assembly, List<Pair<string, CultureInfo>>> pair = assemblyResourceLists[0];
                    if ((pair.First != null) && (pair.Second.Count == 1))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    str = zip ? (notifyScriptLoaded ? "Z" : "z") : (notifyScriptLoaded ? "U" : "u");
                }
                else
                {
                    str = zip ? (notifyScriptLoaded ? "Q" : "q") : (notifyScriptLoaded ? "R" : "r");
                }
                StringBuilder builder = new StringBuilder(str);
                HashCodeCombiner combiner = new HashCodeCombiner();
                bool flag2 = true;
                foreach (Pair<Assembly, List<Pair<string, CultureInfo>>> pair2 in assemblyResourceLists)
                {
                    if (!flag2)
                    {
                        builder.Append('|');
                    }
                    else
                    {
                        flag2 = false;
                    }
                    if (pair2.First != null)
                    {
                        Pair<AssemblyName, string> assemblyInfo = ScriptResourceHandler.GetAssemblyInfo(pair2.First);
                        AssemblyName first = assemblyInfo.First;
                        string second = assemblyInfo.Second;
                        combiner.AddObject(second);
                        if (pair2.First.GlobalAssemblyCache)
                        {
                            builder.Append(first.Name);
                            builder.Append(',');
                            builder.Append(first.Version);
                            builder.Append(',');
                            if (first.CultureInfo != null)
                            {
                                builder.Append(first.CultureInfo);
                            }
                            builder.Append(',');
                            builder.Append(HexParser.ToString(first.GetPublicKeyToken()));
                        }
                        else
                        {
                            builder.Append(first.Name);
                        }
                    }
                    builder.Append('|');
                    bool flag3 = true;
                    foreach (Pair<string, CultureInfo> pair4 in pair2.Second)
                    {
                        if (!flag3)
                        {
                            builder.Append(',');
                        }
                        if (pair2.First != null)
                        {
                            builder.Append(pair4.First);
                            Tuple tuple = new Tuple(new object[] { pair2.First, pair4.First, pair4.Second });
                            string name = (string) _cultureCache[tuple];
                            if (name == null)
                            {
                                ScriptResourceInfo instance = ScriptResourceInfo.GetInstance(pair2.First, pair4.First);
                                if (instance == ScriptResourceInfo.Empty)
                                {
                                    ThrowUnknownResource(pair4.First);
                                }
                                if (pair2.First.GetManifestResourceStream(instance.ScriptName) == null)
                                {
                                    ThrowUnknownResource(pair4.First);
                                }
                                name = ScriptResourceHandler.DetermineNearestAvailableCulture(pair2.First, pair4.First, pair4.Second).Name;
                                _cultureCache[tuple] = name;
                            }
                            builder.Append(flag ? "|" : ",");
                            builder.Append(name);
                        }
                        else
                        {
                            if (!ScriptResourceHandler._bypassVirtualPathResolution)
                            {
                                VirtualPathProvider virtualPathProvider = HostingEnvironment.VirtualPathProvider;
                                if (!virtualPathProvider.FileExists(pair4.First))
                                {
                                    ThrowUnknownResource(pair4.First);
                                }
                                string fileHash = virtualPathProvider.GetFileHash(pair4.First, new string[] { pair4.First });
                                combiner.AddObject(fileHash);
                            }
                            builder.Append(pair4.First);
                        }
                        flag3 = false;
                    }
                }
                if (flag)
                {
                    str5 = _absoluteScriptResourceUrl + Page.EncryptString(builder.ToString()) + "&t=" + combiner.CombinedHashString;
                }
                else
                {
                    builder.Append("|#|");
                    builder.Append(combiner.CombinedHashString);
                    str5 = _absoluteScriptResourceUrl + Page.EncryptString(builder.ToString());
                }
                if (str5.Length > ScriptResourceHandler._maximumResourceUrlLength)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptResourceHandler_ResourceUrlLongerThan1024Characters);
                }
                return str5;
            }

            string IScriptResourceHandler.GetEmptyPageUrl(string title)
            {
                EnsureAbsoluteScriptResourceUrl();
                return (_absoluteScriptResourceUrl + Page.EncryptString('T' + title));
            }

            string IScriptResourceHandler.GetScriptResourceUrl(List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists, bool zip, bool notifyScriptLoaded)
            {
                if (!ScriptResourceHandler.IsCompressionEnabled(HttpContext.Current))
                {
                    zip = false;
                }
                bool flag = true;
                foreach (Pair<Assembly, List<Pair<string, CultureInfo>>> pair in assemblyResourceLists)
                {
                    if (pair.First == null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    return GetScriptResourceUrlImpl(assemblyResourceLists, zip, notifyScriptLoaded);
                }
                List<object> list = new List<object>();
                foreach (Pair<Assembly, List<Pair<string, CultureInfo>>> pair2 in assemblyResourceLists)
                {
                    list.Add(pair2.First);
                    foreach (Pair<string, CultureInfo> pair3 in pair2.Second)
                    {
                        list.Add(pair3.First);
                        list.Add(pair3.Second);
                    }
                }
                list.Add(zip);
                list.Add(notifyScriptLoaded);
                Tuple tuple = new Tuple(list.ToArray());
                string str = (string) _urlCache[tuple];
                if (str == null)
                {
                    str = GetScriptResourceUrlImpl(assemblyResourceLists, zip, notifyScriptLoaded);
                    _urlCache[tuple] = str;
                }
                return str;
            }

            string IScriptResourceHandler.GetScriptResourceUrl(Assembly assembly, string resourceName, CultureInfo culture, bool zip, bool notifyScriptLoaded)
            {
                List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists = new List<Pair<Assembly, List<Pair<string, CultureInfo>>>>();
                List<Pair<string, CultureInfo>> second = new List<Pair<string, CultureInfo>> {
                    new Pair<string, CultureInfo>(resourceName, culture)
                };
                assemblyResourceLists.Add(new Pair<Assembly, List<Pair<string, CultureInfo>>>(assembly, second));
                return ((IScriptResourceHandler) this).GetScriptResourceUrl(assemblyResourceLists, zip, notifyScriptLoaded);
            }

            private static void ThrowUnknownResource(string resourceName)
            {
                throw new HttpException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ScriptResourceHandler_UnknownResource, new object[] { resourceName }));
            }
        }

        internal delegate string VirtualFileReader(string virtualPath, out Encoding encoding);
    }
}

