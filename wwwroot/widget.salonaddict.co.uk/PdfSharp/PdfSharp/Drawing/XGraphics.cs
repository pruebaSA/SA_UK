namespace PdfSharp.Drawing
{
    using PdfSharp;
    using PdfSharp.Drawing.BarCodes;
    using PdfSharp.Drawing.Pdf;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;

    public sealed class XGraphics : IDisposable
    {
        internal XMatrix defaultViewMatrix;
        private bool disposed;
        private bool drawGraphics;
        private XForm form;
        internal System.Drawing.Graphics gfx;
        private GraphicsStateStack gsStack;
        private XGraphicsInternals internals;
        internal Metafile metafile;
        private PdfFontEmbedding mfeh;
        private PdfFontEncoding muh;
        private XPageDirection pageDirection;
        private XPoint pageOrigin;
        private XSize pageSize;
        private XSize pageSizePoints;
        private XGraphicsUnit pageUnit;
        private IXGraphicsRenderer renderer;
        private XSmoothingMode smoothingMode;
        internal XGraphicTargetContext targetContext;
        internal XMatrix transform;
        private SpaceTransformer transformer;

        private XGraphics(XForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }
            this.form = form;
            form.AssociateGraphics(this);
            this.gsStack = new GraphicsStateStack(this);
            this.targetContext = XGraphicTargetContext.GDI;
            if (form.Owner == null)
            {
                MemoryStream stream = new MemoryStream();
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                IntPtr hdc = graphics.GetHdc();
                RectangleF frameRect = new RectangleF(0f, 0f, (float) form.PixelWidth, (float) form.PixelHeight);
                this.metafile = new Metafile(stream, hdc, frameRect, MetafileFrameUnit.Pixel);
                graphics.ReleaseHdc(hdc);
                graphics.Dispose();
                this.gfx = System.Drawing.Graphics.FromImage(this.metafile);
                this.gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                this.drawGraphics = true;
            }
            else
            {
                this.metafile = null;
                this.gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            }
            if (form.Owner != null)
            {
                this.renderer = new XGraphicsPdfRenderer(form, this);
            }
            this.pageSize = form.Size;
            this.Initialize();
        }

        private XGraphics(PdfSharp.Pdf.PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (page.Owner == null)
            {
                throw new ArgumentException("You cannot draw on a page that is not owned by a PdfDocument object.", "page");
            }
            if (page.RenderContent != null)
            {
                throw new InvalidOperationException("An XGraphics object already exists for this page and must be disposed before a new one can be created.");
            }
            if (page.Owner.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot create XGraphics for a page of a document that cannot be modified. Use PdfDocumentOpenMode.Modify.");
            }
            this.gsStack = new GraphicsStateStack(this);
            PdfContent content = null;
            switch (options)
            {
                case XGraphicsPdfPageOptions.Append:
                    break;

                case XGraphicsPdfPageOptions.Prepend:
                    content = page.Contents.PrependContent();
                    goto Label_00A7;

                case XGraphicsPdfPageOptions.Replace:
                    page.Contents.Elements.Clear();
                    break;

                default:
                    goto Label_00A7;
            }
            content = page.Contents.AppendContent();
        Label_00A7:
            page.RenderContent = content;
            this.gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            this.targetContext = XGraphicTargetContext.GDI;
            this.renderer = new XGraphicsPdfRenderer(page, this, options);
            this.pageSizePoints = new XSize((double) page.Width, (double) page.Height);
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    this.pageSize = new XSize((double) page.Width, (double) page.Height);
                    break;

                case XGraphicsUnit.Inch:
                    this.pageSize = new XSize(XUnit.FromPoint((double) page.Width).Inch, XUnit.FromPoint((double) page.Height).Inch);
                    break;

                case XGraphicsUnit.Millimeter:
                    this.pageSize = new XSize(XUnit.FromPoint((double) page.Width).Millimeter, XUnit.FromPoint((double) page.Height).Millimeter);
                    break;

                case XGraphicsUnit.Centimeter:
                    this.pageSize = new XSize(XUnit.FromPoint((double) page.Width).Centimeter, XUnit.FromPoint((double) page.Height).Centimeter);
                    break;

                case XGraphicsUnit.Presentation:
                    this.pageSize = new XSize(XUnit.FromPoint((double) page.Width).Presentation, XUnit.FromPoint((double) page.Height).Presentation);
                    break;

                default:
                    throw new NotImplementedException("unit");
            }
            this.pageUnit = pageUnit;
            this.pageDirection = pageDirection;
            this.Initialize();
        }

        private XGraphics(System.Drawing.Graphics gfx, XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (gfx == null)
            {
                gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            }
            this.gsStack = new GraphicsStateStack(this);
            this.targetContext = XGraphicTargetContext.GDI;
            this.gfx = gfx;
            this.drawGraphics = true;
            this.pageSize = new XSize(size.width, size.height);
            this.pageUnit = pageUnit;
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    this.pageSizePoints = new XSize(size.width, size.height);
                    break;

                case XGraphicsUnit.Inch:
                    this.pageSizePoints = new XSize((double) XUnit.FromInch(size.width), (double) XUnit.FromInch(size.height));
                    break;

                case XGraphicsUnit.Millimeter:
                    this.pageSizePoints = new XSize((double) XUnit.FromMillimeter(size.width), (double) XUnit.FromMillimeter(size.height));
                    break;

                case XGraphicsUnit.Centimeter:
                    this.pageSizePoints = new XSize((double) XUnit.FromCentimeter(size.width), (double) XUnit.FromCentimeter(size.height));
                    break;

                case XGraphicsUnit.Presentation:
                    this.pageSizePoints = new XSize((double) XUnit.FromPresentation(size.width), (double) XUnit.FromPresentation(size.height));
                    break;

                default:
                    throw new NotImplementedException("unit");
            }
            this.pageDirection = pageDirection;
            this.Initialize();
        }

        private void AddTransform(XMatrix transform, XMatrixOrder order)
        {
            XMatrix defaultViewMatrix = this.transform;
            defaultViewMatrix.Multiply(transform, order);
            this.transform = defaultViewMatrix;
            defaultViewMatrix = this.defaultViewMatrix;
            defaultViewMatrix.Multiply(this.transform, XMatrixOrder.Prepend);
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                this.gfx.Transform = (Matrix) defaultViewMatrix;
            }
            if (this.renderer != null)
            {
                this.renderer.Transform = this.transform;
            }
        }

        public XGraphicsContainer BeginContainer() => 
            this.BeginContainer(new XRect(0.0, 0.0, 1.0, 1.0), new XRect(0.0, 0.0, 1.0, 1.0), XGraphicsUnit.Point);

        public XGraphicsContainer BeginContainer(XRect dstrect, XRect srcrect, XGraphicsUnit unit)
        {
            if (unit != XGraphicsUnit.Point)
            {
                throw new ArgumentException("The current implementation supports XGraphicsUnit.Point only.", "unit");
            }
            XGraphicsContainer container = null;
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                container = new XGraphicsContainer(this.gfx.Save());
            }
            InternalGraphicsState state = new InternalGraphicsState(this, container) {
                Transform = this.transform
            };
            this.gsStack.Push(state);
            if (this.renderer != null)
            {
                this.renderer.BeginContainer(container, dstrect, srcrect, unit);
            }
            XMatrix transform = new XMatrix();
            double scaleX = dstrect.Width / srcrect.Width;
            double scaleY = dstrect.Height / srcrect.Height;
            transform.TranslatePrepend(-srcrect.X, -srcrect.Y);
            transform.ScalePrepend(scaleX, scaleY);
            transform.TranslatePrepend(dstrect.X / scaleX, dstrect.Y / scaleY);
            this.AddTransform(transform, XMatrixOrder.Prepend);
            return container;
        }

        public XGraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, XGraphicsUnit unit) => 
            this.BeginContainer(new XRect(dstrect), new XRect(dstrect), unit);

        public XGraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, XGraphicsUnit unit) => 
            this.BeginContainer(new XRect(dstrect), new XRect(dstrect), unit);

        private void CheckXPdfFormConsistence(XImage image)
        {
            XForm form = image as XForm;
            if (form != null)
            {
                form.Finish();
                if ((this.renderer != null) && (this.renderer is XGraphicsPdfRenderer))
                {
                    if ((form.Owner != null) && (form.Owner != ((XGraphicsPdfRenderer) this.renderer).Owner))
                    {
                        throw new InvalidOperationException("A XPdfForm object is always bound to the document it was created for and cannot be drawn in the context of another document.");
                    }
                    if (form == ((XGraphicsPdfRenderer) this.renderer).form)
                    {
                        throw new InvalidOperationException("A XPdfForm cannot be drawn on itself.");
                    }
                }
            }
        }

        public void Clear(XColor color)
        {
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.Clear(color.ToGdiColor());
            }
            if (this.renderer != null)
            {
                this.renderer.Clear(color);
            }
        }

        public static XGraphics CreateMeasureContext(XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection) => 
            new XGraphics(null, size, pageUnit, pageDirection);

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if (this.form != null)
                {
                    this.form.Finish();
                }
                if (this.targetContext == XGraphicTargetContext.GDI)
                {
                    this.gfx.Dispose();
                    this.gfx = null;
                    this.metafile = null;
                }
                this.drawGraphics = false;
                if (this.renderer != null)
                {
                    this.renderer.Close();
                    this.renderer = null;
                }
            }
        }

        public void DrawArc(XPen pen, XRect rect, double startAngle, double sweepAngle)
        {
            this.DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawArc(XPen pen, Rectangle rect, double startAngle, double sweepAngle)
        {
            this.DrawArc(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawArc(XPen pen, RectangleF rect, double startAngle, double sweepAngle)
        {
            this.DrawArc(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (Math.Abs(sweepAngle) >= 360.0)
            {
                this.DrawEllipse(pen, x, y, width, height);
            }
            else
            {
                if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
                {
                    this.gfx.DrawArc(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
                }
                if (this.renderer != null)
                {
                    this.renderer.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
                }
            }
        }

        public void DrawArc(XPen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            this.DrawArc(pen, (double) x, (double) y, (double) width, (double) height, (double) startAngle, (double) sweepAngle);
        }

        public void DrawBarCode(BarCode barcode, XPoint position)
        {
            barcode.Render(this, XBrushes.Black, null, position);
        }

        public void DrawBarCode(BarCode barcode, XBrush brush, XPoint position)
        {
            barcode.Render(this, brush, null, position);
        }

        public void DrawBarCode(BarCode barcode, XBrush brush, XFont font, XPoint position)
        {
            barcode.Render(this, brush, font, position);
        }

        public void DrawBezier(XPen pen, XPoint pt1, XPoint pt2, XPoint pt3, XPoint pt4)
        {
            this.DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void DrawBezier(XPen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            this.DrawBezier(pen, (double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y, (double) pt3.X, (double) pt3.Y, (double) pt4.X, (double) pt4.Y);
        }

        public void DrawBezier(XPen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            this.DrawBezier(pen, (double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y, (double) pt3.X, (double) pt3.Y, (double) pt4.X, (double) pt4.Y);
        }

        public void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawBezier(pen.RealizeGdiPen(), (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
            }
            if (this.renderer != null)
            {
                XPoint[] points = new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2), new XPoint(x3, y3), new XPoint(x4, y4) };
                this.renderer.DrawBeziers(pen, points);
            }
        }

        public void DrawBeziers(XPen pen, XPoint[] points)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            int length = points.Length;
            if (length != 0)
            {
                if (((length - 1) % 3) != 0)
                {
                    throw new ArgumentException("Invalid number of points for bezier curves. Number must fulfil 4+3n.", "points");
                }
                if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
                {
                    this.gfx.DrawBeziers(pen.RealizeGdiPen(), MakePointFArray(points));
                }
                if (this.renderer != null)
                {
                    this.renderer.DrawBeziers(pen, points);
                }
            }
        }

        public void DrawBeziers(XPen pen, Point[] points)
        {
            this.DrawBeziers(pen, MakeXPointArray(points));
        }

        public void DrawBeziers(XPen pen, PointF[] points)
        {
            this.DrawBeziers(pen, MakeXPointArray(points));
        }

        public void DrawClosedCurve(XBrush brush, XPoint[] points)
        {
            this.DrawClosedCurve(null, brush, points, XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, Point[] points)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, PointF[] points)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XPoint[] points)
        {
            this.DrawClosedCurve(pen, null, points, XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XPen pen, Point[] points)
        {
            this.DrawClosedCurve(pen, null, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XPen pen, PointF[] points)
        {
            this.DrawClosedCurve(pen, null, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(null, brush, points, fillmode, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, Point[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), fillmode, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, PointF[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), fillmode, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XPoint[] points, double tension)
        {
            this.DrawClosedCurve(pen, null, points, XFillMode.Alternate, tension);
        }

        public void DrawClosedCurve(XPen pen, Point[] points, double tension)
        {
            this.DrawClosedCurve(pen, null, MakeXPointArray(points), XFillMode.Alternate, tension);
        }

        public void DrawClosedCurve(XPen pen, PointF[] points, double tension)
        {
            this.DrawClosedCurve(pen, null, MakeXPointArray(points), XFillMode.Alternate, tension);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points)
        {
            this.DrawClosedCurve(pen, brush, points, XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, Point[] points)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, PointF[] points)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), XFillMode.Alternate, 0.5);
        }

        public void DrawClosedCurve(XBrush brush, XPoint[] points, XFillMode fillmode, double tension)
        {
            this.DrawClosedCurve(null, brush, points, fillmode, tension);
        }

        public void DrawClosedCurve(XBrush brush, Point[] points, XFillMode fillmode, double tension)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), fillmode, tension);
        }

        public void DrawClosedCurve(XBrush brush, PointF[] points, XFillMode fillmode, double tension)
        {
            this.DrawClosedCurve(null, brush, MakeXPointArray(points), fillmode, tension);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(pen, brush, points, fillmode, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, Point[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), fillmode, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, PointF[] points, XFillMode fillmode)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), fillmode, 0.5);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode, double tension)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            int length = points.Length;
            if (length != 0)
            {
                if (length < 2)
                {
                    throw new ArgumentException("Not enough points.", "points");
                }
                if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
                {
                    if (brush != null)
                    {
                        this.gfx.FillClosedCurve(brush.RealizeGdiBrush(), MakePointFArray(points), (FillMode) fillmode, (float) tension);
                    }
                    if (pen != null)
                    {
                        this.gfx.DrawClosedCurve(pen.RealizeGdiPen(), MakePointFArray(points), (float) tension, (FillMode) fillmode);
                    }
                }
                if (this.renderer != null)
                {
                    this.renderer.DrawClosedCurve(pen, brush, points, tension, fillmode);
                }
            }
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, Point[] points, XFillMode fillmode, double tension)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), fillmode, tension);
        }

        public void DrawClosedCurve(XPen pen, XBrush brush, PointF[] points, XFillMode fillmode, double tension)
        {
            this.DrawClosedCurve(pen, brush, MakeXPointArray(points), fillmode, tension);
        }

        public void DrawCurve(XPen pen, XPoint[] points)
        {
            this.DrawCurve(pen, points, 0.5);
        }

        public void DrawCurve(XPen pen, Point[] points)
        {
            this.DrawCurve(pen, MakePointFArray(points), 0.5);
        }

        public void DrawCurve(XPen pen, PointF[] points)
        {
            this.DrawCurve(pen, MakeXPointArray(points), 0.5);
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
            if (points.Length < 2)
            {
                throw new ArgumentException("DrawCurve requires two or more points.", "points");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawCurve(pen.RealizeGdiPen(), MakePointFArray(points), (float) tension);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawCurve(pen, points, tension);
            }
        }

        public void DrawCurve(XPen pen, Point[] points, double tension)
        {
            this.DrawCurve(pen, MakeXPointArray(points), tension);
        }

        public void DrawCurve(XPen pen, PointF[] points, double tension)
        {
            this.DrawCurve(pen, MakeXPointArray(points), tension);
        }

        public void DrawEllipse(XBrush brush, XRect rect)
        {
            this.DrawEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(XBrush brush, Rectangle rect)
        {
            this.DrawEllipse(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XBrush brush, RectangleF rect)
        {
            this.DrawEllipse(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XPen pen, XRect rect)
        {
            this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(XPen pen, Rectangle rect)
        {
            this.DrawEllipse(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XPen pen, RectangleF rect)
        {
            this.DrawEllipse(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XPen pen, XBrush brush, XRect rect)
        {
            this.DrawEllipse(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(XPen pen, XBrush brush, Rectangle rect)
        {
            this.DrawEllipse(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XPen pen, XBrush brush, RectangleF rect)
        {
            this.DrawEllipse(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawEllipse(XBrush brush, double x, double y, double width, double height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillEllipse(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawEllipse(null, brush, x, y, width, height);
            }
        }

        public void DrawEllipse(XBrush brush, int x, int y, int width, int height)
        {
            this.DrawEllipse(brush, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawEllipse(XPen pen, double x, double y, double width, double height)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawArc(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height, 0f, 360f);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawEllipse(pen, null, x, y, width, height);
            }
        }

        public void DrawEllipse(XPen pen, int x, int y, int width, int height)
        {
            this.DrawEllipse(pen, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillEllipse(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height);
                this.gfx.DrawArc(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height, 0f, 360f);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawEllipse(pen, brush, x, y, width, height);
            }
        }

        public void DrawEllipse(XPen pen, XBrush brush, int x, int y, int width, int height)
        {
            this.DrawEllipse(pen, brush, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawImage(XImage image, XPoint point)
        {
            this.DrawImage(image, point.X, point.Y);
        }

        public void DrawImage(XImage image, XRect rect)
        {
            this.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImage(XImage image, Point point)
        {
            this.DrawImage(image, (double) point.X, (double) point.Y);
        }

        public void DrawImage(XImage image, PointF point)
        {
            this.DrawImage(image, (double) point.X, (double) point.Y);
        }

        public void DrawImage(XImage image, Rectangle rect)
        {
            this.DrawImage(image, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawImage(XImage image, RectangleF rect)
        {
            this.DrawImage(image, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawImage(XImage image, double x, double y)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            this.CheckXPdfFormConsistence(image);
            double pointWidth = image.PointWidth;
            double pointHeight = image.PointHeight;
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                if (image.gdiImage != null)
                {
                    InterpolationMode invalid = InterpolationMode.Invalid;
                    if (!image.Interpolate)
                    {
                        invalid = this.gfx.InterpolationMode;
                        this.gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    this.gfx.DrawImage(image.gdiImage, (float) x, (float) y, (float) pointWidth, (float) pointHeight);
                    if (!image.Interpolate)
                    {
                        this.gfx.InterpolationMode = invalid;
                    }
                }
                else
                {
                    this.DrawMissingImageRect(new XRect(x, y, pointWidth, pointHeight));
                }
            }
            if (this.renderer != null)
            {
                this.renderer.DrawImage(image, x, y, image.PointWidth, image.PointHeight);
            }
        }

        public void DrawImage(XImage image, int x, int y)
        {
            this.DrawImage(image, (double) x, (double) y);
        }

        public void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            this.CheckXPdfFormConsistence(image);
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                if (image.gdiImage != null)
                {
                    InterpolationMode invalid = InterpolationMode.Invalid;
                    if (!image.Interpolate)
                    {
                        invalid = this.gfx.InterpolationMode;
                        this.gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    RectangleF ef = new RectangleF((float) destRect.X, (float) destRect.Y, (float) destRect.Width, (float) destRect.Height);
                    RectangleF ef2 = new RectangleF((float) srcRect.X, (float) srcRect.Y, (float) srcRect.Width, (float) srcRect.Height);
                    this.gfx.DrawImage(image.gdiImage, ef, ef2, GraphicsUnit.Pixel);
                    if (!image.Interpolate)
                    {
                        this.gfx.InterpolationMode = invalid;
                    }
                }
                else
                {
                    this.DrawMissingImageRect(new XRect(destRect.x, destRect.y, destRect.width, destRect.height));
                }
            }
            if (this.renderer != null)
            {
                this.renderer.DrawImage(image, destRect, srcRect, srcUnit);
            }
        }

        public void DrawImage(XImage image, Rectangle destRect, Rectangle srcRect, XGraphicsUnit srcUnit)
        {
            XRect rect = new XRect((double) destRect.X, (double) destRect.Y, (double) destRect.Width, (double) destRect.Height);
            XRect rect2 = new XRect((double) srcRect.X, (double) srcRect.Y, (double) srcRect.Width, (double) srcRect.Height);
            this.DrawImage(image, rect, rect2, srcUnit);
        }

        public void DrawImage(XImage image, RectangleF destRect, RectangleF srcRect, XGraphicsUnit srcUnit)
        {
            XRect rect = new XRect((double) destRect.X, (double) destRect.Y, (double) destRect.Width, (double) destRect.Height);
            XRect rect2 = new XRect((double) srcRect.X, (double) srcRect.Y, (double) srcRect.Width, (double) srcRect.Height);
            this.DrawImage(image, rect, rect2, srcUnit);
        }

        public void DrawImage(XImage image, double x, double y, double width, double height)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            this.CheckXPdfFormConsistence(image);
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                if (image.gdiImage != null)
                {
                    InterpolationMode invalid = InterpolationMode.Invalid;
                    if (!image.Interpolate)
                    {
                        invalid = this.gfx.InterpolationMode;
                        this.gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    this.gfx.DrawImage(image.gdiImage, (float) x, (float) y, (float) width, (float) height);
                    if (!image.Interpolate)
                    {
                        this.gfx.InterpolationMode = invalid;
                    }
                }
                else
                {
                    XImage placeHolder = null;
                    if (image is XPdfForm)
                    {
                        XPdfForm form = image as XPdfForm;
                        if (form.PlaceHolder != null)
                        {
                            placeHolder = form.PlaceHolder;
                        }
                    }
                    if (placeHolder != null)
                    {
                        this.gfx.DrawImage(placeHolder.gdiImage, (float) x, (float) y, (float) width, (float) height);
                    }
                    else
                    {
                        this.DrawMissingImageRect(new XRect(x, y, width, height));
                    }
                }
            }
            if (this.renderer != null)
            {
                this.renderer.DrawImage(image, x, y, width, height);
            }
        }

        public void DrawImage(XImage image, int x, int y, int width, int height)
        {
            this.DrawImage(image, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawLine(XPen pen, XPoint pt1, XPoint pt2)
        {
            this.DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void DrawLine(XPen pen, Point pt1, Point pt2)
        {
            this.DrawLine(pen, (double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y);
        }

        public void DrawLine(XPen pen, PointF pt1, PointF pt2)
        {
            this.DrawLine(pen, (double) pt1.X, (double) pt1.Y, (double) pt2.X, (double) pt2.Y);
        }

        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawLine(pen.RealizeGdiPen(), (float) x1, (float) y1, (float) x2, (float) y2);
            }
            if (this.renderer != null)
            {
                XPoint[] points = new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2) };
                this.renderer.DrawLines(pen, points);
            }
        }

        public void DrawLine(XPen pen, int x1, int y1, int x2, int y2)
        {
            this.DrawLine(pen, (double) x1, (double) y1, (double) x2, (double) y2);
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
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawLines(pen.RealizeGdiPen(), MakePointFArray(points));
            }
            if (this.renderer != null)
            {
                this.renderer.DrawLines(pen, points);
            }
        }

        public void DrawLines(XPen pen, Point[] points)
        {
            this.DrawLines(pen, MakePointFArray(points));
        }

        public void DrawLines(XPen pen, PointF[] points)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            if (this.drawGraphics)
            {
                this.gfx.DrawLines(pen.RealizeGdiPen(), points);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawLines(pen, MakeXPointArray(points));
            }
        }

        public void DrawLines(XPen pen, double x, double y, params double[] value)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            int length = value.Length;
            XPoint[] points = new XPoint[(length / 2) + 1];
            points[0].X = x;
            points[0].Y = y;
            for (int i = 0; i < (length / 2); i++)
            {
                points[i + 1].X = value[2 * i];
                points[i + 1].Y = value[(2 * i) + 1];
            }
            this.DrawLines(pen, points);
        }

        public void DrawMatrixCode(MatrixCode matrixcode, XPoint position)
        {
            matrixcode.Render(this, XBrushes.Black, position);
        }

        public void DrawMatrixCode(MatrixCode matrixcode, XBrush brush, XPoint position)
        {
            matrixcode.Render(this, brush, position);
        }

        private void DrawMissingImageRect(XRect rect)
        {
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                float x = (float) rect.x;
                float y = (float) rect.y;
                float width = (float) rect.width;
                float height = (float) rect.height;
                this.gfx.DrawRectangle(Pens.Red, x, y, width, height);
                this.gfx.DrawLine(Pens.Red, x, y, x + width, y + height);
                this.gfx.DrawLine(Pens.Red, x + width, y, x, y + height);
            }
        }

        public void DrawPath(XBrush brush, XGraphicsPath path)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillPath(brush.RealizeGdiBrush(), path.gdipPath);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPath(null, brush, path);
            }
        }

        public void DrawPath(XPen pen, XGraphicsPath path)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawPath(pen.RealizeGdiPen(), path.gdipPath);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPath(pen, null, path);
            }
        }

        public void DrawPath(XPen pen, XBrush brush, XGraphicsPath path)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                if (brush != null)
                {
                    this.gfx.FillPath(brush.RealizeGdiBrush(), path.gdipPath);
                }
                if (pen != null)
                {
                    this.gfx.DrawPath(pen.RealizeGdiPen(), path.gdipPath);
                }
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPath(pen, brush, path);
            }
        }

        public void DrawPie(XBrush brush, XRect rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XBrush brush, Rectangle rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XBrush brush, RectangleF rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, XRect rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, Rectangle rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, RectangleF rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, XBrush brush, XRect rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, XBrush brush, Rectangle rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XPen pen, XBrush brush, RectangleF rect, double startAngle, double sweepAngle)
        {
            this.DrawPie(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillPie(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPie(null, brush, x, y, width, height, startAngle, sweepAngle);
            }
        }

        public void DrawPie(XBrush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            this.DrawPie(brush, (double) x, (double) y, (double) width, (double) height, (double) startAngle, (double) sweepAngle);
        }

        public void DrawPie(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawPie(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPie(pen, null, x, y, width, height, startAngle, sweepAngle);
            }
        }

        public void DrawPie(XPen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            this.DrawPie(pen, (double) x, (double) y, (double) width, (double) height, (double) startAngle, (double) sweepAngle);
        }

        public void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillPie(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
                this.gfx.DrawPie(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPie(pen, brush, x, y, width, height, startAngle, sweepAngle);
            }
        }

        public void DrawPie(XPen pen, XBrush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            this.DrawPie(pen, brush, (double) x, (double) y, (double) width, (double) height, (double) startAngle, (double) sweepAngle);
        }

        public void DrawPolygon(XPen pen, XPoint[] points)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawPolygon(pen.RealizeGdiPen(), MakePointFArray(points));
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPolygon(pen, null, points, XFillMode.Alternate);
            }
        }

        public void DrawPolygon(XPen pen, Point[] points)
        {
            this.DrawPolygon(pen, MakeXPointArray(points));
        }

        public void DrawPolygon(XPen pen, PointF[] points)
        {
            this.DrawPolygon(pen, MakeXPointArray(points));
        }

        public void DrawPolygon(XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillPolygon(brush.RealizeGdiBrush(), MakePointFArray(points), (FillMode) fillmode);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPolygon(null, brush, points, fillmode);
            }
        }

        public void DrawPolygon(XBrush brush, Point[] points, XFillMode fillmode)
        {
            this.DrawPolygon(brush, MakeXPointArray(points), fillmode);
        }

        public void DrawPolygon(XBrush brush, PointF[] points, XFillMode fillmode)
        {
            this.DrawPolygon(brush, MakeXPointArray(points), fillmode);
        }

        public void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                PointF[] tfArray = MakePointFArray(points);
                this.gfx.FillPolygon(brush.RealizeGdiBrush(), tfArray, (FillMode) fillmode);
                this.gfx.DrawPolygon(pen.RealizeGdiPen(), tfArray);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawPolygon(pen, brush, points, fillmode);
            }
        }

        public void DrawPolygon(XPen pen, XBrush brush, Point[] points, XFillMode fillmode)
        {
            this.DrawPolygon(pen, brush, MakeXPointArray(points), fillmode);
        }

        public void DrawPolygon(XPen pen, XBrush brush, PointF[] points, XFillMode fillmode)
        {
            this.DrawPolygon(pen, brush, MakeXPointArray(points), fillmode);
        }

        public void DrawRectangle(XBrush brush, XRect rect)
        {
            this.DrawRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(XBrush brush, Rectangle rect)
        {
            this.DrawRectangle(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XBrush brush, RectangleF rect)
        {
            this.DrawRectangle(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XPen pen, XRect rect)
        {
            this.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(XPen pen, Rectangle rect)
        {
            this.DrawRectangle(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XPen pen, RectangleF rect)
        {
            this.DrawRectangle(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            this.DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(XPen pen, XBrush brush, Rectangle rect)
        {
            this.DrawRectangle(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XPen pen, XBrush brush, RectangleF rect)
        {
            this.DrawRectangle(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
        }

        public void DrawRectangle(XBrush brush, double x, double y, double width, double height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillRectangle(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawRectangle(null, brush, x, y, width, height);
            }
        }

        public void DrawRectangle(XBrush brush, int x, int y, int width, int height)
        {
            this.DrawRectangle(brush, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawRectangle(XPen pen, double x, double y, double width, double height)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.DrawRectangle(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawRectangle(pen, null, x, y, width, height);
            }
        }

        public void DrawRectangle(XPen pen, int x, int y, int width, int height)
        {
            this.DrawRectangle(pen, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.FillRectangle(brush.RealizeGdiBrush(), (float) x, (float) y, (float) width, (float) height);
                this.gfx.DrawRectangle(pen.RealizeGdiPen(), (float) x, (float) y, (float) width, (float) height);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawRectangle(pen, brush, x, y, width, height);
            }
        }

        public void DrawRectangle(XPen pen, XBrush brush, int x, int y, int width, int height)
        {
            this.DrawRectangle(pen, brush, (double) x, (double) y, (double) width, (double) height);
        }

        public void DrawRectangles(XBrush brush, XRect[] rectangles)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(null, brush, rectangles);
        }

        public void DrawRectangles(XBrush brush, Rectangle[] rectangles)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(null, brush, rectangles);
        }

        public void DrawRectangles(XBrush brush, RectangleF[] rectangles)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(null, brush, rectangles);
        }

        public void DrawRectangles(XPen pen, XRect[] rectangles)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(pen, null, rectangles);
        }

        public void DrawRectangles(XPen pen, Rectangle[] rectangles)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(pen, null, rectangles);
        }

        public void DrawRectangles(XPen pen, RectangleF[] rectangles)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            this.DrawRectangles(pen, null, rectangles);
        }

        public void DrawRectangles(XPen pen, XBrush brush, XRect[] rectangles)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            int length = rectangles.Length;
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                RectangleF[] rects = new RectangleF[length];
                for (int i = 0; i < length; i++)
                {
                    XRect rect = rectangles[i];
                    rects[i] = new RectangleF((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height);
                }
                if (brush != null)
                {
                    this.gfx.FillRectangles(brush.RealizeGdiBrush(), rects);
                }
                if (pen != null)
                {
                    this.gfx.DrawRectangles(pen.RealizeGdiPen(), rects);
                }
            }
            if (this.renderer != null)
            {
                for (int j = 0; j < length; j++)
                {
                    XRect rect2 = rectangles[j];
                    this.renderer.DrawRectangle(pen, brush, rect2.X, rect2.Y, rect2.Width, rect2.Height);
                }
            }
        }

        public void DrawRectangles(XPen pen, XBrush brush, Rectangle[] rectangles)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            if (this.drawGraphics)
            {
                this.gfx.FillRectangles(brush.RealizeGdiBrush(), rectangles);
                this.gfx.DrawRectangles(pen.RealizeGdiPen(), rectangles);
            }
            if (this.renderer != null)
            {
                int length = rectangles.Length;
                for (int i = 0; i < length; i++)
                {
                    Rectangle rectangle = rectangles[i];
                    this.renderer.DrawRectangle(pen, brush, (double) rectangle.X, (double) rectangle.Y, (double) rectangle.Width, (double) rectangle.Height);
                }
            }
        }

        public void DrawRectangles(XPen pen, XBrush brush, RectangleF[] rectangles)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (rectangles == null)
            {
                throw new ArgumentNullException("rectangles");
            }
            if (this.drawGraphics)
            {
                this.gfx.FillRectangles(brush.RealizeGdiBrush(), rectangles);
                this.gfx.DrawRectangles(pen.RealizeGdiPen(), rectangles);
            }
            if (this.renderer != null)
            {
                int length = rectangles.Length;
                for (int i = 0; i < length; i++)
                {
                    RectangleF ef = rectangles[i];
                    this.renderer.DrawRectangle(pen, brush, (double) ef.X, (double) ef.Y, (double) ef.Width, (double) ef.Height);
                }
            }
        }

        public void DrawRoundedRectangle(XBrush brush, XRect rect, XSize ellipseSize)
        {
            this.DrawRoundedRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XBrush brush, Rectangle rect, Size ellipseSize)
        {
            this.DrawRoundedRectangle(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XBrush brush, RectangleF rect, SizeF ellipseSize)
        {
            this.DrawRoundedRectangle(brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, XRect rect, XSize ellipseSize)
        {
            this.DrawRoundedRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, Rectangle rect, Size ellipseSize)
        {
            this.DrawRoundedRectangle(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, RectangleF rect, SizeF ellipseSize)
        {
            this.DrawRoundedRectangle(pen, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, XRect rect, XSize ellipseSize)
        {
            this.DrawRoundedRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, Rectangle rect, Size ellipseSize)
        {
            this.DrawRoundedRectangle(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, RectangleF rect, SizeF ellipseSize)
        {
            this.DrawRoundedRectangle(pen, brush, (double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height, (double) ellipseSize.Width, (double) ellipseSize.Height);
        }

        public void DrawRoundedRectangle(XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            this.DrawRoundedRectangle(null, brush, x, y, width, height, ellipseWidth, ellipseHeight);
        }

        public void DrawRoundedRectangle(XBrush brush, int x, int y, int width, int height, int ellipseWidth, int ellipseHeight)
        {
            this.DrawRoundedRectangle(brush, (double) x, (double) y, (double) width, (double) height, (double) ellipseWidth, (double) ellipseHeight);
        }

        public void DrawRoundedRectangle(XPen pen, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            this.DrawRoundedRectangle(pen, null, x, y, width, height, ellipseWidth, ellipseHeight);
        }

        public void DrawRoundedRectangle(XPen pen, int x, int y, int width, int height, int ellipseWidth, int ellipseHeight)
        {
            this.DrawRoundedRectangle(pen, (double) x, (double) y, (double) width, (double) height, (double) ellipseWidth, (double) ellipseHeight);
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            if ((pen == null) && (brush == null))
            {
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                XGraphicsPath path = new XGraphicsPath();
                path.AddRoundedRectangle(x, y, width, height, ellipseWidth, ellipseHeight);
                this.DrawPath(pen, brush, path);
            }
            if (this.renderer != null)
            {
                this.renderer.DrawRoundedRectangle(pen, brush, x, y, width, height, ellipseWidth, ellipseHeight);
            }
        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, int x, int y, int width, int height, int ellipseWidth, int ellipseHeight)
        {
            this.DrawRoundedRectangle(pen, brush, (double) x, (double) y, (double) width, (double) height, (double) ellipseWidth, (double) ellipseHeight);
        }

        public void DrawString(string s, XFont font, XBrush brush, XPoint point)
        {
            this.DrawString(s, font, brush, new XRect(point.X, point.Y, 0.0, 0.0), XStringFormats.Default);
        }

        public void DrawString(string s, XFont font, XBrush brush, XRect layoutRectangle)
        {
            this.DrawString(s, font, brush, layoutRectangle, XStringFormats.Default);
        }

        public void DrawString(string s, XFont font, XBrush brush, PointF point)
        {
            this.DrawString(s, font, brush, new XRect((double) point.X, (double) point.Y, 0.0, 0.0), XStringFormats.Default);
        }

        public void DrawString(string s, XFont font, XBrush brush, RectangleF layoutRectangle)
        {
            this.DrawString(s, font, brush, new XRect(layoutRectangle), XStringFormats.Default);
        }

        public void DrawString(string s, XFont font, XBrush brush, XPoint point, XStringFormat format)
        {
            this.DrawString(s, font, brush, new XRect(point.X, point.Y, 0.0, 0.0), format);
        }

        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (((format != null) && (format.LineAlignment == XLineAlignment.BaseLine)) && (layoutRectangle.Height != 0.0))
            {
                throw new InvalidOperationException("DrawString: With XLineAlignment.BaseLine the height of the layout rectangle must be 0.");
            }
            if (text.Length != 0)
            {
                if (format == null)
                {
                    format = XStringFormats.Default;
                }
                if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
                {
                    RectangleF ef = layoutRectangle.ToRectangleF();
                    if (format.LineAlignment == XLineAlignment.BaseLine)
                    {
                        double height = font.GetHeight(this);
                        int lineSpacing = font.FontFamily.GetLineSpacing(font.Style);
                        int cellAscent = font.FontFamily.GetCellAscent(font.Style);
                        font.FontFamily.GetCellDescent(font.Style);
                        double num4 = (height * cellAscent) / ((double) lineSpacing);
                        num4 = (height * font.cellAscent) / ((double) font.cellSpace);
                        ef.Offset(0f, (float) -num4);
                    }
                    this.gfx.DrawString(text, font.RealizeGdiFont(), brush.RealizeGdiBrush(), ef, format?.RealizeGdiStringFormat());
                }
                if (this.renderer != null)
                {
                    this.renderer.DrawString(text, font, brush, layoutRectangle, format);
                }
            }
        }

        public void DrawString(string s, XFont font, XBrush brush, double x, double y)
        {
            this.DrawString(s, font, brush, new XRect(x, y, 0.0, 0.0), XStringFormats.Default);
        }

        public void DrawString(string s, XFont font, XBrush brush, PointF point, XStringFormat format)
        {
            this.DrawString(s, font, brush, new XRect((double) point.X, (double) point.Y, 0.0, 0.0), format);
        }

        public void DrawString(string s, XFont font, XBrush brush, RectangleF layoutRectangle, XStringFormat format)
        {
            this.DrawString(s, font, brush, new XRect(layoutRectangle), format);
        }

        public void DrawString(string s, XFont font, XBrush brush, double x, double y, XStringFormat format)
        {
            this.DrawString(s, font, brush, new XRect(x, y, 0.0, 0.0), format);
        }

        public void EndContainer(XGraphicsContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.gsStack.Restore(container.InternalState);
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                this.gfx.Restore(container.GdiState);
            }
            this.transform = container.InternalState.Transform;
            if (this.renderer != null)
            {
                this.renderer.EndContainer(container);
            }
        }

        public static XGraphics FromForm(XForm form)
        {
            if (form.gfx != null)
            {
                return form.gfx;
            }
            return new XGraphics(form);
        }

        public static XGraphics FromGraphics(System.Drawing.Graphics graphics, XSize size) => 
            new XGraphics(graphics, size, XGraphicsUnit.Point, XPageDirection.Downwards);

        public static XGraphics FromGraphics(System.Drawing.Graphics graphics, XSize size, XGraphicsUnit unit) => 
            new XGraphics(graphics, size, unit, XPageDirection.Downwards);

        public static XGraphics FromPdfForm(XPdfForm form)
        {
            if (form.gfx != null)
            {
                return form.gfx;
            }
            return new XGraphics(form);
        }

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page) => 
            new XGraphics(page, XGraphicsPdfPageOptions.Append, XGraphicsUnit.Point, XPageDirection.Downwards);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XGraphicsPdfPageOptions options) => 
            new XGraphics(page, options, XGraphicsUnit.Point, XPageDirection.Downwards);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XGraphicsUnit unit) => 
            new XGraphics(page, XGraphicsPdfPageOptions.Append, unit, XPageDirection.Downwards);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XPageDirection pageDirection) => 
            new XGraphics(page, XGraphicsPdfPageOptions.Append, XGraphicsUnit.Point, pageDirection);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit unit) => 
            new XGraphics(page, options, unit, XPageDirection.Downwards);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XGraphicsPdfPageOptions options, XPageDirection pageDirection) => 
            new XGraphics(page, options, XGraphicsUnit.Point, pageDirection);

        public static XGraphics FromPdfPage(PdfSharp.Pdf.PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit unit, XPageDirection pageDirection) => 
            new XGraphics(page, options, unit, pageDirection);

        private void Initialize()
        {
            this.pageOrigin = new XPoint();
            XMatrix transform = new XMatrix();
            double height = this.pageSize.height;
            PdfSharp.Pdf.PdfPage pdfPage = this.PdfPage;
            XPoint point = new XPoint();
            if ((pdfPage != null) && pdfPage.TrimMargins.AreSet)
            {
                height += pdfPage.TrimMargins.Top.Point + pdfPage.TrimMargins.Bottom.Point;
                point = new XPoint(pdfPage.TrimMargins.Left.Point, pdfPage.TrimMargins.Top.Point);
            }
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                if (this.gfx != null)
                {
                    transform = this.gfx.Transform;
                }
                switch (this.pageUnit)
                {
                    case XGraphicsUnit.Inch:
                        transform.ScalePrepend(72.0);
                        break;

                    case XGraphicsUnit.Millimeter:
                        transform.ScalePrepend(2.8346456692913389);
                        break;

                    case XGraphicsUnit.Centimeter:
                        transform.ScalePrepend(28.346456692913385);
                        break;

                    case XGraphicsUnit.Presentation:
                        transform.ScalePrepend(0.75);
                        break;
                }
            }
            if (this.pageDirection == XPageDirection.Upwards)
            {
                transform.Prepend(new XMatrix(1.0, 0.0, 0.0, -1.0, 0.0, height));
            }
            if (point != new XPoint())
            {
                transform.TranslatePrepend(point.x, point.y);
            }
            this.defaultViewMatrix = transform;
            this.transform = new XMatrix();
        }

        public void IntersectClip(XGraphicsPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (this.drawGraphics && (this.targetContext == XGraphicTargetContext.GDI))
            {
                this.gfx.SetClip(path.gdipPath, CombineMode.Intersect);
            }
            if (this.renderer != null)
            {
                this.renderer.SetClip(path, XCombineMode.Intersect);
            }
        }

        public void IntersectClip(XRect rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            this.IntersectClip(path);
        }

        public void IntersectClip(Rectangle rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            this.IntersectClip(path);
        }

        public void IntersectClip(RectangleF rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            this.IntersectClip(path);
        }

        internal static PointF[] MakePointFArray(XPoint[] points)
        {
            if (points == null)
            {
                return null;
            }
            int length = points.Length;
            PointF[] tfArray = new PointF[length];
            for (int i = 0; i < length; i++)
            {
                tfArray[i].X = (float) points[i].x;
                tfArray[i].Y = (float) points[i].y;
            }
            return tfArray;
        }

        internal static PointF[] MakePointFArray(Point[] points)
        {
            if (points == null)
            {
                return null;
            }
            int length = points.Length;
            PointF[] tfArray = new PointF[length];
            for (int i = 0; i < length; i++)
            {
                tfArray[i].X = points[i].X;
                tfArray[i].Y = points[i].Y;
            }
            return tfArray;
        }

        internal static XPoint[] MakeXPointArray(Point[] points)
        {
            if (points == null)
            {
                return null;
            }
            int length = points.Length;
            XPoint[] pointArray = new XPoint[length];
            for (int i = 0; i < length; i++)
            {
                pointArray[i].x = points[i].X;
                pointArray[i].y = points[i].Y;
            }
            return pointArray;
        }

        internal static XPoint[] MakeXPointArray(PointF[] points)
        {
            if (points == null)
            {
                return null;
            }
            int length = points.Length;
            XPoint[] pointArray = new XPoint[length];
            for (int i = 0; i < length; i++)
            {
                pointArray[i].x = points[i].X;
                pointArray[i].y = points[i].Y;
            }
            return pointArray;
        }

        public XSize MeasureString(string text, XFont font) => 
            this.MeasureString(text, font, XStringFormats.Default);

        public XSize MeasureString(string text, XFont font, XStringFormat stringFormat)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (stringFormat == null)
            {
                throw new ArgumentNullException("stringFormat");
            }
            return XSize.FromSizeF(this.gfx.MeasureString(text, font.RealizeGdiFont(), new PointF(0f, 0f), stringFormat.RealizeGdiStringFormat()));
        }

        public void MultiplyTransform(XMatrix matrix)
        {
            XMatrix transform = new XMatrix();
            transform.Prepend(matrix);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.Prepend(matrix);
            this.AddTransform(transform, order);
        }

        [Obsolete("Use Save/Restore pairs to reset clip area.", true)]
        public void ResetClip()
        {
            throw new InvalidOperationException("ResetClip is obsolete. Use Save/Restore instead.");
        }

        [Obsolete("Use Save/Restore to reset transformation.")]
        public void ResetTransform()
        {
            throw new InvalidOperationException(PSSR.ObsoleteFunktionCalled);
        }

        public void Restore()
        {
            if (this.gsStack.Count == 0)
            {
                throw new InvalidOperationException("Cannot restore without preceding save operation.");
            }
            this.Restore(this.gsStack.Current.state);
        }

        public void Restore(XGraphicsState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                this.gsStack.Restore(state.InternalState);
                this.gfx.Restore(state.GdiState);
                this.transform = state.InternalState.Transform;
            }
            if (this.renderer != null)
            {
                this.renderer.Restore(state);
            }
        }

        public void RotateAtTransform(double angle, XPoint point)
        {
            XMatrix transform = new XMatrix();
            transform.RotateAtPrepend(angle, point);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void RotateAtTransform(double angle, XPoint point, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.RotateAtPrepend(angle, point);
            this.AddTransform(transform, order);
        }

        public void RotateTransform(double angle)
        {
            XMatrix transform = new XMatrix();
            transform.RotatePrepend(angle);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void RotateTransform(double angle, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.RotatePrepend(angle);
            this.AddTransform(transform, order);
        }

        public XGraphicsState Save()
        {
            XGraphicsState state = null;
            if (this.targetContext == XGraphicTargetContext.GDI)
            {
                state = new XGraphicsState(this.gfx.Save());
                InternalGraphicsState state2 = new InternalGraphicsState(this, state) {
                    Transform = this.transform
                };
                this.gsStack.Push(state2);
            }
            if (this.renderer != null)
            {
                this.renderer.Save(state);
            }
            return state;
        }

        public void ScaleTransform(double scaleXY)
        {
            XMatrix transform = new XMatrix();
            transform.ScalePrepend(scaleXY, scaleXY);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void ScaleTransform(double scaleXY, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.ScalePrepend(scaleXY, scaleXY);
            this.AddTransform(transform, order);
        }

        public void ScaleTransform(double scaleX, double scaleY)
        {
            XMatrix transform = new XMatrix();
            transform.ScalePrepend(scaleX, scaleY);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void ScaleTransform(double scaleX, double scaleY, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.ScalePrepend(scaleX, scaleY);
            this.AddTransform(transform, order);
        }

        [Obsolete("Use IntersectClip", true)]
        public void SetClip(XGraphicsPath path)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        [Obsolete("Use IntersectClip", true)]
        public void SetClip(XRect rect)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        [Obsolete("Use IntersectClip")]
        public void SetClip(Rectangle rect)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        [Obsolete("Use IntersectClip")]
        public void SetClip(RectangleF rect)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        [Obsolete("Use IntersectClip", true)]
        public void SetClip(XGraphicsPath path, XCombineMode combineMode)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        [Obsolete("Use IntersectClip", true)]
        public void SetClip(XRect rect, XCombineMode combineMode)
        {
            throw new InvalidOperationException("Function is obsolete. Use IntersectClip.");
        }

        public void ShearTransform(double shearX, double shearY)
        {
            XMatrix transform = new XMatrix();
            transform.ShearPrepend(shearX, shearY);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void ShearTransform(double shearX, double shearY, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.ShearPrepend(shearX, shearY);
            this.AddTransform(transform, order);
        }

        public void TranslateTransform(double dx, double dy)
        {
            XMatrix transform = new XMatrix();
            transform.TranslatePrepend(dx, dy);
            this.AddTransform(transform, XMatrixOrder.Prepend);
        }

        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            XMatrix transform = new XMatrix();
            transform.TranslatePrepend(dx, dy);
            this.AddTransform(transform, order);
        }

        public void WriteComment(string comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            bool drawGraphics = this.drawGraphics;
            if (this.renderer != null)
            {
                this.renderer.WriteComment(comment);
            }
        }

        public System.Drawing.Graphics Graphics =>
            this.gfx;

        public int GraphicsStateLevel =>
            this.gsStack.Count;

        public XGraphicsInternals Internals
        {
            get
            {
                if (this.internals == null)
                {
                    this.internals = new XGraphicsInternals(this);
                }
                return this.internals;
            }
        }

        public PdfFontEmbedding MFEH
        {
            get => 
                this.mfeh;
            set
            {
                this.mfeh = value;
            }
        }

        public PdfFontEncoding MUH
        {
            get => 
                this.muh;
            set
            {
                this.muh = value;
            }
        }

        public XPageDirection PageDirection
        {
            get => 
                this.pageDirection;
            set
            {
                if (value != XPageDirection.Downwards)
                {
                    throw new NotImplementedException("PageDirection must be XPageDirection.Downwards in current implementation.");
                }
            }
        }

        public XPoint PageOrigin
        {
            get => 
                this.pageOrigin;
            set
            {
                if (value != new XPoint())
                {
                    throw new NotImplementedException("PageOrigin cannot be modified in current implementation.");
                }
            }
        }

        public XSize PageSize =>
            this.pageSize;

        public XGraphicsUnit PageUnit =>
            this.pageUnit;

        public PdfSharp.Pdf.PdfPage PdfPage
        {
            get
            {
                XGraphicsPdfRenderer renderer = this.renderer as XGraphicsPdfRenderer;
                if (renderer == null)
                {
                    return null;
                }
                return renderer.page;
            }
        }

        public XSmoothingMode SmoothingMode
        {
            get
            {
                if (this.targetContext == XGraphicTargetContext.GDI)
                {
                    return (XSmoothingMode) this.gfx.SmoothingMode;
                }
                return this.smoothingMode;
            }
            set
            {
                this.smoothingMode = value;
                if (this.targetContext == XGraphicTargetContext.GDI)
                {
                    this.gfx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode) value;
                }
            }
        }

        [Obsolete]
        public XMatrix Transform
        {
            get => 
                this.transform;
            set
            {
                throw new InvalidOperationException(PSSR.ObsoleteFunktionCalled);
            }
        }

        public SpaceTransformer Transformer
        {
            get
            {
                if (this.transformer == null)
                {
                    this.transformer = new SpaceTransformer(this);
                }
                return this.transformer;
            }
        }

        public class SpaceTransformer
        {
            private XGraphics gfx;

            internal SpaceTransformer(XGraphics gfx)
            {
                this.gfx = gfx;
            }

            public XRect WorldToDefaultPage(XRect rect)
            {
                XPoint[] points = new XPoint[] { new XPoint(rect.x, rect.y), new XPoint(rect.x + rect.width, rect.y), new XPoint(rect.x, rect.y + rect.height), new XPoint(rect.x + rect.width, rect.y + rect.height) };
                this.gfx.transform.TransformPoints(points);
                double height = this.gfx.PageSize.height;
                points[0].y = height - points[0].y;
                points[1].y = height - points[1].y;
                points[2].y = height - points[2].y;
                points[3].y = height - points[3].y;
                double x = Math.Min(Math.Min(points[0].x, points[1].x), Math.Min(points[2].x, points[3].x));
                double num3 = Math.Max(Math.Max(points[0].x, points[1].x), Math.Max(points[2].x, points[3].x));
                double y = Math.Min(Math.Min(points[0].y, points[1].y), Math.Min(points[2].y, points[3].y));
                double num5 = Math.Max(Math.Max(points[0].y, points[1].y), Math.Max(points[2].y, points[3].y));
                return new XRect(x, y, num3 - x, num5 - y);
            }
        }

        public class XGraphicsInternals
        {
            private XGraphics gfx;

            internal XGraphicsInternals(XGraphics gfx)
            {
                this.gfx = gfx;
            }

            public void SetPdfTz(double value)
            {
                XGraphicsPdfRenderer renderer = this.gfx.renderer as XGraphicsPdfRenderer;
                if (renderer != null)
                {
                    renderer.AppendFormat(string.Format(CultureInfo.InvariantCulture, "{0:0.###} Tz\n", new object[] { value }), new object[0]);
                }
            }

            public System.Drawing.Graphics Graphics =>
                this.gfx.gfx;
        }
    }
}

