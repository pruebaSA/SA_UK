namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [EditorBrowsable(EditorBrowsableState.Advanced), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ControlSkin
    {
        private ControlSkinDelegate _controlSkinDelegate;
        private Type _controlType;

        public ControlSkin(Type controlType, ControlSkinDelegate themeDelegate)
        {
            this._controlType = controlType;
            this._controlSkinDelegate = themeDelegate;
        }

        public void ApplySkin(Control control)
        {
            this._controlSkinDelegate(control);
        }

        public Type ControlType =>
            this._controlType;
    }
}

