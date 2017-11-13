namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;

    internal class PageBreakRenderer : Renderer
    {
        private PageBreak pageBreak;

        internal PageBreakRenderer(XGraphics gfx, PageBreak pageBreak, FieldInfos fieldInfos) : base(gfx, pageBreak, fieldInfos)
        {
            this.pageBreak = pageBreak;
        }

        internal PageBreakRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            base.renderInfo = renderInfo;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            PageBreakRenderInfo info = new PageBreakRenderInfo {
                pageBreakFormatInfo = new PageBreakFormatInfo()
            };
            base.renderInfo = info;
            info.LayoutInfo.PageBreakBefore = true;
            info.LayoutInfo.ContentArea = new Rectangle(area.Y, area.Y, 0, 0);
            info.pageBreak = this.pageBreak;
        }

        internal override void Render()
        {
        }

        internal override LayoutInfo InitialLayoutInfo =>
            new LayoutInfo { PageBreakBefore=true };
    }
}

