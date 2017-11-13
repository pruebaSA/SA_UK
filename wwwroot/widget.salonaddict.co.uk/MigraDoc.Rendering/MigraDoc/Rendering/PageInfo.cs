namespace MigraDoc.Rendering
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using System;

    public class PageInfo
    {
        private XUnit height;
        private PageOrientation orientation;
        private XUnit width;

        internal PageInfo(XUnit width, XUnit height, PageOrientation orientation)
        {
            this.width = width;
            this.height = height;
            this.orientation = orientation;
        }

        public XUnit Height =>
            this.height;

        public PageOrientation Orientation =>
            this.orientation;

        public XUnit Width =>
            this.width;
    }
}

