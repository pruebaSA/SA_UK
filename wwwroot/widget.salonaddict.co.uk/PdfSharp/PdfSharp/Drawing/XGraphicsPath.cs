namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public sealed class XGraphicsPath
    {
        private XFillMode fillMode;
        internal GraphicsPath gdipPath;

        public XGraphicsPath()
        {
            this.gdipPath = new GraphicsPath();
        }

        public XGraphicsPath(PointF[] points, byte[] types, XFillMode fillMode)
        {
            this.gdipPath = new GraphicsPath(points, types, (System.Drawing.Drawing2D.FillMode) fillMode);
        }

        public void AddArc(XRect rect, double startAngle, double sweepAngle)
        {
            this.AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddArc(Rectangle rect, double startAngle, double sweepAngle)
        {
            this.AddArc((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void AddArc(RectangleF rect, double startAngle, double sweepAngle)
        {
            this.AddArc((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void AddArc(double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            this.gdipPath.AddArc((float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
        }

        public void AddArc(int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            this.AddArc((double) x, (double) y, (double) width, (double) height, (double) startAngle, (double) sweepAngle);
        }

        public void AddBezier(XPoint pt1, XPoint pt2, XPoint pt3, XPoint pt4)
        {
            this.AddBezier(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
        {
            this.AddBezier((double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y, (double) pt3.X, (double) pt3.Y, (double) pt4.X, (double) pt4.Y);
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            this.AddBezier((double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y, (double) pt3.X, (double) pt3.Y, (double) pt4.X, (double) pt4.Y);
        }

        public void AddBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            this.gdipPath.AddBezier((float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
        }

        public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            this.AddBezier((double) x1, (double) y1, (double) x2, (double) y2, (double) x3, (double) y3, (double) x4, (double) y4);
        }

        public void AddBeziers(XPoint[] points)
        {
            if (points == null)
            {
                new ArgumentNullException("points");
            }
            if (points.Length < 4)
            {
                throw new ArgumentException("At least four points required for bezier curve.", "points");
            }
            if (((points.Length - 1) % 3) != 0)
            {
                throw new ArgumentException("Invalid number of points for bezier curve. Number must fulfil 4+3n.", "points");
            }
            this.gdipPath.AddBeziers(XGraphics.MakePointFArray(points));
        }

        public void AddBeziers(Point[] points)
        {
            this.AddBeziers(XGraphics.MakeXPointArray(points));
        }

        public void AddBeziers(PointF[] points)
        {
            this.AddBeziers(XGraphics.MakeXPointArray(points));
        }

        public void AddClosedCurve(XPoint[] points)
        {
            this.AddClosedCurve(points, 0.5);
        }

        public void AddClosedCurve(Point[] points)
        {
            this.AddClosedCurve(XGraphics.MakeXPointArray(points), 0.5);
        }

        public void AddClosedCurve(PointF[] points)
        {
            this.AddClosedCurve(XGraphics.MakeXPointArray(points), 0.5);
        }

        public void AddClosedCurve(XPoint[] points, double tension)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            int length = points.Length;
            if (length != 0)
            {
                if (length < 2)
                {
                    throw new ArgumentException("Not enough points.", "points");
                }
                this.gdipPath.AddClosedCurve(XGraphics.MakePointFArray(points), (float) tension);
            }
        }

        public void AddClosedCurve(Point[] points, double tension)
        {
            this.AddClosedCurve(XGraphics.MakeXPointArray(points), tension);
        }

        public void AddClosedCurve(PointF[] points, double tension)
        {
            this.AddClosedCurve(XGraphics.MakeXPointArray(points), tension);
        }

        public void AddCurve(XPoint[] points)
        {
            this.AddCurve(points, 0.5);
        }

        public void AddCurve(Point[] points)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points));
        }

        public void AddCurve(PointF[] points)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points));
        }

        public void AddCurve(XPoint[] points, double tension)
        {
            if (points.Length < 2)
            {
                throw new ArgumentException("AddCurve requires two or more points.", "points");
            }
            this.gdipPath.AddCurve(XGraphics.MakePointFArray(points), (float) tension);
        }

        public void AddCurve(Point[] points, double tension)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points), tension);
        }

        public void AddCurve(PointF[] points, double tension)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points), tension);
        }

        public void AddCurve(XPoint[] points, int offset, int numberOfSegments, double tension)
        {
            this.gdipPath.AddCurve(XGraphics.MakePointFArray(points), offset, numberOfSegments, (float) tension);
        }

        public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points), offset, numberOfSegments, (double) tension);
        }

        public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
        {
            this.AddCurve(XGraphics.MakeXPointArray(points), offset, numberOfSegments, (double) tension);
        }

        public void AddEllipse(XRect rect)
        {
            this.AddEllipse(rect.x, rect.y, rect.width, rect.height);
        }

        public void AddEllipse(Rectangle rect)
        {
            this.AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void AddEllipse(RectangleF rect)
        {
            this.AddEllipse((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void AddEllipse(double x, double y, double width, double height)
        {
            this.gdipPath.AddEllipse((float) x, (float) y, (float) width, (float) height);
        }

        public void AddEllipse(int x, int y, int width, int height)
        {
            this.AddEllipse((double) x, (double) y, (double) width, (double) height);
        }

        public void AddLine(XPoint pt1, XPoint pt2)
        {
            this.AddLine(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void AddLine(Point pt1, Point pt2)
        {
            this.AddLine((double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y);
        }

        public void AddLine(PointF pt1, PointF pt2)
        {
            this.AddLine((double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y);
        }

        public void AddLine(double x1, double y1, double x2, double y2)
        {
            this.gdipPath.AddLine((float) x1, (float) y1, (float) x2, (float) y2);
        }

        public void AddLine(int x1, int y1, int x2, int y2)
        {
            this.AddLine((double) x1, (double) y1, (double) x2, (double) y2);
        }

        public void AddLines(XPoint[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Length != 0)
            {
                this.gdipPath.AddLines(XGraphics.MakePointFArray(points));
            }
        }

        public void AddLines(Point[] points)
        {
            this.AddLines(XGraphics.MakeXPointArray(points));
        }

        public void AddLines(PointF[] points)
        {
            this.AddLines(XGraphics.MakeXPointArray(points));
        }

        public void AddPath(XGraphicsPath path, bool connect)
        {
            this.gdipPath.AddPath(path.gdipPath, connect);
        }

        public void AddPie(XRect rect, double startAngle, double sweepAngle)
        {
            this.AddPie(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void AddPie(Rectangle rect, double startAngle, double sweepAngle)
        {
            this.gdipPath.AddPie(rect, (float) startAngle, (float) sweepAngle);
        }

        public void AddPie(RectangleF rect, double startAngle, double sweepAngle)
        {
            this.AddPie((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void AddPie(double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            this.gdipPath.AddPie((float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
        }

        public void AddPie(int x, int y, int width, int height, double startAngle, double sweepAngle)
        {
            this.AddPie((double) x, (double) y, (double) width, (double) height, startAngle, sweepAngle);
        }

        public void AddPolygon(XPoint[] points)
        {
            this.gdipPath.AddPolygon(XGraphics.MakePointFArray(points));
        }

        public void AddPolygon(Point[] points)
        {
            this.gdipPath.AddPolygon(points);
        }

        public void AddPolygon(PointF[] points)
        {
            this.gdipPath.AddPolygon(points);
        }

        public void AddRectangle(XRect rect)
        {
            this.gdipPath.AddRectangle(rect.ToRectangleF());
        }

        public void AddRectangle(Rectangle rect)
        {
            this.AddRectangle(new XRect(rect));
        }

        public void AddRectangle(RectangleF rect)
        {
            this.AddRectangle(new XRect(rect));
        }

        public void AddRectangle(double x, double y, double width, double height)
        {
            this.AddRectangle(new XRect(x, y, width, height));
        }

        public void AddRectangle(int x, int y, int width, int height)
        {
            this.AddRectangle(new XRect((double) x, (double) y, (double) width, (double) height));
        }

        public void AddRectangles(XRect[] rects)
        {
            int length = rects.Length;
            for (int i = 0; i < length; i++)
            {
                this.gdipPath.AddRectangle(rects[i].ToRectangleF());
            }
        }

        public void AddRectangles(Rectangle[] rects)
        {
            int length = rects.Length;
            for (int i = 0; i < length; i++)
            {
                this.AddRectangle(rects[i]);
            }
            this.gdipPath.AddRectangles(rects);
        }

        public void AddRectangles(RectangleF[] rects)
        {
            int length = rects.Length;
            for (int i = 0; i < length; i++)
            {
                this.AddRectangle(rects[i]);
            }
            this.gdipPath.AddRectangles(rects);
        }

        public void AddRoundedRectangle(XRect rect, SizeF ellipseSize)
        {
            this.AddRoundedRectangle(rect.X, rect.Y, rect.Width, rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void AddRoundedRectangle(Rectangle rect, Size ellipseSize)
        {
            this.AddRoundedRectangle((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void AddRoundedRectangle(RectangleF rect, SizeF ellipseSize)
        {
            this.AddRoundedRectangle((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void AddRoundedRectangle(double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            this.gdipPath.AddArc((float) ((x + width) - ellipseWidth), (float) y, (float) ellipseWidth, (float) ellipseHeight, -90f, 90f);
            this.gdipPath.AddArc((float) ((x + width) - ellipseWidth), (float) ((y + height) - ellipseHeight), (float) ellipseWidth, (float) ellipseHeight, 0f, 90f);
            this.gdipPath.AddArc((float) x, (float) ((y + height) - ellipseHeight), (float) ellipseWidth, (float) ellipseHeight, 90f, 90f);
            this.gdipPath.AddArc((float) x, (float) y, (float) ellipseWidth, (float) ellipseHeight, 180f, 90f);
            this.gdipPath.CloseFigure();
        }

        public void AddRoundedRectangle(int x, int y, int width, int height, int ellipseWidth, int ellipseHeight)
        {
            this.AddRoundedRectangle((double) x, (double) y, (double) width, (double) height, (double) ellipseWidth, (double) ellipseHeight);
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, XPoint origin, XStringFormat format)
        {
            try
            {
                this.gdipPath.AddString(s, family.gdiFamily, (int) style, (float) emSize, origin.ToPointF(), format.RealizeGdiStringFormat());
            }
            catch
            {
            }
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, XRect layoutRect, XStringFormat format)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (family == null)
            {
                throw new ArgumentNullException("family");
            }
            if ((format.LineAlignment == XLineAlignment.BaseLine) && (layoutRect.Height != 0.0))
            {
                throw new InvalidOperationException("DrawString: With XLineAlignment.BaseLine the height of the layout rectangle must be 0.");
            }
            if (s.Length != 0)
            {
                if (format == null)
                {
                    format = XStringFormats.Default;
                }
                XFont font = new XFont(family.Name, emSize, style);
                RectangleF ef = layoutRect.ToRectangleF();
                if (format.LineAlignment == XLineAlignment.BaseLine)
                {
                    double height = font.GetHeight();
                    int lineSpacing = font.FontFamily.GetLineSpacing(font.Style);
                    int cellAscent = font.FontFamily.GetCellAscent(font.Style);
                    font.FontFamily.GetCellDescent(font.Style);
                    double num4 = (height * cellAscent) / ((double) lineSpacing);
                    num4 = (height * font.cellAscent) / ((double) font.cellSpace);
                    ef.Offset(0f, (float) -num4);
                }
                this.gdipPath.AddString(s, family.gdiFamily, (int) style, (float) emSize, ef, format.RealizeGdiStringFormat());
            }
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, Point origin, XStringFormat format)
        {
            this.AddString(s, family, style, emSize, new XRect((double) origin.X, (double) origin.Y, 0.0, 0.0), format);
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, PointF origin, XStringFormat format)
        {
            this.AddString(s, family, style, emSize, new XRect((double) origin.X, (double) origin.Y, 0.0, 0.0), format);
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, Rectangle layoutRect, XStringFormat format)
        {
            this.gdipPath.AddString(s, family.gdiFamily, (int) style, (float) emSize, layoutRect, format.RealizeGdiStringFormat());
        }

        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, RectangleF layoutRect, XStringFormat format)
        {
            this.gdipPath.AddString(s, family.gdiFamily, (int) style, (float) emSize, layoutRect, format.RealizeGdiStringFormat());
        }

        public XGraphicsPath Clone()
        {
            XGraphicsPath path = (XGraphicsPath) base.MemberwiseClone();
            path.gdipPath = this.gdipPath.Clone() as GraphicsPath;
            return path;
        }

        public void CloseFigure()
        {
            this.gdipPath.CloseFigure();
        }

        public void Flatten()
        {
            this.gdipPath.Flatten();
        }

        public void Flatten(XMatrix matrix)
        {
            this.gdipPath.Flatten(matrix.ToGdiMatrix());
        }

        public void Flatten(XMatrix matrix, double flatness)
        {
            this.gdipPath.Flatten(matrix.ToGdiMatrix(), (float) flatness);
        }

        public void StartFigure()
        {
            this.gdipPath.StartFigure();
        }

        public void Widen(XPen pen)
        {
            this.gdipPath.Widen(pen.RealizeGdiPen());
        }

        public void Widen(XPen pen, XMatrix matrix)
        {
            this.gdipPath.Widen(pen.RealizeGdiPen(), matrix.ToGdiMatrix());
        }

        public void Widen(XPen pen, XMatrix matrix, double flatness)
        {
            this.gdipPath.Widen(pen.RealizeGdiPen(), matrix.ToGdiMatrix(), (float) flatness);
        }

        public XFillMode FillMode
        {
            get => 
                this.fillMode;
            set
            {
                this.fillMode = value;
                this.gdipPath.FillMode = (System.Drawing.Drawing2D.FillMode) value;
            }
        }

        public XGraphicsPathInternals Internals =>
            new XGraphicsPathInternals(this);
    }
}

