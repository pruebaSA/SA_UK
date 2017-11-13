namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class FormattedTextFrame : IAreaProvider
    {
        private XUnit contentHeight;
        private DocumentRenderer documentRenderer;
        private FieldInfos fieldInfos;
        private TopDownFormatter formatter;
        private XGraphics gfx;
        private bool isFirstArea;
        private ArrayList renderInfos;
        private TextFrame textframe;

        internal FormattedTextFrame(TextFrame textframe, DocumentRenderer documentRenderer, FieldInfos fieldInfos)
        {
            this.textframe = textframe;
            this.fieldInfos = fieldInfos;
            this.documentRenderer = documentRenderer;
        }

        private Rectangle CalcContentRect()
        {
            XUnit point;
            XUnit width = new LineFormatRenderer(this.textframe.LineFormat, this.gfx).GetWidth();
            XUnit x = ((double) width) / 2.0;
            XUnit y = ((double) width) / 2.0;
            if ((this.textframe.Orientation == TextOrientation.Horizontal) || (this.textframe.Orientation == TextOrientation.HorizontalRotatedFarEast))
            {
                point = this.textframe.Width.Point;
                x = ((double) x) + ((double) this.textframe.MarginLeft);
                y = ((double) y) + ((double) this.textframe.MarginTop);
                point = ((double) point) - ((double) x);
                point = ((double) point) - (((double) this.textframe.MarginRight) + (((double) width) / 2.0));
            }
            else
            {
                point = this.textframe.Height.Point;
                if (this.textframe.Orientation == TextOrientation.Upward)
                {
                    x = ((double) x) + ((double) this.textframe.MarginBottom);
                    y = ((double) y) + ((double) this.textframe.MarginLeft);
                    point = ((double) point) - ((double) x);
                    point = ((double) point) - (((double) this.textframe.MarginTop) + (((double) width) / 2.0));
                }
                else
                {
                    x = ((double) x) + ((double) this.textframe.MarginTop);
                    y = ((double) y) + ((double) this.textframe.MarginRight);
                    point = ((double) point) - ((double) x);
                    point = ((double) point) - (((double) this.textframe.MarginBottom) + (((double) width) / 2.0));
                }
            }
            return new Rectangle(x, y, point, 1.7976931348623157E+308);
        }

        internal void Format(XGraphics gfx)
        {
            this.gfx = gfx;
            this.isFirstArea = true;
            this.formatter = new TopDownFormatter(this, this.documentRenderer, this.textframe.Elements);
            this.formatter.FormatOnAreas(gfx, false);
            this.contentHeight = RenderInfo.GetTotalHeight(this.GetRenderInfos());
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

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            XUnit unit;
            Rectangle rectangle = this.CalcContentRect();
            switch (layoutInfo.HorizontalAlignment)
            {
                case ElementAlignment.Near:
                {
                    if (layoutInfo.Left == 0)
                    {
                        return false;
                    }
                    Area contentArea = layoutInfo.ContentArea;
                    contentArea.X = ((double) contentArea.X) + ((double) layoutInfo.Left);
                    return true;
                }
                case ElementAlignment.Center:
                    unit = ((double) rectangle.Width) - ((double) layoutInfo.ContentArea.Width);
                    unit = ((double) rectangle.X) + (((double) unit) / 2.0);
                    layoutInfo.ContentArea.X = unit;
                    return true;

                case ElementAlignment.Far:
                    unit = ((double) rectangle.X) + ((double) rectangle.Width);
                    unit = ((double) unit) - ((double) layoutInfo.ContentArea.Width);
                    unit = ((double) unit) - ((double) layoutInfo.MarginRight);
                    layoutInfo.ContentArea.X = unit;
                    return true;
            }
            return false;
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo) => 
            false;

        Area IAreaProvider.ProbeNextArea() => 
            null;

        void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
        {
            this.renderInfos = renderInfos;
        }

        private XUnit ContentHeight =>
            this.contentHeight;

        FieldInfos IAreaProvider.AreaFieldInfos =>
            this.fieldInfos;
    }
}

