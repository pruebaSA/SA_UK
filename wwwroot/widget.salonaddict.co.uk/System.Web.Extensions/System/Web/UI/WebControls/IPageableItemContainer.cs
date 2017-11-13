namespace System.Web.UI.WebControls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IPageableItemContainer
    {
        event EventHandler<PageEventArgs> TotalRowCountAvailable;

        void SetPageProperties(int startRowIndex, int maximumRows, bool databind);

        int MaximumRows { get; }

        int StartRowIndex { get; }
    }
}

