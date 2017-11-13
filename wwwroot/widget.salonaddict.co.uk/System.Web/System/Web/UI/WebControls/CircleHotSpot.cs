namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CircleHotSpot : HotSpot
    {
        public override string GetCoordinates() => 
            string.Concat(new object[] { this.X, ",", this.Y, ",", this.Radius });

        protected internal override string MarkupName =>
            "circle";

        [WebCategory("Appearance"), WebSysDescription("CircleHotSpot_Radius"), DefaultValue(0)]
        public int Radius
        {
            get
            {
                object obj2 = base.ViewState["Radius"];
                if (obj2 != null)
                {
                    return (int) obj2;
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                base.ViewState["Radius"] = value;
            }
        }

        [DefaultValue(0), WebCategory("Appearance"), WebSysDescription("CircleHotSpot_X")]
        public int X
        {
            get
            {
                object obj2 = base.ViewState["X"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["X"] = value;
            }
        }

        [WebSysDescription("CircleHotSpot_Y"), WebCategory("Appearance"), DefaultValue(0)]
        public int Y
        {
            get
            {
                object obj2 = base.ViewState["Y"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["Y"] = value;
            }
        }
    }
}

