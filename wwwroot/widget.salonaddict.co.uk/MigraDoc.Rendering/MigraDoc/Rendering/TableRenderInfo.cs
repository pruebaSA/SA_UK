namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    internal class TableRenderInfo : RenderInfo
    {
        private TableFormatInfo formatInfo = new TableFormatInfo();
        internal Table table;

        internal TableRenderInfo()
        {
        }

        internal override MigraDoc.DocumentObjectModel.DocumentObject DocumentObject =>
            this.table;

        internal override MigraDoc.Rendering.FormatInfo FormatInfo =>
            this.formatInfo;
    }
}

