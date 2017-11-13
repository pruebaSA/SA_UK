namespace System.Windows.Forms
{
    using System;

    public class VScrollProperties : ScrollProperties
    {
        public VScrollProperties(ScrollableControl container) : base(container)
        {
        }

        internal override int HorizontalDisplayPosition =>
            base.ParentControl.DisplayRectangle.X;

        internal override int Orientation =>
            1;

        internal override int PageSize =>
            base.ParentControl.ClientRectangle.Height;

        internal override int VerticalDisplayPosition =>
            -base.value;
    }
}

