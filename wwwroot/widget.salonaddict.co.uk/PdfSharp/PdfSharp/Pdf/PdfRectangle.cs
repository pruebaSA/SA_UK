namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Drawing;

    [DebuggerDisplay("X1={X1.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}, X2={X2.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}, Y1={Y1.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}, Y2={Y2.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}")]
    public sealed class PdfRectangle : PdfItem
    {
        public static readonly PdfRectangle Empty = new PdfRectangle();
        private double x1;
        private double x2;
        private double y1;
        private double y2;

        public PdfRectangle()
        {
        }

        public PdfRectangle(XRect rect)
        {
            this.x1 = rect.x;
            this.y1 = rect.y;
            this.x2 = rect.x + rect.width;
            this.y2 = rect.y + rect.height;
        }

        internal PdfRectangle(PdfItem item)
        {
            if ((item != null) && !(item is PdfNull))
            {
                if (item is PdfReference)
                {
                    item = ((PdfReference) item).Value;
                }
                PdfArray array = item as PdfArray;
                if (array == null)
                {
                    throw new InvalidOperationException(PSSR.UnexpectedTokenInPdfFile);
                }
                this.x1 = array.Elements.GetReal(0);
                this.y1 = array.Elements.GetReal(1);
                this.x2 = array.Elements.GetReal(2);
                this.y2 = array.Elements.GetReal(3);
            }
        }

        public PdfRectangle(XPoint pt1, XPoint pt2)
        {
            this.x1 = pt1.X;
            this.y1 = pt1.Y;
            this.x2 = pt2.X;
            this.y2 = pt2.Y;
        }

        public PdfRectangle(XPoint pt, XSize size)
        {
            this.x1 = pt.X;
            this.y1 = pt.Y;
            this.x2 = pt.X + size.Width;
            this.y2 = pt.Y + size.Height;
        }

        public PdfRectangle(PointF pt1, PointF pt2)
        {
            this.x1 = pt1.X;
            this.y1 = pt1.Y;
            this.x2 = pt2.X;
            this.y2 = pt2.Y;
        }

        public PdfRectangle(PointF pt, SizeF size)
        {
            this.x1 = pt.X;
            this.y1 = pt.Y;
            this.x2 = pt.X + size.Width;
            this.y2 = pt.Y + size.Height;
        }

        internal PdfRectangle(double x1, double y1, double x2, double y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public PdfRectangle Clone() => 
            ((PdfRectangle) this.Copy());

        public bool Contains(XPoint pt) => 
            this.Contains(pt.X, pt.Y);

        public bool Contains(XRect rect) => 
            ((((this.x1 <= rect.X) && ((rect.X + rect.Width) <= this.x2)) && (this.y1 <= rect.Y)) && ((rect.Y + rect.Height) <= this.y2));

        public bool Contains(PdfRectangle rect) => 
            ((((this.x1 <= rect.x1) && (rect.x2 <= this.x2)) && (this.y1 <= rect.y1)) && (rect.y2 <= this.y2));

        public bool Contains(PointF pt) => 
            this.Contains((double) pt.X, (double) pt.Y);

        public bool Contains(RectangleF rect) => 
            ((((this.x1 <= rect.X) && ((rect.X + rect.Width) <= this.x2)) && (this.y1 <= rect.Y)) && ((rect.Y + rect.Height) <= this.y2));

        public bool Contains(double x, double y) => 
            ((((this.x1 <= x) && (x <= this.x2)) && (this.y1 <= y)) && (y <= this.y2));

        protected override object Copy() => 
            ((PdfRectangle) base.Copy());

        public override bool Equals(object obj)
        {
            if (!(obj is PdfRectangle))
            {
                return false;
            }
            PdfRectangle rectangle = (PdfRectangle) obj;
            return ((((rectangle.x1 == this.x1) && (rectangle.y1 == this.y1)) && (rectangle.x2 == this.x2)) && (rectangle.y2 == this.y2));
        }

        public override int GetHashCode() => 
            ((int) (((((uint) this.x1) ^ ((((uint) this.y1) << 13) | (((uint) this.y1) >> 0x13))) ^ ((((uint) this.x2) << 0x1a) | (((uint) this.x2) >> 6))) ^ ((((uint) this.y2) << 7) | (((uint) this.y2) >> 0x19))));

        public static bool operator ==(PdfRectangle left, PdfRectangle right)
        {
            if (left == null)
            {
                return (right == null);
            }
            if (right == null)
            {
                return false;
            }
            return ((((left.x1 == right.x1) && (left.y1 == right.y1)) && (left.x2 == right.x2)) && (left.y2 == right.y2));
        }

        public static bool operator !=(PdfRectangle left, PdfRectangle right) => 
            !(left == right);

        public override string ToString() => 
            PdfEncoders.Format("[{0:0.###} {1:0.###} {2:0.###} {3:0.###}]", new object[] { this.x1, this.y1, this.x2, this.y2 });

        public XRect ToXRect() => 
            new XRect(this.x1, this.y1, this.Width, this.Height);

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public double Height =>
            (this.y2 - this.y1);

        public bool IsEmpty =>
            ((((this.x1 == 0.0) && (this.y1 == 0.0)) && (this.x2 == 0.0)) && (this.y2 == 0.0));

        public XPoint Location =>
            new XPoint(this.x1, this.y1);

        public XSize Size =>
            new XSize(this.x2 - this.x1, this.y2 - this.y1);

        public double Width =>
            (this.x2 - this.x1);

        public double X1 =>
            this.x1;

        public double X2 =>
            this.x2;

        public double Y1 =>
            this.y1;

        public double Y2 =>
            this.y2;
    }
}

