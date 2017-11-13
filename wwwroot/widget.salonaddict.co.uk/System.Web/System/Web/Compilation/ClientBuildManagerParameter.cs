namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ClientBuildManagerParameter
    {
        private System.Web.Compilation.PrecompilationFlags _precompilationFlags;
        private string _strongNameKeyContainer;
        private string _strongNameKeyFile;

        public System.Web.Compilation.PrecompilationFlags PrecompilationFlags
        {
            get => 
                this._precompilationFlags;
            set
            {
                this._precompilationFlags = value;
            }
        }

        public string StrongNameKeyContainer
        {
            get => 
                this._strongNameKeyContainer;
            set
            {
                this._strongNameKeyContainer = value;
            }
        }

        public string StrongNameKeyFile
        {
            get => 
                this._strongNameKeyFile;
            set
            {
                this._strongNameKeyFile = value;
            }
        }
    }
}

