namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    internal class TableRenderer : RendererBase
    {
        private Table table;

        internal TableRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.table = domObj as Table;
        }

        internal override void Render()
        {
            DocumentRelations.GetParent(this.table);
            MergedCellList list = new MergedCellList(this.table);
            foreach (Row row in this.table.Rows)
            {
                new RowRenderer(row, base.docRenderer) { CellList = list }.Render();
            }
        }
    }
}

