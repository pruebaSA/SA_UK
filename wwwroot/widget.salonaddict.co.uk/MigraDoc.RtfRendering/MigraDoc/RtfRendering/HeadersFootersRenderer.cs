namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class HeadersFootersRenderer : RendererBase
    {
        private HeadersFooters headersFooters;
        private MigraDoc.DocumentObjectModel.PageSetup pageSetup;

        internal HeadersFootersRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.headersFooters = domObj as HeadersFooters;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            object valueAsIntended = this.GetValueAsIntended("Primary");
            if (valueAsIntended != null)
            {
                this.RenderHeaderFooter((HeaderFooter) valueAsIntended, HeaderFooterIndex.Primary);
            }
            valueAsIntended = this.GetValueAsIntended("FirstPage");
            if (valueAsIntended != null)
            {
                this.RenderHeaderFooter((HeaderFooter) valueAsIntended, HeaderFooterIndex.FirstPage);
            }
            valueAsIntended = this.GetValueAsIntended("EvenPage");
            if (valueAsIntended != null)
            {
                this.RenderHeaderFooter((HeaderFooter) valueAsIntended, HeaderFooterIndex.EvenPage);
            }
        }

        private void RenderHeaderFooter(HeaderFooter hdrFtr, HeaderFooterIndex renderAs)
        {
            new HeaderFooterRenderer(hdrFtr, base.docRenderer) { 
                PageSetup = this.pageSetup,
                RenderAs = renderAs
            }.Render();
        }

        internal MigraDoc.DocumentObjectModel.PageSetup PageSetup
        {
            set
            {
                this.pageSetup = value;
            }
        }
    }
}

