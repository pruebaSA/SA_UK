namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DetailsViewPagerRow : DetailsViewRow, INonBindingContainer, INamingContainer
    {
        public DetailsViewPagerRow(int rowIndex, DataControlRowType rowType, DataControlRowState rowState) : base(rowIndex, rowType, rowState)
        {
        }
    }
}

