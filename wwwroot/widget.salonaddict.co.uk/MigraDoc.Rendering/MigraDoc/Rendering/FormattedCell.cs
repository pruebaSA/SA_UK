namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class FormattedCell : IAreaProvider
    {
        private BordersRenderer bordersRenderer;
        private Cell cell;
        private XUnit contentHeight = 0;
        private DocumentRenderer documentRenderer;
        private FieldInfos fieldInfos;
        private TopDownFormatter formatter;
        private XGraphics gfx;
        private bool isFirstArea = true;
        private ArrayList renderInfos;
        private XUnit xOffset;
        private XUnit yOffset;

        internal FormattedCell(Cell cell, DocumentRenderer documentRenderer, Borders cellBorders, FieldInfos fieldInfos, XUnit xOffset, XUnit yOffset)
        {
            this.cell = cell;
            this.fieldInfos = fieldInfos;
            this.yOffset = yOffset;
            this.xOffset = xOffset;
            this.bordersRenderer = new BordersRenderer(cellBorders, null);
            this.documentRenderer = documentRenderer;
        }

        private XUnit CalcContentHeight(DocumentRenderer documentRenderer)
        {
            XUnit totalHeight = RenderInfo.GetTotalHeight(this.GetRenderInfos());
            if (totalHeight == 0)
            {
                totalHeight = ((double) ParagraphRenderer.GetLineHeight(this.cell.Format, this.gfx, documentRenderer)) + ((double) this.cell.Format.SpaceBefore);
                totalHeight = ((double) totalHeight) + ((double) this.cell.Format.SpaceAfter);
            }
            return totalHeight;
        }

        private Rectangle CalcContentRect()
        {
            Column column = this.cell.Column;
            XUnit width = ((double) this.InnerWidth) - column.LeftPadding.Point;
            Column column2 = this.cell.Table.Columns[column.Index + this.cell.MergeRight];
            width = ((double) width) - column2.RightPadding.Point;
            return new Rectangle(this.xOffset, this.yOffset, width, 1.7976931348623157E+308);
        }

        internal void Format(XGraphics gfx)
        {
            this.gfx = gfx;
            this.formatter = new TopDownFormatter(this, this.documentRenderer, this.cell.Elements);
            this.formatter.FormatOnAreas(gfx, false);
            this.contentHeight = this.CalcContentHeight(this.documentRenderer);
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
                Rectangle rectangle = this.CalcContentRect();
                this.isFirstArea = false;
                return rectangle;
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
            this.contentHeight;

        internal XUnit InnerHeight
        {
            get
            {
                Row row = this.cell.Row;
                XUnit point = row.TopPadding.Point;
                point = ((double) point) + row.BottomPadding.Point;
                switch (row.HeightRule)
                {
                    case RowHeightRule.Auto:
                        return (((double) point) + ((double) this.contentHeight));

                    case RowHeightRule.Exactly:
                        return row.Height.Point;
                }
                return Math.Max((double) row.Height, ((double) point) + ((double) this.contentHeight));
            }
        }

        internal XUnit InnerWidth
        {
            get
            {
                XUnit unit = 0;
                int index = this.cell.Column.Index;
                for (int i = 0; i <= this.cell.MergeRight; i++)
                {
                    int num3 = index + i;
                    unit = ((double) unit) + ((double) this.cell.Table.Columns[num3].Width);
                }
                return (((double) unit) - ((double) this.bordersRenderer.GetWidth(BorderType.Right)));
            }
        }

        FieldInfos IAreaProvider.AreaFieldInfos =>
            this.fieldInfos;
    }
}

