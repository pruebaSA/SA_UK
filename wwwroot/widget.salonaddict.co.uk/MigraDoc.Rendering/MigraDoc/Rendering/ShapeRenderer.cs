namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using PdfSharp.Drawing;
    using System;

    internal abstract class ShapeRenderer : Renderer
    {
        protected FillFormatRenderer fillFormatRenderer;
        protected LineFormatRenderer lineFormatRenderer;
        protected Shape shape;

        internal ShapeRenderer(XGraphics gfx, Shape shape, FieldInfos fieldInfos) : base(gfx, shape, fieldInfos)
        {
            this.shape = shape;
            LineFormat lineFormat = (LineFormat) this.shape.GetValue("LineFormat", GV.ReadOnly);
            this.lineFormatRenderer = new LineFormatRenderer(lineFormat, gfx);
        }

        internal ShapeRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.shape = (Shape) renderInfo.DocumentObject;
            LineFormat lineFormat = (LineFormat) this.shape.GetValue("LineFormat", GV.ReadOnly);
            this.lineFormatRenderer = new LineFormatRenderer(lineFormat, gfx);
            FillFormat fillFormat = (FillFormat) this.shape.GetValue("FillFormat", GV.ReadOnly);
            this.fillFormatRenderer = new FillFormatRenderer(fillFormat, gfx);
        }

        private void FinishLayoutInfo(Area area)
        {
            LayoutInfo layoutInfo = base.renderInfo.LayoutInfo;
            Area area2 = new Rectangle(area.X, area.Y, this.ShapeWidth, this.ShapeHeight);
            layoutInfo.ContentArea = area2;
            layoutInfo.MarginTop = this.shape.WrapFormat.DistanceTop.Point;
            layoutInfo.MarginLeft = this.shape.WrapFormat.DistanceLeft.Point;
            layoutInfo.MarginBottom = this.shape.WrapFormat.DistanceBottom.Point;
            layoutInfo.MarginRight = this.shape.WrapFormat.DistanceRight.Point;
            layoutInfo.KeepTogether = true;
            layoutInfo.KeepWithNext = false;
            layoutInfo.PageBreakBefore = false;
            layoutInfo.MinWidth = this.ShapeWidth;
            if (this.shape.Top.ShapePosition == ShapePosition.Undefined)
            {
                layoutInfo.Top = this.shape.Top.Position.Point;
            }
            layoutInfo.VerticalAlignment = this.GetVerticalAlignment();
            layoutInfo.HorizontalAlignment = this.GetHorizontalAlignment();
            if (this.shape.Left.ShapePosition == ShapePosition.Undefined)
            {
                layoutInfo.Left = this.shape.Left.Position.Point;
            }
            layoutInfo.HorizontalReference = this.GetHorizontalReference();
            layoutInfo.VerticalReference = this.GetVerticalReference();
            layoutInfo.Floating = this.GetFloating();
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            bool flag = (this.GetFloating() == Floating.None) || (((double) this.ShapeHeight) <= ((double) area.Height));
            ((ShapeFormatInfo) base.renderInfo.FormatInfo).fits = flag;
            this.FinishLayoutInfo(area);
        }

        private Floating GetFloating()
        {
            if ((this.shape.RelativeVertical != RelativeVertical.Line) && (this.shape.RelativeVertical != RelativeVertical.Paragraph))
            {
                return Floating.None;
            }
            switch (this.shape.WrapFormat.Style)
            {
                case WrapStyle.None:
                case WrapStyle.Through:
                    return Floating.None;
            }
            return Floating.TopBottom;
        }

        private ElementAlignment GetHorizontalAlignment()
        {
            switch (this.shape.Left.ShapePosition)
            {
                case ShapePosition.Right:
                    return ElementAlignment.Far;

                case ShapePosition.Center:
                    return ElementAlignment.Center;

                case ShapePosition.Inside:
                    return ElementAlignment.Inside;

                case ShapePosition.Outside:
                    return ElementAlignment.Outside;
            }
            return ElementAlignment.Near;
        }

        private HorizontalReference GetHorizontalReference()
        {
            switch (this.shape.RelativeHorizontal)
            {
                case RelativeHorizontal.Margin:
                    return HorizontalReference.PageMargin;

                case RelativeHorizontal.Page:
                    return HorizontalReference.Page;
            }
            return HorizontalReference.AreaBoundary;
        }

        private ElementAlignment GetVerticalAlignment()
        {
            switch (this.shape.Top.ShapePosition)
            {
                case ShapePosition.Center:
                    return ElementAlignment.Center;

                case ShapePosition.Bottom:
                    return ElementAlignment.Far;
            }
            return ElementAlignment.Near;
        }

        private VerticalReference GetVerticalReference()
        {
            switch (this.shape.RelativeVertical)
            {
                case RelativeVertical.Margin:
                    return VerticalReference.PageMargin;

                case RelativeVertical.Page:
                    return VerticalReference.Page;
            }
            return VerticalReference.PreviousElement;
        }

        protected void RenderFilling()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            this.fillFormatRenderer.Render(contentArea.X, contentArea.Y, contentArea.Width, contentArea.Height);
        }

        protected void RenderLine()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            XUnit width = this.lineFormatRenderer.GetWidth();
            XUnit unit2 = ((double) contentArea.Width) - ((double) width);
            XUnit height = ((double) contentArea.Height) - ((double) width);
            this.lineFormatRenderer.Render(contentArea.X, contentArea.Y, unit2, height);
        }

        internal override LayoutInfo InitialLayoutInfo
        {
            get
            {
                LayoutInfo info = new LayoutInfo {
                    MarginTop = this.shape.WrapFormat.DistanceTop.Point,
                    MarginLeft = this.shape.WrapFormat.DistanceLeft.Point,
                    MarginBottom = this.shape.WrapFormat.DistanceBottom.Point,
                    MarginRight = this.shape.WrapFormat.DistanceRight.Point,
                    KeepTogether = true,
                    KeepWithNext = false,
                    PageBreakBefore = false,
                    VerticalReference = this.GetVerticalReference(),
                    HorizontalReference = this.GetHorizontalReference(),
                    Floating = this.GetFloating()
                };
                if ((info.Floating == Floating.TopBottom) && !this.shape.Top.Position.IsEmpty)
                {
                    info.MarginTop = Math.Max((double) info.MarginTop, (double) this.shape.Top.Position);
                }
                return info;
            }
        }

        protected virtual XUnit ShapeHeight =>
            (((double) this.shape.Height) + ((double) this.lineFormatRenderer.GetWidth()));

        protected virtual XUnit ShapeWidth =>
            (((double) this.shape.Width) + ((double) this.lineFormatRenderer.GetWidth()));
    }
}

