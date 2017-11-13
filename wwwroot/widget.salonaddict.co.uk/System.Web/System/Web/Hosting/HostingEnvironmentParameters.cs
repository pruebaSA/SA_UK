namespace System.Web.Hosting
{
    using System;
    using System.Web.Compilation;
    using System.Web.Util;

    [Serializable]
    internal class HostingEnvironmentParameters
    {
        private System.Web.Compilation.ClientBuildManagerParameter _clientBuildManagerParameter;
        private HostingEnvironmentFlags _hostingFlags;
        private string _precompTargetPhysicalDir;

        public System.Web.Compilation.ClientBuildManagerParameter ClientBuildManagerParameter
        {
            get => 
                this._clientBuildManagerParameter;
            set
            {
                this._clientBuildManagerParameter = value;
            }
        }

        public HostingEnvironmentFlags HostingFlags
        {
            get => 
                this._hostingFlags;
            set
            {
                this._hostingFlags = value;
            }
        }

        public string PrecompilationTargetPhysicalDirectory
        {
            get => 
                this._precompTargetPhysicalDir;
            set
            {
                this._precompTargetPhysicalDir = FileUtil.FixUpPhysicalDirectory(value);
            }
        }
    }
}

