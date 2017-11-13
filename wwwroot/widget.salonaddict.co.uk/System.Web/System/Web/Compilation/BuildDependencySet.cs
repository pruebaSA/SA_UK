namespace System.Web.Compilation
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BuildDependencySet
    {
        private BuildResult _result;

        internal BuildDependencySet(BuildResult result)
        {
            this._result = result;
        }

        public string HashCode =>
            this._result.VirtualPathDependenciesHash;

        public IEnumerable VirtualPaths =>
            this._result.VirtualPathDependencies;
    }
}

