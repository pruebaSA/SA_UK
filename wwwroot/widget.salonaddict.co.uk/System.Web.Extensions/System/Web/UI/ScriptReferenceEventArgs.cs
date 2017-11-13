namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptReferenceEventArgs : EventArgs
    {
        private readonly ScriptReference _script;

        public ScriptReferenceEventArgs(ScriptReference script)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }
            this._script = script;
        }

        public ScriptReference Script =>
            this._script;
    }
}

