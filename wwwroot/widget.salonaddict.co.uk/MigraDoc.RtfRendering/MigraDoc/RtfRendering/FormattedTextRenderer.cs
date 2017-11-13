namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class FormattedTextRenderer : RendererBase
    {
        private FormattedText formattedText;

        internal FormattedTextRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.formattedText = domObj as FormattedText;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            base.rtfWriter.StartContent();
            this.RenderStyleAndFont();
            foreach (DocumentObject obj2 in this.formattedText.Elements)
            {
                RendererFactory.CreateRenderer(obj2, base.docRenderer).Render();
            }
            base.rtfWriter.EndContent();
        }

        private void RenderStyleAndFont()
        {
            bool flag = false;
            if (!this.formattedText.IsNull("Style"))
            {
                Style style = this.formattedText.Document.Styles[this.formattedText.Style];
                if ((style != null) && (style.Type == StyleType.Character))
                {
                    flag = true;
                }
            }
            if (this.GetValueAsIntended("Font") != null)
            {
                if (flag)
                {
                    base.rtfWriter.WriteControlWithStar("cs", base.docRenderer.GetStyleIndex(this.formattedText.Style));
                }
                RendererFactory.CreateRenderer(this.formattedText.Font, base.docRenderer).Render();
            }
        }
    }
}

