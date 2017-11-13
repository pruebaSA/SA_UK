namespace PdfSharp.Drawing
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public sealed class XLinearGradientBrush : XBrush
    {
        internal XColor color1;
        internal XColor color2;
        internal XLinearGradientMode linearGradientMode;
        internal XMatrix matrix;
        internal XPoint point1;
        internal XPoint point2;
        internal XRect rect;
        internal bool useRect;

        public XLinearGradientBrush(XPoint point1, XPoint point2, XColor color1, XColor color2)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.color1 = color1;
            this.color2 = color2;
        }

        public XLinearGradientBrush(XRect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
        {
            if (!Enum.IsDefined(typeof(XLinearGradientMode), linearGradientMode))
            {
                throw new InvalidEnumArgumentException("linearGradientMode", (int) linearGradientMode, typeof(XLinearGradientMode));
            }
            if ((rect.Width == 0.0) || (rect.Height == 0.0))
            {
                throw new ArgumentException("Invalid rectangle.", "rect");
            }
            this.useRect = true;
            this.color1 = color1;
            this.color2 = color2;
            this.rect = rect;
            this.linearGradientMode = linearGradientMode;
        }

        public XLinearGradientBrush(Point point1, Point point2, XColor color1, XColor color2) : this(new XPoint(point1), new XPoint(point2), color1, color2)
        {
        }

        public XLinearGradientBrush(PointF point1, PointF point2, XColor color1, XColor color2) : this(new XPoint(point1), new XPoint(point2), color1, color2)
        {
        }

        public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode) : this(new XRect(rect), color1, color2, linearGradientMode)
        {
        }

        public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode) : this(new XRect(rect), color1, color2, linearGradientMode)
        {
        }

        public void MultiplyTransform(XMatrix matrix)
        {
            this.matrix.Prepend(matrix);
        }

        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            this.matrix.Multiply(matrix, order);
        }

        internal override Brush RealizeGdiBrush()
        {
            LinearGradientBrush brush;
            if (this.useRect)
            {
                brush = new LinearGradientBrush(this.rect.ToRectangleF(), this.color1.ToGdiColor(), this.color2.ToGdiColor(), (LinearGradientMode) this.linearGradientMode);
            }
            else
            {
                brush = new LinearGradientBrush(this.point1.ToPointF(), this.point2.ToPointF(), this.color1.ToGdiColor(), this.color2.ToGdiColor());
            }
            if (!this.matrix.IsIdentity)
            {
                brush.Transform = this.matrix.ToGdiMatrix();
            }
            return brush;
        }

        public void ResetTransform()
        {
            this.matrix = new XMatrix();
        }

        public void RotateTransform(double angle)
        {
            this.matrix.RotatePrepend(angle);
        }

        public void RotateTransform(double angle, XMatrixOrder order)
        {
            this.matrix.Rotate(angle, order);
        }

        public void ScaleTransform(double sx, double sy)
        {
            this.matrix.ScalePrepend(sx, sy);
        }

        public void ScaleTransform(double sx, double sy, XMatrixOrder order)
        {
            this.matrix.Scale(sx, sy, order);
        }

        public void TranslateTransform(double dx, double dy)
        {
            this.matrix.TranslatePrepend(dx, dy);
        }

        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            this.matrix.Translate(dx, dy, order);
        }

        public XMatrix Transform
        {
            get => 
                this.matrix;
            set
            {
                this.matrix = value;
            }
        }
    }
}

