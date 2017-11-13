namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    internal class BarCodeRenderInfo
    {
        public double BarHeight;
        public XBrush Brush;
        public XPoint CurrPos;
        public int CurrPosInString;
        public XFont Font;
        public XGraphics Gfx;
        public XPoint Position;
        public double ThinBarWidth;

        public BarCodeRenderInfo(XGraphics gfx, XBrush brush, XFont font, XPoint position)
        {
            this.Gfx = gfx;
            this.Brush = brush;
            this.Font = font;
            this.Position = position;
        }
    }
}

