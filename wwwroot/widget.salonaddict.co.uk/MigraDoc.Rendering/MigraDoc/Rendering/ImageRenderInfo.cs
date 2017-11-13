namespace MigraDoc.Rendering
{
    internal class ImageRenderInfo : ShapeRenderInfo
    {
        private ImageFormatInfo formatInfo;

        internal override MigraDoc.Rendering.FormatInfo FormatInfo
        {
            get
            {
                if (this.formatInfo == null)
                {
                    this.formatInfo = new ImageFormatInfo();
                }
                return this.formatInfo;
            }
        }
    }
}

