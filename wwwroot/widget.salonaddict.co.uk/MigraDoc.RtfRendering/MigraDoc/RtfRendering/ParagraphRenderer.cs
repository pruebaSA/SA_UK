namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    internal class ParagraphRenderer : StyleAndFormatRenderer
    {
        private Paragraph paragraph;

        public ParagraphRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.paragraph = domObj as Paragraph;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            DocumentElements parent = DocumentRelations.GetParent(this.paragraph) as DocumentElements;
            base.rtfWriter.WriteControl("pard");
            bool flag = DocumentRelations.GetParent(parent) is Cell;
            bool flag2 = !flag && (DocumentRelations.GetParent(parent) is Footnote);
            if (flag)
            {
                base.rtfWriter.WriteControl("intbl");
            }
            base.RenderStyleAndFormat();
            if (!this.paragraph.IsNull("Elements"))
            {
                this.RenderContent();
            }
            base.EndStyleAndFormatAfterContent();
            if ((!flag && !flag2) || (this.paragraph != parent.LastObject))
            {
                base.rtfWriter.WriteControl("par");
            }
        }

        private void RenderContent()
        {
            DocumentElements parent = DocumentRelations.GetParent(this.paragraph) as DocumentElements;
            if ((DocumentRelations.GetParent(parent) is Footnote) && (this.paragraph == parent.First))
            {
                new FootnoteRenderer(DocumentRelations.GetParent(parent) as Footnote, base.docRenderer).RenderReference();
            }
            foreach (DocumentObject obj2 in this.paragraph.Elements)
            {
                if (((obj2 != this.paragraph.Elements.LastObject) || !(obj2 is Character)) || (((Character) obj2).SymbolName != ((SymbolName) (-201326591))))
                {
                    RendererBase base2 = RendererFactory.CreateRenderer(obj2, base.docRenderer);
                    if (base2 != null)
                    {
                        base2.Render();
                    }
                }
            }
        }
    }
}

