namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RectangleHotSpot : HotSpot
    {
        public override string GetCoordinates() => 
            string.Concat(new object[] { this.Left, ",", this.Top, ",", this.Right, ",", this.Bottom });

        [WebSysDescription("RectangleHotSpot_Bottom"), DefaultValue(0), WebCategory("Appearance")]
        public int Bottom
        {
            get
            {
                object obj2 = base.ViewState["Bottom"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["Bottom"] = value;
            }
        }

        [DefaultValue(0), WebSysDescription("RectangleHotSpot_Left"), WebCategory("Appearance")]
        public int Left
        {
            get
            {
                object obj2 = base.ViewState["Left"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["Left"] = value;
            }
        }

        protected internal override string MarkupName =>
            "rect";

        [DefaultValue(0), WebSysDescription("RectangleHotSpot_Right"), WebCategory("Appearance")]
        public int Right
        {
            get
            {
                object obj2 = base.ViewState["Right"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["Right"] = value;
            }
        }

        [WebSysDescription("RectangleHotSpot_Top"), WebCategory("Appearance"), DefaultValue(0)]
        public int Top
        {
            get
            {
                object obj2 = base.ViewState["Top"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                base.ViewState["Top"] = value;
            }
        }
    }
}

