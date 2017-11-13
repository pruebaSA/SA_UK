namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class PageFieldRenderer : NumericFieldRendererBase
    {
        private PageField pageField;

        internal PageFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.pageField = domObj as PageField;
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("PAGE");
            base.TranslateFormat();
            base.EndField();
        }
    }
}

