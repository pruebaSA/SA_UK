namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing.Drawing2D;

    public sealed class XGraphicsPathInternals
    {
        private XGraphicsPath path;

        internal XGraphicsPathInternals(XGraphicsPath path)
        {
            this.path = path;
        }

        public GraphicsPath GdiPath =>
            this.path.gdipPath;
    }
}

