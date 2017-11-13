namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class SectionRenderer : RendererBase
    {
        private Section section;

        internal SectionRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.section = domObj as Section;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            Sections parent = DocumentRelations.GetParent(this.section) as Sections;
            if (this.section != parent.First)
            {
                base.rtfWriter.WriteControl("pard");
                base.rtfWriter.WriteControl("sect");
            }
            base.rtfWriter.WriteControl("sectd");
            base.docRenderer.RenderSectionProperties();
            object pageSetup = this.section.PageSetup;
            if (pageSetup != null)
            {
                RendererFactory.CreateRenderer((PageSetup) pageSetup, base.docRenderer).Render();
            }
            object valueAsIntended = this.GetValueAsIntended("Headers");
            if (valueAsIntended != null)
            {
                new HeadersFootersRenderer(valueAsIntended as HeadersFooters, base.docRenderer) { PageSetup = (PageSetup) pageSetup }.Render();
            }
            object obj4 = this.GetValueAsIntended("Footers");
            if (obj4 != null)
            {
                new HeadersFootersRenderer(obj4 as HeadersFooters, base.docRenderer) { PageSetup = (PageSetup) pageSetup }.Render();
            }
            if (!this.section.IsNull("Elements"))
            {
                foreach (DocumentObject obj5 in this.section.Elements)
                {
                    RendererBase base2 = RendererFactory.CreateRenderer(obj5, base.docRenderer);
                    if (base2 != null)
                    {
                        base2.Render();
                    }
                }
            }
        }
    }
}

