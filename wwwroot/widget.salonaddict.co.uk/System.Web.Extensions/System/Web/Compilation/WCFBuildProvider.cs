namespace System.Web.Compilation
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Design;
    using System.Data.Services.Client;
    using System.Data.Services.Design;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Compilation.WCFModel;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Resources;
    using System.Xml;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WCFBuildProvider : System.Web.Compilation.BuildProvider
    {
        internal const string DataSvcMapExtension = ".datasvcmap";
        private const int FRAMEWORK_VERSION_35 = 0x30005;
        internal const string SvcMapExtension = ".svcmap";
        private const string TOOL_CONFIG_ITEM_NAME = "Reference.config";
        internal const string WebRefDirectoryName = "App_WebReferences";

        private static string CalculateGeneratedNamespace(string webReferencesRootVirtualPath, string virtualPath)
        {
            webReferencesRootVirtualPath = VirtualPathUtility.AppendTrailingSlash(webReferencesRootVirtualPath);
            virtualPath = VirtualPathUtility.AppendTrailingSlash(virtualPath);
            if (webReferencesRootVirtualPath.Length == virtualPath.Length)
            {
                return string.Empty;
            }
            virtualPath = VirtualPathUtility.RemoveTrailingSlash(virtualPath).Substring(webReferencesRootVirtualPath.Length);
            string[] strArray = virtualPath.Split(new char[] { '/' });
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = MakeValidTypeNameFromString(strArray[i]);
            }
            return string.Join(".", strArray);
        }

        private static void CollectErrorMessages(IEnumerable errors, StringBuilder collectedMessages)
        {
            foreach (ProxyGenerationError error in errors)
            {
                if (!error.IsWarning)
                {
                    if (collectedMessages.Length > 0)
                    {
                        collectedMessages.Append(Environment.NewLine);
                    }
                    collectedMessages.Append(ConvertToBuildProviderErrorMessage(error));
                }
            }
        }

        private static string ConvertToBuildProviderErrorMessage(ProxyGenerationError generationError)
        {
            string message = generationError.Message;
            if (string.IsNullOrEmpty(generationError.MetadataFile))
            {
                return message;
            }
            if (generationError.LineNumber < 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "'{0}': {1}", new object[] { generationError.MetadataFile, message });
            }
            if (generationError.LinePosition < 0)
            {
                return string.Format(CultureInfo.CurrentCulture, "'{0}' ({1}): {2}", new object[] { generationError.MetadataFile, generationError.LineNumber, message });
            }
            return string.Format(CultureInfo.CurrentCulture, "'{0}' ({1},{2}): {3}", new object[] { generationError.MetadataFile, generationError.LineNumber, generationError.LinePosition, message });
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            foreach (VirtualFile file in this.GetVirtualDirectory(base.VirtualPath).Files)
            {
                string extension = Path.GetExtension(file.VirtualPath);
                if (extension.Equals(".svcmap", StringComparison.OrdinalIgnoreCase))
                {
                    string mapFilePath = HostingEnvironment.MapPath(file.VirtualPath);
                    CodeCompileUnit compileUnit = this.GenerateCodeFromServiceMapFile(mapFilePath);
                    assemblyBuilder.AddCodeCompileUnit(this, compileUnit);
                }
                else if (extension.Equals(".datasvcmap", StringComparison.OrdinalIgnoreCase))
                {
                    string str3 = HostingEnvironment.MapPath(file.VirtualPath);
                    this.GenerateCodeFromDataServiceMapFile(str3, assemblyBuilder);
                }
            }
        }

        private void GenerateCodeFromDataServiceMapFile(string mapFilePath, AssemblyBuilder assemblyBuilder)
        {
            try
            {
                assemblyBuilder.AddAssemblyReference(typeof(DataServiceContext).Assembly);
                DataSvcMapFile file = new DataSvcMapFileLoader(mapFilePath).LoadMapFile(Path.GetFileName(mapFilePath));
                if (file.MetadataList[0].ErrorInLoading != null)
                {
                    throw file.MetadataList[0].ErrorInLoading;
                }
                string content = file.MetadataList[0].Content;
                EntityClassGenerator generator = new EntityClassGenerator(LanguageOption.GenerateCSharpCode);
                using (TextWriter writer = assemblyBuilder.CreateCodeFile(this))
                {
                    generator.GenerateCode(XmlReader.Create(new StringReader(content)), writer, this.GetGeneratedNamespace());
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}: {1}", new object[] { Path.GetFileName(mapFilePath), message }), exception);
            }
        }

        private CodeCompileUnit GenerateCodeFromServiceMapFile(string mapFilePath)
        {
            CodeCompileUnit targetCompileUnit;
            try
            {
                string generatedNamespace = this.GetGeneratedNamespace();
                SvcMapFile svcMapFile = new SvcMapFileLoader(mapFilePath).LoadMapFile(Path.GetFileName(mapFilePath));
                HandleProxyGenerationErrors(svcMapFile.LoadErrors);
                CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("c#");
                VSWCFServiceContractGenerator generator = VSWCFServiceContractGenerator.GenerateCodeAndConfiguration(svcMapFile, this.GetToolConfig(svcMapFile, mapFilePath), codeDomProvider, generatedNamespace, null, null, new ImportExtensionServiceProvider(), new TypeResolver(), 0x30005, typeof(TypedDataSetSchemaImporterExtensionFx35));
                string referenceDisplayName = string.IsNullOrEmpty(generatedNamespace) ? Path.GetFileName(mapFilePath) : generatedNamespace;
                VerifyGeneratedCodeAndHandleErrors(referenceDisplayName, svcMapFile, generator.TargetCompileUnit, generator.ImportErrors, generator.ProxyGenerationErrors);
                targetCompileUnit = generator.TargetCompileUnit;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}: {1}", new object[] { Path.GetFileName(mapFilePath), message }), exception);
            }
            return targetCompileUnit;
        }

        private static string GetAppDomainAppVirtualPath()
        {
            string appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            if (appDomainAppVirtualPath == null)
            {
                throw new InvalidOperationException();
            }
            return VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.ToAbsolute(appDomainAppVirtualPath));
        }

        private string GetGeneratedNamespace()
        {
            string webRefDirectoryVirtualPath = GetWebRefDirectoryVirtualPath();
            string virtualPath = base.VirtualPath;
            if (virtualPath == null)
            {
                throw new InvalidOperationException();
            }
            return CalculateGeneratedNamespace(webRefDirectoryVirtualPath, virtualPath);
        }

        private System.Configuration.Configuration GetToolConfig(SvcMapFile mapFile, string mapFilePath)
        {
            string configFileBaseName = null;
            VirtualDirectoryMapping mapping;
            if ((mapFile != null) && (mapFilePath != null))
            {
                foreach (ExtensionFile file in mapFile.Extensions)
                {
                    if (string.Equals(file.Name, "Reference.config", StringComparison.Ordinal))
                    {
                        configFileBaseName = file.FileName;
                    }
                }
            }
            WebConfigurationFileMap fileMap = new WebConfigurationFileMap();
            if (configFileBaseName != null)
            {
                mapping = new VirtualDirectoryMapping(Path.GetDirectoryName(mapFilePath), true, configFileBaseName);
            }
            else
            {
                mapping = new VirtualDirectoryMapping(HostingEnvironment.ApplicationPhysicalPath, true);
            }
            fileMap.VirtualDirectories.Add("/", mapping);
            return WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/", HostingEnvironment.SiteName);
        }

        private VirtualDirectory GetVirtualDirectory(string virtualPath) => 
            HostingEnvironment.VirtualPathProvider.GetDirectory(base.VirtualPath);

        private static string GetWebRefDirectoryVirtualPath() => 
            VirtualPathUtility.Combine(GetAppDomainAppVirtualPath(), @"App_WebReferences\");

        private static void HandleProxyGenerationErrors(IEnumerable errors)
        {
            foreach (ProxyGenerationError error in errors)
            {
                if (!error.IsWarning && (error.ErrorGeneratorState != ProxyGenerationError.GeneratorState.GenerateCode))
                {
                    throw new InvalidOperationException(ConvertToBuildProviderErrorMessage(error));
                }
            }
        }

        private static bool IsAnyTypeGenerated(CodeCompileUnit compileUnit)
        {
            if (compileUnit != null)
            {
                foreach (CodeNamespace namespace2 in compileUnit.Namespaces)
                {
                    if (namespace2.Types.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static string MakeValidTypeNameFromString(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < typeName.Length; i++)
            {
                if ((i == 0) && char.IsDigit(typeName[0]))
                {
                    builder.Append('_');
                }
                if (char.IsLetterOrDigit(typeName[i]))
                {
                    builder.Append(typeName[i]);
                }
                else
                {
                    builder.Append('_');
                }
            }
            string str = builder.ToString();
            if (str.Equals("_", StringComparison.Ordinal))
            {
                str = "__";
            }
            return str;
        }

        private static void VerifyGeneratedCodeAndHandleErrors(string referenceDisplayName, SvcMapFile mapFile, CodeCompileUnit generatedCode, IEnumerable importErrors, IEnumerable generatorErrors)
        {
            HandleProxyGenerationErrors(importErrors);
            HandleProxyGenerationErrors(generatorErrors);
            if (((mapFile.MetadataList.Count > 0) && (mapFile.ClientOptions.ServiceContractMappingList.Count == 0)) && !IsAnyTypeGenerated(generatedCode))
            {
                StringBuilder collectedMessages = new StringBuilder();
                CollectErrorMessages(importErrors, collectedMessages);
                CollectErrorMessages(generatorErrors, collectedMessages);
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_FailedToGenerateCode, new object[] { referenceDisplayName, collectedMessages.ToString() }));
            }
        }

        private class ImportExtensionServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) => 
                null;
        }

        private class TypeResolver : IContractGeneratorReferenceTypeLoader
        {
            private Assembly[] _referencedAssemblies;

            void IContractGeneratorReferenceTypeLoader.LoadAllAssemblies(out IEnumerable<Assembly> loadedAssemblies, out IEnumerable<Exception> loadingErrors)
            {
                loadedAssemblies = this.ReferencedAssemblies;
                loadingErrors = new Exception[0];
            }

            Assembly IContractGeneratorReferenceTypeLoader.LoadAssembly(string assemblyName)
            {
                AssemblyName reference = new AssemblyName(assemblyName);
                foreach (Assembly assembly in this.ReferencedAssemblies)
                {
                    if (AssemblyName.ReferenceMatchesDefinition(reference, assembly.GetName()))
                    {
                        return assembly;
                    }
                }
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_FailedToLoadAssembly, new object[] { assemblyName }));
            }

            Type IContractGeneratorReferenceTypeLoader.LoadType(string typeName) => 
                BuildManager.GetType(typeName, true);

            private IEnumerable<Assembly> ReferencedAssemblies
            {
                get
                {
                    if (this._referencedAssemblies == null)
                    {
                        ICollection referencedAssemblies = BuildManager.GetReferencedAssemblies();
                        this._referencedAssemblies = new Assembly[referencedAssemblies.Count];
                        referencedAssemblies.CopyTo(this._referencedAssemblies, 0);
                    }
                    return this._referencedAssemblies;
                }
            }
        }
    }
}

