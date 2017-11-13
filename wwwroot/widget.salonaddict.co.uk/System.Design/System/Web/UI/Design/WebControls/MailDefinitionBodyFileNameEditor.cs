namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Design;
    using System.Security.Permissions;
    using System.Web.UI.Design;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class MailDefinitionBodyFileNameEditor : UrlEditor
    {
        protected override string Caption =>
            System.Design.SR.GetString("MailDefinitionBodyFileNameEditor_DefaultCaption");

        protected override string Filter =>
            System.Design.SR.GetString("MailDefinitionBodyFileNameEditor_DefaultFilter");
    }
}

