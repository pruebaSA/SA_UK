namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class ShadingRenderer : RendererBase
    {
        private bool isCellShading;
        private Shading shading;

        internal ShadingRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.shading = domObj as Shading;
            this.isCellShading = !(DocumentRelations.GetParent(this.shading) is ParagraphFormat);
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            object valueAsIntended = this.GetValueAsIntended("visible");
            object obj3 = this.GetValueAsIntended("Color");
            if (((valueAsIntended == null) || ((bool) valueAsIntended)) && (obj3 != null))
            {
                base.Translate("Color", this.isCellShading ? "clcbpat" : "cbpat");
            }
        }
    }
}

