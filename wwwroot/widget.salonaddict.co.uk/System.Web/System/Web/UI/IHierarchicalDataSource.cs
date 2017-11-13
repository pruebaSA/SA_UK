namespace System.Web.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IHierarchicalDataSource
    {
        event EventHandler DataSourceChanged;

        HierarchicalDataSourceView GetHierarchicalView(string viewPath);
    }
}

