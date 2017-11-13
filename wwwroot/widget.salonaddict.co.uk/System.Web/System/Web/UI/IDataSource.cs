namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IDataSource
    {
        event EventHandler DataSourceChanged;

        DataSourceView GetView(string viewName);
        ICollection GetViewNames();
    }
}

