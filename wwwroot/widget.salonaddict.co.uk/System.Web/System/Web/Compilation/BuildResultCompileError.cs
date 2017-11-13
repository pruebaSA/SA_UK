namespace System.Web.Compilation
{
    using System;
    using System.Web;

    internal class BuildResultCompileError : BuildResult
    {
        private HttpCompileException _compileException;

        internal BuildResultCompileError(VirtualPath virtualPath, HttpCompileException compileException)
        {
            base.VirtualPath = virtualPath;
            this._compileException = compileException;
        }

        internal override bool CacheToDisk =>
            false;

        internal HttpCompileException CompileException =>
            this._compileException;

        internal override DateTime MemoryCacheExpiration =>
            DateTime.UtcNow.AddSeconds(10.0);
    }
}

