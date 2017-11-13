namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class TabStopRenderer : RendererBase
    {
        private TabStop tabStop;

        internal TabStopRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.tabStop = domObj as TabStop;
        }

        internal override void Render()
        {
            base.Translate("Alignment", "tq");
            if (!this.tabStop.IsNull("Leader") && (this.tabStop.Leader != TabLeader.Spaces))
            {
                base.Translate("Leader", "tl");
            }
            base.Translate("Position", "tx");
        }
    }
}

