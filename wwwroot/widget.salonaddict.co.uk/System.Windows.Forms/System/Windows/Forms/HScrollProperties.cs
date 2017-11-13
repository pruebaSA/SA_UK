namespace System.Windows.Forms
{
    using System;

    public class HScrollProperties : ScrollProperties
    {
        public HScrollProperties(ScrollableControl container) : base(container)
        {
        }

        internal override int HorizontalDisplayPosition =>
            -base.value;

        internal override int Orientation =>
            0;

        internal override int PageSize =>
            base.ParentControl.ClientRectangle.Width;

        internal override int VerticalDisplayPosition =>
            base.ParentControl.DisplayRectangle.Y;
    }
}

