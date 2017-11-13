namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class PageBreakRenderer : RendererBase
    {
        internal PageBreakRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
        }

        internal override void Render()
        {
            base.rtfWriter.WriteControl("page");
        }
    }
}

