namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class SectionPagesFieldRenderer : NumericFieldRendererBase
    {
        private SectionPagesField sectionPagesField;

        internal SectionPagesFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.sectionPagesField = domObj as SectionPagesField;
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("SECTIONPAGES");
            base.TranslateFormat();
            base.EndField();
        }
    }
}

