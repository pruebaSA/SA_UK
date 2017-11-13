namespace System.Web.Compilation
{
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.Util;

    internal static class CompilationUtil
    {
        internal const string CodeDomProviderOptionPath = "system.codedom/compilers/compiler/ProviderOption/";

        internal static void CheckCompilerOptionsAllowed(string compilerOptions, bool config, string file, int line)
        {
            if (!string.IsNullOrEmpty(compilerOptions) && !HttpRuntime.HasUnmanagedPermission())
            {
                string message = System.Web.SR.GetString("Insufficient_trust_for_attribute", new object[] { "compilerOptions" });
                if (config)
                {
                    throw new ConfigurationErrorsException(message, file, line);
                }
                throw new HttpException(message);
            }
        }

        internal static CodeDomProvider CreateCodeDomProvider(Type codeDomProviderType)
        {
            CodeDomProvider provider = CreateCodeDomProviderWithPropertyOptions(codeDomProviderType);
            if (provider != null)
            {
                return provider;
            }
            return (CodeDomProvider) Activator.CreateInstance(codeDomProviderType);
        }

        internal static CodeDomProvider CreateCodeDomProviderNonPublic(Type codeDomProviderType)
        {
            CodeDomProvider provider = CreateCodeDomProviderWithPropertyOptions(codeDomProviderType);
            if (provider != null)
            {
                return provider;
            }
            return (CodeDomProvider) HttpRuntime.CreateNonPublicInstance(codeDomProviderType);
        }

        [ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
        private static CodeDomProvider CreateCodeDomProviderWithPropertyOptions(Type codeDomProviderType)
        {
            IDictionary<string, string> providerOptions = GetProviderOptions(codeDomProviderType);
            if ((providerOptions != null) && (providerOptions.Count > 0))
            {
                ConstructorInfo constructor = codeDomProviderType.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
                if (constructor != null)
                {
                    return (CodeDomProvider) constructor.Invoke(new object[] { providerOptions });
                }
            }
            return null;
        }

        internal static AssemblyCollection GetAssembliesForAppLevel() => 
            RuntimeConfig.GetAppConfig().Compilation.Assemblies;

        internal static Type GetBuildProviderTypeFromExtension(CompilationSection config, string extension, BuildProviderAppliesTo neededFor, bool failIfUnknown)
        {
            System.Web.Configuration.BuildProvider provider = config.BuildProviders[extension];
            Type c = null;
            if (((provider != null) && (provider.TypeInternal != typeof(IgnoreFileBuildProvider))) && (provider.TypeInternal != typeof(ForceCopyBuildProvider)))
            {
                c = provider.TypeInternal;
            }
            if (((neededFor == BuildProviderAppliesTo.Web) && BuildManager.PrecompilingForUpdatableDeployment) && !typeof(BaseTemplateBuildProvider).IsAssignableFrom(c))
            {
                c = null;
            }
            if (c != null)
            {
                if ((neededFor & provider.AppliesToInternal) != 0)
                {
                    return c;
                }
            }
            else if ((neededFor != BuildProviderAppliesTo.Resources) && (config.GetCompilerInfoFromExtension(extension, false) != null))
            {
                return typeof(SourceFileBuildProvider);
            }
            if (failIfUnknown)
            {
                throw new HttpException(System.Web.SR.GetString("Unknown_buildprovider_extension", new object[] { extension, neededFor.ToString() }));
            }
            return null;
        }

        internal static Type GetBuildProviderTypeFromExtension(VirtualPath configPath, string extension, BuildProviderAppliesTo neededFor, bool failIfUnknown) => 
            GetBuildProviderTypeFromExtension(RuntimeConfig.GetConfig(configPath).Compilation, extension, neededFor, failIfUnknown);

        internal static CompilerType GetCodeDefaultLanguageCompilerInfo() => 
            new CompilerType(typeof(VBCodeProvider), null);

        internal static CodeSubDirectoriesCollection GetCodeSubDirectories()
        {
            CodeSubDirectoriesCollection codeSubDirectories = RuntimeConfig.GetAppConfig().Compilation.CodeSubDirectories;
            if (codeSubDirectories != null)
            {
                codeSubDirectories.EnsureRuntimeValidation();
            }
            return codeSubDirectories;
        }

        private static CompilerType GetCompilerInfoFromExtension(VirtualPath configPath, string extension) => 
            RuntimeConfig.GetConfig(configPath).Compilation.GetCompilerInfoFromExtension(extension, true);

        internal static CompilerType GetCompilerInfoFromLanguage(VirtualPath configPath, string language) => 
            RuntimeConfig.GetConfig(configPath).Compilation.GetCompilerInfoFromLanguage(language);

        internal static CompilerType GetCompilerInfoFromVirtualPath(VirtualPath virtualPath)
        {
            string extension = virtualPath.Extension;
            if (extension.Length == 0)
            {
                throw new HttpException(System.Web.SR.GetString("Empty_extension", new object[] { virtualPath }));
            }
            return GetCompilerInfoFromExtension(virtualPath, extension);
        }

        internal static CompilerType GetCSharpCompilerInfo(CompilationSection compConfig, VirtualPath configPath)
        {
            if (compConfig == null)
            {
                compConfig = RuntimeConfig.GetConfig(configPath).Compilation;
            }
            if (compConfig.DefaultLanguage == null)
            {
                return new CompilerType(typeof(CSharpCodeProvider), null);
            }
            return compConfig.GetCompilerInfoFromLanguage("c#");
        }

        internal static CompilerType GetDefaultLanguageCompilerInfo(CompilationSection compConfig, VirtualPath configPath)
        {
            if (compConfig == null)
            {
                compConfig = RuntimeConfig.GetConfig(configPath).Compilation;
            }
            if (compConfig.DefaultLanguage == null)
            {
                return GetCodeDefaultLanguageCompilerInfo();
            }
            return compConfig.GetCompilerInfoFromLanguage(compConfig.DefaultLanguage);
        }

        [ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
        private static IDictionary<string, string> GetProviderOptions(CompilerInfo ci)
        {
            PropertyInfo property = ci.GetType().GetProperty("ProviderOptions", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                return (IDictionary<string, string>) property.GetValue(ci, null);
            }
            return null;
        }

        [ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
        internal static IDictionary<string, string> GetProviderOptions(Type codeDomProviderType)
        {
            CodeDomProvider provider = (CodeDomProvider) Activator.CreateInstance(codeDomProviderType);
            if (CodeDomProvider.IsDefinedExtension(provider.FileExtension))
            {
                return GetProviderOptions(CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(provider.FileExtension)));
            }
            return null;
        }

        internal static long GetRecompilationHash(CompilationSection ps)
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            combiner.AddObject(ps.Debug);
            combiner.AddObject(ps.Strict);
            combiner.AddObject(ps.Explicit);
            combiner.AddObject(ps.Batch);
            combiner.AddObject(ps.OptimizeCompilations);
            combiner.AddObject(ps.BatchTimeout);
            combiner.AddObject(ps.MaxBatchGeneratedFileSize);
            combiner.AddObject(ps.MaxBatchSize);
            combiner.AddObject(ps.NumRecompilesBeforeAppRestart);
            combiner.AddObject(ps.DefaultLanguage);
            combiner.AddObject(ps.UrlLinePragmas);
            if (ps.AssemblyPostProcessorTypeInternal != null)
            {
                combiner.AddObject(ps.AssemblyPostProcessorTypeInternal.FullName);
            }
            foreach (Compiler compiler in ps.Compilers)
            {
                combiner.AddObject(compiler.Language);
                combiner.AddObject(compiler.Extension);
                combiner.AddObject(compiler.Type);
                combiner.AddObject(compiler.WarningLevel);
                combiner.AddObject(compiler.CompilerOptions);
            }
            foreach (System.Web.Configuration.ExpressionBuilder builder in ps.ExpressionBuilders)
            {
                combiner.AddObject(builder.ExpressionPrefix);
                combiner.AddObject(builder.Type);
            }
            AssemblyCollection assemblies = ps.Assemblies;
            if (assemblies.Count == 0)
            {
                combiner.AddObject("__clearassemblies");
            }
            else
            {
                foreach (AssemblyInfo info in assemblies)
                {
                    combiner.AddObject(info.Assembly);
                }
            }
            BuildProviderCollection buildProviders = ps.BuildProviders;
            if (buildProviders.Count == 0)
            {
                combiner.AddObject("__clearbuildproviders");
            }
            else
            {
                foreach (System.Web.Configuration.BuildProvider provider in buildProviders)
                {
                    combiner.AddObject(provider.Type);
                    combiner.AddObject(provider.Extension);
                }
            }
            CodeSubDirectoriesCollection codeSubDirectories = ps.CodeSubDirectories;
            if (codeSubDirectories.Count == 0)
            {
                combiner.AddObject("__clearcodesubdirs");
            }
            else
            {
                foreach (CodeSubDirectory directory in codeSubDirectories)
                {
                    combiner.AddObject(directory.DirectoryName);
                }
            }
            CompilerInfo[] allCompilerInfo = CodeDomProvider.GetAllCompilerInfo();
            if (allCompilerInfo != null)
            {
                foreach (CompilerInfo info2 in allCompilerInfo)
                {
                    if (info2.IsCodeDomProviderTypeValid)
                    {
                        string compilerOptions = info2.CreateDefaultCompilerParameters().CompilerOptions;
                        if (!string.IsNullOrEmpty(compilerOptions))
                        {
                            Type codeDomProviderType = info2.CodeDomProviderType;
                            if (codeDomProviderType != null)
                            {
                                combiner.AddObject(codeDomProviderType.FullName);
                            }
                            combiner.AddObject(compilerOptions);
                        }
                        if (info2.CodeDomProviderType != null)
                        {
                            IDictionary<string, string> providerOptions = GetProviderOptions(info2);
                            if ((providerOptions != null) && (providerOptions.Count > 0))
                            {
                                string fullName = info2.CodeDomProviderType.FullName;
                                foreach (string str3 in providerOptions.Keys)
                                {
                                    string str4 = providerOptions[str3];
                                    combiner.AddObject(fullName + ":" + str3 + "=" + str4);
                                }
                            }
                        }
                    }
                }
            }
            return combiner.CombinedHash;
        }

        internal static int GetRecompilationsBeforeAppRestarts() => 
            RuntimeConfig.GetAppConfig().Compilation.NumRecompilesBeforeAppRestart;

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.MemberAccess)]
        internal static Type GetTypeFromAssemblies(AssemblyCollection assembliesCollection, string typeName, bool ignoreCase)
        {
            if (assembliesCollection == null)
            {
                return null;
            }
            Type t = null;
            foreach (AssemblyInfo info in assembliesCollection)
            {
                foreach (Assembly assembly in info.AssemblyInternal)
                {
                    Type type2 = assembly.GetType(typeName, false, ignoreCase);
                    if (type2 != null)
                    {
                        if ((t != null) && (type2 != t))
                        {
                            throw new HttpException(System.Web.SR.GetString("Ambiguous_type", new object[] { typeName, Util.GetAssemblySafePathFromType(t), Util.GetAssemblySafePathFromType(type2) }));
                        }
                        t = type2;
                    }
                }
            }
            return t;
        }

        internal static bool IsBatchingEnabled(string configPath) => 
            RuntimeConfig.GetConfig(configPath).Compilation.Batch;

        internal static bool IsCompilerVersion35(Type codeDomProviderType)
        {
            string str;
            IDictionary<string, string> providerOptions = GetProviderOptions(codeDomProviderType);
            if ((providerOptions == null) || !providerOptions.TryGetValue("CompilerVersion", out str))
            {
                return false;
            }
            if (str == "v2.0")
            {
                return false;
            }
            if (str != "v3.5")
            {
                throw new ConfigurationException(System.Web.SR.GetString("Invalid_attribute_value", new object[] { str, "system.codedom/compilers/compiler/ProviderOption/CompilerVersion" }));
            }
            return true;
        }

        internal static bool IsDebuggingEnabled(HttpContext context) => 
            RuntimeConfig.GetConfig(context).Compilation.Debug;

        internal static Type LoadTypeWithChecks(string typeName, Type requiredBaseType, Type requiredBaseType2, ConfigurationElement elem, string propertyName)
        {
            Type type = ConfigUtil.GetType(typeName, propertyName, elem);
            if (requiredBaseType2 == null)
            {
                ConfigUtil.CheckAssignableType(requiredBaseType, type, elem, propertyName);
                return type;
            }
            ConfigUtil.CheckAssignableType(requiredBaseType, requiredBaseType2, type, elem, propertyName);
            return type;
        }

        internal static bool NeedToCopyFile(VirtualPath virtualPath, bool updatable, out bool createStub)
        {
            createStub = false;
            CompilationSection compilation = RuntimeConfig.GetConfig(virtualPath).Compilation;
            string extension = virtualPath.Extension;
            System.Web.Configuration.BuildProvider provider = compilation.BuildProviders[extension];
            if (provider != null)
            {
                if ((BuildProviderAppliesTo.Web & provider.AppliesToInternal) == 0)
                {
                    return true;
                }
                if (provider.TypeInternal == typeof(ForceCopyBuildProvider))
                {
                    return true;
                }
                if ((provider.TypeInternal != typeof(IgnoreFileBuildProvider)) && BuildManager.PrecompilingForUpdatableDeployment)
                {
                    return true;
                }
                createStub = true;
                if (((provider.TypeInternal == typeof(UserControlBuildProvider)) || (provider.TypeInternal == typeof(MasterPageBuildProvider))) || (provider.TypeInternal == typeof(IgnoreFileBuildProvider)))
                {
                    createStub = false;
                }
                return false;
            }
            if (compilation.GetCompilerInfoFromExtension(extension, false) != null)
            {
                return false;
            }
            if (System.Web.Util.StringUtil.EqualsIgnoreCase(extension, ".asax"))
            {
                return false;
            }
            if (!updatable && System.Web.Util.StringUtil.EqualsIgnoreCase(extension, ".skin"))
            {
                return false;
            }
            return true;
        }
    }
}

