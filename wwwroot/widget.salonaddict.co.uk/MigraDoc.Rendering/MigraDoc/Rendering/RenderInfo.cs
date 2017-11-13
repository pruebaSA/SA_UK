namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;

    internal abstract class RenderInfo
    {
        private MigraDoc.Rendering.LayoutInfo layoutInfo = new MigraDoc.Rendering.LayoutInfo();

        protected RenderInfo()
        {
        }

        internal static XUnit GetTotalHeight(RenderInfo[] renderInfos)
        {
            if ((renderInfos == null) || (renderInfos.Length == 0))
            {
                return 0;
            }
            int index = renderInfos.Length - 1;
            RenderInfo info = renderInfos[0];
            RenderInfo info2 = renderInfos[index];
            MigraDoc.Rendering.LayoutInfo layoutInfo = info.LayoutInfo;
            MigraDoc.Rendering.LayoutInfo info4 = info2.LayoutInfo;
            XUnit unit = ((double) layoutInfo.ContentArea.Y) - ((double) layoutInfo.MarginTop);
            XUnit unit2 = ((double) info4.ContentArea.Y) + ((double) info4.ContentArea.Height);
            unit2 = ((double) unit2) + ((double) info4.MarginBottom);
            return (((double) unit2) - ((double) unit));
        }

        internal virtual void RemoveEnding()
        {
        }

        internal abstract MigraDoc.DocumentObjectModel.DocumentObject DocumentObject { get; }

        internal abstract MigraDoc.Rendering.FormatInfo FormatInfo { get; }

        internal MigraDoc.Rendering.LayoutInfo LayoutInfo =>
            this.layoutInfo;
    }
}

