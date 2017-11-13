namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class NumPagesFieldRenderer : NumericFieldRendererBase
    {
        private NumPagesField numPagesField;

        internal NumPagesFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.numPagesField = domObj as NumPagesField;
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("NUMPAGES");
            base.TranslateFormat();
            base.EndField();
        }
    }
}

