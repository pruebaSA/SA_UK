namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartMovingEventArgs : WebPartCancelEventArgs
    {
        private WebPartZoneBase _zone;
        private int _zoneIndex;

        public WebPartMovingEventArgs(WebPart webPart, WebPartZoneBase zone, int zoneIndex) : base(webPart)
        {
            this._zone = zone;
            this._zoneIndex = zoneIndex;
        }

        public WebPartZoneBase Zone
        {
            get => 
                this._zone;
            set
            {
                this._zone = value;
            }
        }

        public int ZoneIndex
        {
            get => 
                this._zoneIndex;
            set
            {
                this._zoneIndex = value;
            }
        }
    }
}

