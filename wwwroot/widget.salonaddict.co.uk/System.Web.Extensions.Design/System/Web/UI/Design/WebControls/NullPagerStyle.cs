namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources.Design;
    using System.Web.UI.WebControls;

    internal class NullPagerStyle : DesignerPagerStyle
    {
        public NullPagerStyle(ISite site) : base(site)
        {
        }

        public override void ApplyStyle(DataPager pager)
        {
            pager.Fields.Clear();
        }

        public override DataPager CreatePager() => 
            null;

        public override bool IsPagerType(DataPager pager)
        {
            if ((pager != null) && (pager.Fields.Count != 0))
            {
                return false;
            }
            return true;
        }

        public override string Name =>
            AtlasWebDesign.DesignerPagerStyle_NullPager;
    }
}

