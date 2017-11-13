namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class FormattedTextArea : IAreaProvider
    {
        private DocumentRenderer documentRenderer;
        private FieldInfos fieldInfos;
        private TopDownFormatter formatter;
        private XGraphics gfx;
        private XUnit innerWidth = (double) 1.0 / (double) 0.0;
        private bool isFirstArea;
        private ArrayList renderInfos;
        internal TextArea textArea;

        internal FormattedTextArea(DocumentRenderer documentRenderer, TextArea textArea, FieldInfos fieldInfos)
        {
            this.textArea = textArea;
            this.fieldInfos = fieldInfos;
            this.documentRenderer = documentRenderer;
        }

        private Rectangle CalcContentRect()
        {
            XUnit width = (((double) this.InnerWidth) - ((double) this.textArea.LeftPadding)) - ((double) this.textArea.RightPadding);
            return new Rectangle(0, 0, width, 1.7976931348623157E+308);
        }

        private XUnit CalcInherentWidth()
        {
            XUnit unit = 0;
            foreach (DocumentObject obj2 in this.textArea.Elements)
            {
                Renderer renderer = Renderer.Create(this.gfx, this.documentRenderer, obj2, this.fieldInfos);
                if (renderer != null)
                {
                    renderer.Format(new Rectangle(0, 0, 1.7976931348623157E+308, 1.7976931348623157E+308), null);
                    unit = Math.Max((double) renderer.RenderInfo.LayoutInfo.MinWidth, (double) unit);
                }
            }
            unit = ((double) unit) + ((double) this.textArea.LeftPadding);
            return (((double) unit) + ((double) this.textArea.RightPadding));
        }

        internal void Format(XGraphics gfx)
        {
            this.gfx = gfx;
            this.isFirstArea = true;
            this.formatter = new TopDownFormatter(this, this.documentRenderer, this.textArea.Elements);
            this.formatter.FormatOnAreas(gfx, false);
        }

        internal RenderInfo[] GetRenderInfos()
        {
            if (this.renderInfos != null)
            {
                return (RenderInfo[]) this.renderInfos.ToArray(typeof(RenderInfo));
            }
            return null;
        }

        Area IAreaProvider.GetNextArea()
        {
            if (this.isFirstArea)
            {
                return this.CalcContentRect();
            }
            return null;
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo) => 
            false;

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo) => 
            false;

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo) => 
            false;

        Area IAreaProvider.ProbeNextArea() => 
            null;

        void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
        {
            this.renderInfos = renderInfos;
        }

        internal XUnit ContentHeight =>
            RenderInfo.GetTotalHeight(this.GetRenderInfos());

        internal XUnit InnerHeight
        {
            get
            {
                if (this.textArea.IsNull("Height"))
                {
                    return ((((double) this.ContentHeight) + ((double) this.textArea.TopPadding)) + ((double) this.textArea.BottomPadding));
                }
                return this.textArea.Height.Point;
            }
        }

        internal XUnit InnerWidth
        {
            get
            {
                if (double.IsNaN((double) this.innerWidth))
                {
                    if (!this.textArea.IsNull("Width"))
                    {
                        this.innerWidth = this.textArea.Width.Point;
                    }
                    else
                    {
                        this.innerWidth = this.CalcInherentWidth();
                    }
                }
                return this.innerWidth;
            }
            set
            {
                this.innerWidth = value;
            }
        }

        FieldInfos IAreaProvider.AreaFieldInfos =>
            this.fieldInfos;
    }
}

