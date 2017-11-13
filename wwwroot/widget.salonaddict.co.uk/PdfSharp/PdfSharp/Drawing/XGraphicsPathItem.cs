namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;

    internal sealed class XGraphicsPathItem
    {
        public XPoint[] points;
        public XGraphicsPathItemType type;

        public XGraphicsPathItem(XGraphicsPathItemType type)
        {
            this.type = type;
            this.points = null;
        }

        public XGraphicsPathItem(XGraphicsPathItemType type, params XPoint[] points)
        {
            this.type = type;
            this.points = (XPoint[]) points.Clone();
        }

        public XGraphicsPathItem(XGraphicsPathItemType type, params PointF[] points)
        {
            this.type = type;
            this.points = XGraphics.MakeXPointArray(points);
        }

        public XGraphicsPathItem Clone()
        {
            XGraphicsPathItem item = base.MemberwiseClone() as XGraphicsPathItem;
            item.points = this.points.Clone() as XPoint[];
            return item;
        }
    }
}

