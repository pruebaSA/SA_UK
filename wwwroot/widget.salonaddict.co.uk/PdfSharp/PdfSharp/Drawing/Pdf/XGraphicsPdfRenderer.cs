namespace PdfSharp.Drawing.Pdf
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class XGraphicsPdfRenderer : IXGraphicsRenderer
    {
        private int clipLevel;
        internal PdfColorMode colorMode;
        private StringBuilder content;
        internal XMatrix defaultViewMatrix;
        internal XForm form;
        private XGraphics gfx;
        private PdfGraphicsState gfxState;
        private Stack<PdfGraphicsState> gfxStateStack;
        private const int GraphicsStackLevelInitial = 0;
        private const int GraphicsStackLevelPageSpace = 1;
        private const int GraphicsStackLevelWorldSpace = 2;
        private XGraphicsPdfPageOptions options;
        internal PdfPage page;
        private StreamMode streamMode;

        public XGraphicsPdfRenderer(XForm form, XGraphics gfx)
        {
            this.gfxStateStack = new Stack<PdfGraphicsState>();
            this.form = form;
            this.colorMode = form.Owner.Options.ColorMode;
            this.gfx = gfx;
            this.content = new StringBuilder();
            form.pdfRenderer = this;
            this.gfxState = new PdfGraphicsState(this);
        }

        public XGraphicsPdfRenderer(PdfPage page, XGraphics gfx, XGraphicsPdfPageOptions options)
        {
            this.gfxStateStack = new Stack<PdfGraphicsState>();
            this.page = page;
            this.colorMode = page.document.Options.ColorMode;
            this.options = options;
            this.gfx = gfx;
            this.content = new StringBuilder();
            page.RenderContent.pdfRenderer = this;
            this.gfxState = new PdfGraphicsState(this);
        }

        private void AdjustTextMatrix(ref XPoint pos)
        {
            XPoint point = pos;
            pos -= new XVector(this.gfxState.realizedTextPosition.x, this.gfxState.realizedTextPosition.y);
            this.gfxState.realizedTextPosition = point;
        }

        internal void Append(string value)
        {
            this.content.Append(value);
        }

        private void AppendCurveSegment(XPoint pt0, XPoint pt1, XPoint pt2, XPoint pt3, double tension3)
        {
            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { pt1.X + (tension3 * (pt2.X - pt0.X)), pt1.Y + (tension3 * (pt2.Y - pt0.Y)), pt2.X - (tension3 * (pt3.X - pt1.X)), pt2.Y - (tension3 * (pt3.Y - pt1.Y)), pt2.X, pt2.Y });
        }

        internal void AppendFormat(string format, params object[] args)
        {
            this.content.AppendFormat(CultureInfo.InvariantCulture, format, args);
        }

        private void AppendPartialArc(double x, double y, double width, double height, double startAngle, double sweepAngle, PathStart pathStart, XMatrix matrix)
        {
            double num = startAngle;
            if (num < 0.0)
            {
                num += (1.0 + Math.Floor((double) (Math.Abs(num) / 360.0))) * 360.0;
            }
            else if (num > 360.0)
            {
                num -= Math.Floor((double) (num / 360.0)) * 360.0;
            }
            double num2 = sweepAngle;
            if (num2 < -360.0)
            {
                num2 = -360.0;
            }
            else if (num2 > 360.0)
            {
                num2 = 360.0;
            }
            if ((num == 0.0) && (num2 < 0.0))
            {
                num = 360.0;
            }
            else if ((num == 360.0) && (num2 > 0.0))
            {
                num = 0.0;
            }
            bool flag = Math.Abs(num2) <= 90.0;
            num2 = num + num2;
            if (num2 < 0.0)
            {
                num2 += (1.0 + Math.Floor((double) (Math.Abs(num2) / 360.0))) * 360.0;
            }
            bool clockwise = sweepAngle > 0.0;
            int num3 = this.Quatrant(num, true, clockwise);
            int num4 = this.Quatrant(num2, false, clockwise);
            if ((num3 == num4) && flag)
            {
                this.AppendPartialArcQuadrant(x, y, width, height, num, num2, pathStart, matrix);
                return;
            }
            int num5 = num3;
            bool flag3 = true;
            while (true)
            {
                if ((num5 == num3) && flag3)
                {
                    double num6 = (num5 * 90) + (clockwise ? 90 : 0);
                    this.AppendPartialArcQuadrant(x, y, width, height, num, num6, pathStart, matrix);
                }
                else if (num5 == num4)
                {
                    double num7 = (num5 * 90) + (clockwise ? 0 : 90);
                    this.AppendPartialArcQuadrant(x, y, width, height, num7, num2, PathStart.Ignore1st, matrix);
                }
                else
                {
                    double num8 = (num5 * 90) + (clockwise ? 0 : 90);
                    double num9 = (num5 * 90) + (clockwise ? 90 : 0);
                    this.AppendPartialArcQuadrant(x, y, width, height, num8, num9, PathStart.Ignore1st, matrix);
                }
                if ((num5 == num4) && flag)
                {
                    return;
                }
                flag = true;
                if (clockwise)
                {
                    num5 = (num5 == 3) ? 0 : (num5 + 1);
                }
                else
                {
                    num5 = (num5 == 0) ? 3 : (num5 - 1);
                }
                flag3 = false;
            }
        }

        private void AppendPartialArcQuadrant(double x, double y, double width, double height, double α, double β, PathStart pathStart, XMatrix matrix)
        {
            double num7;
            double num8;
            XPoint point;
            XPoint point2;
            XPoint point3;
            if (β > 360.0)
            {
                β -= Math.Floor((double) (β / 360.0)) * 360.0;
            }
            double num = width / 2.0;
            double num2 = height / 2.0;
            double num3 = x + num;
            double num4 = y + num2;
            bool flag = false;
            if ((α >= 180.0) && (β >= 180.0))
            {
                α -= 180.0;
                β -= 180.0;
                flag = true;
            }
            if (width == height)
            {
                α *= 0.017453292519943295;
                β *= 0.017453292519943295;
            }
            else
            {
                α *= 0.017453292519943295;
                num7 = Math.Sin(α);
                if (Math.Abs(num7) > 1E-10)
                {
                    α = 1.5707963267948966 - Math.Atan((num2 * Math.Cos(α)) / (num * num7));
                }
                β *= 0.017453292519943295;
                num8 = Math.Sin(β);
                if (Math.Abs(num8) > 1E-10)
                {
                    β = 1.5707963267948966 - Math.Atan((num2 * Math.Cos(β)) / (num * num8));
                }
            }
            double num9 = (4.0 * (1.0 - Math.Cos((α - β) / 2.0))) / (3.0 * Math.Sin((β - α) / 2.0));
            num7 = Math.Sin(α);
            double num5 = Math.Cos(α);
            num8 = Math.Sin(β);
            double num6 = Math.Cos(β);
            if (flag)
            {
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        point = matrix.Transform(new XPoint(num3 - (num * num5), num4 - (num2 * num7)));
                        this.AppendFormat("{0:0.###} {1:0.###} m\n", new object[] { point.x, point.y });
                        break;

                    case PathStart.LineTo1st:
                        point = matrix.Transform(new XPoint(num3 - (num * num5), num4 - (num2 * num7)));
                        this.AppendFormat("{0:0.###} {1:0.###} l\n", new object[] { point.x, point.y });
                        break;
                }
            }
            else
            {
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        point = matrix.Transform(new XPoint(num3 + (num * num5), num4 + (num2 * num7)));
                        this.AppendFormat("{0:0.###} {1:0.###} m\n", new object[] { point.x, point.y });
                        break;

                    case PathStart.LineTo1st:
                        point = matrix.Transform(new XPoint(num3 + (num * num5), num4 + (num2 * num7)));
                        this.AppendFormat("{0:0.###} {1:0.###} l\n", new object[] { point.x, point.y });
                        break;
                }
                point = matrix.Transform(new XPoint(num3 + (num * (num5 - (num9 * num7))), num4 + (num2 * (num7 + (num9 * num5)))));
                point2 = matrix.Transform(new XPoint(num3 + (num * (num6 + (num9 * num8))), num4 + (num2 * (num8 - (num9 * num6)))));
                point3 = matrix.Transform(new XPoint(num3 + (num * num6), num4 + (num2 * num8)));
                this.AppendFormat("{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###} c\n", new object[] { point.x, point.y, point2.x, point2.y, point3.x, point3.y });
                return;
            }
            point = matrix.Transform(new XPoint(num3 - (num * (num5 - (num9 * num7))), num4 - (num2 * (num7 + (num9 * num5)))));
            point2 = matrix.Transform(new XPoint(num3 - (num * (num6 + (num9 * num8))), num4 - (num2 * (num8 - (num9 * num6)))));
            point3 = matrix.Transform(new XPoint(num3 - (num * num6), num4 - (num2 * num8)));
            this.AppendFormat("{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###} c\n", new object[] { point.x, point.y, point2.x, point2.y, point3.x, point3.y });
        }

        internal void AppendPath(GraphicsPath path)
        {
            int pointCount = path.PointCount;
            if (pointCount != 0)
            {
                PointF[] pathPoints = path.PathPoints;
                byte[] pathTypes = path.PathTypes;
                for (int i = 0; i < pointCount; i++)
                {
                    byte num3 = pathTypes[i];
                    switch ((num3 & 7))
                    {
                        case 0:
                            this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { pathPoints[i].X, pathPoints[i].Y });
                            break;

                        case 1:
                            this.AppendFormat("{0:0.####} {1:0.####} l\n", new object[] { pathPoints[i].X, pathPoints[i].Y });
                            if ((num3 & 0x80) != 0)
                            {
                                this.Append("h\n");
                            }
                            break;

                        case 3:
                            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { pathPoints[i].X, pathPoints[i].Y, pathPoints[++i].X, pathPoints[i].Y, pathPoints[++i].X, pathPoints[i].Y });
                            if ((pathTypes[i] & 0x80) != 0)
                            {
                                this.Append("h\n");
                            }
                            break;
                    }
                }
            }
        }

        private void AppendStrokeFill(XPen pen, XBrush brush, XFillMode fillMode, bool closePath)
        {
            if (closePath)
            {
                this.content.Append("h ");
            }
            if (fillMode == XFillMode.Winding)
            {
                if ((pen != null) && (brush != null))
                {
                    this.content.Append("B\n");
                }
                else if (pen != null)
                {
                    this.content.Append("S\n");
                }
                else
                {
                    this.content.Append("f\n");
                }
            }
            else if ((pen != null) && (brush != null))
            {
                this.content.Append("B*\n");
            }
            else if (pen != null)
            {
                this.content.Append("S\n");
            }
            else
            {
                this.content.Append("f*\n");
            }
        }

        public void BeginContainer(XGraphicsContainer container, XRect dstrect, XRect srcrect, XGraphicsUnit unit)
        {
            this.BeginGraphic();
            this.RealizeTransform();
            this.gfxState.InternalState = container.InternalState;
            this.SaveState();
        }

        internal void BeginGraphic()
        {
            if (this.streamMode != StreamMode.Graphic)
            {
                if (this.streamMode == StreamMode.Text)
                {
                    this.content.Append("ET\n");
                }
                this.streamMode = StreamMode.Graphic;
            }
        }

        private void BeginPage()
        {
            if (this.gfxState.Level == 0)
            {
                this.defaultViewMatrix = new XMatrix();
                if (this.gfx.PageDirection != XPageDirection.Downwards)
                {
                    switch (this.gfx.PageUnit)
                    {
                        case XGraphicsUnit.Inch:
                            this.defaultViewMatrix.ScalePrepend(72.0);
                            break;

                        case XGraphicsUnit.Millimeter:
                            this.defaultViewMatrix.ScalePrepend(2.8346456692913389);
                            break;

                        case XGraphicsUnit.Centimeter:
                            this.defaultViewMatrix.ScalePrepend(28.346456692913385);
                            break;
                    }
                    this.SaveState();
                    double[] elements = this.defaultViewMatrix.GetElements();
                    this.AppendFormat("{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###} cm ", new object[] { elements[0], elements[1], elements[2], elements[3], elements[4], elements[5] });
                }
                else
                {
                    double height = this.Size.Height;
                    XPoint point = new XPoint();
                    if ((this.page != null) && this.page.TrimMargins.AreSet)
                    {
                        height += this.page.TrimMargins.Top.Point + this.page.TrimMargins.Bottom.Point;
                        point = new XPoint(this.page.TrimMargins.Left.Point, this.page.TrimMargins.Top.Point);
                    }
                    if ((this.page != null) && (this.page.Elements.GetInteger("/Rotate") == 90))
                    {
                        this.defaultViewMatrix.RotatePrepend(90.0);
                        this.defaultViewMatrix.ScalePrepend(1.0, -1.0);
                    }
                    else
                    {
                        this.defaultViewMatrix.TranslatePrepend(0.0, height);
                        this.defaultViewMatrix.Scale(1.0, -1.0, XMatrixOrder.Prepend);
                    }
                    switch (this.gfx.PageUnit)
                    {
                        case XGraphicsUnit.Inch:
                            this.defaultViewMatrix.ScalePrepend(72.0);
                            break;

                        case XGraphicsUnit.Millimeter:
                            this.defaultViewMatrix.ScalePrepend(2.8346456692913389);
                            break;

                        case XGraphicsUnit.Centimeter:
                            this.defaultViewMatrix.ScalePrepend(28.346456692913385);
                            break;
                    }
                    if (point != new XPoint())
                    {
                        this.defaultViewMatrix.TranslatePrepend(point.x, point.y);
                    }
                    this.SaveState();
                    double[] numArray = this.defaultViewMatrix.GetElements();
                    this.AppendFormat("{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###} cm ", new object[] { numArray[0], numArray[1], numArray[2], numArray[3], numArray[4], numArray[5] });
                    this.AppendFormat("-100 Tz\n", new object[0]);
                }
            }
        }

        public void Clear(XColor color)
        {
            if (!this.gfx.transform.IsIdentity)
            {
                throw new NotImplementedException("Transform must be identity to clear the canvas.");
            }
            XBrush brush = new XSolidBrush(color);
            this.DrawRectangle(null, brush, 0.0, 0.0, this.Size.Width, this.Size.Height);
        }

        public void Close()
        {
            if (this.page != null)
            {
                this.page.RenderContent.CreateStream(PdfEncoders.RawEncoding.GetBytes(this.GetContent()));
                this.gfx = null;
                this.page.RenderContent.pdfRenderer = null;
                this.page.RenderContent = null;
                this.page = null;
            }
            else if (this.form != null)
            {
                this.form.pdfForm.CreateStream(PdfEncoders.RawEncoding.GetBytes(this.GetContent()));
                this.gfx = null;
                this.form.pdfRenderer = null;
                this.form = null;
            }
        }

        public void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            this.Realize(pen);
            this.AppendPartialArc(x, y, width, height, startAngle, sweepAngle, PathStart.MoveTo1st, new XMatrix());
            this.AppendStrokeFill(pen, null, XFillMode.Alternate, false);
        }

        public void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            XPoint[] points = new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2), new XPoint(x3, y3), new XPoint(x4, y4) };
            this.DrawBeziers(pen, points);
        }

        public void DrawBeziers(XPen pen, XPoint[] points)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            int length = points.Length;
            if (length != 0)
            {
                if (((length - 1) % 3) != 0)
                {
                    throw new ArgumentException("Invalid number of points for bezier curves. Number must fulfil 4+3n.", "points");
                }
                this.Realize(pen);
                this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { points[0].X, points[0].Y });
                for (int i = 1; i < length; i += 3)
                {
                    this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, points[i + 2].X, points[i + 2].Y });
                }
                this.AppendStrokeFill(pen, null, XFillMode.Alternate, false);
            }
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, double tension, XFillMode fillmode)
        {
            int length = points.Length;
            if (length != 0)
            {
                if (length < 2)
                {
                    throw new ArgumentException("Not enough points", "points");
                }
                tension /= 3.0;
                this.Realize(pen, brush);
                this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { points[0].X, points[0].Y });
                if (length == 2)
                {
                    this.AppendCurveSegment(points[0], points[0], points[1], points[1], tension);
                }
                else
                {
                    this.AppendCurveSegment(points[length - 1], points[0], points[1], points[2], tension);
                    for (int i = 1; i < (length - 2); i++)
                    {
                        this.AppendCurveSegment(points[i - 1], points[i], points[i + 1], points[i + 2], tension);
                    }
                    this.AppendCurveSegment(points[length - 3], points[length - 2], points[length - 1], points[0], tension);
                    this.AppendCurveSegment(points[length - 2], points[length - 1], points[0], points[1], tension);
                }
                this.AppendStrokeFill(pen, brush, fillmode, true);
            }
        }

        public void DrawCurve(XPen pen, XPoint[] points, double tension)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            int length = points.Length;
            if (length != 0)
            {
                if (length < 2)
                {
                    throw new ArgumentException("Not enough points", "points");
                }
                tension /= 3.0;
                this.Realize(pen);
                this.AppendFormat("{0:0.###} {1:0.###} m\n", new object[] { points[0].X, points[0].Y });
                if (length == 2)
                {
                    this.AppendCurveSegment(points[0], points[0], points[1], points[1], tension);
                }
                else
                {
                    this.AppendCurveSegment(points[0], points[0], points[1], points[2], tension);
                    for (int i = 1; i < (length - 2); i++)
                    {
                        this.AppendCurveSegment(points[i - 1], points[i], points[i + 1], points[i + 2], tension);
                    }
                    this.AppendCurveSegment(points[length - 3], points[length - 2], points[length - 1], points[length - 1], tension);
                }
                this.AppendStrokeFill(pen, null, XFillMode.Alternate, false);
            }
        }

        public void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            this.Realize(pen, brush);
            XRect rect = new XRect(x, y, width, height);
            double num = rect.Width / 2.0;
            double num2 = rect.Height / 2.0;
            double num3 = num * 0.5522847498;
            double num4 = num2 * 0.5522847498;
            double num5 = rect.X + num;
            double num6 = rect.Y + num2;
            this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { num5 + num, num6 });
            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { num5 + num, num6 + num4, num5 + num3, num6 + num2, num5, num6 + num2 });
            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { num5 - num3, num6 + num2, num5 - num, num6 + num4, num5 - num, num6 });
            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { num5 - num, num6 - num4, num5 - num3, num6 - num2, num5, num6 - num2 });
            this.AppendFormat("{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n", new object[] { num5 + num3, num6 - num2, num5 + num, num6 - num4, num5 + num, num6 });
            this.AppendStrokeFill(pen, brush, XFillMode.Winding, true);
        }

        public void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit)
        {
            double x = destRect.X;
            double y = destRect.Y;
            double width = destRect.Width;
            double height = destRect.Height;
            string str = this.Realize(image);
            if (!(image is XForm))
            {
                if (this.gfx.PageDirection == XPageDirection.Downwards)
                {
                    this.AppendFormat("q {2:0.####} 0 0 -{3:0.####} {0:0.####} {4:0.####} cm {5} Do\nQ\n", new object[] { x, y, width, height, y + height, str });
                }
                else
                {
                    this.AppendFormat("q {2:0.####} 0 0 {3:0.####} {0:0.####} {1:0.####} cm {4} Do Q\n", new object[] { x, y, width, height, str });
                }
            }
            else
            {
                this.BeginPage();
                XForm form = (XForm) image;
                form.Finish();
                this.Owner.FormTable.GetForm(form);
                double num5 = width / image.PointWidth;
                double num6 = height / image.PointHeight;
                if ((num5 != 0.0) && (num6 != 0.0))
                {
                    if (this.gfx.PageDirection == XPageDirection.Downwards)
                    {
                        this.AppendFormat("q {2:0.####} 0 0 -{3:0.####} {0:0.####} {4:0.####} cm 100 Tz {5} Do Q\n", new object[] { x, y, num5, num6, y + height, str });
                    }
                    else
                    {
                        this.AppendFormat("q {2:0.####} 0 0 {3:0.####} {0:0.####} {1:0.####} cm {4} Do Q\n", new object[] { x, y, num5, num6, str });
                    }
                }
            }
        }

        public void DrawImage(XImage image, double x, double y, double width, double height)
        {
            string str = this.Realize(image);
            if (!(image is XForm))
            {
                if (this.gfx.PageDirection == XPageDirection.Downwards)
                {
                    this.AppendFormat("q {2:0.####} 0 0 -{3:0.####} {0:0.####} {4:0.####} cm {5} Do Q\n", new object[] { x, y, width, height, y + height, str });
                }
                else
                {
                    this.AppendFormat("q {2:0.####} 0 0 {3:0.####} {0:0.####} {1:0.####} cm {4} Do Q\n", new object[] { x, y, width, height, str });
                }
            }
            else
            {
                this.BeginPage();
                XForm form = (XForm) image;
                form.Finish();
                this.Owner.FormTable.GetForm(form);
                double num = width / image.PointWidth;
                double num2 = height / image.PointHeight;
                if ((num != 0.0) && (num2 != 0.0))
                {
                    if (this.gfx.PageDirection == XPageDirection.Downwards)
                    {
                        this.AppendFormat("q {2:0.####} 0 0 -{3:0.####} {0:0.####} {4:0.####} cm 100 Tz {5} Do Q\n", new object[] { x, y, num, num2, y + height, str });
                    }
                    else
                    {
                        this.AppendFormat("q {2:0.####} 0 0 {3:0.####} {0:0.####} {1:0.####} cm {4} Do Q\n", new object[] { x, y, num, num2, str });
                    }
                }
            }
        }

        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            XPoint[] points = new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2) };
            this.DrawLines(pen, points);
        }

        public void DrawLines(XPen pen, XPoint[] points)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            int length = points.Length;
            if (length != 0)
            {
                this.Realize(pen);
                this.AppendFormat("{0:0.###} {1:0.###} m\n", new object[] { points[0].X, points[0].Y });
                for (int i = 1; i < length; i++)
                {
                    this.AppendFormat("{0:0.###} {1:0.###} l\n", new object[] { points[i].X, points[i].Y });
                }
                this.content.Append("S\n");
            }
        }

        public void DrawPath(XPen pen, XBrush brush, XGraphicsPath path)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen");
            }
            this.Realize(pen, brush);
            this.AppendPath(path.gdipPath);
            this.AppendStrokeFill(pen, brush, path.FillMode, false);
        }

        public void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            this.Realize(pen, brush);
            this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { x + (width / 2.0), y + (height / 2.0) });
            this.AppendPartialArc(x, y, width, height, startAngle, sweepAngle, PathStart.LineTo1st, new XMatrix());
            this.AppendStrokeFill(pen, brush, XFillMode.Alternate, true);
        }

        public void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            this.Realize(pen, brush);
            int length = points.Length;
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            this.AppendFormat("{0:0.####} {1:0.####} m\n", new object[] { points[0].X, points[0].Y });
            for (int i = 1; i < length; i++)
            {
                this.AppendFormat("{0:0.####} {1:0.####} l\n", new object[] { points[i].X, points[i].Y });
            }
            this.AppendStrokeFill(pen, brush, fillmode, true);
        }

        public void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush");
            }
            this.Realize(pen, brush);
            this.AppendFormat("{0:0.###} {1:0.###} {2:0.###} {3:0.###} re\n", new object[] { x, y, width, height });
            if ((pen != null) && (brush != null))
            {
                this.content.Append("B\n");
            }
            else if (pen != null)
            {
                this.content.Append("S\n");
            }
            else
            {
                this.content.Append("f\n");
            }
        }

        public void DrawRectangles(XPen pen, XBrush brush, XRect[] rects)
        {
            int length = rects.Length;
            for (int i = 0; i < length; i++)
            {
                XRect rect = rects[i];
                this.DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRoundedRectangle(x, y, width, height, ellipseWidth, ellipseHeight);
            this.DrawPath(pen, brush, path);
        }

        public void DrawString(string s, XFont font, XBrush brush, XRect rect, XStringFormat format)
        {
            this.Realize(font, brush, 0);
            double x = rect.X;
            double y = rect.Y;
            double height = font.GetHeight(this.gfx);
            double num4 = (height * font.cellAscent) / ((double) font.cellSpace);
            double num5 = (height * font.cellDescent) / ((double) font.cellSpace);
            double width = this.gfx.MeasureString(s, font).Width;
            bool flag = (font.Style & XFontStyle.Bold) != XFontStyle.Regular;
            bool flag2 = (font.Style & XFontStyle.Italic) != XFontStyle.Regular;
            bool flag3 = (font.Style & XFontStyle.Strikeout) != XFontStyle.Regular;
            bool flag4 = (font.Style & XFontStyle.Underline) != XFontStyle.Regular;
            switch (format.Alignment)
            {
                case XStringAlignment.Center:
                    x += (rect.Width - width) / 2.0;
                    break;

                case XStringAlignment.Far:
                    x += rect.Width - width;
                    break;
            }
            if (this.Gfx.PageDirection == XPageDirection.Downwards)
            {
                switch (format.LineAlignment)
                {
                    case XLineAlignment.Near:
                        y += num4;
                        break;

                    case XLineAlignment.Center:
                        y += (((num4 * 3.0) / 4.0) / 2.0) + (rect.Height / 2.0);
                        break;

                    case XLineAlignment.Far:
                        y += -num5 + rect.Height;
                        break;
                }
            }
            else
            {
                switch (format.LineAlignment)
                {
                    case XLineAlignment.Near:
                        y += num5;
                        break;

                    case XLineAlignment.Center:
                        y += (-((num4 * 3.0) / 4.0) / 2.0) + (rect.Height / 2.0);
                        break;

                    case XLineAlignment.Far:
                        y += -num4 + rect.Height;
                        break;
                }
            }
            PdfFont realizedFont = this.gfxState.realizedFont;
            realizedFont.AddChars(s);
            OpenTypeDescriptor descriptor = realizedFont.FontDescriptor.descriptor;
            if (flag)
            {
                bool isBoldFace = descriptor.IsBoldFace;
            }
            if (flag2)
            {
                bool flag5 = descriptor.IsBoldFace;
            }
            if (font.Unicode)
            {
                string str = "";
                for (int i = 0; i < s.Length; i++)
                {
                    char ch = s[i];
                    int num8 = 0;
                    if (descriptor.fontData.cmap.symbol)
                    {
                        num8 = ch + (descriptor.fontData.os2.usFirstCharIndex & 0xff00);
                        num8 = descriptor.CharCodeToGlyphIndex((char) num8);
                    }
                    else
                    {
                        num8 = descriptor.CharCodeToGlyphIndex(ch);
                    }
                    str = str + ((char) num8);
                }
                s = str;
                byte[] bytes = PdfEncoders.FormatStringLiteral(PdfEncoders.RawUnicodeEncoding.GetBytes(s), true, false, true, null);
                string str2 = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
                XPoint pos = new XPoint(x, y);
                this.AdjustTextMatrix(ref pos);
                this.AppendFormat("{0:0.####} {1:0.####} Td {2} Tj\n", new object[] { pos.x, pos.y, str2 });
            }
            else
            {
                byte[] buffer2 = PdfEncoders.WinAnsiEncoding.GetBytes(s);
                XPoint point2 = new XPoint(x, y);
                this.AdjustTextMatrix(ref point2);
                this.AppendFormat("{0:0.####} {1:0.####} Td {2} Tj\n", new object[] { point2.x, point2.y, PdfEncoders.ToStringLiteral(buffer2, false, null) });
            }
            if (flag4)
            {
                double num9 = (height * realizedFont.FontDescriptor.descriptor.UnderlinePosition) / ((double) font.cellSpace);
                double num10 = (height * realizedFont.FontDescriptor.descriptor.UnderlineThickness) / ((double) font.cellSpace);
                this.DrawRectangle(null, brush, x, y - num9, width, num10);
            }
            if (flag3)
            {
                double num11 = (height * realizedFont.FontDescriptor.descriptor.StrikeoutPosition) / ((double) font.cellSpace);
                double num12 = (height * realizedFont.FontDescriptor.descriptor.StrikeoutSize) / ((double) font.cellSpace);
                this.DrawRectangle(null, brush, x, (y - num11) - num12, width, num12);
            }
        }

        [Conditional("DEBUG")]
        private void DumpPathData(PathData pathData)
        {
            int length = pathData.Points.Length;
            for (int i = 0; i < length; i++)
            {
                PdfEncoders.Format("{0:X}   {1:####0.000} {2:####0.000}", new object[] { pathData.Types[i], pathData.Points[i].X, pathData.Points[i].Y });
            }
        }

        public void EndContainer(XGraphicsContainer container)
        {
            this.BeginGraphic();
            this.RestoreState(container.InternalState);
        }

        private void EndPage()
        {
            if (this.streamMode == StreamMode.Text)
            {
                this.content.Append("ET\n");
                this.streamMode = StreamMode.Graphic;
            }
            while (this.gfxStateStack.Count != 0)
            {
                this.RestoreState();
            }
        }

        private string GetContent()
        {
            this.EndPage();
            return this.content.ToString();
        }

        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            if (this.page != null)
            {
                return this.page.GetFontName(font, out pdfFont);
            }
            return this.form.GetFontName(font, out pdfFont);
        }

        internal string GetFormName(XForm form)
        {
            if (this.page != null)
            {
                return this.page.GetFormName(form);
            }
            return this.form.GetFormName(form);
        }

        internal string GetImageName(XImage image)
        {
            if (this.page != null)
            {
                return this.page.GetImageName(image);
            }
            return this.form.GetImageName(image);
        }

        private int Quatrant(double φ, bool start, bool clockwise)
        {
            if (φ > 360.0)
            {
                φ -= Math.Floor((double) (φ / 360.0)) * 360.0;
            }
            int num = (int) (φ / 90.0);
            if ((num * 90) == φ)
            {
                if ((start && !clockwise) || (!start && clockwise))
                {
                    num = (num == 0) ? 3 : (num - 1);
                }
                return num;
            }
            return (clockwise ? (((int) Math.Floor((double) (φ / 90.0))) % 4) : ((int) Math.Floor((double) (φ / 90.0))));
        }

        private void Realize(XBrush brush)
        {
            this.Realize(null, brush);
        }

        private string Realize(XImage image)
        {
            this.BeginPage();
            this.BeginGraphic();
            this.RealizeTransform();
            if (image is XForm)
            {
                return this.GetFormName(image as XForm);
            }
            return this.GetImageName(image);
        }

        private void Realize(XPen pen)
        {
            this.Realize(pen, null);
        }

        private void Realize(XPen pen, XBrush brush)
        {
            this.BeginPage();
            this.BeginGraphic();
            this.RealizeTransform();
            if (pen != null)
            {
                this.gfxState.RealizePen(pen, this.colorMode);
            }
            if (brush != null)
            {
                this.gfxState.RealizeBrush(brush, this.colorMode);
            }
        }

        private void Realize(XFont font, XBrush brush, int renderMode)
        {
            this.BeginPage();
            this.RealizeTransform();
            if (this.streamMode != StreamMode.Text)
            {
                this.streamMode = StreamMode.Text;
                this.content.Append("BT\n");
                this.gfxState.realizedTextPosition = new XPoint();
            }
            this.gfxState.RealizeFont(font, brush, renderMode);
        }

        private void RealizeTransform()
        {
            this.BeginPage();
            if (this.gfxState.Level == 1)
            {
                this.BeginGraphic();
                this.SaveState();
            }
            if (this.gfxState.MustRealizeCtm)
            {
                this.BeginGraphic();
                this.gfxState.RealizeCtm();
            }
        }

        public void ResetClip()
        {
            if (this.clipLevel != 0)
            {
                if (this.clipLevel != this.gfxState.Level)
                {
                    throw new NotImplementedException("Cannot reset clip region in an inner graphic state level.");
                }
                this.BeginGraphic();
                InternalGraphicsState internalState = this.gfxState.InternalState;
                XMatrix transform = this.gfxState.Transform;
                this.RestoreState();
                this.SaveState();
                this.gfxState.InternalState = internalState;
                this.gfxState.Transform = transform;
            }
        }

        public void Restore(XGraphicsState state)
        {
            this.BeginGraphic();
            this.RestoreState(state.InternalState);
        }

        private void RestoreState()
        {
            this.gfxState = this.gfxStateStack.Pop();
            this.Append("Q\n");
        }

        private PdfGraphicsState RestoreState(InternalGraphicsState state)
        {
            int num = 1;
            PdfGraphicsState state2 = this.gfxStateStack.Pop();
            while (state2.InternalState != state)
            {
                this.Append("Q\n");
                num++;
                state2 = this.gfxStateStack.Pop();
            }
            this.Append("Q\n");
            this.gfxState = state2;
            return state2;
        }

        public void Save(XGraphicsState state)
        {
            this.BeginGraphic();
            this.RealizeTransform();
            this.gfxState.InternalState = state.InternalState;
            this.SaveState();
        }

        private void SaveState()
        {
            this.gfxStateStack.Push(this.gfxState);
            this.gfxState = this.gfxState.Clone();
            this.gfxState.Level = this.gfxStateStack.Count;
            this.Append("q\n");
        }

        public void SetClip(XGraphicsPath path, XCombineMode combineMode)
        {
            if (path == null)
            {
                throw new NotImplementedException("SetClip with no path.");
            }
            if (this.gfxState.Level < 2)
            {
                this.RealizeTransform();
            }
            if (combineMode == XCombineMode.Replace)
            {
                if (this.clipLevel != 0)
                {
                    if (this.clipLevel != this.gfxState.Level)
                    {
                        throw new NotImplementedException("Cannot set new clip region in an inner graphic state level.");
                    }
                    this.ResetClip();
                }
                this.clipLevel = this.gfxState.Level;
            }
            else if ((combineMode == XCombineMode.Intersect) && (this.clipLevel == 0))
            {
                this.clipLevel = this.gfxState.Level;
            }
            this.gfxState.SetAndRealizeClipPath(path);
        }

        public void SetPageTransform(XPageDirection direction, XPoint origion, XGraphicsUnit unit)
        {
            if (this.gfxStateStack.Count > 0)
            {
                throw new InvalidOperationException("PageTransformation can be modified only when the graphics stack is empty.");
            }
            throw new NotImplementedException("SetPageTransform");
        }

        public void WriteComment(string comment)
        {
            comment.Replace("\n", "\n% ");
            this.Append("% " + comment + "\n");
        }

        internal XGraphics Gfx =>
            this.gfx;

        internal PdfDocument Owner
        {
            get
            {
                if (this.page != null)
                {
                    return this.page.Owner;
                }
                return this.form.Owner;
            }
        }

        public XGraphicsPdfPageOptions PageOptions =>
            this.options;

        internal PdfResources Resources
        {
            get
            {
                if (this.page != null)
                {
                    return this.page.Resources;
                }
                return this.form.Resources;
            }
        }

        internal XSize Size
        {
            get
            {
                if (this.page != null)
                {
                    return new XSize((double) this.page.Width, (double) this.page.Height);
                }
                return this.form.Size;
            }
        }

        public XMatrix Transform
        {
            set
            {
                this.gfxState.Transform = value;
            }
        }
    }
}

