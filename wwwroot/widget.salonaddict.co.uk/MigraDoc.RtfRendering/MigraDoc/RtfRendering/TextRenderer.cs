namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class TextRenderer : RendererBase
    {
        private Text text;

        internal TextRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.text = domObj as Text;
        }

        internal override void Render()
        {
            if (!this.text.IsNull("Content"))
            {
                base.rtfWriter.WriteText(this.text.Content);
            }
        }
    }
}

