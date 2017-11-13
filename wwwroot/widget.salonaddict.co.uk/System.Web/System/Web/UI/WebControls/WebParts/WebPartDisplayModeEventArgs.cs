namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartDisplayModeEventArgs : EventArgs
    {
        private WebPartDisplayMode _oldDisplayMode;

        public WebPartDisplayModeEventArgs(WebPartDisplayMode oldDisplayMode)
        {
            this._oldDisplayMode = oldDisplayMode;
        }

        public WebPartDisplayMode OldDisplayMode
        {
            get => 
                this._oldDisplayMode;
            set
            {
                this._oldDisplayMode = value;
            }
        }
    }
}

