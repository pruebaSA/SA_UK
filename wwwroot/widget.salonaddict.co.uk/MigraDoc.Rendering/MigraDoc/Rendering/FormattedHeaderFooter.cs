namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class FormattedHeaderFooter : IAreaProvider
    {
        private XUnit contentHeight;
        private Rectangle contentRect;
        private DocumentRenderer documentRenderer;
        private FieldInfos fieldInfos;
        private TopDownFormatter formatter;
        private XGraphics gfx;
        private HeaderFooter headerFooter;
        private bool isFirstArea;
        private ArrayList renderInfos;

        internal FormattedHeaderFooter(HeaderFooter headerFooter, DocumentRenderer documentRenderer, FieldInfos fieldInfos)
        {
            this.headerFooter = headerFooter;
            this.fieldInfos = fieldInfos;
            this.documentRenderer = documentRenderer;
        }

        internal void Format(XGraphics gfx)
        {
            this.gfx = gfx;
            this.isFirstArea = true;
            this.formatter = new TopDownFormatter(this, this.documentRenderer, this.headerFooter.Elements);
            this.formatter.FormatOnAreas(gfx, false);
            this.contentHeight = RenderInfo.GetTotalHeight(this.GetRenderInfos());
        }

        internal RenderInfo[] GetRenderInfos()
        {
            if (this.renderInfos != null)
            {
                return (RenderInfo[]) this.renderInfos.ToArray(typeof(RenderInfo));
            }
            return new RenderInfo[0];
        }

        Area IAreaProvider.GetNextArea()
        {
            if (this.isFirstArea)
            {
                return new Rectangle(this.ContentRect.X, this.ContentRect.Y, this.ContentRect.Width, 1.7976931348623157E+308);
            }
            return null;
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo) => 
            false;

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo) => 
            ((IAreaProvider) this.documentRenderer.FormattedDocument).PositionHorizontally(layoutInfo);

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo) => 
            ((IAreaProvider) this.documentRenderer.FormattedDocument).PositionVertically(layoutInfo);

        Area IAreaProvider.ProbeNextArea() => 
            null;

        void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
        {
            this.renderInfos = renderInfos;
        }

        private XUnit ContentHeight =>
            this.contentHeight;

        internal Rectangle ContentRect
        {
            get => 
                this.contentRect;
            set
            {
                this.contentRect = value;
            }
        }

        FieldInfos IAreaProvider.AreaFieldInfos =>
            this.fieldInfos;
    }
}

