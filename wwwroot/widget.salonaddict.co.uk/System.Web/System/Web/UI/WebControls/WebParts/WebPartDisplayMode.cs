namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class WebPartDisplayMode
    {
        private string _name;

        protected WebPartDisplayMode(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this._name = name;
        }

        public virtual bool IsEnabled(WebPartManager webPartManager)
        {
            if (this.RequiresPersonalization)
            {
                return webPartManager.Personalization.IsModifiable;
            }
            return true;
        }

        public virtual bool AllowPageDesign =>
            false;

        public virtual bool AssociatedWithToolZone =>
            false;

        public string Name =>
            this._name;

        public virtual bool RequiresPersonalization =>
            false;

        public virtual bool ShowHiddenWebParts =>
            false;
    }
}

