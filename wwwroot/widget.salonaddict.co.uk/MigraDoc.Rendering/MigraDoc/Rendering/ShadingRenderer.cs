namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using PdfSharp.Drawing;
    using System;

    internal class ShadingRenderer
    {
        private XBrush brush;
        private XGraphics gfx;
        private Shading shading;

        public ShadingRenderer(XGraphics gfx, Shading shading)
        {
            this.gfx = gfx;
            this.shading = shading;
            this.RealizeBrush();
        }

        private bool IsVisible()
        {
            if (!this.shading.IsNull("Visible"))
            {
                return this.shading.Visible;
            }
            return !this.shading.IsNull("Color");
        }

        private void RealizeBrush()
        {
            if ((this.shading != null) && this.IsVisible())
            {
                this.brush = new XSolidBrush(ColorHelper.ToXColor(this.shading.Color, this.shading.Document.UseCmykColor));
            }
        }

        internal void Render(XUnit x, XUnit y, XUnit width, XUnit height)
        {
            if ((this.shading != null) && (this.brush != null))
            {
                this.gfx.DrawRectangle(this.brush, x.Point, y.Point, width.Point, height.Point);
            }
        }
    }
}

