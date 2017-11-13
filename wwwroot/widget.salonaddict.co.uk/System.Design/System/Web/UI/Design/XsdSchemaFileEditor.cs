namespace System.Web.UI.Design
{
    using System;
    using System.Design;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class XsdSchemaFileEditor : UrlEditor
    {
        protected override string Caption =>
            System.Design.SR.GetString("XsdSchemaFileEditor_Caption");

        protected override string Filter =>
            System.Design.SR.GetString("XsdSchemaFileEditor_Filter");
    }
}

