namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class WebPartTransformer
    {
        protected WebPartTransformer()
        {
        }

        public virtual Control CreateConfigurationControl() => 
            null;

        protected internal virtual void LoadConfigurationState(object savedState)
        {
        }

        protected internal virtual object SaveConfigurationState() => 
            null;

        public abstract object Transform(object providerData);
    }
}

