namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using PdfSharp.Drawing;
    using System;

    internal class FillFormatRenderer
    {
        private FillFormat fillFormat;
        private XGraphics gfx;

        public FillFormatRenderer(FillFormat fillFormat, XGraphics gfx)
        {
            this.gfx = gfx;
            this.fillFormat = fillFormat;
        }

        private XBrush GetBrush()
        {
            if ((this.fillFormat != null) && this.IsVisible())
            {
                return new XSolidBrush(ColorHelper.ToXColor(this.fillFormat.Color, this.fillFormat.Document.UseCmykColor));
            }
            return null;
        }

        private bool IsVisible()
        {
            if (!this.fillFormat.IsNull("Visible"))
            {
                return this.fillFormat.Visible;
            }
            return !this.fillFormat.IsNull("Color");
        }

        internal void Render(XUnit x, XUnit y, XUnit width, XUnit height)
        {
            XBrush brush = this.GetBrush();
            if (brush != null)
            {
                this.gfx.DrawRectangle(brush, x.Point, y.Point, width.Point, height.Point);
            }
        }
    }
}

