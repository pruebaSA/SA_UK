namespace System.Web.Compilation
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Profile;
    using System.Web.UI;
    using System.Web.Util;
    using System.Xml;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Medium)]
    public sealed class BuildManager
    {
        private Assembly _appResourcesAssembly;
        private IDictionary _assemblyResolveMapping;
        private BuildResultCache[] _caches;
        private ClientBuildManagerCallback _cbmCallback;
        private ArrayList _codeAssemblies;
        private System.Web.Compilation.CompilationStage _compilationStage;
        private StringSet _excludedCodeSubdirectories;
        private StringSet _excludedTopLevelDirectories;
        private StringSet _forbiddenTopLevelDirectories;
        private Dictionary<string, string> _generatedFileTable;
        private BuildResultCompiledGlobalAsaxType _globalAsaxBuildResult;
        private VirtualPath _globalAsaxVirtualPath;
        private static Exception _initializeException;
        private bool _isPrecompiledApp;
        private bool _isPrecompiledAppComputed;
        private bool _isUpdatablePrecompiledApp;
        private static SimpleRecyclingCache _keyCache = new SimpleRecyclingCache();
        private Hashtable _localResourcesAssemblies = new Hashtable();
        private MemoryBuildResultCache _memoryCache;
        private bool _optimizeCompilations;
        private static bool _parseErrorReported;
        private bool _performingPrecompilation;
        private PrecompilationFlags _precompilationFlags;
        private bool _precompilingApp;
        private string _precompTargetPhysicalDir;
        private Type _profileType;
        private static RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        private VirtualPath _scriptVirtualDir;
        private bool _skipTopLevelCompilationExceptions;
        private string _strongNameKeyContainer;
        private string _strongNameKeyFile;
        private static BuildManager _theBuildManager = new BuildManager();
        private static bool _theBuildManagerInitialized;
        private bool _throwOnFirstParseError = true;
        private Dictionary<string, AssemblyReferenceInfo> _topLevelAssembliesIndexTable;
        private Exception _topLevelFileCompilationException;
        private bool _topLevelFilesCompiledCompleted;
        private bool _topLevelFilesCompiledStarted;
        private List<Assembly> _topLevelReferencedAssemblies;
        private string _webHashFilePath;
        internal const string AppBrowserCapAssemblyNamePrefix = "App_Browsers";
        internal const string AppThemeAssemblyNamePrefix = "App_Theme_";
        internal const string AssemblyNamePrefix = "App_";
        private const string BatchCompilationSlotName = "BatchCompileChk";
        private const string CircularReferenceCheckerSlotName = "CircRefChk";
        private const string CodeDirectoryAssemblyName = "App_Code";
        internal const string GlobalAsaxAssemblyName = "App_global.asax";
        internal const string GlobalThemeAssemblyNamePrefix = "App_GlobalTheme_";
        private const string LicensesAssemblyName = "App_Licenses";
        private const string LocalResourcesDirectoryAssemblyName = "App_LocalResources";
        private const string precompMarkerFileName = "PrecompiledApp.config";
        private const string ResourcesDirectoryAssemblyName = "App_GlobalResources";
        private static long s_topLevelHash;
        internal const string SubCodeDirectoryAssemblyNamePrefix = "App_SubCode_";
        internal const string UpdatableInheritReplacementToken = "__ASPNET_INHERITS";
        internal const string WebAssemblyNamePrefix = "App_Web_";
        private const string WebRefDirectoryAssemblyName = "App_WebReferences";

        private BuildManager()
        {
        }

        private bool BatchCompileWebDirectory(VirtualDirectory vdir, VirtualPath virtualDir, bool ignoreErrors)
        {
            if (virtualDir == null)
            {
                virtualDir = vdir.VirtualPathObject;
            }
            if (vdir == null)
            {
                vdir = HostingEnvironment.VirtualPathProvider.GetDirectory(virtualDir);
            }
            CaseInsensitiveStringSet data = CallContext.GetData("BatchCompileChk") as CaseInsensitiveStringSet;
            if (data == null)
            {
                data = new CaseInsensitiveStringSet();
                CallContext.SetData("BatchCompileChk", data);
            }
            if (data.Contains(vdir.VirtualPath))
            {
                return false;
            }
            data.Add(vdir.VirtualPath);
            if (this._precompilingApp)
            {
                ignoreErrors = false;
            }
            return this.BatchCompileWebDirectoryInternal(vdir, ignoreErrors);
        }

        private bool BatchCompileWebDirectoryInternal(VirtualDirectory vdir, bool ignoreErrors)
        {
            WebDirectoryBatchCompiler compiler = new WebDirectoryBatchCompiler(vdir);
            if (ignoreErrors)
            {
                compiler.SetIgnoreErrors();
                try
                {
                    compiler.Process();
                    goto Label_0023;
                }
                catch
                {
                    return false;
                }
            }
            compiler.Process();
        Label_0023:
            return true;
        }

        internal static bool CacheBuildResult(string cacheKey, BuildResult result, DateTime utcStart) => 
            _theBuildManager.CacheBuildResultInternal(cacheKey, result, 0L, utcStart);

        private bool CacheBuildResultInternal(string cacheKey, BuildResult result, long hashCode, DateTime utcStart)
        {
            result.EnsureVirtualPathDependenciesHashComputed();
            for (int i = 0; i < this._caches.Length; i++)
            {
                this._caches[i].CacheBuildResult(cacheKey, result, hashCode, utcStart);
            }
            if (!TimeStampChecker.CheckFilesStillValid(cacheKey, result.VirtualPathDependencies))
            {
                this._memoryCache.RemoveAssemblyAndCleanupDependencies(result as BuildResultCompiledAssemblyBase);
                return false;
            }
            return true;
        }

        internal static bool CacheVPathBuildResult(VirtualPath virtualPath, BuildResult result, DateTime utcStart) => 
            _theBuildManager.CacheVPathBuildResultInternal(virtualPath, result, utcStart);

        private bool CacheVPathBuildResultInternal(VirtualPath virtualPath, BuildResult result, DateTime utcStart) => 
            CacheBuildResult(GetCacheKeyFromVirtualPath(virtualPath), result, utcStart);

        internal static void CallAppInitializeMethod()
        {
            _theBuildManager.EnsureTopLevelFilesCompiled();
            CodeDirectoryCompiler.CallAppInitializeMethod();
        }

        private void CheckTopLevelFilesUpToDate(StandardDiskBuildResultCache diskCache)
        {
            bool gotLock = false;
            try
            {
                CompilationLock.GetLock(ref gotLock);
                this.CheckTopLevelFilesUpToDate2(diskCache);
            }
            finally
            {
                if (gotLock)
                {
                    CompilationLock.ReleaseLock();
                }
            }
        }

        private void CheckTopLevelFilesUpToDate2(StandardDiskBuildResultCache diskCache)
        {
            long preservedSpecialFilesCombinedHash = diskCache.GetPreservedSpecialFilesCombinedHash();
            if (preservedSpecialFilesCombinedHash != 0L)
            {
                diskCache.RemoveOldTempFiles();
            }
            HashCodeCombiner combiner = new HashCodeCombiner();
            combiner.AddObject(HttpRuntime.AppDomainAppPathInternal);
            string fullyQualifiedName = typeof(HttpRuntime).Module.FullyQualifiedName;
            combiner.AddFile(fullyQualifiedName);
            string machineConfigurationFilePath = HttpConfigurationSystem.MachineConfigurationFilePath;
            combiner.AddFile(machineConfigurationFilePath);
            string rootWebConfigurationFilePath = HttpConfigurationSystem.RootWebConfigurationFilePath;
            combiner.AddFile(rootWebConfigurationFilePath);
            RuntimeConfig appConfig = RuntimeConfig.GetAppConfig();
            CompilationSection compilation = appConfig.Compilation;
            if (!BuildManagerHost.InClientBuildManager)
            {
                this._optimizeCompilations = compilation.OptimizeCompilations;
            }
            if (!OptimizeCompilations)
            {
                string binDirectoryInternal = HttpRuntime.BinDirectoryInternal;
                combiner.AddDirectory(binDirectoryInternal);
                combiner.AddResourcesDirectory(HttpRuntime.ResourcesDirectoryVirtualPath.MapPathInternal());
                combiner.AddDirectory(HttpRuntime.WebRefDirectoryVirtualPath.MapPathInternal());
                combiner.AddDirectory(HttpRuntime.CodeDirectoryVirtualPath.MapPathInternal());
                combiner.AddFile(GlobalAsaxVirtualPath.MapPathInternal());
            }
            combiner.AddObject(compilation.RecompilationHash);
            ProfileSection profile = appConfig.Profile;
            combiner.AddObject(profile.RecompilationHash);
            combiner.AddObject(appConfig.Globalization.FileEncoding);
            TrustSection trust = appConfig.Trust;
            combiner.AddObject(trust.Level);
            combiner.AddObject(trust.OriginUrl);
            combiner.AddObject(ProfileManager.Enabled);
            combiner.AddObject(PrecompilingWithDebugInfo);
            s_topLevelHash = combiner.CombinedHash;
            if (PrecompilingForCleanBuild || (combiner.CombinedHash != preservedSpecialFilesCombinedHash))
            {
                bool precompilingForCleanBuild = PrecompilingForCleanBuild;
                diskCache.RemoveAllCodegenFiles();
                diskCache.SavePreservedSpecialFilesCombinedHash(combiner.CombinedHash);
            }
            HttpRuntime.FileChangesMonitor.StartMonitoringFile(this._webHashFilePath, new FileChangeEventHandler(this.OnWebHashFileChange));
        }

        private void CompileCodeDirectories()
        {
            VirtualPath codeDirectoryVirtualPath = HttpRuntime.CodeDirectoryVirtualPath;
            CodeSubDirectoriesCollection codeSubDirectories = CompilationUtil.GetCodeSubDirectories();
            if (codeSubDirectories != null)
            {
                foreach (CodeSubDirectory directory in codeSubDirectories)
                {
                    VirtualPath virtualDir = codeDirectoryVirtualPath.SimpleCombineWithDir(directory.DirectoryName);
                    string assemblyName = "App_SubCode_" + directory.AssemblyName;
                    this.CompileCodeDirectory(virtualDir, CodeDirectoryType.SubCode, assemblyName, null);
                }
            }
            this.EnsureExcludedCodeSubDirectoriesComputed();
            this.CompileCodeDirectory(codeDirectoryVirtualPath, CodeDirectoryType.MainCode, "App_Code", this._excludedCodeSubdirectories);
        }

        private Assembly CompileCodeDirectory(VirtualPath virtualDir, CodeDirectoryType dirType, string assemblyName, StringSet excludedSubdirectories)
        {
            bool isDirectoryAllowed = true;
            if (IsPrecompiledApp)
            {
                if (this.IsUpdatablePrecompiledAppInternal && (dirType == CodeDirectoryType.LocalResources))
                {
                    isDirectoryAllowed = true;
                }
                else
                {
                    isDirectoryAllowed = false;
                }
            }
            AssemblyReferenceInfo info = new AssemblyReferenceInfo(this._topLevelReferencedAssemblies.Count);
            this._topLevelAssembliesIndexTable[virtualDir.VirtualPathString] = info;
            Assembly item = CodeDirectoryCompiler.GetCodeDirectoryAssembly(virtualDir, dirType, assemblyName, excludedSubdirectories, isDirectoryAllowed);
            if (item != null)
            {
                info.Assembly = item;
                if (dirType == CodeDirectoryType.LocalResources)
                {
                    return item;
                }
                this._topLevelReferencedAssemblies.Add(item);
                if ((dirType == CodeDirectoryType.MainCode) || (dirType == CodeDirectoryType.SubCode))
                {
                    if (this._codeAssemblies == null)
                    {
                        this._codeAssemblies = new ArrayList();
                    }
                    this._codeAssemblies.Add(item);
                }
                if (this._assemblyResolveMapping == null)
                {
                    this._assemblyResolveMapping = new Hashtable(StringComparer.OrdinalIgnoreCase);
                }
                this._assemblyResolveMapping[assemblyName] = item;
                if (dirType == CodeDirectoryType.MainCode)
                {
                    this._profileType = ProfileBuildProvider.GetProfileTypeFromAssembly(item, IsPrecompiledApp);
                    this._assemblyResolveMapping["__code"] = item;
                }
            }
            return item;
        }

        private void CompileGlobalAsax()
        {
            this._globalAsaxBuildResult = ApplicationBuildProvider.GetGlobalAsaxBuildResult(IsPrecompiledApp);
            HttpApplicationFactory.SetupFileChangeNotifications();
            if (this._globalAsaxBuildResult != null)
            {
                for (Type type = this._globalAsaxBuildResult.ResultType; type.Assembly != typeof(HttpRuntime).Assembly; type = type.BaseType)
                {
                    this._topLevelReferencedAssemblies.Add(type.Assembly);
                }
            }
        }

        private void CompileResourcesDirectory()
        {
            VirtualPath resourcesDirectoryVirtualPath = HttpRuntime.ResourcesDirectoryVirtualPath;
            this._appResourcesAssembly = this.CompileCodeDirectory(resourcesDirectoryVirtualPath, CodeDirectoryType.AppResources, "App_GlobalResources", null);
        }

        private BuildResult CompileWebFile(VirtualPath virtualPath)
        {
            BuildResult buildResult = null;
            if (this._topLevelFilesCompiledCompleted)
            {
                VirtualPath parent = virtualPath.Parent;
                if (this.IsBatchEnabledForDirectory(parent))
                {
                    this.BatchCompileWebDirectory(null, parent, true);
                    string cacheKeyFromVirtualPath = GetCacheKeyFromVirtualPath(virtualPath);
                    buildResult = this._memoryCache.GetBuildResult(cacheKeyFromVirtualPath);
                    if (buildResult != null)
                    {
                        if (buildResult is BuildResultCompileError)
                        {
                            throw ((BuildResultCompileError) buildResult).CompileException;
                        }
                        return buildResult;
                    }
                }
            }
            DateTime utcNow = DateTime.UtcNow;
            string outputAssemblyName = "App_Web_" + GenerateRandomAssemblyName(GetGeneratedAssemblyBaseName(virtualPath), false);
            BuildProvidersCompiler compiler = new BuildProvidersCompiler(virtualPath, outputAssemblyName);
            System.Web.Compilation.BuildProvider o = CreateBuildProvider(virtualPath, compiler.CompConfig, compiler.ReferencedAssemblies, true);
            compiler.SetBuildProviders(new SingleObjectCollection(o));
            try
            {
                CompilerResults results = compiler.PerformBuild();
                buildResult = o.GetBuildResult(results);
            }
            catch (HttpCompileException exception)
            {
                if (exception.DontCache)
                {
                    throw;
                }
                buildResult = new BuildResultCompileError(virtualPath, exception);
                o.SetBuildResultDependencies(buildResult);
                exception.VirtualPathDependencies = o.VirtualPathDependencies;
                this.CacheVPathBuildResultInternal(virtualPath, buildResult, utcNow);
                exception.DontCache = true;
                throw;
            }
            if (buildResult == null)
            {
                return null;
            }
            this.CacheVPathBuildResultInternal(virtualPath, buildResult, utcNow);
            return buildResult;
        }

        private void CompileWebRefDirectory()
        {
            this.CompileCodeDirectory(HttpRuntime.WebRefDirectoryVirtualPath, CodeDirectoryType.WebReferences, "App_WebReferences", null);
        }

        private void CopyCompiledAssembliesToDestinationBin(string fromDir, string toDir)
        {
            bool flag = false;
            foreach (FileData data in (IEnumerable) FileEnumerator.Create(fromDir))
            {
                if (!flag)
                {
                    Directory.CreateDirectory(toDir);
                }
                flag = true;
                if (data.IsDirectory)
                {
                    if (Util.IsCultureName(data.Name))
                    {
                        string str = Path.Combine(fromDir, data.Name);
                        string str2 = Path.Combine(toDir, data.Name);
                        this.CopyCompiledAssembliesToDestinationBin(str, str2);
                    }
                }
                else
                {
                    switch (Path.GetExtension(data.Name))
                    {
                        case ".dll":
                        case ".pdb":
                        {
                            string sourceFileName = Path.Combine(fromDir, data.Name);
                            string destFileName = Path.Combine(toDir, data.Name);
                            File.Copy(sourceFileName, destFileName, true);
                            break;
                        }
                    }
                }
            }
        }

        private void CopyPrecompiledFile(VirtualFile vfile, string destPhysicalPath)
        {
            bool flag;
            if (CompilationUtil.NeedToCopyFile(vfile.VirtualPathObject, PrecompilingForUpdatableDeployment, out flag))
            {
                string sourceFileName = HostingEnvironment.MapPathInternal(vfile.VirtualPath);
                if (File.Exists(destPhysicalPath))
                {
                    BuildResultCompiledType type = GetVPathBuildResult(null, vfile.VirtualPathObject, true, false) as BuildResultCompiledType;
                    Encoding encodingFromConfigPath = Util.GetEncodingFromConfigPath(vfile.VirtualPathObject);
                    string str2 = Util.StringFromFile(destPhysicalPath, ref encodingFromConfigPath).Replace("__ASPNET_INHERITS", Util.GetAssemblyQualifiedTypeName(type.ResultType));
                    StreamWriter writer = new StreamWriter(destPhysicalPath, false, encodingFromConfigPath);
                    writer.Write(str2);
                    writer.Close();
                }
                else
                {
                    File.Copy(sourceFileName, destPhysicalPath, false);
                }
                Util.ClearReadOnlyAttribute(destPhysicalPath);
            }
            else if (flag)
            {
                StreamWriter writer2 = new StreamWriter(destPhysicalPath);
                writer2.Write(System.Web.SR.GetString("Precomp_stub_file"));
                writer2.Close();
            }
        }

        private void CopyStaticFilesRecursive(VirtualDirectory sourceVdir, string destPhysicalDir, bool topLevel)
        {
            VerifyUnrelatedSourceAndDest(HostingEnvironment.MapPathInternal(sourceVdir.VirtualPath), destPhysicalDir);
            bool flag = false;
            foreach (VirtualFileBase base2 in sourceVdir.Children)
            {
                string str2 = Path.Combine(destPhysicalDir, base2.Name);
                if (base2.IsDirectory)
                {
                    if ((!topLevel || ((!StringUtil.EqualsIgnoreCase(base2.Name, "App_Code") && !StringUtil.EqualsIgnoreCase(base2.Name, "App_GlobalResources")) && !StringUtil.EqualsIgnoreCase(base2.Name, "App_WebReferences"))) && (PrecompilingForUpdatableDeployment || !StringUtil.EqualsIgnoreCase(base2.Name, "App_LocalResources")))
                    {
                        this.CopyStaticFilesRecursive(base2 as VirtualDirectory, str2, false);
                    }
                }
                else
                {
                    if (!flag)
                    {
                        flag = true;
                        Directory.CreateDirectory(destPhysicalDir);
                    }
                    this.CopyPrecompiledFile(base2 as VirtualFile, str2);
                }
            }
        }

        internal static System.Web.Compilation.BuildProvider CreateBuildProvider(VirtualPath virtualPath, CompilationSection compConfig, ICollection referencedAssemblies, bool failIfUnknown) => 
            CreateBuildProvider(virtualPath, BuildProviderAppliesTo.Web, compConfig, referencedAssemblies, failIfUnknown);

        internal static System.Web.Compilation.BuildProvider CreateBuildProvider(VirtualPath virtualPath, BuildProviderAppliesTo neededFor, CompilationSection compConfig, ICollection referencedAssemblies, bool failIfUnknown)
        {
            string extension = virtualPath.Extension;
            Type type = CompilationUtil.GetBuildProviderTypeFromExtension(compConfig, extension, neededFor, failIfUnknown);
            if (type == null)
            {
                return null;
            }
            System.Web.Compilation.BuildProvider provider = (System.Web.Compilation.BuildProvider) HttpRuntime.CreatePublicInstance(type);
            provider.SetVirtualPath(virtualPath);
            provider.SetReferencedAssemblies(referencedAssemblies);
            return provider;
        }

        public static object CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType) => 
            CreateInstanceFromVirtualPath(VirtualPath.CreateNonRelative(virtualPath), requiredBaseType, null, false, false);

        internal static object CreateInstanceFromVirtualPath(VirtualPath virtualPath, Type requiredBaseType, HttpContext context, bool allowCrossApp, bool noAssert)
        {
            ITypedWebObjectFactory factory = GetVirtualPathObjectFactory(virtualPath, context, allowCrossApp, noAssert);
            if (factory == null)
            {
                return null;
            }
            Util.CheckAssignableType(requiredBaseType, factory.InstantiatedType);
            using (new ClientImpersonationContext(context))
            {
                return factory.CreateInstance();
            }
        }

        private void CreatePrecompMarkerFile()
        {
            Directory.CreateDirectory(this._precompTargetPhysicalDir);
            using (StreamWriter writer = new StreamWriter(Path.Combine(this._precompTargetPhysicalDir, "PrecompiledApp.config"), false, Encoding.UTF8))
            {
                writer.Write("<precompiledApp version=\"2\" updatable=\"");
                if (PrecompilingForUpdatableDeployment)
                {
                    writer.Write("true");
                }
                else
                {
                    writer.Write("false");
                }
                writer.Write("\"/>");
            }
        }

        private bool DeletePrecompTargetDirectory()
        {
            try
            {
                if (this._precompTargetPhysicalDir != null)
                {
                    foreach (FileData data in (IEnumerable) FileEnumerator.Create(this._precompTargetPhysicalDir))
                    {
                        if (data.IsDirectory)
                        {
                            Directory.Delete(data.FullName, true);
                        }
                        else
                        {
                            Util.DeleteFileNoException(data.FullName);
                        }
                    }
                }
            }
            catch
            {
            }
            return !Util.IsNonEmptyDirectory(this._precompTargetPhysicalDir);
        }

        private void EnsureExcludedCodeSubDirectoriesComputed()
        {
            if (this._excludedCodeSubdirectories == null)
            {
                this._excludedCodeSubdirectories = new CaseInsensitiveStringSet();
                CodeSubDirectoriesCollection codeSubDirectories = CompilationUtil.GetCodeSubDirectories();
                if (codeSubDirectories != null)
                {
                    foreach (CodeSubDirectory directory in codeSubDirectories)
                    {
                        this._excludedCodeSubdirectories.Add(directory.DirectoryName);
                    }
                }
            }
        }

        private void EnsureFirstTimeDirectoryInit(VirtualPath virtualDir)
        {
            if (((!PrecompilingForUpdatableDeployment && (virtualDir != null)) && !this._localResourcesAssemblies.Contains(virtualDir)) && virtualDir.IsWithinAppRoot)
            {
                bool flag;
                VirtualPath path = virtualDir.SimpleCombineWithDir("App_LocalResources");
                try
                {
                    flag = path.DirectoryExists();
                }
                catch
                {
                    this._localResourcesAssemblies[virtualDir] = null;
                    return;
                }
                try
                {
                    HttpRuntime.StartListeningToLocalResourcesDirectory(path);
                }
                catch
                {
                    if (flag)
                    {
                        throw;
                    }
                }
                Assembly assembly = null;
                if (flag)
                {
                    string localResourcesAssemblyName = GetLocalResourcesAssemblyName(virtualDir);
                    bool gotLock = false;
                    try
                    {
                        CompilationLock.GetLock(ref gotLock);
                        assembly = this.CompileCodeDirectory(path, CodeDirectoryType.LocalResources, localResourcesAssemblyName, null);
                    }
                    finally
                    {
                        if (gotLock)
                        {
                            CompilationLock.ReleaseLock();
                        }
                    }
                }
                this._localResourcesAssemblies[virtualDir] = assembly;
            }
        }

        private void EnsureFirstTimeDirectoryInitForDependencies(ICollection dependencies)
        {
            foreach (string str in dependencies)
            {
                VirtualPath parent = VirtualPath.Create(str).Parent;
                this.EnsureFirstTimeDirectoryInit(parent);
            }
        }

        internal void EnsureTopLevelFilesCompiled()
        {
            if ((this._topLevelFileCompilationException != null) && !SkipTopLevelCompilationExceptions)
            {
                this.ReportTopLevelCompilationException();
            }
            if (!this._topLevelFilesCompiledStarted)
            {
                using (new ApplicationImpersonationContext())
                {
                    bool gotLock = false;
                    _parseErrorReported = false;
                    try
                    {
                        CompilationLock.GetLock(ref gotLock);
                        if ((this._topLevelFileCompilationException != null) && !SkipTopLevelCompilationExceptions)
                        {
                            this.ReportTopLevelCompilationException();
                        }
                        if (!this._topLevelFilesCompiledStarted)
                        {
                            this._topLevelFilesCompiledStarted = true;
                            this._topLevelReferencedAssemblies = new List<Assembly>();
                            this._topLevelAssembliesIndexTable = new Dictionary<string, AssemblyReferenceInfo>(StringComparer.OrdinalIgnoreCase);
                            this._topLevelReferencedAssemblies.Add(typeof(HttpRuntime).Assembly);
                            this._topLevelReferencedAssemblies.Add(typeof(Component).Assembly);
                            this._compilationStage = System.Web.Compilation.CompilationStage.TopLevelFiles;
                            this.CompileResourcesDirectory();
                            this.CompileWebRefDirectory();
                            this.CompileCodeDirectories();
                            this._compilationStage = System.Web.Compilation.CompilationStage.GlobalAsax;
                            this.CompileGlobalAsax();
                            this._compilationStage = System.Web.Compilation.CompilationStage.BrowserCapabilities;
                            BrowserCapabilitiesCompiler.GetBrowserCapabilitiesType();
                            HttpCapabilitiesBase emptyHttpCapabilitiesBase = HttpCapabilitiesBase.EmptyHttpCapabilitiesBase;
                            this._compilationStage = System.Web.Compilation.CompilationStage.AfterTopLevelFiles;
                        }
                    }
                    catch (Exception exception)
                    {
                        this._topLevelFileCompilationException = exception;
                        if (!SkipTopLevelCompilationExceptions)
                        {
                            if (!_parseErrorReported && !(exception is HttpCompileException))
                            {
                                this.ReportTopLevelCompilationException();
                            }
                            throw;
                        }
                    }
                    finally
                    {
                        this._topLevelFilesCompiledCompleted = true;
                        if (gotLock)
                        {
                            CompilationLock.ReleaseLock();
                        }
                    }
                }
            }
        }

        private void FailIfPrecompiledApp()
        {
            if (IsPrecompiledApp)
            {
                throw new HttpException(System.Web.SR.GetString("Already_precomp"));
            }
        }

        internal static string GenerateRandomAssemblyName(string baseName) => 
            GenerateRandomAssemblyName(baseName, true);

        internal static string GenerateRandomAssemblyName(string baseName, bool topLevel)
        {
            if (PrecompilingForDeployment)
            {
                return baseName;
            }
            if (OptimizeCompilations && topLevel)
            {
                return baseName;
            }
            return (baseName = baseName + "." + GenerateRandomFileName());
        }

        private static string GenerateRandomFileName()
        {
            byte[] data = new byte[6];
            lock (_rng)
            {
                _rng.GetBytes(data);
            }
            return Convert.ToBase64String(data).ToLower(CultureInfo.InvariantCulture).Replace('/', '-').Replace('+', '_');
        }

        internal static BuildResult GetBuildResultFromCache(string cacheKey) => 
            _theBuildManager.GetBuildResultFromCacheInternal(cacheKey, false, null, 0L);

        internal static BuildResult GetBuildResultFromCache(string cacheKey, VirtualPath virtualPath) => 
            _theBuildManager.GetBuildResultFromCacheInternal(cacheKey, false, virtualPath, 0L);

        private BuildResult GetBuildResultFromCacheInternal(string cacheKey, bool keyFromVPP, VirtualPath virtualPath, long hashCode)
        {
            BuildResult result = null;
            if (!_theBuildManagerInitialized)
            {
                return null;
            }
            result = this._memoryCache.GetBuildResult(cacheKey, virtualPath, hashCode);
            if (result != null)
            {
                return this.PostProcessFoundBuildResult(result, keyFromVPP, virtualPath);
            }
            lock (this)
            {
                int index = 0;
                while (index < this._caches.Length)
                {
                    result = this._caches[index].GetBuildResult(cacheKey, virtualPath, hashCode);
                    if (result != null)
                    {
                        if (result.VirtualPathDependencies != null)
                        {
                            this.EnsureFirstTimeDirectoryInitForDependencies(result.VirtualPathDependencies);
                        }
                        break;
                    }
                    if ((index == 0) && (virtualPath != null))
                    {
                        VirtualPath parent = virtualPath.Parent;
                        this.EnsureFirstTimeDirectoryInit(parent);
                    }
                    index++;
                }
                if (result == null)
                {
                    return null;
                }
                result = this.PostProcessFoundBuildResult(result, keyFromVPP, virtualPath);
                if (result == null)
                {
                    return null;
                }
                for (int i = 0; i < index; i++)
                {
                    this._caches[i].CacheBuildResult(cacheKey, result, DateTime.UtcNow);
                }
                return result;
            }
        }

        internal static long GetBuildResultHashCodeIfCached(HttpContext context, string virtualPath)
        {
            BuildResult result = GetVPathBuildResult(context, VirtualPath.Create(virtualPath), true, false);
            if (result == null)
            {
                return 0L;
            }
            string virtualPathDependenciesHash = result.VirtualPathDependenciesHash;
            return result.ComputeHashCode(s_topLevelHash, (long) StringUtil.GetStringHashCode(virtualPathDependenciesHash));
        }

        public static BuildDependencySet GetCachedBuildDependencySet(HttpContext context, string virtualPath)
        {
            BuildResult result = GetVPathBuildResult(context, VirtualPath.Create(virtualPath), true, false);
            if (result == null)
            {
                return null;
            }
            return new BuildDependencySet(result);
        }

        internal static string GetCacheKeyFromVirtualPath(VirtualPath virtualPath)
        {
            bool flag;
            return GetCacheKeyFromVirtualPath(virtualPath, out flag);
        }

        private static string GetCacheKeyFromVirtualPath(VirtualPath virtualPath, out bool keyFromVPP)
        {
            string cacheKey = virtualPath.GetCacheKey();
            if (cacheKey != null)
            {
                keyFromVPP = true;
                return cacheKey.ToLowerInvariant();
            }
            keyFromVPP = false;
            cacheKey = _keyCache[virtualPath.VirtualPathString] as string;
            if (cacheKey == null)
            {
                cacheKey = GetCacheKeyFromVirtualPathInternal(virtualPath);
                _keyCache[virtualPath.VirtualPathString] = cacheKey;
            }
            return cacheKey;
        }

        private static string GetCacheKeyFromVirtualPathInternal(VirtualPath virtualPath)
        {
            string str3;
            string str = UrlPath.RemoveSlashFromPathIfNeeded(virtualPath.AppRelativeVirtualPathString.ToLowerInvariant());
            int length = str.LastIndexOf('/');
            if (str == "~")
            {
                return "root";
            }
            string str2 = str.Substring(length + 1);
            if (length <= 0)
            {
                str3 = "/";
            }
            else
            {
                str3 = str.Substring(0, length);
            }
            return (str2 + "." + StringUtil.GetStringHashCode(str3).ToString("x", CultureInfo.InvariantCulture));
        }

        internal string[] GetCodeDirectories()
        {
            VirtualPath codeDirectoryVirtualPath = HttpRuntime.CodeDirectoryVirtualPath;
            if (!codeDirectoryVirtualPath.DirectoryExists())
            {
                return new string[0];
            }
            CodeSubDirectoriesCollection codeSubDirectories = CompilationUtil.GetCodeSubDirectories();
            int num = 1;
            if (codeSubDirectories != null)
            {
                num += codeSubDirectories.Count;
            }
            string[] strArray = new string[num];
            int num2 = 0;
            if (codeSubDirectories != null)
            {
                foreach (CodeSubDirectory directory in codeSubDirectories)
                {
                    VirtualPath path2 = codeDirectoryVirtualPath.SimpleCombineWithDir(directory.DirectoryName);
                    strArray[num2++] = path2.VirtualPathString;
                }
            }
            strArray[num2++] = codeDirectoryVirtualPath.VirtualPathString;
            return strArray;
        }

        internal void GetCodeDirectoryInformation(VirtualPath virtualCodeDir, out Type codeDomProviderType, out CompilerParameters compilerParameters, out string generatedFilesDir)
        {
            System.Web.Compilation.CompilationStage stage = this._compilationStage;
            try
            {
                this.GetCodeDirectoryInformationInternal(virtualCodeDir, out codeDomProviderType, out compilerParameters, out generatedFilesDir);
            }
            finally
            {
                this._compilationStage = stage;
            }
        }

        private void GetCodeDirectoryInformationInternal(VirtualPath virtualCodeDir, out Type codeDomProviderType, out CompilerParameters compilerParameters, out string generatedFilesDir)
        {
            StringSet excludedSubdirectories = null;
            CodeDirectoryType mainCode;
            if (virtualCodeDir == HttpRuntime.CodeDirectoryVirtualPath)
            {
                this.EnsureExcludedCodeSubDirectoriesComputed();
                excludedSubdirectories = this._excludedCodeSubdirectories;
                mainCode = CodeDirectoryType.MainCode;
                this._compilationStage = System.Web.Compilation.CompilationStage.TopLevelFiles;
            }
            else if (virtualCodeDir == HttpRuntime.ResourcesDirectoryVirtualPath)
            {
                mainCode = CodeDirectoryType.AppResources;
                this._compilationStage = System.Web.Compilation.CompilationStage.TopLevelFiles;
            }
            else if (string.Compare(virtualCodeDir.VirtualPathString, 0, HttpRuntime.WebRefDirectoryVirtualPath.VirtualPathString, 0, HttpRuntime.WebRefDirectoryVirtualPath.VirtualPathString.Length, StringComparison.OrdinalIgnoreCase) == 0)
            {
                virtualCodeDir = HttpRuntime.WebRefDirectoryVirtualPath;
                mainCode = CodeDirectoryType.WebReferences;
                this._compilationStage = System.Web.Compilation.CompilationStage.TopLevelFiles;
            }
            else if (string.Compare(virtualCodeDir.FileName, "App_LocalResources", StringComparison.OrdinalIgnoreCase) == 0)
            {
                mainCode = CodeDirectoryType.LocalResources;
                this._compilationStage = System.Web.Compilation.CompilationStage.AfterTopLevelFiles;
            }
            else
            {
                mainCode = CodeDirectoryType.SubCode;
                this._compilationStage = System.Web.Compilation.CompilationStage.TopLevelFiles;
            }
            AssemblyReferenceInfo info = TheBuildManager.TopLevelAssembliesIndexTable[virtualCodeDir.VirtualPathString];
            if (info == null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("Invalid_CodeSubDirectory_Not_Exist", new object[] { virtualCodeDir }));
            }
            CodeDirectoryCompiler.GetCodeDirectoryInformation(virtualCodeDir, mainCode, excludedSubdirectories, info.ReferenceIndex, out codeDomProviderType, out compilerParameters, out generatedFilesDir);
            Assembly assembly = info.Assembly;
            if (assembly != null)
            {
                compilerParameters.OutputAssembly = assembly.Location;
            }
        }

        public static Assembly GetCompiledAssembly(string virtualPath)
        {
            BuildResult vPathBuildResult = GetVPathBuildResult(VirtualPath.Create(virtualPath));
            if (vPathBuildResult == null)
            {
                return null;
            }
            BuildResultCompiledAssemblyBase base2 = vPathBuildResult as BuildResultCompiledAssemblyBase;
            return base2?.ResultAssembly;
        }

        public static string GetCompiledCustomString(string virtualPath)
        {
            BuildResult vPathBuildResult = GetVPathBuildResult(VirtualPath.Create(virtualPath));
            if (vPathBuildResult == null)
            {
                return null;
            }
            BuildResultCustomString str = vPathBuildResult as BuildResultCustomString;
            return str?.CustomString;
        }

        public static Type GetCompiledType(string virtualPath)
        {
            if (virtualPath == null)
            {
                throw new ArgumentNullException("virtualPath");
            }
            return GetCompiledType(VirtualPath.Create(virtualPath));
        }

        internal static Type GetCompiledType(VirtualPath virtualPath)
        {
            BuildResultCompiledType type = GetVirtualPathObjectFactory(virtualPath, null, false, false) as BuildResultCompiledType;
            return type?.ResultType;
        }

        internal static Type GetCompiledType(VirtualPath virtualPath, ClientBuildManagerCallback callback)
        {
            Type compiledType;
            bool skipTopLevelCompilationExceptions = SkipTopLevelCompilationExceptions;
            bool throwOnFirstParseError = ThrowOnFirstParseError;
            try
            {
                SkipTopLevelCompilationExceptions = false;
                ThrowOnFirstParseError = false;
                _theBuildManager._cbmCallback = callback;
                compiledType = GetCompiledType(virtualPath);
            }
            finally
            {
                _theBuildManager._cbmCallback = null;
                SkipTopLevelCompilationExceptions = skipTopLevelCompilationExceptions;
                ThrowOnFirstParseError = throwOnFirstParseError;
            }
            return compiledType;
        }

        private static string GetGeneratedAssemblyBaseName(VirtualPath virtualPath) => 
            GetCacheKeyFromVirtualPath(virtualPath);

        internal static BuildResultCompiledGlobalAsaxType GetGlobalAsaxBuildResult() => 
            _theBuildManager.GetGlobalAsaxBuildResultInternal();

        private BuildResultCompiledGlobalAsaxType GetGlobalAsaxBuildResultInternal()
        {
            this.EnsureTopLevelFilesCompiled();
            return this._globalAsaxBuildResult;
        }

        internal static Type GetGlobalAsaxType() => 
            _theBuildManager.GetGlobalAsaxTypeInternal();

        private Type GetGlobalAsaxTypeInternal()
        {
            this.EnsureTopLevelFilesCompiled();
            return this._globalAsaxBuildResult?.ResultType;
        }

        internal static Assembly GetLocalResourcesAssembly(VirtualPath virtualDir) => 
            ((Assembly) _theBuildManager._localResourcesAssemblies[virtualDir]);

        internal static string GetLocalResourcesAssemblyName(VirtualPath virtualDir) => 
            ("App_LocalResources." + GetGeneratedAssemblyBaseName(virtualDir));

        internal static string GetNormalizedCodeAssemblyName(string assemblyName)
        {
            if (assemblyName.StartsWith("App_Code", StringComparison.Ordinal))
            {
                return "App_Code";
            }
            foreach (CodeSubDirectory directory in CompilationUtil.GetCodeSubDirectories())
            {
                if (assemblyName.StartsWith("App_SubCode_" + directory.AssemblyName + ".", StringComparison.Ordinal))
                {
                    return directory.AssemblyName;
                }
            }
            return null;
        }

        internal static string GetNormalizedTypeName(Type t)
        {
            string normalizedCodeAssemblyName = GetNormalizedCodeAssemblyName(t.Assembly.FullName);
            if (normalizedCodeAssemblyName == null)
            {
                return t.AssemblyQualifiedName;
            }
            return (t.FullName + ", " + normalizedCodeAssemblyName);
        }

        internal static Type GetProfileType() => 
            _theBuildManager.GetProfileTypeInternal();

        private Type GetProfileTypeInternal()
        {
            this.EnsureTopLevelFilesCompiled();
            return this._profileType;
        }

        public static ICollection GetReferencedAssemblies()
        {
            CompilationSection compilation = RuntimeConfig.GetAppConfig().Compilation;
            _theBuildManager.EnsureTopLevelFilesCompiled();
            return GetReferencedAssemblies(compilation);
        }

        internal static ICollection GetReferencedAssemblies(CompilationSection compConfig)
        {
            AssemblySet set = AssemblySet.Create(TheBuildManager.TopLevelReferencedAssemblies);
            foreach (AssemblyInfo info in compConfig.Assemblies)
            {
                Assembly[] assemblyInternal = info.AssemblyInternal;
                if (assemblyInternal == null)
                {
                    lock (compConfig)
                    {
                        assemblyInternal = info.AssemblyInternal;
                        if (assemblyInternal == null)
                        {
                            assemblyInternal = info.AssemblyInternal = compConfig.LoadAssembly(info);
                        }
                    }
                }
                for (int i = 0; i < assemblyInternal.Length; i++)
                {
                    if (assemblyInternal[i] != null)
                    {
                        set.Add(assemblyInternal[i]);
                    }
                }
            }
            return set;
        }

        internal static ICollection GetReferencedAssemblies(CompilationSection compConfig, int removeIndex)
        {
            AssemblySet set = new AssemblySet();
            foreach (AssemblyInfo info in compConfig.Assemblies)
            {
                Assembly[] assemblyInternal = info.AssemblyInternal;
                if (assemblyInternal == null)
                {
                    lock (compConfig)
                    {
                        assemblyInternal = info.AssemblyInternal;
                        if (assemblyInternal == null)
                        {
                            assemblyInternal = info.AssemblyInternal = compConfig.LoadAssembly(info);
                        }
                    }
                }
                for (int j = 0; j < assemblyInternal.Length; j++)
                {
                    if (assemblyInternal[j] != null)
                    {
                        set.Add(assemblyInternal[j]);
                    }
                }
            }
            for (int i = 0; i < removeIndex; i++)
            {
                set.Add(TheBuildManager.TopLevelReferencedAssemblies[i]);
            }
            return set;
        }

        public static Type GetType(string typeName, bool throwOnError) => 
            GetType(typeName, throwOnError, false);

        public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            Type t = null;
            if (Util.TypeNameContainsAssembly(typeName))
            {
                t = Type.GetType(typeName, throwOnError, ignoreCase);
                if (t != null)
                {
                    return t;
                }
            }
            if (!InitializeBuildManager())
            {
                return Type.GetType(typeName, throwOnError, ignoreCase);
            }
            try
            {
                t = typeof(BuildManager).Assembly.GetType(typeName, false, ignoreCase);
            }
            catch (ArgumentException exception)
            {
                throw new HttpException(System.Web.SR.GetString("Invalid_type", new object[] { typeName }), exception);
            }
            if (t == null)
            {
                _theBuildManager.EnsureTopLevelFilesCompiled();
                t = Util.GetTypeFromAssemblies(TheBuildManager.TopLevelReferencedAssemblies, typeName, ignoreCase);
                if (t != null)
                {
                    return t;
                }
                AssemblyCollection assembliesForAppLevel = CompilationUtil.GetAssembliesForAppLevel();
                if (assembliesForAppLevel != null)
                {
                    Type type2 = CompilationUtil.GetTypeFromAssemblies(assembliesForAppLevel, typeName, ignoreCase);
                    if (type2 != null)
                    {
                        if ((t != null) && (type2 != t))
                        {
                            throw new HttpException(System.Web.SR.GetString("Ambiguous_type", new object[] { typeName, Util.GetAssemblySafePathFromType(t), Util.GetAssemblySafePathFromType(type2) }));
                        }
                        t = type2;
                    }
                }
                if ((t == null) && throwOnError)
                {
                    throw new HttpException(System.Web.SR.GetString("Invalid_type", new object[] { typeName }));
                }
            }
            return t;
        }

        internal static Type GetTypeFromCodeAssembly(string typeName, bool ignoreCase)
        {
            if (CodeAssemblies == null)
            {
                return null;
            }
            return Util.GetTypeFromAssemblies(CodeAssemblies, typeName, ignoreCase);
        }

        internal static TextWriter GetUpdatableDeploymentTargetWriter(VirtualPath virtualPath, Encoding fileEncoding)
        {
            if (!PrecompilingForUpdatableDeployment)
            {
                return null;
            }
            string str = virtualPath.AppRelativeVirtualPathString.Substring(2);
            string path = Path.Combine(_theBuildManager._precompTargetPhysicalDir, str);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return new StreamWriter(path, false, fileEncoding);
        }

        public static ICollection GetVirtualPathDependencies(string virtualPath)
        {
            CompilationSection compilation = RuntimeConfig.GetRootWebConfig().Compilation;
            return CreateBuildProvider(VirtualPath.Create(virtualPath), compilation, null, false)?.GetBuildResultVirtualPathDependencies();
        }

        private static ITypedWebObjectFactory GetVirtualPathObjectFactory(VirtualPath virtualPath, HttpContext context, bool allowCrossApp, bool noAssert)
        {
            BuildResult result;
            if (virtualPath == null)
            {
                throw new ArgumentNullException("virtualPath");
            }
            if (_theBuildManager._topLevelFileCompilationException != null)
            {
                _theBuildManager.ReportTopLevelCompilationException();
            }
            if (HttpRuntime.IsFullTrust || noAssert)
            {
                result = GetVPathBuildResultWithNoAssert(context, virtualPath, false, allowCrossApp, false);
            }
            else
            {
                result = GetVPathBuildResultWithAssert(context, virtualPath, false, allowCrossApp, false);
            }
            return (result as ITypedWebObjectFactory);
        }

        internal static BuildResult GetVPathBuildResult(VirtualPath virtualPath) => 
            GetVPathBuildResult(null, virtualPath, false, false, false);

        internal static BuildResult GetVPathBuildResult(HttpContext context, VirtualPath virtualPath) => 
            GetVPathBuildResult(context, virtualPath, false, false, false);

        internal static BuildResult GetVPathBuildResult(HttpContext context, VirtualPath virtualPath, bool noBuild, bool allowCrossApp) => 
            GetVPathBuildResult(context, virtualPath, noBuild, allowCrossApp, false);

        internal static BuildResult GetVPathBuildResult(HttpContext context, VirtualPath virtualPath, bool noBuild, bool allowCrossApp, bool allowBuildInPrecompile)
        {
            if (HttpRuntime.IsFullTrust)
            {
                return GetVPathBuildResultWithNoAssert(context, virtualPath, noBuild, allowCrossApp, allowBuildInPrecompile);
            }
            return GetVPathBuildResultWithAssert(context, virtualPath, noBuild, allowCrossApp, allowBuildInPrecompile);
        }

        internal static BuildResult GetVPathBuildResultFromCache(VirtualPath virtualPath) => 
            TheBuildManager.GetVPathBuildResultFromCacheInternal(virtualPath);

        private BuildResult GetVPathBuildResultFromCacheInternal(VirtualPath virtualPath)
        {
            bool flag;
            string cacheKeyFromVirtualPath = GetCacheKeyFromVirtualPath(virtualPath, out flag);
            return this.GetBuildResultFromCacheInternal(cacheKeyFromVirtualPath, flag, virtualPath, 0L);
        }

        private BuildResult GetVPathBuildResultInternal(VirtualPath virtualPath, bool noBuild, bool allowCrossApp, bool allowBuildInPrecompile)
        {
            if (this._compilationStage == System.Web.Compilation.CompilationStage.TopLevelFiles)
            {
                throw new HttpException(System.Web.SR.GetString("Too_early_for_webfile", new object[] { virtualPath }));
            }
            BuildResult vPathBuildResultFromCacheInternal = this.GetVPathBuildResultFromCacheInternal(virtualPath);
            if (vPathBuildResultFromCacheInternal == null)
            {
                if (noBuild)
                {
                    return null;
                }
                this.ValidateVirtualPathInternal(virtualPath, allowCrossApp, false);
                Util.CheckVirtualFileExists(virtualPath);
                if (this.IsNonUpdatablePrecompiledApp && !allowBuildInPrecompile)
                {
                    throw new HttpException(System.Web.SR.GetString("Cant_update_precompiled_app", new object[] { virtualPath }));
                }
                bool gotLock = false;
                try
                {
                    CompilationLock.GetLock(ref gotLock);
                    vPathBuildResultFromCacheInternal = this.GetVPathBuildResultFromCacheInternal(virtualPath);
                    if (vPathBuildResultFromCacheInternal != null)
                    {
                        return vPathBuildResultFromCacheInternal;
                    }
                    VirtualPathSet data = CallContext.GetData("CircRefChk") as VirtualPathSet;
                    if (data == null)
                    {
                        data = new VirtualPathSet();
                        CallContext.SetData("CircRefChk", data);
                    }
                    if (data.Contains(virtualPath))
                    {
                        throw new HttpException(System.Web.SR.GetString("Circular_include"));
                    }
                    data.Add(virtualPath);
                    try
                    {
                        this.EnsureTopLevelFilesCompiled();
                        vPathBuildResultFromCacheInternal = this.CompileWebFile(virtualPath);
                    }
                    finally
                    {
                        data.Remove(virtualPath);
                    }
                }
                finally
                {
                    if (gotLock)
                    {
                        CompilationLock.ReleaseLock();
                    }
                }
            }
            return vPathBuildResultFromCacheInternal;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        internal static BuildResult GetVPathBuildResultWithAssert(HttpContext context, VirtualPath virtualPath, bool noBuild, bool allowCrossApp, bool allowBuildInPrecompile) => 
            GetVPathBuildResultWithNoAssert(context, virtualPath, noBuild, allowCrossApp, allowBuildInPrecompile);

        internal static BuildResult GetVPathBuildResultWithNoAssert(HttpContext context, VirtualPath virtualPath, bool noBuild, bool allowCrossApp, bool allowBuildInPrecompile)
        {
            using (new ApplicationImpersonationContext())
            {
                return _theBuildManager.GetVPathBuildResultInternal(virtualPath, noBuild, allowCrossApp, allowBuildInPrecompile);
            }
        }

        private void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.ResolveAssembly);
            this._globalAsaxVirtualPath = HttpRuntime.AppDomainAppVirtualPathObject.SimpleCombine("global.asax");
            this._webHashFilePath = Path.Combine(HttpRuntime.CodegenDirInternal, @"hash\hash.web");
            this._skipTopLevelCompilationExceptions = BuildManagerHost.InClientBuildManager;
            this.SetPrecompilationInfo(HostingEnvironment.HostingParameters);
            if (this._precompTargetPhysicalDir != null)
            {
                this.FailIfPrecompiledApp();
                this.PrecompilationModeInitialize();
            }
            else if (IsPrecompiledApp)
            {
                this.PrecompiledAppRuntimeModeInitialize();
            }
            else
            {
                this.RegularAppRuntimeModeInitialize();
            }
            this._scriptVirtualDir = Util.GetScriptLocation();
            this._excludedTopLevelDirectories = new CaseInsensitiveStringSet();
            this._excludedTopLevelDirectories.Add("bin");
            this._excludedTopLevelDirectories.Add("App_Code");
            this._excludedTopLevelDirectories.Add("App_GlobalResources");
            this._excludedTopLevelDirectories.Add("App_LocalResources");
            this._excludedTopLevelDirectories.Add("App_WebReferences");
            this._excludedTopLevelDirectories.Add("App_Themes");
            this._forbiddenTopLevelDirectories = new CaseInsensitiveStringSet();
            this._forbiddenTopLevelDirectories.Add("App_Code");
            this._forbiddenTopLevelDirectories.Add("App_GlobalResources");
            this._forbiddenTopLevelDirectories.Add("App_LocalResources");
            this._forbiddenTopLevelDirectories.Add("App_WebReferences");
            this._forbiddenTopLevelDirectories.Add("App_Themes");
            this.LoadLicensesAssemblyIfExists();
        }

        internal static bool InitializeBuildManager()
        {
            if (_initializeException != null)
            {
                throw new HttpException(_initializeException.Message, _initializeException);
            }
            if (!_theBuildManagerInitialized)
            {
                if (!HttpRuntime.FusionInited)
                {
                    return false;
                }
                if (HttpRuntime.TrustLevel == null)
                {
                    return false;
                }
                _theBuildManagerInitialized = true;
                try
                {
                    _theBuildManager.Initialize();
                }
                catch (Exception exception)
                {
                    _theBuildManagerInitialized = false;
                    _initializeException = exception;
                    throw;
                }
            }
            return true;
        }

        private bool IsBatchEnabledForDirectory(VirtualPath virtualDir)
        {
            if (CompileWithFixedAssemblyNames)
            {
                return false;
            }
            if (PrecompilingForDeployment)
            {
                return true;
            }
            if (BuildManagerHost.InClientBuildManager && !PerformingPrecompilation)
            {
                return false;
            }
            return CompilationUtil.IsBatchingEnabled(virtualDir.VirtualPathString);
        }

        internal static bool IsReservedAssemblyName(string assemblyName)
        {
            if (((string.Compare(assemblyName, "App_Code", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(assemblyName, "App_GlobalResources", StringComparison.OrdinalIgnoreCase) != 0)) && ((string.Compare(assemblyName, "App_WebReferences", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(assemblyName, "App_global.asax", StringComparison.OrdinalIgnoreCase) != 0)))
            {
                return false;
            }
            return true;
        }

        private void LoadLicensesAssemblyIfExists()
        {
            if (File.Exists(Path.Combine(HttpRuntime.BinDirectoryInternal, "App_Licenses.dll")))
            {
                Assembly.Load("App_Licenses");
            }
        }

        private void OnWebHashFileChange(object sender, FileChangeEvent e)
        {
            HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.BuildManagerChange, "Change in " + this._webHashFilePath);
        }

        private BuildResult PostProcessFoundBuildResult(BuildResult result, bool keyFromVPP, VirtualPath virtualPath)
        {
            if ((!keyFromVPP && (virtualPath != null)) && (virtualPath != result.VirtualPath))
            {
                return null;
            }
            if (!(result is BuildResultCompileError))
            {
                return result;
            }
            HttpCompileException compileException = ((BuildResultCompileError) result).CompileException;
            if (!PerformingPrecompilation)
            {
                this.ReportErrorsFromException(compileException);
            }
            throw compileException;
        }

        private void PrecompilationModeInitialize()
        {
            BuildResultCache cache2;
            this._memoryCache = new MemoryBuildResultCache(HttpRuntime.CacheInternal);
            StandardDiskBuildResultCache diskCache = new StandardDiskBuildResultCache(HttpRuntime.CodegenDirInternal);
            string cacheDir = Path.Combine(this._precompTargetPhysicalDir, "bin");
            if (PrecompilingForUpdatableDeployment)
            {
                cache2 = new UpdatablePrecompilerDiskBuildResultCache(cacheDir);
            }
            else
            {
                cache2 = new PrecompilerDiskBuildResultCache(cacheDir);
            }
            this._caches = new BuildResultCache[] { this._memoryCache, cache2, diskCache };
            this.CheckTopLevelFilesUpToDate(diskCache);
        }

        internal void PrecompileApp(ClientBuildManagerCallback callback)
        {
            bool skipTopLevelCompilationExceptions = SkipTopLevelCompilationExceptions;
            try
            {
                this._cbmCallback = callback;
                ThrowOnFirstParseError = false;
                SkipTopLevelCompilationExceptions = false;
                this.PrecompileApp(HttpRuntime.AppDomainAppVirtualPathObject);
            }
            finally
            {
                SkipTopLevelCompilationExceptions = skipTopLevelCompilationExceptions;
                ThrowOnFirstParseError = true;
                this._cbmCallback = null;
            }
        }

        private void PrecompileApp(VirtualPath startingVirtualDir)
        {
            using (new ApplicationImpersonationContext())
            {
                try
                {
                    PerformingPrecompilation = true;
                    this.PrecompileAppInternal(startingVirtualDir);
                }
                catch
                {
                    this.DeletePrecompTargetDirectory();
                    throw;
                }
                finally
                {
                    PerformingPrecompilation = false;
                }
            }
        }

        private void PrecompileAppInternal(VirtualPath startingVirtualDir)
        {
            this.FailIfPrecompiledApp();
            VirtualDirectory vdir = startingVirtualDir.GetDirectory();
            this.EnsureTopLevelFilesCompiled();
            try
            {
                _parseErrorReported = false;
                this.PrecompileWebDirectoriesRecursive(vdir, true);
                this.PrecompileThemeDirectories();
            }
            catch (HttpParseException exception)
            {
                if (!_parseErrorReported)
                {
                    this.ReportErrorsFromException(exception);
                }
                throw;
            }
            if (this._precompTargetPhysicalDir != null)
            {
                string toDir = Path.Combine(this._precompTargetPhysicalDir, "bin");
                this.CopyCompiledAssembliesToDestinationBin(HttpRuntime.CodegenDirInternal, toDir);
            }
            if (this._precompTargetPhysicalDir != null)
            {
                this.CopyStaticFilesRecursive(vdir, this._precompTargetPhysicalDir, true);
            }
        }

        private void PrecompiledAppRuntimeModeInitialize()
        {
            this._memoryCache = new MemoryBuildResultCache(HttpRuntime.CacheInternal);
            BuildResultCache cache = new PrecompiledSiteDiskBuildResultCache(HttpRuntime.BinDirectoryInternal);
            StandardDiskBuildResultCache diskCache = new StandardDiskBuildResultCache(HttpRuntime.CodegenDirInternal);
            this._caches = new BuildResultCache[] { this._memoryCache, cache, diskCache };
            this.CheckTopLevelFilesUpToDate(diskCache);
        }

        private void PrecompileThemeDirectories()
        {
            string path = Path.Combine(HttpRuntime.AppDomainAppPathInternal, "App_Themes");
            if (Directory.Exists(path))
            {
                foreach (string str2 in Directory.GetDirectories(path))
                {
                    string fileName = Path.GetFileName(str2);
                    ThemeDirectoryCompiler.GetThemeBuildResultType(null, fileName);
                }
            }
        }

        private void PrecompileWebDirectoriesRecursive(VirtualDirectory vdir, bool topLevel)
        {
            foreach (VirtualDirectory directory in vdir.Directories)
            {
                if ((!topLevel || !this._excludedTopLevelDirectories.Contains(directory.Name)) && (directory.Name != "_vti_cnf"))
                {
                    this.PrecompileWebDirectoriesRecursive(directory, false);
                }
            }
            try
            {
                this._precompilingApp = true;
                if (this.IsBatchEnabledForDirectory(vdir.VirtualPathObject))
                {
                    this.BatchCompileWebDirectory(vdir, null, false);
                }
                else
                {
                    new NonBatchDirectoryCompiler(vdir).Process();
                }
            }
            finally
            {
                this._precompilingApp = false;
            }
        }

        private static bool ReadPrecompMarkerFile(string appRoot, out bool updatable)
        {
            updatable = false;
            string path = Path.Combine(appRoot, "PrecompiledApp.config");
            if (!File.Exists(path))
            {
                return false;
            }
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(path);
            }
            catch
            {
                return false;
            }
            XmlNode documentElement = document.DocumentElement;
            if ((documentElement == null) || (documentElement.Name != "precompiledApp"))
            {
                return false;
            }
            System.Web.Configuration.HandlerBase.GetAndRemoveBooleanAttribute(documentElement, "updatable", ref updatable);
            return true;
        }

        private void RegularAppRuntimeModeInitialize()
        {
            this._memoryCache = new MemoryBuildResultCache(HttpRuntime.CacheInternal);
            StandardDiskBuildResultCache diskCache = new StandardDiskBuildResultCache(HttpRuntime.CodegenDirInternal);
            this._caches = new BuildResultCache[] { this._memoryCache, diskCache };
            this.CheckTopLevelFilesUpToDate(diskCache);
        }

        internal static void ReportDirectoryCompilationProgress(VirtualPath virtualDir)
        {
            if ((CBMCallback != null) && virtualDir.DirectoryExists())
            {
                string message = System.Web.SR.GetString("Directory_progress", new object[] { virtualDir.VirtualPathString });
                CBMCallback.ReportProgress(message);
            }
        }

        private void ReportErrorsFromException(Exception e)
        {
            if (CBMCallback != null)
            {
                if (e is HttpCompileException)
                {
                    foreach (CompilerError error in ((HttpCompileException) e).Results.Errors)
                    {
                        CBMCallback.ReportCompilerError(error);
                    }
                }
                else if (e is HttpParseException)
                {
                    foreach (ParserError error2 in ((HttpParseException) e).ParserErrors)
                    {
                        ReportParseError(error2);
                    }
                }
            }
        }

        internal static void ReportParseError(ParserError parseError)
        {
            if (CBMCallback != null)
            {
                _parseErrorReported = true;
                CBMCallback.ReportParseError(parseError);
            }
        }

        private void ReportTopLevelCompilationException()
        {
            this.ReportErrorsFromException(this._topLevelFileCompilationException);
            throw new HttpException(this._topLevelFileCompilationException.Message, this._topLevelFileCompilationException);
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            if (this._assemblyResolveMapping != null)
            {
                string name = e.Name;
                Assembly assembly = (Assembly) this._assemblyResolveMapping[name];
                if (assembly != null)
                {
                    return assembly;
                }
                string normalizedCodeAssemblyName = GetNormalizedCodeAssemblyName(name);
                if (normalizedCodeAssemblyName != null)
                {
                    return (Assembly) this._assemblyResolveMapping[normalizedCodeAssemblyName];
                }
            }
            return null;
        }

        internal void SetPrecompilationInfo(HostingEnvironmentParameters hostingParameters)
        {
            if ((hostingParameters != null) && (hostingParameters.ClientBuildManagerParameter != null))
            {
                this._precompilationFlags = hostingParameters.ClientBuildManagerParameter.PrecompilationFlags;
                this._strongNameKeyFile = hostingParameters.ClientBuildManagerParameter.StrongNameKeyFile;
                this._strongNameKeyContainer = hostingParameters.ClientBuildManagerParameter.StrongNameKeyContainer;
                this._precompTargetPhysicalDir = hostingParameters.PrecompilationTargetPhysicalDirectory;
                if (this._precompTargetPhysicalDir != null)
                {
                    if (Util.IsNonEmptyDirectory(this._precompTargetPhysicalDir))
                    {
                        bool flag;
                        if ((this._precompilationFlags & PrecompilationFlags.OverwriteTarget) == PrecompilationFlags.Default)
                        {
                            throw new HttpException(System.Web.SR.GetString("Dir_not_empty"));
                        }
                        if (!ReadPrecompMarkerFile(this._precompTargetPhysicalDir, out flag))
                        {
                            throw new HttpException(System.Web.SR.GetString("Dir_not_empty_not_precomp"));
                        }
                        if (!this.DeletePrecompTargetDirectory())
                        {
                            Thread.Sleep(250);
                            if (!this.DeletePrecompTargetDirectory())
                            {
                                Thread.Sleep(0x3e8);
                                if (!this.DeletePrecompTargetDirectory())
                                {
                                    throw new HttpException(System.Web.SR.GetString("Cant_delete_dir"));
                                }
                            }
                        }
                    }
                    this.CreatePrecompMarkerFile();
                }
            }
        }

        internal static void ValidateCodeFileVirtualPath(VirtualPath virtualPath)
        {
            _theBuildManager.ValidateVirtualPathInternal(virtualPath, false, true);
        }

        private void ValidateVirtualPathInternal(VirtualPath virtualPath, bool allowCrossApp, bool codeFile)
        {
            if (!allowCrossApp)
            {
                virtualPath.FailIfNotWithinAppRoot();
            }
            else if (!virtualPath.IsWithinAppRoot)
            {
                return;
            }
            if (HttpRuntime.AppDomainAppVirtualPathObject != virtualPath)
            {
                int length = HttpRuntime.AppDomainAppVirtualPathString.Length;
                string virtualPathString = virtualPath.VirtualPathString;
                if (virtualPathString.Length >= length)
                {
                    int index = virtualPathString.IndexOf('/', length);
                    if (index >= 0)
                    {
                        string o = virtualPathString.Substring(length, index - length);
                        if (this._forbiddenTopLevelDirectories.Contains(o))
                        {
                            throw new HttpException(System.Web.SR.GetString("Illegal_special_dir", new object[] { virtualPathString, o }));
                        }
                    }
                }
            }
        }

        internal static void VerifyUnrelatedSourceAndDest(string sourcePhysicalDir, string destPhysicalDir)
        {
            sourcePhysicalDir = FileUtil.FixUpPhysicalDirectory(sourcePhysicalDir);
            destPhysicalDir = FileUtil.FixUpPhysicalDirectory(destPhysicalDir);
            if (StringUtil.StringStartsWithIgnoreCase(sourcePhysicalDir, destPhysicalDir) || StringUtil.StringStartsWithIgnoreCase(destPhysicalDir, sourcePhysicalDir))
            {
                throw new HttpException(System.Web.SR.GetString("Illegal_precomp_dir", new object[] { destPhysicalDir, sourcePhysicalDir }));
            }
        }

        internal static Assembly AppResourcesAssembly =>
            _theBuildManager._appResourcesAssembly;

        internal static ClientBuildManagerCallback CBMCallback =>
            _theBuildManager._cbmCallback;

        public static IList CodeAssemblies
        {
            get
            {
                _theBuildManager.EnsureTopLevelFilesCompiled();
                return _theBuildManager._codeAssemblies;
            }
        }

        internal static System.Web.Compilation.CompilationStage CompilationStage =>
            _theBuildManager._compilationStage;

        internal static bool CompileWithAllowPartiallyTrustedCallersAttribute =>
            ((_theBuildManager._precompilationFlags & PrecompilationFlags.AllowPartiallyTrustedCallers) != PrecompilationFlags.Default);

        internal static bool CompileWithDelaySignAttribute =>
            ((_theBuildManager._precompilationFlags & PrecompilationFlags.DelaySign) != PrecompilationFlags.Default);

        private static bool CompileWithFixedAssemblyNames =>
            ((_theBuildManager._precompilationFlags & PrecompilationFlags.FixedNames) != PrecompilationFlags.Default);

        internal static Dictionary<string, string> GenerateFileTable
        {
            get
            {
                if (_theBuildManager._generatedFileTable == null)
                {
                    _theBuildManager._generatedFileTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _theBuildManager._generatedFileTable;
            }
        }

        internal static VirtualPath GlobalAsaxVirtualPath =>
            _theBuildManager._globalAsaxVirtualPath;

        private bool IsNonUpdatablePrecompiledApp =>
            (IsPrecompiledApp && !this._isUpdatablePrecompiledApp);

        internal static bool IsPrecompiledApp =>
            _theBuildManager.IsPrecompiledAppInternal;

        private bool IsPrecompiledAppInternal
        {
            get
            {
                if (!this._isPrecompiledAppComputed)
                {
                    this._isPrecompiledApp = ReadPrecompMarkerFile(HttpRuntime.AppDomainAppPathInternal, out this._isUpdatablePrecompiledApp);
                    this._isPrecompiledAppComputed = true;
                }
                return this._isPrecompiledApp;
            }
        }

        internal static bool IsUpdatablePrecompiledApp =>
            _theBuildManager.IsUpdatablePrecompiledAppInternal;

        private bool IsUpdatablePrecompiledAppInternal =>
            (IsPrecompiledApp && this._isUpdatablePrecompiledApp);

        internal static bool OptimizeCompilations =>
            _theBuildManager._optimizeCompilations;

        internal static bool PerformingPrecompilation
        {
            get => 
                _theBuildManager._performingPrecompilation;
            set
            {
                _theBuildManager._performingPrecompilation = value;
            }
        }

        private static bool PrecompilingForCleanBuild =>
            ((_theBuildManager._precompilationFlags & PrecompilationFlags.Clean) != PrecompilationFlags.Default);

        internal static bool PrecompilingForDeployment =>
            (_theBuildManager._precompTargetPhysicalDir != null);

        internal static bool PrecompilingForUpdatableDeployment
        {
            get
            {
                if (!PrecompilingForDeployment)
                {
                    return false;
                }
                return ((_theBuildManager._precompilationFlags & PrecompilationFlags.Updatable) != PrecompilationFlags.Default);
            }
        }

        internal static bool PrecompilingWithCodeAnalysisSymbol =>
            ((_theBuildManager._precompilationFlags & PrecompilationFlags.CodeAnalysis) != PrecompilationFlags.Default);

        internal static bool PrecompilingWithDebugInfo
        {
            get
            {
                if (!PrecompilingForDeployment)
                {
                    return false;
                }
                return ((_theBuildManager._precompilationFlags & PrecompilationFlags.ForceDebug) != PrecompilationFlags.Default);
            }
        }

        internal static VirtualPath ScriptVirtualDir =>
            _theBuildManager._scriptVirtualDir;

        internal static bool SkipTopLevelCompilationExceptions
        {
            get => 
                _theBuildManager._skipTopLevelCompilationExceptions;
            set
            {
                _theBuildManager._skipTopLevelCompilationExceptions = value;
            }
        }

        internal static string StrongNameKeyContainer =>
            _theBuildManager._strongNameKeyContainer;

        internal static string StrongNameKeyFile =>
            _theBuildManager._strongNameKeyFile;

        internal static BuildManager TheBuildManager =>
            _theBuildManager;

        internal static bool ThrowOnFirstParseError
        {
            get => 
                _theBuildManager._throwOnFirstParseError;
            set
            {
                _theBuildManager._throwOnFirstParseError = value;
            }
        }

        private IDictionary<string, AssemblyReferenceInfo> TopLevelAssembliesIndexTable =>
            this._topLevelAssembliesIndexTable;

        private List<Assembly> TopLevelReferencedAssemblies =>
            this._topLevelReferencedAssemblies;

        internal static string WebHashFilePath =>
            _theBuildManager._webHashFilePath;
    }
}

