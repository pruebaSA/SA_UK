namespace MigraDoc.Rendering
{
    using System;

    internal class TextFrameRenderInfo : ShapeRenderInfo
    {
        private TextFrameFormatInfo formatInfo;

        internal TextFrameRenderInfo()
        {
        }

        internal override MigraDoc.Rendering.FormatInfo FormatInfo
        {
            get
            {
                if (this.formatInfo == null)
                {
                    this.formatInfo = new TextFrameFormatInfo();
                }
                return this.formatInfo;
            }
        }
    }
}

