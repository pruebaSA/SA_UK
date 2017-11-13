namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources.Design;
    using System.Web.UI.WebControls;

    internal class CustomPagerStyle : DesignerPagerStyle
    {
        public CustomPagerStyle(ISite site) : base(site)
        {
        }

        public override void ApplyStyle(DataPager pager)
        {
        }

        public override string Name =>
            AtlasWebDesign.DesignerPagerStyle_CustomPager;
    }
}

