namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class PageSetupRenderer : RendererBase
    {
        private PageSetup pageSetup;

        internal PageSetupRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.pageSetup = domObj as PageSetup;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            object valueAsIntended = this.GetValueAsIntended("DifferentFirstPageHeaderFooter");
            if ((valueAsIntended != null) && ((bool) valueAsIntended))
            {
                base.rtfWriter.WriteControl("titlepg");
            }
            valueAsIntended = this.GetValueAsIntended("Orientation");
            if ((valueAsIntended != null) && (((Orientation) valueAsIntended) == Orientation.Landscape))
            {
                base.rtfWriter.WriteControl("lndscpsxn");
            }
            this.RenderPageSize((valueAsIntended == null) ? Orientation.Portrait : ((Orientation) valueAsIntended));
            this.RenderPageDistances();
            this.RenderSectionStart();
        }

        private void RenderPageDistances()
        {
            base.Translate("LeftMargin", "marglsxn");
            base.Translate("RightMargin", "margrsxn");
            base.Translate("TopMargin", "margtsxn");
            base.Translate("BottomMargin", "margbsxn");
            base.Translate("MirrorMargins", "margmirsxn");
            base.Translate("HeaderDistance", "headery");
            base.Translate("FooterDistance", "footery");
        }

        private void RenderPageSize(Orientation orient)
        {
            if (orient == Orientation.Landscape)
            {
                base.RenderUnit("pghsxn", this.pageSetup.PageWidth);
                base.RenderUnit("pgwsxn", this.pageSetup.PageHeight);
            }
            else
            {
                base.RenderUnit("pghsxn", this.pageSetup.PageHeight);
                base.RenderUnit("pgwsxn", this.pageSetup.PageWidth);
            }
        }

        private void RenderSectionStart()
        {
            base.Translate("SectionStart", "sbk");
            object valueAsIntended = this.GetValueAsIntended("StartingNumber");
            if ((valueAsIntended != null) && (((int) valueAsIntended) >= 0))
            {
                base.rtfWriter.WriteControl("pgnrestart");
                base.rtfWriter.WriteControl("pgnstarts", (int) valueAsIntended);
            }
        }
    }
}

