namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class SectionFieldRenderer : NumericFieldRendererBase
    {
        private SectionField sectionField;

        internal SectionFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.sectionField = domObj as SectionField;
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("SECTION");
            base.TranslateFormat();
            base.EndField();
        }
    }
}

