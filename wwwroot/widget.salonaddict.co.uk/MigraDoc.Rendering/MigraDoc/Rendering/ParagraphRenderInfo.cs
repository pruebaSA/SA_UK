namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class ParagraphRenderInfo : RenderInfo
    {
        private ParagraphFormatInfo formatInfo = new ParagraphFormatInfo();
        internal Paragraph paragraph;

        internal ParagraphRenderInfo()
        {
        }

        internal override void RemoveEnding()
        {
            ((ParagraphFormatInfo) this.FormatInfo).RemoveEnding();
            Area contentArea = base.LayoutInfo.ContentArea;
            contentArea.Height = ((double) contentArea.Height) - ((double) base.LayoutInfo.TrailingHeight);
        }

        internal override MigraDoc.DocumentObjectModel.DocumentObject DocumentObject =>
            this.paragraph;

        internal override MigraDoc.Rendering.FormatInfo FormatInfo =>
            this.formatInfo;
    }
}

