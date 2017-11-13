namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RegisteredExpandoAttribute
    {
        private System.Web.UI.Control _control;
        private string _controlId;
        private bool _encode;
        private string _name;
        private string _value;

        internal RegisteredExpandoAttribute(System.Web.UI.Control control, string controlId, string name, string value, bool encode)
        {
            this._control = control;
            this._controlId = controlId;
            this._name = name;
            this._value = value;
            this._encode = encode;
        }

        public System.Web.UI.Control Control =>
            this._control;

        public string ControlId =>
            this._controlId;

        public bool Encode =>
            this._encode;

        public string Name =>
            this._name;

        public string Value =>
            this._value;
    }
}

