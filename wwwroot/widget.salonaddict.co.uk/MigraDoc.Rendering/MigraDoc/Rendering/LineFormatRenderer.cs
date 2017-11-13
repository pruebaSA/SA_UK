namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using PdfSharp.Drawing;
    using System;

    internal class LineFormatRenderer
    {
        private XGraphics gfx;
        private LineFormat lineFormat;

        public LineFormatRenderer(LineFormat lineFormat, XGraphics gfx)
        {
            this.lineFormat = lineFormat;
            this.gfx = gfx;
        }

        private XColor GetColor()
        {
            Color black = Colors.Black;
            if ((this.lineFormat != null) && !this.lineFormat.Color.IsEmpty)
            {
                black = this.lineFormat.Color;
            }
            return ColorHelper.ToXColor(black, this.lineFormat.Document.UseCmykColor);
        }

        private XPen GetPen(XUnit width)
        {
            if (width == 0)
            {
                return null;
            }
            XPen pen = new XPen(this.GetColor(), (double) width);
            switch (this.lineFormat.DashStyle)
            {
                case DashStyle.Solid:
                    pen.DashStyle = XDashStyle.Solid;
                    return pen;

                case DashStyle.Dash:
                    pen.DashStyle = XDashStyle.Dash;
                    return pen;

                case DashStyle.DashDot:
                    pen.DashStyle = XDashStyle.DashDot;
                    return pen;

                case DashStyle.DashDotDot:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    return pen;

                case DashStyle.SquareDot:
                    pen.DashStyle = XDashStyle.Dot;
                    return pen;
            }
            return pen;
        }

        internal XUnit GetWidth()
        {
            if (this.lineFormat == null)
            {
                return 0;
            }
            if (!this.lineFormat.IsNull("Visible") && !this.lineFormat.Visible)
            {
                return 0;
            }
            if (!this.lineFormat.IsNull("Width"))
            {
                return this.lineFormat.Width.Point;
            }
            if ((this.lineFormat.IsNull("Color") && this.lineFormat.IsNull("Style")) && !this.lineFormat.Visible)
            {
                return 0;
            }
            return 1;
        }

        internal void Render(XUnit xPosition, XUnit yPosition, XUnit width, XUnit height)
        {
            XUnit unit = this.GetWidth();
            if (((double) unit) > 0.0)
            {
                XPen pen = this.GetPen(unit);
                this.gfx.DrawRectangle(pen, (double) xPosition, (double) yPosition, (double) width, (double) height);
            }
        }
    }
}

