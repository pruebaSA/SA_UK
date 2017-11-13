namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CompositeScriptReferenceEventArgs : EventArgs
    {
        private readonly CompositeScriptReference _compositeScript;

        public CompositeScriptReferenceEventArgs(CompositeScriptReference compositeScript)
        {
            if (compositeScript == null)
            {
                throw new ArgumentNullException("compositeScript");
            }
            this._compositeScript = compositeScript;
        }

        public CompositeScriptReference CompositeScript =>
            this._compositeScript;
    }
}

