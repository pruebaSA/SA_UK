namespace System.Web.UI.WebControls.Adapters
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HierarchicalDataBoundControlAdapter : WebControlAdapter
    {
        protected internal virtual void PerformDataBinding()
        {
            this.Control.PerformDataBinding();
        }

        protected HierarchicalDataBoundControl Control =>
            ((HierarchicalDataBoundControl) base.Control);
    }
}

