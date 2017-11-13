namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using PdfSharp.Drawing;
    using System;
    using System.Collections;

    internal class TableRenderer : Renderer
    {
        private SortedList bottomBorderMap;
        private SortedList connectedColumnsMap;
        private SortedList connectedRowsMap;
        private int currRow;
        private bool doHorizontalBreak;
        private int endRow;
        private SortedList formattedCells;
        private int lastHeaderColumn;
        private int lastHeaderRow;
        private XUnit leftBorderOffset;
        private MergedCellList mergedCells;
        private int startRow;
        private XUnit startX;
        private XUnit startY;
        private Table table;

        internal TableRenderer(XGraphics gfx, Table documentObject, FieldInfos fieldInfos) : base(gfx, documentObject, fieldInfos)
        {
            this.leftBorderOffset = -1;
            this.endRow = -1;
            this.table = documentObject;
        }

        internal TableRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.leftBorderOffset = -1;
            this.endRow = -1;
            this.table = (Table) base.renderInfo.DocumentObject;
        }

        private XUnit CalcBottomBorderWidth(Cell cell)
        {
            Borders effectiveBorders = this.mergedCells.GetEffectiveBorders(cell);
            if (effectiveBorders != null)
            {
                BordersRenderer renderer = new BordersRenderer(effectiveBorders, base.gfx);
                return renderer.GetWidth(BorderType.Bottom);
            }
            return 0;
        }

        private int CalcLastConnectedColumn(int column)
        {
            int num = column;
            foreach (Cell cell in this.mergedCells)
            {
                if (cell.Column.Index <= num)
                {
                    int num2 = Math.Max(cell.Column.KeepWith, cell.MergeRight);
                    if (num < (cell.Column.Index + num2))
                    {
                        num = cell.Column.Index + num2;
                    }
                }
            }
            return num;
        }

        private int CalcLastConnectedRow(int row)
        {
            int num = row;
            foreach (Cell cell in this.mergedCells)
            {
                if (cell.Row.Index <= num)
                {
                    int num2 = Math.Max(cell.Row.KeepWith, cell.MergeDown);
                    if (num < (cell.Row.Index + num2))
                    {
                        num = cell.Row.Index + num2;
                    }
                }
            }
            return num;
        }

        private void CalcLastHeaderColumn()
        {
            this.lastHeaderColumn = -1;
            foreach (Column column in this.table.Columns)
            {
                if (!column.HeadingFormat)
                {
                    break;
                }
                this.lastHeaderColumn = column.Index;
            }
            if (this.lastHeaderColumn >= 0)
            {
                this.lastHeaderRow = this.CalcLastConnectedColumn(this.lastHeaderColumn);
            }
            if (this.lastHeaderRow == (this.table.Rows.Count - 1))
            {
                this.lastHeaderRow = -1;
            }
        }

        private void CalcLastHeaderRow()
        {
            this.lastHeaderRow = -1;
            foreach (Row row in this.table.Rows)
            {
                if (!row.HeadingFormat)
                {
                    break;
                }
                this.lastHeaderRow = row.Index;
            }
            if (this.lastHeaderRow >= 0)
            {
                this.lastHeaderRow = this.CalcLastConnectedRow(this.lastHeaderRow);
            }
            if (this.lastHeaderRow == (this.table.Rows.Count - 1))
            {
                this.lastHeaderRow = -1;
            }
        }

        private XUnit CalcMaxTopBorderWidth(int row)
        {
            XUnit unit = 0;
            if (this.table.Rows.Count > row)
            {
                int num = this.mergedCells.BinarySearch(this.table[row, 0], new CellComparer());
                Cell cell = this.mergedCells[num];
                while (num < this.mergedCells.Count)
                {
                    cell = this.mergedCells[num];
                    if (cell.Row.Index > row)
                    {
                        return unit;
                    }
                    if (!cell.IsNull("Borders"))
                    {
                        BordersRenderer renderer = new BordersRenderer(cell.Borders, base.gfx);
                        XUnit width = 0;
                        width = renderer.GetWidth(BorderType.Top);
                        if (((double) width) > ((double) unit))
                        {
                            unit = width;
                        }
                    }
                    num++;
                }
            }
            return unit;
        }

        private XUnit CalcStartingHeight()
        {
            XUnit unit = 0;
            if (this.lastHeaderRow >= 0)
            {
                unit = (XUnit) this.bottomBorderMap[this.lastHeaderRow + 1];
                return (((double) unit) + ((double) this.CalcMaxTopBorderWidth(0)));
            }
            if (this.table.Rows.Count > this.startRow)
            {
                unit = this.CalcMaxTopBorderWidth(this.startRow);
            }
            return unit;
        }

        private void CreateBottomBorderMap()
        {
            this.bottomBorderMap = new SortedList();
            this.bottomBorderMap.Add(0, XUnit.FromPoint(0.0));
            while (!this.bottomBorderMap.ContainsKey(this.table.Rows.Count))
            {
                this.CreateNextBottomBorderPosition();
            }
        }

        private void CreateConnectedColumns()
        {
            this.connectedColumnsMap = new SortedList();
            foreach (Cell cell in this.mergedCells)
            {
                if (!this.connectedColumnsMap.ContainsKey(cell.Column.Index))
                {
                    int num = this.CalcLastConnectedColumn(cell.Column.Index);
                    this.connectedColumnsMap[cell.Column.Index] = num;
                }
            }
        }

        private void CreateConnectedRows()
        {
            this.connectedRowsMap = new SortedList();
            foreach (Cell cell in this.mergedCells)
            {
                if (!this.connectedRowsMap.ContainsKey(cell.Row.Index))
                {
                    int num = this.CalcLastConnectedRow(cell.Row.Index);
                    this.connectedRowsMap[cell.Row.Index] = num;
                }
            }
        }

        private void CreateNextBottomBorderPosition()
        {
            int index = this.bottomBorderMap.Count - 1;
            int key = (int) this.bottomBorderMap.GetKey(index);
            XUnit byIndex = (XUnit) this.bottomBorderMap.GetByIndex(index);
            Cell minMergedCell = this.GetMinMergedCell(key);
            FormattedCell cell2 = (FormattedCell) this.formattedCells[minMergedCell];
            XUnit unit2 = ((double) byIndex) + ((double) cell2.InnerHeight);
            unit2 = ((double) unit2) + ((double) this.CalcBottomBorderWidth(minMergedCell));
            foreach (Cell cell3 in this.mergedCells)
            {
                if (cell3.Row.Index > (minMergedCell.Row.Index + minMergedCell.MergeDown))
                {
                    break;
                }
                if ((cell3.Row.Index + cell3.MergeDown) == (minMergedCell.Row.Index + minMergedCell.MergeDown))
                {
                    FormattedCell cell4 = (FormattedCell) this.formattedCells[cell3];
                    XUnit unit3 = (XUnit) this.bottomBorderMap[cell3.Row.Index];
                    XUnit unit4 = ((double) unit3) + ((double) cell4.InnerHeight);
                    unit4 = ((double) unit4) + ((double) this.CalcBottomBorderWidth(cell3));
                    if (((double) unit4) > ((double) unit2))
                    {
                        unit2 = unit4;
                    }
                }
            }
            this.bottomBorderMap.Add((minMergedCell.Row.Index + minMergedCell.MergeDown) + 1, unit2);
        }

        private void FinishLayoutInfo(Area area, XUnit currentHeight, XUnit startingHeight)
        {
            LayoutInfo layoutInfo = base.renderInfo.LayoutInfo;
            layoutInfo.StartingHeight = startingHeight;
            layoutInfo.TrailingHeight = 0;
            if (this.currRow >= 0)
            {
                layoutInfo.ContentArea = new Rectangle(area.X, area.Y, 0, currentHeight);
                XUnit leftBorderOffset = this.LeftBorderOffset;
                foreach (Column column in this.table.Columns)
                {
                    leftBorderOffset = ((double) leftBorderOffset) + ((double) column.Width);
                }
                layoutInfo.ContentArea.Width = leftBorderOffset;
            }
            layoutInfo.MinWidth = layoutInfo.ContentArea.Width;
            if (!this.table.Rows.IsNull("LeftIndent"))
            {
                layoutInfo.Left = this.table.Rows.LeftIndent.Point;
            }
            else if ((this.table.Rows.Alignment == RowAlignment.Left) && (this.table.Columns.Count > 0))
            {
                XUnit unit2 = ((double) this.LeftBorderOffset) + ((double) this.table.Columns[0].LeftPadding);
                layoutInfo.Left = -((double) unit2);
            }
            switch (this.table.Rows.Alignment)
            {
                case RowAlignment.Left:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Near;
                    return;

                case RowAlignment.Center:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Center;
                    return;

                case RowAlignment.Right:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Far;
                    return;
            }
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            DocumentElements parent = DocumentRelations.GetParent(this.table) as DocumentElements;
            if (parent != null)
            {
                Section section = DocumentRelations.GetParent(parent) as Section;
                if (section != null)
                {
                    this.doHorizontalBreak = section.PageSetup.HorizontalPageBreak;
                }
            }
            base.renderInfo = new TableRenderInfo();
            this.InitFormat(area, previousFormatInfo);
            XUnit unit = this.CalcStartingHeight();
            XUnit unit2 = unit;
            XUnit unit3 = 0;
            if ((this.startRow > (this.lastHeaderRow + 1)) && (this.startRow < this.table.Rows.Count))
            {
                unit3 = ((double) ((XUnit) this.bottomBorderMap[this.startRow])) - ((double) unit);
            }
            else
            {
                unit3 = -((double) this.CalcMaxTopBorderWidth(0));
            }
            int startRow = this.startRow;
            XUnit currentHeight = 0;
            XUnit startingHeight = 0;
            bool flag = false;
            while (startRow < this.table.Rows.Count)
            {
                bool flag2 = startRow == this.startRow;
                startRow = (int) this.connectedRowsMap[startRow];
                unit2 = ((double) ((XUnit) this.bottomBorderMap[startRow + 1])) - ((double) unit3);
                if (flag2 && (((double) unit2) > (((double) base.MaxElementHeight) - ((double) Renderer.Tolerance))))
                {
                    unit2 = ((double) base.MaxElementHeight) - ((double) Renderer.Tolerance);
                }
                if (startingHeight == 0)
                {
                    if (((double) unit2) > ((double) area.Height))
                    {
                        flag = true;
                        break;
                    }
                    startingHeight = unit2;
                }
                if (((double) unit2) > ((double) area.Height))
                {
                    break;
                }
                this.currRow = startRow;
                currentHeight = unit2;
                startRow++;
            }
            if (!flag)
            {
                TableFormatInfo formatInfo = (TableFormatInfo) base.renderInfo.FormatInfo;
                formatInfo.startRow = this.startRow;
                formatInfo.isEnding = this.currRow >= (this.table.Rows.Count - 1);
                formatInfo.endRow = this.currRow;
            }
            this.FinishLayoutInfo(area, currentHeight, startingHeight);
        }

        private void FormatCells()
        {
            this.formattedCells = new SortedList(new CellComparer());
            foreach (Cell cell in this.mergedCells)
            {
                FormattedCell cell2 = new FormattedCell(cell, base.documentRenderer, this.mergedCells.GetEffectiveBorders(cell), base.fieldInfos, 0, 0);
                cell2.Format(base.gfx);
                this.formattedCells.Add(cell, cell2);
            }
        }

        private Rectangle GetInnerRect(XUnit startingHeight, Cell cell)
        {
            BordersRenderer renderer = new BordersRenderer(this.mergedCells.GetEffectiveBorders(cell), base.gfx);
            FormattedCell cell2 = (FormattedCell) this.formattedCells[cell];
            XUnit innerWidth = cell2.InnerWidth;
            XUnit startY = this.startY;
            if (cell.Row.Index > this.lastHeaderRow)
            {
                startY = ((double) startY) + ((double) startingHeight);
            }
            else
            {
                startY = ((double) startY) + ((double) this.CalcMaxTopBorderWidth(0));
            }
            XUnit unit3 = (XUnit) this.bottomBorderMap[cell.Row.Index];
            startY = ((double) startY) + ((double) unit3);
            if (cell.Row.Index > this.lastHeaderRow)
            {
                startY = ((double) startY) - ((double) ((XUnit) this.bottomBorderMap[this.startRow]));
            }
            XUnit unit4 = (XUnit) this.bottomBorderMap[(cell.Row.Index + cell.MergeDown) + 1];
            XUnit height = ((double) unit4) - ((double) unit3);
            height = ((double) height) - ((double) renderer.GetWidth(BorderType.Bottom));
            XUnit startX = this.startX;
            for (int i = 0; i < cell.Column.Index; i++)
            {
                startX = ((double) startX) + ((double) this.table.Columns[i].Width);
            }
            return new Rectangle(((double) startX) + ((double) this.LeftBorderOffset), startY, innerWidth, height);
        }

        private Cell GetMinMergedCell(int row)
        {
            int count = this.table.Rows.Count;
            Cell cell = null;
            foreach (Cell cell2 in this.mergedCells)
            {
                if (cell2.Row.Index == row)
                {
                    if (cell2.MergeDown == 0)
                    {
                        return cell2;
                    }
                    if (cell2.MergeDown < count)
                    {
                        count = cell2.MergeDown;
                        cell = cell2;
                    }
                }
                else if (cell2.Row.Index > row)
                {
                    return cell;
                }
            }
            return cell;
        }

        private void InitFormat(Area area, FormatInfo previousFormatInfo)
        {
            TableFormatInfo info = (TableFormatInfo) previousFormatInfo;
            TableRenderInfo info2 = new TableRenderInfo {
                table = this.table
            };
            base.renderInfo = info2;
            if (info != null)
            {
                this.mergedCells = info.mergedCells;
                this.formattedCells = info.formattedCells;
                this.bottomBorderMap = info.bottomBorderMap;
                this.lastHeaderRow = info.lastHeaderRow;
                this.connectedRowsMap = info.connectedRowsMap;
                this.startRow = info.endRow + 1;
            }
            else
            {
                this.mergedCells = new MergedCellList(this.table);
                this.FormatCells();
                this.CalcLastHeaderRow();
                this.CreateConnectedRows();
                this.CreateBottomBorderMap();
                if (this.doHorizontalBreak)
                {
                    this.CalcLastHeaderColumn();
                    this.CreateConnectedColumns();
                }
                this.startRow = this.lastHeaderRow + 1;
            }
            ((TableFormatInfo) info2.FormatInfo).mergedCells = this.mergedCells;
            ((TableFormatInfo) info2.FormatInfo).formattedCells = this.formattedCells;
            ((TableFormatInfo) info2.FormatInfo).bottomBorderMap = this.bottomBorderMap;
            ((TableFormatInfo) info2.FormatInfo).connectedRowsMap = this.connectedRowsMap;
            ((TableFormatInfo) info2.FormatInfo).lastHeaderRow = this.lastHeaderRow;
        }

        private void InitRendering()
        {
            TableFormatInfo formatInfo = (TableFormatInfo) base.renderInfo.FormatInfo;
            this.bottomBorderMap = formatInfo.bottomBorderMap;
            this.connectedRowsMap = formatInfo.connectedRowsMap;
            this.formattedCells = formatInfo.formattedCells;
            this.currRow = formatInfo.startRow;
            this.startRow = formatInfo.startRow;
            this.endRow = formatInfo.endRow;
            this.mergedCells = formatInfo.mergedCells;
            this.lastHeaderRow = formatInfo.lastHeaderRow;
            this.startX = base.renderInfo.LayoutInfo.ContentArea.X;
            this.startY = base.renderInfo.LayoutInfo.ContentArea.Y;
        }

        internal override void Render()
        {
            this.InitRendering();
            this.RenderHeaderRows();
            if (this.startRow < this.table.Rows.Count)
            {
                Cell cell = this.table[this.startRow, 0];
                for (int i = this.mergedCells.BinarySearch(this.table[this.startRow, 0], new CellComparer()); i < this.mergedCells.Count; i++)
                {
                    cell = this.mergedCells[i];
                    if (cell.Row.Index > this.endRow)
                    {
                        return;
                    }
                    this.RenderCell(cell);
                }
            }
        }

        private void RenderBorders(Cell cell, Rectangle innerRect)
        {
            XUnit x = innerRect.X;
            XUnit left = ((double) x) + ((double) innerRect.Width);
            XUnit y = innerRect.Y;
            XUnit top = ((double) innerRect.Y) + ((double) innerRect.Height);
            Borders effectiveBorders = this.mergedCells.GetEffectiveBorders(cell);
            BordersRenderer renderer = new BordersRenderer(effectiveBorders, base.gfx);
            XUnit width = renderer.GetWidth(BorderType.Bottom);
            XUnit unit6 = renderer.GetWidth(BorderType.Left);
            XUnit unit7 = renderer.GetWidth(BorderType.Top);
            XUnit unit8 = renderer.GetWidth(BorderType.Right);
            renderer.RenderVertically(BorderType.Right, left, y, (((double) top) + ((double) width)) - ((double) y));
            renderer.RenderVertically(BorderType.Left, ((double) x) - ((double) unit6), y, (((double) top) + ((double) width)) - ((double) y));
            renderer.RenderHorizontally(BorderType.Bottom, ((double) x) - ((double) unit6), top, ((((double) left) + ((double) unit8)) + ((double) unit6)) - ((double) x));
            renderer.RenderHorizontally(BorderType.Top, ((double) x) - ((double) unit6), ((double) y) - ((double) unit7), ((((double) left) + ((double) unit8)) + ((double) unit6)) - ((double) x));
            this.RenderDiagonalBorders(effectiveBorders, innerRect);
        }

        private void RenderCell(Cell cell)
        {
            Rectangle innerRect = this.GetInnerRect(this.CalcStartingHeight(), cell);
            this.RenderShading(cell, innerRect);
            this.RenderContent(cell, innerRect);
            this.RenderBorders(cell, innerRect);
        }

        private void RenderContent(Cell cell, Rectangle innerRect)
        {
            FormattedCell cell2 = (FormattedCell) this.formattedCells[cell];
            RenderInfo[] renderInfos = cell2.GetRenderInfos();
            if (renderInfos != null)
            {
                XUnit unit3;
                VerticalAlignment verticalAlignment = cell.VerticalAlignment;
                XUnit contentHeight = cell2.ContentHeight;
                XUnit height = innerRect.Height;
                XUnit xShift = ((double) innerRect.X) + ((double) cell.Column.LeftPadding);
                switch (verticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        unit3 = ((double) innerRect.Y) + ((double) innerRect.Height);
                        unit3 = ((double) unit3) - ((double) cell.Row.BottomPadding);
                        unit3 = ((double) unit3) - ((double) contentHeight);
                        break;

                    case VerticalAlignment.Center:
                        unit3 = ((double) innerRect.Y) + ((double) cell.Row.TopPadding);
                        unit3 = ((double) unit3) + ((((double) innerRect.Y) + ((double) innerRect.Height)) - ((double) cell.Row.BottomPadding));
                        unit3 = ((double) unit3) - ((double) contentHeight);
                        unit3 = ((double) unit3) / 2.0;
                        break;

                    default:
                        unit3 = ((double) innerRect.Y) + ((double) cell.Row.TopPadding);
                        break;
                }
                base.RenderByInfos(xShift, unit3, renderInfos);
            }
        }

        private void RenderDiagonalBorders(Borders mergedBorders, Rectangle innerRect)
        {
            BordersRenderer renderer = new BordersRenderer(mergedBorders, base.gfx);
            renderer.RenderDiagonally(BorderType.DiagonalDown, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
            renderer.RenderDiagonally(BorderType.DiagonalUp, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
        }

        private void RenderHeaderRows()
        {
            if (this.lastHeaderRow >= 0)
            {
                foreach (Cell cell in this.mergedCells)
                {
                    if (cell.Row.Index <= this.lastHeaderRow)
                    {
                        this.RenderCell(cell);
                    }
                }
            }
        }

        private void RenderShading(Cell cell, Rectangle innerRect)
        {
            new ShadingRenderer(base.gfx, cell.Shading).Render(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
        }

        internal override LayoutInfo InitialLayoutInfo =>
            new LayoutInfo { 
                KeepTogether=this.table.KeepTogether,
                KeepWithNext=false,
                MarginBottom=0,
                MarginLeft=0,
                MarginTop=0,
                MarginRight=0
            };

        private XUnit LeftBorderOffset
        {
            get
            {
                if (((double) this.leftBorderOffset) < 0.0)
                {
                    if ((this.table.Rows.Count > 0) && (this.table.Columns.Count > 0))
                    {
                        this.leftBorderOffset = new BordersRenderer(this.mergedCells.GetEffectiveBorders(this.table[0, 0]), base.gfx).GetWidth(BorderType.Left);
                    }
                    else
                    {
                        this.leftBorderOffset = 0;
                    }
                }
                return this.leftBorderOffset;
            }
        }
    }
}

