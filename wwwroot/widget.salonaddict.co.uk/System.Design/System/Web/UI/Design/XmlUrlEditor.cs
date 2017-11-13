namespace System.Web.UI.Design
{
    using System;
    using System.Design;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class XmlUrlEditor : UrlEditor
    {
        protected override string Caption =>
            System.Design.SR.GetString("UrlPicker_XmlCaption");

        protected override string Filter =>
            System.Design.SR.GetString("UrlPicker_XmlFilter");

        protected override UrlBuilderOptions Options =>
            UrlBuilderOptions.NoAbsolute;
    }
}

