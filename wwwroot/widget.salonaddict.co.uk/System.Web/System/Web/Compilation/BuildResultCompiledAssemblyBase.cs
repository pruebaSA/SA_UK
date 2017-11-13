namespace System.Web.Compilation
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.Util;

    internal abstract class BuildResultCompiledAssemblyBase : BuildResult
    {
        private static string s_codegenDir;

        protected BuildResultCompiledAssemblyBase()
        {
        }

        private static bool AssemblyIsInCodegenDir(Assembly a)
        {
            FileInfo info = new FileInfo(Util.GetAssemblyCodeBase(a));
            string str2 = FileUtil.RemoveTrailingDirectoryBackSlash(info.Directory.FullName);
            if (s_codegenDir == null)
            {
                s_codegenDir = FileUtil.RemoveTrailingDirectoryBackSlash(HttpRuntime.CodegenDir);
            }
            return string.Equals(str2, s_codegenDir, StringComparison.OrdinalIgnoreCase);
        }

        private static bool AssemblyIsInvalid(Assembly a)
        {
            string assemblyCodeBase = Util.GetAssemblyCodeBase(a);
            if (FileUtil.FileExists(assemblyCodeBase))
            {
                return DiskBuildResultCache.HasDotDeleteFile(assemblyCodeBase);
            }
            return true;
        }

        private static void CheckAssemblyIsValid(Assembly a, Hashtable checkedAssemblies)
        {
            checkedAssemblies.Add(a, null);
            foreach (AssemblyName name in a.GetReferencedAssemblies())
            {
                Assembly assembly = Assembly.Load(name);
                if ((!assembly.GlobalAssemblyCache && AssemblyIsInCodegenDir(assembly)) && !checkedAssemblies.Contains(assembly))
                {
                    if (AssemblyIsInvalid(assembly))
                    {
                        throw new InvalidOperationException();
                    }
                    CheckAssemblyIsValid(assembly, checkedAssemblies);
                }
            }
        }

        protected override void ComputeHashCode(HashCodeCombiner hashCodeCombiner)
        {
            base.ComputeHashCode(hashCodeCombiner);
            CompilationSection compilation = RuntimeConfig.GetConfig(base.VirtualPath).Compilation;
            hashCodeCombiner.AddObject(compilation.RecompilationHash);
        }

        internal static Assembly GetPreservedAssembly(PreservationFileReader pfr)
        {
            Assembly assembly2;
            string attribute = pfr.GetAttribute("assembly");
            if (attribute == null)
            {
                return null;
            }
            try
            {
                Assembly a = Assembly.Load(attribute);
                if (AssemblyIsInvalid(a))
                {
                    throw new InvalidOperationException();
                }
                CheckAssemblyIsValid(a, new Hashtable());
                assembly2 = a;
            }
            catch
            {
                pfr.DiskCache.RemoveAssemblyAndRelatedFiles(attribute);
                throw;
            }
            return assembly2;
        }

        internal override void RemoveOutOfDateResources(PreservationFileReader pfr)
        {
            base.ReadPreservedFlags(pfr);
            if (!this.UsesExistingAssembly)
            {
                string attribute = pfr.GetAttribute("assembly");
                if (attribute != null)
                {
                    pfr.DiskCache.RemoveAssemblyAndRelatedFiles(attribute);
                }
            }
        }

        internal override void SetPreservedAttributes(PreservationFileWriter pfw)
        {
            base.SetPreservedAttributes(pfw);
            if (this.ResultAssembly != null)
            {
                string fullName;
                if (this.ResultAssembly.GlobalAssemblyCache)
                {
                    fullName = this.ResultAssembly.FullName;
                }
                else
                {
                    fullName = this.ResultAssembly.GetName().Name;
                }
                pfw.SetAttribute("assembly", fullName);
            }
        }

        internal override bool IsUnloadable =>
            (this.ResultAssembly == null);

        internal abstract Assembly ResultAssembly { get; set; }

        internal bool UsesExistingAssembly
        {
            get => 
                this._flags[0x20000];
            set
            {
                this._flags[0x20000] = value;
            }
        }
    }
}

