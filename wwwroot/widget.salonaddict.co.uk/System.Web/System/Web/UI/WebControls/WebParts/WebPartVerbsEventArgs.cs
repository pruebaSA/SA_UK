namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartVerbsEventArgs : EventArgs
    {
        private WebPartVerbCollection _verbs;

        public WebPartVerbsEventArgs() : this(null)
        {
        }

        public WebPartVerbsEventArgs(WebPartVerbCollection verbs)
        {
            this._verbs = verbs;
        }

        public WebPartVerbCollection Verbs
        {
            get
            {
                if (this._verbs == null)
                {
                    return WebPartVerbCollection.Empty;
                }
                return this._verbs;
            }
            set
            {
                this._verbs = value;
            }
        }
    }
}

