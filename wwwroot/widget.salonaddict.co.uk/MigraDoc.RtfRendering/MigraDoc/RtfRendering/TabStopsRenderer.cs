namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class TabStopsRenderer : RendererBase
    {
        private TabStops tabStops;

        internal TabStopsRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.tabStops = domObj as TabStops;
        }

        internal override void Render()
        {
            foreach (TabStop stop in this.tabStops)
            {
                RendererFactory.CreateRenderer(stop, base.docRenderer).Render();
            }
        }
    }
}

