namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    internal class RowRenderer : RendererBase
    {
        private MergedCellList cellList;
        private Row row;

        internal RowRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.row = domObj as Row;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            base.rtfWriter.WriteControl("trowd");
            new RowsRenderer(DocumentRelations.GetParent(this.row) as Rows, base.docRenderer).Render();
            this.RenderRowHeight();
            base.rtfWriter.WriteControl("trkeep");
            base.Translate("HeadingFormat", "trhdr");
            int index = this.row.Index;
            for (int i = 0; i <= this.row.Index; i++)
            {
                object obj2 = this.row.Table.Rows[i].GetValue("KeepWith");
                if ((obj2 != null) && ((((int) obj2) + i) > index))
                {
                    base.rtfWriter.WriteControl("trkeepfollow");
                }
            }
            this.RenderTopBottomPadding();
            for (int j = 0; j < this.row.Table.Columns.Count; j++)
            {
                Cell domObj = this.row.Cells[j];
                new CellFormatRenderer(domObj, base.docRenderer) { CellList = this.cellList }.Render();
            }
            foreach (Cell cell2 in this.row.Cells)
            {
                new CellRenderer(cell2, base.docRenderer) { CellList = this.cellList }.Render();
            }
            base.rtfWriter.WriteControl("row");
        }

        private void RenderRowHeight()
        {
            object valueAsIntended = this.GetValueAsIntended("Height");
            object obj3 = this.GetValueAsIntended("HeightRule");
            if (obj3 != null)
            {
                switch (((RowHeightRule) obj3))
                {
                    case RowHeightRule.AtLeast:
                        base.Translate("Height", "trrh", RtfUnit.Twips, "0", false);
                        return;

                    case RowHeightRule.Auto:
                        base.rtfWriter.WriteControl("trrh", 0);
                        return;

                    case RowHeightRule.Exactly:
                        if (valueAsIntended != null)
                        {
                            Unit unit = (Unit) valueAsIntended;
                            base.RenderUnit("trrh", -unit.Point);
                        }
                        return;
                }
            }
            else
            {
                base.Translate("Height", "trrh", RtfUnit.Twips, "0", false);
            }
        }

        private void RenderTopBottomPadding()
        {
            string str = "trpadd";
            string str2 = "trpaddf";
            object obj2 = this.row.GetValue("TopPadding", GV.GetNull);
            if (obj2 == null)
            {
                obj2 = Unit.FromCentimeter(0.0);
            }
            base.rtfWriter.WriteControl(str + "t", RendererBase.ToRtfUnit((Unit) obj2, RtfUnit.Twips));
            base.rtfWriter.WriteControl(str2 + "t", 3);
            obj2 = this.row.GetValue("BottomPadding", GV.GetNull);
            if (obj2 == null)
            {
                obj2 = Unit.FromCentimeter(0.0);
            }
            base.rtfWriter.WriteControl(str + "b", RendererBase.ToRtfUnit((Unit) obj2, RtfUnit.Twips));
            base.rtfWriter.WriteControl(str2 + "b", 3);
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

