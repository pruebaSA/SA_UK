namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class FormViewPagerRow : FormViewRow, INonBindingContainer, INamingContainer
    {
        public FormViewPagerRow(int rowIndex, DataControlRowType rowType, DataControlRowState rowState) : base(rowIndex, rowType, rowState)
        {
        }
    }
}

