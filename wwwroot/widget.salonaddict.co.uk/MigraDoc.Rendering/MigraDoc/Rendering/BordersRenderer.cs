namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using PdfSharp.Drawing;
    using System;

    internal class BordersRenderer
    {
        private Borders borders;
        private XGraphics gfx;

        internal BordersRenderer(Borders borders, XGraphics gfx)
        {
            this.gfx = gfx;
            this.borders = borders;
        }

        private Border GetBorder(BorderType type) => 
            ((Border) this.borders.GetValue(type.ToString(), GV.ReadOnly));

        private XColor GetColor(BorderType type)
        {
            Color black = Colors.Black;
            Border border = this.GetBorder(type);
            if ((border != null) && !border.Color.IsEmpty)
            {
                black = border.Color;
            }
            else if (!this.borders.Color.IsEmpty)
            {
                black = this.borders.Color;
            }
            return ColorHelper.ToXColor(black, this.borders.Document.UseCmykColor);
        }

        private XPen GetPen(BorderType type)
        {
            XUnit width = this.GetWidth(type);
            if (width == 0)
            {
                return null;
            }
            XPen pen = new XPen(this.GetColor(type), (double) width);
            switch (this.GetStyle(type))
            {
                case BorderStyle.Dot:
                    pen.DashStyle = XDashStyle.Dot;
                    return pen;

                case BorderStyle.DashSmallGap:
                    pen.DashPattern = new double[] { 5.0, 1.0 };
                    return pen;

                case BorderStyle.DashLargeGap:
                    pen.DashPattern = new double[] { 3.0, 3.0 };
                    return pen;

                case BorderStyle.DashDot:
                    pen.DashStyle = XDashStyle.DashDot;
                    return pen;

                case BorderStyle.DashDotDot:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    return pen;
            }
            pen.DashStyle = XDashStyle.Solid;
            return pen;
        }

        private BorderStyle GetStyle(BorderType type)
        {
            BorderStyle single = BorderStyle.Single;
            Border border = this.GetBorder(type);
            if ((border != null) && !border.IsNull("Style"))
            {
                return border.Style;
            }
            if (!this.borders.IsNull("Style"))
            {
                single = this.borders.Style;
            }
            return single;
        }

        internal XUnit GetWidth(BorderType type)
        {
            if (this.borders != null)
            {
                Border border = this.GetBorder(type);
                if (border != null)
                {
                    if (!border.IsNull("Visible") && !border.Visible)
                    {
                        return 0;
                    }
                    if ((border != null) && !border.IsNull("Width"))
                    {
                        return border.Width.Point;
                    }
                    if ((!border.IsNull("Color") || !border.IsNull("Style")) || border.Visible)
                    {
                        if (!this.borders.IsNull("Width"))
                        {
                            return this.borders.Width.Point;
                        }
                        return 0.5;
                    }
                }
                else if ((type != BorderType.DiagonalDown) && (type != BorderType.DiagonalUp))
                {
                    if (!this.borders.IsNull("Visible") && !this.borders.Visible)
                    {
                        return 0;
                    }
                    if (!this.borders.IsNull("Width"))
                    {
                        return this.borders.Width.Point;
                    }
                    if ((!this.borders.IsNull("Color") || !this.borders.IsNull("Style")) || this.borders.Visible)
                    {
                        return 0.5;
                    }
                }
            }
            return 0;
        }

        internal bool IsRendered(BorderType borderType)
        {
            if (this.borders != null)
            {
                switch (borderType)
                {
                    case BorderType.Top:
                        return (!this.borders.IsNull("Top") && (((double) this.GetWidth(borderType)) > 0.0));

                    case BorderType.Left:
                        return (!this.borders.IsNull("Left") && (((double) this.GetWidth(borderType)) > 0.0));

                    case BorderType.Bottom:
                        return (!this.borders.IsNull("Bottom") && (((double) this.GetWidth(borderType)) > 0.0));

                    case BorderType.Right:
                        return (!this.borders.IsNull("Right") && (((double) this.GetWidth(borderType)) > 0.0));

                    case BorderType.DiagonalDown:
                        return (!this.borders.IsNull("DiagonalDown") && (((double) this.GetWidth(borderType)) > 0.0));

                    case BorderType.DiagonalUp:
                        return (!this.borders.IsNull("DiagonalUp") && (((double) this.GetWidth(borderType)) > 0.0));
                }
            }
            return false;
        }

        internal void RenderDiagonally(BorderType type, XUnit left, XUnit top, XUnit width, XUnit height)
        {
            if (this.GetWidth(type) != 0)
            {
                XGraphicsState state = this.gfx.Save();
                this.gfx.IntersectClip(new XRect((double) left, (double) top, (double) width, (double) height));
                if (type == BorderType.DiagonalDown)
                {
                    this.gfx.DrawLine(this.GetPen(type), (double) left, (double) top, ((double) left) + ((double) width), ((double) top) + ((double) height));
                }
                else if (type == BorderType.DiagonalUp)
                {
                    this.gfx.DrawLine(this.GetPen(type), (double) left, ((double) top) + ((double) height), ((double) left) + ((double) width), (double) top);
                }
                this.gfx.Restore(state);
            }
        }

        internal void RenderHorizontally(BorderType type, XUnit left, XUnit top, XUnit width)
        {
            XUnit unit = this.GetWidth(type);
            if (unit != 0)
            {
                top = ((double) top) + (((double) unit) / 2.0);
                this.gfx.DrawLine(this.GetPen(type), ((double) left) + ((double) width), (double) top, (double) left, (double) top);
            }
        }

        internal void RenderVertically(BorderType type, XUnit left, XUnit top, XUnit height)
        {
            XUnit width = this.GetWidth(type);
            if (width != 0)
            {
                left = ((double) left) + (((double) width) / 2.0);
                this.gfx.DrawLine(this.GetPen(type), (double) left, ((double) top) + ((double) height), (double) left, (double) top);
            }
        }
    }
}

