namespace System.Web.UI.Design
{
    using System;
    using System.Design;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class ImageUrlEditor : UrlEditor
    {
        protected override string Caption =>
            System.Design.SR.GetString("UrlPicker_ImageCaption");

        protected override string Filter =>
            System.Design.SR.GetString("UrlPicker_ImageFilter");
    }
}

