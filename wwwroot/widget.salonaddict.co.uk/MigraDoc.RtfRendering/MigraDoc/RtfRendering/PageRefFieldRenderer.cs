namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class PageRefFieldRenderer : NumericFieldRendererBase
    {
        private PageRefField pageRefField;

        internal PageRefFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.pageRefField = domObj as PageRefField;
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("PAGEREF ");
            base.rtfWriter.WriteText(BookmarkFieldRenderer.MakeValidBookmarkName(this.pageRefField.Name));
            base.TranslateFormat();
            base.EndField();
        }
    }
}

