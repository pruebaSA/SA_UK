namespace System.Drawing.Printing
{
    using System;
    using System.Drawing;

    public sealed class PreviewPageInfo
    {
        private System.Drawing.Image image;
        private Size physicalSize = Size.Empty;

        public PreviewPageInfo(System.Drawing.Image image, Size physicalSize)
        {
            this.image = image;
            this.physicalSize = physicalSize;
        }

        public System.Drawing.Image Image =>
            this.image;

        public Size PhysicalSize =>
            this.physicalSize;
    }
}

