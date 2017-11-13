namespace MigraDoc.Rendering
{
    using System;

    internal class ChartRenderInfo : ShapeRenderInfo
    {
        private ChartFormatInfo formatInfo;

        internal ChartRenderInfo()
        {
        }

        internal override MigraDoc.Rendering.FormatInfo FormatInfo
        {
            get
            {
                if (this.formatInfo == null)
                {
                    this.formatInfo = new ChartFormatInfo();
                }
                return this.formatInfo;
            }
        }
    }
}

