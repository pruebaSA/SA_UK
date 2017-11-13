namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    internal class CellFormatRenderer : RendererBase
    {
        private Cell cell;
        private MergedCellList cellList;
        private Cell coveringCell;

        internal CellFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.cell = domObj as Cell;
        }

        private int GetRightCellBoundary()
        {
            int num = this.coveringCell.Column.Index + this.coveringCell.MergeRight;
            double point = RowsRenderer.CalculateLeftIndent(this.cell.Table.Rows).Point;
            for (int i = 0; i <= num; i++)
            {
                object obj2 = this.cell.Table.Columns[i].GetValue("Width", GV.GetNull);
                if (obj2 != null)
                {
                    Unit unit2 = (Unit) obj2;
                    point += unit2.Point;
                }
                else
                {
                    Unit unit3 = "2.5cm";
                    point += unit3.Point;
                }
            }
            return RendererBase.ToRtfUnit(new Unit(point), RtfUnit.Twips);
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            this.coveringCell = this.cellList.GetCoveringCell(this.cell);
            Borders effectiveBorders = this.cellList.GetEffectiveBorders(this.coveringCell);
            if (this.cell.Column.Index == this.coveringCell.Column.Index)
            {
                if (effectiveBorders != null)
                {
                    new BordersRenderer(effectiveBorders, base.docRenderer) { 
                        leaveAwayLeft = this.cell.Column.Index != this.coveringCell.Column.Index,
                        leaveAwayTop = this.cell.Row.Index != this.coveringCell.Row.Index,
                        leaveAwayBottom = this.cell.Row.Index != (this.coveringCell.Row.Index + this.coveringCell.MergeDown),
                        leaveAwayRight = false,
                        ParentCell = this.cell
                    }.Render();
                }
                if (this.cell == this.coveringCell)
                {
                    this.RenderLeftRightPadding();
                    base.Translate("VerticalAlignment", "clvertal");
                }
                object obj2 = this.coveringCell.GetValue("Shading", GV.GetNull);
                if (obj2 != null)
                {
                    new ShadingRenderer((DocumentObject) obj2, base.docRenderer).Render();
                }
                if ((this.cell.Row.Index == this.coveringCell.Row.Index) && (this.coveringCell.MergeDown > 0))
                {
                    base.rtfWriter.WriteControl("clvmgf");
                }
                if (this.cell.Row.Index > this.coveringCell.Row.Index)
                {
                    base.rtfWriter.WriteControl("clvmrg");
                }
                base.rtfWriter.WriteControl("cellx", this.GetRightCellBoundary());
            }
        }

        private void RenderLeftRightPadding()
        {
            string str = "clpad";
            string str2 = "clpadf";
            object obj2 = this.cell.Column.GetValue("LeftPadding", GV.GetNull);
            if (obj2 == null)
            {
                obj2 = Unit.FromCentimeter(0.12);
            }
            base.rtfWriter.WriteControl(str + "t", RendererBase.ToRtfUnit((Unit) obj2, RtfUnit.Twips));
            base.rtfWriter.WriteControl(str2 + "t", 3);
            obj2 = this.cell.Column.GetValue("RightPadding", GV.GetNull);
            if (obj2 == null)
            {
                obj2 = Unit.FromCentimeter(0.12);
            }
            base.rtfWriter.WriteControl(str + "r", RendererBase.ToRtfUnit((Unit) obj2, RtfUnit.Twips));
            base.rtfWriter.WriteControl(str2 + "r", 3);
        }

        internal MergedCellList CellList
        {
            set
            {
                this.cellList = value;
            }
        }
    }
}

