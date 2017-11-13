namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class FootnoteRenderer : RendererBase
    {
        private Footnote footnote;

        public FootnoteRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.footnote = domObj as Footnote;
        }

        internal override void Render()
        {
            this.RenderReference();
            this.RenderContent();
        }

        private void RenderContent()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("footnote");
            foreach (DocumentObject obj2 in this.footnote.Elements)
            {
                RendererBase base2 = RendererFactory.CreateRenderer(obj2, base.docRenderer);
                if (base2 != null)
                {
                    base2.Render();
                }
            }
            base.rtfWriter.EndContent();
        }

        internal void RenderReference()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("super");
            if (this.footnote.IsNull("Reference"))
            {
                base.rtfWriter.WriteControl("chftn");
            }
            else
            {
                base.rtfWriter.WriteText(this.footnote.Reference);
            }
            base.rtfWriter.EndContent();
        }
    }
}

