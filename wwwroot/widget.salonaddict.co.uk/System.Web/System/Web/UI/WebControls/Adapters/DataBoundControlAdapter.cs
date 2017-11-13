namespace System.Web.UI.WebControls.Adapters
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataBoundControlAdapter : WebControlAdapter
    {
        protected internal virtual void PerformDataBinding(IEnumerable data)
        {
            this.Control.PerformDataBinding(data);
        }

        protected DataBoundControl Control =>
            ((DataBoundControl) base.Control);
    }
}

