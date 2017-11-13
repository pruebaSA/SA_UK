namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    internal class FontRenderer : RendererBase
    {
        private Font font;

        internal FontRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.font = domObj as Font;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            string valueAsIntended = (string) this.GetValueAsIntended("Name");
            if (valueAsIntended != null)
            {
                base.rtfWriter.WriteControl("f", base.docRenderer.GetFontIndex(valueAsIntended));
            }
            base.Translate("Size", "fs", RtfUnit.HalfPts, (string) null, false);
            base.TranslateBool("Bold", "b", "b0", false);
            base.Translate("Underline", "ul");
            base.TranslateBool("Italic", "i", "i0", false);
            base.Translate("Color", "cf");
            object obj2 = this.font.GetValue("Subscript", GV.GetNull);
            if ((obj2 != null) && ((bool) obj2))
            {
                base.rtfWriter.WriteControl("sub");
            }
            object obj3 = this.font.GetValue("Superscript", GV.GetNull);
            if ((obj3 != null) && ((bool) obj3))
            {
                base.rtfWriter.WriteControl("super");
            }
        }
    }
}

