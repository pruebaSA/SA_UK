namespace System.Web.Compilation
{
    using System;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Util;

    internal class NonBatchDirectoryCompiler
    {
        private CompilationSection _compConfig;
        private VirtualDirectory _vdir;

        internal NonBatchDirectoryCompiler(VirtualDirectory vdir)
        {
            this._vdir = vdir;
            this._compConfig = RuntimeConfig.GetConfig(this._vdir.VirtualPath).Compilation;
        }

        internal void Process()
        {
            foreach (VirtualFile file in this._vdir.Files)
            {
                string extension = UrlPath.GetExtension(file.VirtualPath);
                Type type = CompilationUtil.GetBuildProviderTypeFromExtension(this._compConfig, extension, BuildProviderAppliesTo.Web, false);
                if (((type != null) && (type != typeof(SourceFileBuildProvider))) && (type != typeof(ResXBuildProvider)))
                {
                    BuildManager.GetVPathBuildResult(file.VirtualPathObject);
                }
            }
        }
    }
}

