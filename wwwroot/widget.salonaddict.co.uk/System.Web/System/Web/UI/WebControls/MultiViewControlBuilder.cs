namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MultiViewControlBuilder : ControlBuilder
    {
        public override void AppendSubBuilder(ControlBuilder subBuilder)
        {
            if (subBuilder is CodeBlockBuilder)
            {
                throw new Exception(System.Web.SR.GetString("Multiview_rendering_block_not_allowed"));
            }
            base.AppendSubBuilder(subBuilder);
        }
    }
}

