namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class PageBreakRenderInfo : RenderInfo
    {
        internal PageBreak pageBreak;
        internal PageBreakFormatInfo pageBreakFormatInfo;

        internal PageBreakRenderInfo()
        {
        }

        internal override MigraDoc.DocumentObjectModel.DocumentObject DocumentObject =>
            this.pageBreak;

        internal override MigraDoc.Rendering.FormatInfo FormatInfo =>
            this.pageBreakFormatInfo;
    }
}

