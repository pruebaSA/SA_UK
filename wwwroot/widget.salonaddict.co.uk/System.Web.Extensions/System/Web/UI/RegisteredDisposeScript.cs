namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RegisteredDisposeScript
    {
        private System.Web.UI.Control _control;
        private UpdatePanel _parentUpdatePanel;
        private string _script;

        internal RegisteredDisposeScript(System.Web.UI.Control control, string disposeScript, UpdatePanel parentUpdatePanel)
        {
            this._control = control;
            this._script = disposeScript;
            this._parentUpdatePanel = parentUpdatePanel;
        }

        public System.Web.UI.Control Control =>
            this._control;

        internal UpdatePanel ParentUpdatePanel =>
            this._parentUpdatePanel;

        public string Script =>
            this._script;
    }
}

