namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class StyleRenderer : RendererBase
    {
        private Style style;
        private Styles styles;

        internal StyleRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.style = domObj as Style;
            this.styles = DocumentRelations.GetParent(this.style) as Styles;
        }

        internal override void Render()
        {
            base.rtfWriter.StartContent();
            int index = this.styles.GetIndex(this.style.Name);
            RendererBase base2 = null;
            if (this.style.Type == StyleType.Character)
            {
                base.rtfWriter.WriteControlWithStar("cs", index);
                base.rtfWriter.WriteControl("additive");
                base2 = RendererFactory.CreateRenderer(this.style.Font, base.docRenderer);
            }
            else
            {
                base.rtfWriter.WriteControl("s", index);
                base2 = RendererFactory.CreateRenderer(this.style.ParagraphFormat, base.docRenderer);
            }
            if (this.style.BaseStyle != "")
            {
                int num2 = this.styles.GetIndex(this.style.BaseStyle);
                base.rtfWriter.WriteControl("sbasedon", num2);
            }
            base2.Render();
            base.rtfWriter.WriteText(this.style.Name);
            base.rtfWriter.WriteSeparator();
            base.rtfWriter.EndContent();
        }
    }
}

