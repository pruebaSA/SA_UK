namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HierarchicalDataSourceView
    {
        protected HierarchicalDataSourceView()
        {
        }

        public abstract IHierarchicalEnumerable Select();
    }
}

