namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    internal class CellRenderer : StyleAndFormatRenderer
    {
        private Cell cell;
        private MergedCellList cellList;

        internal CellRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.cell = domObj as Cell;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            Cell coveringCell = this.cellList.GetCoveringCell(this.cell);
            if (this.cell.Column.Index == coveringCell.Column.Index)
            {
                bool flag = false;
                if (!this.cell.IsNull("Elements") && (this.cell == coveringCell))
                {
                    foreach (DocumentObject obj2 in this.cell.Elements)
                    {
                        RendererBase base2 = RendererFactory.CreateRenderer(obj2, base.docRenderer);
                        if (base2 != null)
                        {
                            base2.Render();
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    base.rtfWriter.WriteControl("pard");
                    base.RenderStyleAndFormat();
                    base.rtfWriter.WriteControl("intbl");
                    base.EndStyleAndFormatAfterContent();
                }
                base.rtfWriter.WriteControl("cell");
            }
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

