namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class ParagraphFormatRenderer : RendererBase
    {
        private ParagraphFormat format;

        internal ParagraphFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.format = domObj as ParagraphFormat;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            base.Translate("Alignment", "q");
            base.Translate("SpaceBefore", "sb");
            base.Translate("SpaceAfter", "sa");
            base.TranslateBool("WidowControl", "widctlpar", "nowidctlpar", false);
            base.Translate("PageBreakBefore", "pagebb");
            base.Translate("KeepTogether", "keep");
            base.Translate("KeepWithNext", "keepn");
            base.Translate("FirstLineIndent", "fi");
            base.Translate("LeftIndent", "li");
            base.Translate("LeftIndent", "lin");
            base.Translate("RightIndent", "ri");
            base.Translate("RightIndent", "rin");
            object valueAsIntended = this.GetValueAsIntended("OutlineLevel");
            if ((valueAsIntended != null) && (((OutlineLevel) valueAsIntended) != OutlineLevel.BodyText))
            {
                base.Translate("OutlineLevel", "outlinelevel");
            }
            Unit unit = (Unit) this.GetValueAsIntended("LineSpacing");
            switch (((LineSpacingRule) this.GetValueAsIntended("LineSpacingRule")))
            {
                case LineSpacingRule.OnePtFive:
                    base.rtfWriter.WriteControl("sl", 360);
                    break;

                case LineSpacingRule.Double:
                    base.rtfWriter.WriteControl("sl", 480);
                    break;

                case LineSpacingRule.AtLeast:
                    base.Translate("LineSpacing", "sl");
                    break;

                case LineSpacingRule.Exactly:
                    base.rtfWriter.WriteControl("sl", RendererBase.ToTwips(-unit.Point));
                    break;

                case LineSpacingRule.Multiple:
                    base.rtfWriter.WriteControl("sl", RendererBase.ToRtfUnit(unit, RtfUnit.Lines));
                    break;
            }
            base.Translate("LineSpacingRule", "slmult");
            object obj3 = this.GetValueAsIntended("Shading");
            if (obj3 != null)
            {
                new ShadingRenderer((DocumentObject) obj3, base.docRenderer).Render();
            }
            object obj4 = this.GetValueAsIntended("Font");
            if (obj4 != null)
            {
                RendererFactory.CreateRenderer((Font) obj4, base.docRenderer).Render();
            }
            object obj5 = this.GetValueAsIntended("Borders");
            if (obj5 != null)
            {
                new BordersRenderer((Borders) obj5, base.docRenderer) { ParentFormat = this.format }.Render();
            }
            object obj6 = this.GetValueAsIntended("TabStops");
            if (obj6 != null)
            {
                RendererFactory.CreateRenderer((TabStops) obj6, base.docRenderer).Render();
            }
            object obj7 = this.GetValueAsIntended("ListInfo");
            if (obj7 != null)
            {
                int listNumber = ListInfoOverrideRenderer.GetListNumber((ListInfo) obj7);
                if (listNumber > 0)
                {
                    base.rtfWriter.WriteControl("ls", listNumber);
                }
            }
        }
    }
}

