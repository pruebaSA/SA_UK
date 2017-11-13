namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal abstract class StyleAndFormatRenderer : RendererBase
    {
        private bool hasCharacterStyle;

        internal StyleAndFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
        }

        protected void EndStyleAndFormatAfterContent()
        {
            if (this.hasCharacterStyle)
            {
                base.rtfWriter.EndContent();
            }
        }

        protected void RenderStyleAndFormat()
        {
            object valueAsIntended = this.GetValueAsIntended("Style");
            object obj3 = valueAsIntended;
            Style style = base.docRenderer.Document.Styles[(string) valueAsIntended];
            this.hasCharacterStyle = false;
            if (style != null)
            {
                if (style.Type == StyleType.Character)
                {
                    this.hasCharacterStyle = true;
                    obj3 = "Normal";
                }
            }
            else
            {
                obj3 = null;
            }
            if (obj3 != null)
            {
                base.rtfWriter.WriteControl("s", base.docRenderer.GetStyleIndex((string) obj3));
            }
            ParagraphFormat domObj = this.GetValueAsIntended("Format") as ParagraphFormat;
            RendererFactory.CreateRenderer(domObj, base.docRenderer).Render();
            base.rtfWriter.WriteControl("brdrbtw");
            if (this.hasCharacterStyle)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControlWithStar("cs", base.docRenderer.GetStyleIndex((string) valueAsIntended));
                object obj4 = this.GetValueAsIntended("Format.Font");
                if (obj4 != null)
                {
                    new FontRenderer((Font) obj4, base.docRenderer).Render();
                }
            }
        }
    }
}

