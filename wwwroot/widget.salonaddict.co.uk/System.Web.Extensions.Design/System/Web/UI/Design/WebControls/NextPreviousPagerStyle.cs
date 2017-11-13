namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources.Design;
    using System.Web.UI.WebControls;

    internal class NextPreviousPagerStyle : DesignerPagerStyle
    {
        public NextPreviousPagerStyle(ISite site) : base(site)
        {
        }

        public override void ApplyStyle(DataPager pager)
        {
            pager.Fields.Clear();
            NextPreviousPagerField field = new NextPreviousPagerField {
                ButtonType = ButtonType.Button,
                ShowFirstPageButton = true,
                ShowNextPageButton = true,
                ShowPreviousPageButton = true,
                ShowLastPageButton = true
            };
            pager.Fields.Add(field);
        }

        public override string Name =>
            AtlasWebDesign.DesignerPagerStyle_NextPrevPager;
    }
}

