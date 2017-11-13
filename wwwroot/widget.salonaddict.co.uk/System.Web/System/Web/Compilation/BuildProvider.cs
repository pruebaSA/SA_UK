namespace System.Web.Compilation
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.UI;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.High), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.High)]
    public abstract class BuildProvider
    {
        private BuildProviderSet _buildProviderDependencies;
        private ICollection _referencedAssemblies;
        private System.Web.VirtualPath _virtualPath;
        internal const int contributedCode = 0x20;
        internal const int dontThrowOnFirstParseError = 0x10;
        internal SimpleBitVector32 flags;
        internal const int ignoreControlProperties = 8;
        internal const int ignoreParseErrors = 4;
        internal const int isDependedOn = 1;
        internal const int noBuildResult = 2;

        protected BuildProvider()
        {
        }

        internal void AddBuildProviderDependency(BuildProvider dependentBuildProvider)
        {
            if (this._buildProviderDependencies == null)
            {
                this._buildProviderDependencies = new BuildProviderSet();
            }
            this._buildProviderDependencies.Add(dependentBuildProvider);
            dependentBuildProvider.flags[1] = true;
        }

        internal virtual BuildResult CreateBuildResult(CompilerResults results)
        {
            BuildResult result;
            if (this.flags[2])
            {
                return null;
            }
            Type generatedType = this.GetGeneratedType(results);
            if (generatedType != null)
            {
                BuildResultCompiledType type2 = this.CreateBuildResult(generatedType);
                if ((results == null) || (generatedType.Assembly != results.CompiledAssembly))
                {
                    type2.UsesExistingAssembly = true;
                }
                result = type2;
            }
            else
            {
                string customString = this.GetCustomString(results);
                if (customString != null)
                {
                    result = new BuildResultCustomString(this.flags[0x20] ? results.CompiledAssembly : null, customString);
                }
                else
                {
                    if (results == null)
                    {
                        return null;
                    }
                    result = new BuildResultCompiledAssembly(results.CompiledAssembly);
                }
            }
            int resultFlags = (int) this.GetResultFlags(results);
            if (resultFlags != 0)
            {
                resultFlags &= 0xffff;
                result.Flags |= resultFlags;
            }
            return result;
        }

        internal virtual BuildResultCompiledType CreateBuildResult(Type t) => 
            new BuildResultCompiledType(t);

        public virtual void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
        }

        internal BuildResult GetBuildResult(CompilerResults results)
        {
            BuildResult result = this.CreateBuildResult(results);
            if (result == null)
            {
                return null;
            }
            result.VirtualPath = this.VirtualPathObject;
            this.SetBuildResultDependencies(result);
            return result;
        }

        internal virtual ICollection GetBuildResultVirtualPathDependencies() => 
            null;

        protected internal virtual CodeCompileUnit GetCodeCompileUnit(out IDictionary linePragmasTable)
        {
            CodeSnippetCompileUnit unit = new CodeSnippetCompileUnit(Util.StringFromVirtualPath(this.VirtualPathObject));
            LinePragmaCodeInfo info = new LinePragmaCodeInfo(1, 1, 1, -1, false);
            linePragmasTable = new Hashtable();
            linePragmasTable[1] = info;
            return unit;
        }

        internal static CompilerType GetCompilerTypeFromBuildProvider(BuildProvider buildProvider)
        {
            HttpContext context = null;
            CompilerType type2;
            if (EtwTrace.IsTraceEnabled(5, 1) && ((context = HttpContext.Current) != null))
            {
                EtwTrace.Trace(EtwTraceType.ETW_TYPE_PARSE_ENTER, context.WorkerRequest);
            }
            try
            {
                CompilerType codeCompilerType = buildProvider.CodeCompilerType;
                if (codeCompilerType != null)
                {
                    CompilationUtil.CheckCompilerOptionsAllowed(codeCompilerType.CompilerParameters.CompilerOptions, false, null, 0);
                }
                type2 = codeCompilerType;
            }
            finally
            {
                if (EtwTrace.IsTraceEnabled(5, 1) && (context != null))
                {
                    EtwTrace.Trace(EtwTraceType.ETW_TYPE_PARSE_LEAVE, context.WorkerRequest);
                }
            }
            return type2;
        }

        internal virtual ICollection GetCompileWithDependencies() => 
            null;

        internal string GetCultureName() => 
            Util.GetCultureName(this.VirtualPath);

        public virtual string GetCustomString(CompilerResults results) => 
            null;

        protected CompilerType GetDefaultCompilerType() => 
            CompilationUtil.GetDefaultLanguageCompilerInfo(null, this.VirtualPathObject);

        protected CompilerType GetDefaultCompilerTypeForLanguage(string language) => 
            CompilationUtil.GetCompilerInfoFromLanguage(this.VirtualPathObject, language);

        internal static string GetDisplayName(BuildProvider buildProvider)
        {
            if (buildProvider.VirtualPath != null)
            {
                return buildProvider.VirtualPath;
            }
            return buildProvider.GetType().Name;
        }

        public virtual Type GetGeneratedType(CompilerResults results) => 
            null;

        internal virtual ICollection GetGeneratedTypeNames() => 
            null;

        public virtual BuildProviderResultFlags GetResultFlags(CompilerResults results) => 
            BuildProviderResultFlags.Default;

        protected TextReader OpenReader() => 
            this.OpenReader(this.VirtualPathObject);

        protected TextReader OpenReader(string virtualPath) => 
            this.OpenReader(System.Web.VirtualPath.Create(virtualPath));

        internal TextReader OpenReader(System.Web.VirtualPath virtualPath) => 
            Util.ReaderFromStream(this.OpenStream(virtualPath), virtualPath);

        protected Stream OpenStream() => 
            this.OpenStream(this.VirtualPath);

        protected Stream OpenStream(string virtualPath) => 
            VirtualPathProvider.OpenFile(virtualPath);

        internal Stream OpenStream(System.Web.VirtualPath virtualPath) => 
            virtualPath.OpenFile();

        internal void SetBuildResultDependencies(BuildResult result)
        {
            result.AddVirtualPathDependencies(this.VirtualPathDependencies);
        }

        internal void SetContributedCode()
        {
            this.flags[0x20] = true;
        }

        internal void SetNoBuildResult()
        {
            this.flags[2] = true;
        }

        internal void SetReferencedAssemblies(ICollection referencedAssemblies)
        {
            this._referencedAssemblies = referencedAssemblies;
        }

        internal void SetVirtualPath(System.Web.VirtualPath virtualPath)
        {
            this._virtualPath = virtualPath;
        }

        internal virtual IAssemblyDependencyParser AssemblyDependencyParser =>
            null;

        internal BuildProviderSet BuildProviderDependencies =>
            this._buildProviderDependencies;

        public virtual CompilerType CodeCompilerType =>
            null;

        internal bool IgnoreControlProperties
        {
            get => 
                this.flags[8];
            set
            {
                this.flags[8] = value;
            }
        }

        internal virtual bool IgnoreParseErrors
        {
            get => 
                this.flags[4];
            set
            {
                this.flags[4] = value;
            }
        }

        internal bool IsDependedOn =>
            this.flags[1];

        protected ICollection ReferencedAssemblies =>
            this._referencedAssemblies;

        internal bool ThrowOnFirstParseError
        {
            get => 
                !this.flags[0x10];
            set
            {
                this.flags[0x10] = !value;
            }
        }

        protected internal string VirtualPath =>
            System.Web.VirtualPath.GetVirtualPathString(this._virtualPath);

        public virtual ICollection VirtualPathDependencies =>
            new SingleObjectCollection(this.VirtualPath);

        internal System.Web.VirtualPath VirtualPathObject =>
            this._virtualPath;
    }
}

