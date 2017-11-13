namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public abstract class XBrush
    {
        protected XBrush()
        {
        }

        public static implicit operator XBrush(Brush brush)
        {
            SolidBrush brush3 = brush as SolidBrush;
            if (brush3 != null)
            {
                return new XSolidBrush(brush3.Color);
            }
            if (brush is LinearGradientBrush)
            {
                throw new NotImplementedException("Brush type not yet supported by PDFsharp.");
            }
            throw new NotImplementedException("Brush type not supported by PDFsharp.");
        }

        internal abstract Brush RealizeGdiBrush();
    }
}

