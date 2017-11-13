namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TitleStyle : TableItemStyle
    {
        public TitleStyle()
        {
            this.Wrap = false;
        }

        [DefaultValue(false)]
        public override bool Wrap
        {
            get => 
                base.Wrap;
            set
            {
                base.Wrap = value;
            }
        }
    }
}

