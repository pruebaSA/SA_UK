namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PolygonHotSpot : HotSpot
    {
        public override string GetCoordinates() => 
            this.Coordinates;

        [WebCategory("Appearance"), WebSysDescription("PolygonHotSpot_Coordinates"), DefaultValue("")]
        public string Coordinates
        {
            get
            {
                string str = base.ViewState["Coordinates"] as string;
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.ViewState["Coordinates"] = value;
            }
        }

        protected internal override string MarkupName =>
            "poly";
    }
}

