namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources.Design;
    using System.Web.UI.WebControls;

    internal class NumericPagerStyle : DesignerPagerStyle
    {
        public NumericPagerStyle(ISite site) : base(site)
        {
        }

        public override void ApplyStyle(DataPager pager)
        {
            pager.Fields.Clear();
            NextPreviousPagerField field = new NextPreviousPagerField {
                ShowFirstPageButton = true,
                ShowNextPageButton = false,
                ShowPreviousPageButton = false,
                ShowLastPageButton = false,
                ButtonType = ButtonType.Button
            };
            pager.Fields.Add(field);
            NumericPagerField field2 = new NumericPagerField {
                ButtonCount = 5
            };
            pager.Fields.Add(field2);
            NextPreviousPagerField field3 = new NextPreviousPagerField {
                ShowFirstPageButton = false,
                ShowNextPageButton = false,
                ShowPreviousPageButton = false,
                ShowLastPageButton = true,
                ButtonType = ButtonType.Button
            };
            pager.Fields.Add(field3);
        }

        public override string Name =>
            AtlasWebDesign.DesignerPagerStyle_NumericPager;
    }
}

