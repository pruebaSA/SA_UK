namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RegisteredHiddenField
    {
        private System.Web.UI.Control _control;
        private string _initialValue;
        private string _name;

        internal RegisteredHiddenField(System.Web.UI.Control control, string hiddenFieldName, string hiddenFieldInitialValue)
        {
            this._control = control;
            this._name = hiddenFieldName;
            this._initialValue = hiddenFieldInitialValue;
        }

        public System.Web.UI.Control Control =>
            this._control;

        public string InitialValue =>
            this._initialValue;

        public string Name =>
            this._name;
    }
}

