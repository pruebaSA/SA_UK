namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using PdfSharp.Drawing;
    using System;

    internal abstract class Renderer
    {
        protected DocumentObject documentObject;
        protected DocumentRenderer documentRenderer;
        protected MigraDoc.Rendering.FieldInfos fieldInfos;
        protected XGraphics gfx;
        private XUnit maxElementHeight;
        protected MigraDoc.Rendering.RenderInfo renderInfo;
        internal static readonly XUnit Tolerance = XUnit.FromPoint(0.001);

        internal Renderer(XGraphics gfx, DocumentObject documentObject, MigraDoc.Rendering.FieldInfos fieldInfos)
        {
            this.maxElementHeight = -1;
            this.documentObject = documentObject;
            this.gfx = gfx;
            this.fieldInfos = fieldInfos;
        }

        internal Renderer(XGraphics gfx, MigraDoc.Rendering.RenderInfo renderInfo, MigraDoc.Rendering.FieldInfos fieldInfos)
        {
            this.maxElementHeight = -1;
            this.documentObject = renderInfo.DocumentObject;
            this.gfx = gfx;
            this.renderInfo = renderInfo;
            this.fieldInfos = fieldInfos;
        }

        internal static Renderer Create(XGraphics gfx, DocumentRenderer documentRenderer, DocumentObject documentObject, MigraDoc.Rendering.FieldInfos fieldInfos)
        {
            Renderer renderer = null;
            if (documentObject is Paragraph)
            {
                renderer = new ParagraphRenderer(gfx, (Paragraph) documentObject, fieldInfos);
            }
            else if (documentObject is Table)
            {
                renderer = new TableRenderer(gfx, (Table) documentObject, fieldInfos);
            }
            else if (documentObject is PageBreak)
            {
                renderer = new PageBreakRenderer(gfx, (PageBreak) documentObject, fieldInfos);
            }
            else if (documentObject is TextFrame)
            {
                renderer = new TextFrameRenderer(gfx, (TextFrame) documentObject, fieldInfos);
            }
            else if (documentObject is Chart)
            {
                renderer = new ChartRenderer(gfx, (Chart) documentObject, fieldInfos);
            }
            else if (documentObject is Image)
            {
                renderer = new ImageRenderer(gfx, (Image) documentObject, fieldInfos);
            }
            if (renderer != null)
            {
                renderer.documentRenderer = documentRenderer;
            }
            return renderer;
        }

        internal static Renderer Create(XGraphics gfx, DocumentRenderer documentRenderer, MigraDoc.Rendering.RenderInfo renderInfo, MigraDoc.Rendering.FieldInfos fieldInfos)
        {
            Renderer renderer = null;
            if (renderInfo.DocumentObject is Paragraph)
            {
                renderer = new ParagraphRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is Table)
            {
                renderer = new TableRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is PageBreak)
            {
                renderer = new PageBreakRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is TextFrame)
            {
                renderer = new TextFrameRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is Chart)
            {
                renderer = new ChartRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is Chart)
            {
                renderer = new ChartRenderer(gfx, renderInfo, fieldInfos);
            }
            else if (renderInfo.DocumentObject is Image)
            {
                renderer = new ImageRenderer(gfx, renderInfo, fieldInfos);
            }
            if (renderer != null)
            {
                renderer.documentRenderer = documentRenderer;
            }
            return renderer;
        }

        internal abstract void Format(Area area, FormatInfo previousFormatInfo);
        internal abstract void Render();
        protected void RenderByInfos(MigraDoc.Rendering.RenderInfo[] renderInfos)
        {
            this.RenderByInfos(0, 0, renderInfos);
        }

        protected void RenderByInfos(XUnit xShift, XUnit yShift, MigraDoc.Rendering.RenderInfo[] renderInfos)
        {
            if (renderInfos != null)
            {
                foreach (MigraDoc.Rendering.RenderInfo info in renderInfos)
                {
                    XUnit x = info.LayoutInfo.ContentArea.X;
                    XUnit y = info.LayoutInfo.ContentArea.Y;
                    Area contentArea = info.LayoutInfo.ContentArea;
                    contentArea.X = ((double) contentArea.X) + ((double) xShift);
                    Area area2 = info.LayoutInfo.ContentArea;
                    area2.Y = ((double) area2.Y) + ((double) yShift);
                    Create(this.gfx, this.documentRenderer, info, this.fieldInfos).Render();
                    info.LayoutInfo.ContentArea.X = x;
                    info.LayoutInfo.ContentArea.Y = y;
                }
            }
        }

        internal MigraDoc.Rendering.FieldInfos FieldInfos
        {
            set
            {
                this.fieldInfos = value;
            }
        }

        internal abstract LayoutInfo InitialLayoutInfo { get; }

        internal XUnit MaxElementHeight
        {
            get => 
                this.maxElementHeight;
            set
            {
                this.maxElementHeight = value;
            }
        }

        internal MigraDoc.Rendering.RenderInfo RenderInfo =>
            this.renderInfo;
    }
}

