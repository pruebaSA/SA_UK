namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RegisteredArrayDeclaration
    {
        private System.Web.UI.Control _control;
        private string _name;
        private string _value;

        internal RegisteredArrayDeclaration(System.Web.UI.Control control, string arrayName, string arrayValue)
        {
            this._control = control;
            this._name = arrayName;
            this._value = arrayValue;
        }

        public System.Web.UI.Control Control =>
            this._control;

        public string Name =>
            this._name;

        public string Value =>
            this._value;
    }
}

