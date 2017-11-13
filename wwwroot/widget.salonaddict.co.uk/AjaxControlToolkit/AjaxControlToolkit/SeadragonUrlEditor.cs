namespace AjaxControlToolkit
{
    using System;
    using System.Security.Permissions;
    using System.Web.UI.Design;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class SeadragonUrlEditor : UrlEditor
    {
        protected override string Caption =>
            base.Caption;

        protected override string Filter =>
            "DZI Files (*.dzi)|*.dzi|XML Files (*.xml)|*.xml";
    }
}

