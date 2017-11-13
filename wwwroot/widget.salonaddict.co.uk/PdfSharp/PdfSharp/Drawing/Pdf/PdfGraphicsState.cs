namespace PdfSharp.Drawing.Pdf
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Globalization;
    using System.Text;

    internal sealed class PdfGraphicsState : ICloneable
    {
        internal InternalGraphicsState InternalState;
        internal int Level;
        public bool MustRealizeCtm;
        private XMatrix realizedCtm;
        private string realizedDashPattern;
        private XDashStyle realizedDashStyle = ~XDashStyle.Solid;
        private XColor realizedFillColor = XColor.Empty;
        internal PdfFont realizedFont;
        private string realizedFontName = string.Empty;
        private double realizedFontSize;
        private int realizedLineCap = -1;
        private int realizedLineJoin = -1;
        private double realizedLineWith = -1.0;
        private double realizedMiterLimit = -1.0;
        private XColor realizedStrokeColor = XColor.Empty;
        public XPoint realizedTextPosition;
        private readonly XGraphicsPdfRenderer renderer;
        private XMatrix unrealizedCtm;

        public PdfGraphicsState(XGraphicsPdfRenderer renderer)
        {
            this.renderer = renderer;
        }

        public PdfGraphicsState Clone() => 
            ((PdfGraphicsState) base.MemberwiseClone());

        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            if (!matrix.IsIdentity)
            {
                this.MustRealizeCtm = true;
                this.unrealizedCtm.Multiply(matrix, order);
            }
        }

        public void PopState()
        {
            this.renderer.Append("Q/n");
        }

        public void PushState()
        {
            this.renderer.Append("q/n");
        }

        public void RealizeBrush(XBrush brush, PdfColorMode colorMode)
        {
            if (brush is XSolidBrush)
            {
                XColor color = ((XSolidBrush) brush).Color;
                color = ColorSpaceHelper.EnsureColorMode(colorMode, color);
                if (colorMode != PdfColorMode.Cmyk)
                {
                    if (this.realizedFillColor.Rgb != color.Rgb)
                    {
                        this.renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Rgb));
                        this.renderer.Append(" rg\n");
                    }
                }
                else if (!ColorSpaceHelper.IsEqualCmyk(this.realizedFillColor, color))
                {
                    this.renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Cmyk));
                    this.renderer.Append(" k\n");
                }
                if ((this.renderer.Owner.Version >= 14) && (this.realizedFillColor.A != color.A))
                {
                    PdfExtGState extGStateNonStroke = this.renderer.Owner.ExtGStateTable.GetExtGStateNonStroke(color.A);
                    string str = this.renderer.Resources.AddExtGState(extGStateNonStroke);
                    this.renderer.AppendFormat("{0} gs\n", new object[] { str });
                    if ((this.renderer.page != null) && (color.A < 1.0))
                    {
                        this.renderer.page.transparencyUsed = true;
                    }
                }
                this.realizedFillColor = color;
            }
            else if (brush is XLinearGradientBrush)
            {
                XMatrix defaultViewMatrix = this.renderer.defaultViewMatrix;
                defaultViewMatrix.Prepend(this.Transform);
                PdfShadingPattern pattern = new PdfShadingPattern(this.renderer.Owner);
                pattern.SetupFromBrush((XLinearGradientBrush) brush, defaultViewMatrix);
                string str2 = this.renderer.Resources.AddPattern(pattern);
                this.renderer.AppendFormat("/Pattern cs\n", new object[] { str2 });
                this.renderer.AppendFormat("{0} scn\n", new object[] { str2 });
                this.realizedFillColor = XColor.Empty;
            }
        }

        private void RealizeClipPath(XGraphicsPath clipPath)
        {
            this.renderer.BeginGraphic();
            this.RealizeCtm();
            this.renderer.AppendPath(clipPath.gdipPath);
            if (clipPath.FillMode == XFillMode.Winding)
            {
                this.renderer.Append("W n\n");
            }
            else
            {
                this.renderer.Append("W* n\n");
            }
        }

        public void RealizeCtm()
        {
            if (this.MustRealizeCtm)
            {
                double[] elements = this.unrealizedCtm.GetElements();
                this.renderer.AppendFormat("{0:0.######} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} cm\n", new object[] { elements[0], elements[1], elements[2], elements[3], elements[4], elements[5] });
                this.realizedCtm.Prepend(this.unrealizedCtm);
                this.unrealizedCtm = new XMatrix();
                this.MustRealizeCtm = false;
            }
        }

        public void RealizeFont(XFont font, XBrush brush, int renderMode)
        {
            this.RealizeBrush(brush, this.renderer.colorMode);
            this.realizedFont = null;
            string fontName = this.renderer.GetFontName(font, out this.realizedFont);
            if ((fontName != this.realizedFontName) || (this.realizedFontSize != font.Size))
            {
                if (this.renderer.Gfx.PageDirection == XPageDirection.Downwards)
                {
                    this.renderer.AppendFormat("{0} {1:0.###} Tf\n", new object[] { fontName, -font.Size });
                }
                else
                {
                    this.renderer.AppendFormat("{0} {1:0.###} Tf\n", new object[] { fontName, font.Size });
                }
                this.realizedFontName = fontName;
                this.realizedFontSize = font.Size;
            }
        }

        public void RealizePen(XPen pen, PdfColorMode colorMode)
        {
            XColor color = pen.Color;
            color = ColorSpaceHelper.EnsureColorMode(colorMode, color);
            if (this.realizedLineWith != pen.width)
            {
                this.renderer.AppendFormat("{0:0.###} w\n", new object[] { pen.width });
                this.realizedLineWith = pen.width;
            }
            if (this.realizedLineCap != pen.lineCap)
            {
                this.renderer.AppendFormat("{0} J\n", new object[] { (int) pen.lineCap });
                this.realizedLineCap = (int) pen.lineCap;
            }
            if (this.realizedLineJoin != pen.lineJoin)
            {
                this.renderer.AppendFormat("{0} j\n", new object[] { (int) pen.lineJoin });
                this.realizedLineJoin = (int) pen.lineJoin;
            }
            if (((this.realizedLineCap == 0) && (this.realizedMiterLimit != ((int) pen.miterLimit))) && (((int) pen.miterLimit) != 0))
            {
                this.renderer.AppendFormat("{0} M\n", new object[] { (int) pen.miterLimit });
                this.realizedMiterLimit = (int) pen.miterLimit;
            }
            if ((this.realizedDashStyle != pen.dashStyle) || (pen.dashStyle == XDashStyle.Custom))
            {
                double width = pen.Width;
                double num2 = 3.0 * width;
                XDashStyle dashStyle = pen.DashStyle;
                if (width == 0.0)
                {
                    dashStyle = XDashStyle.Solid;
                }
                switch (dashStyle)
                {
                    case XDashStyle.Solid:
                        this.renderer.Append("[]0 d\n");
                        break;

                    case XDashStyle.Dash:
                        this.renderer.AppendFormat("[{0:0.##} {1:0.##}]0 d\n", new object[] { num2, width });
                        break;

                    case XDashStyle.Dot:
                        this.renderer.AppendFormat("[{0:0.##}]0 d\n", new object[] { width });
                        break;

                    case XDashStyle.DashDot:
                        this.renderer.AppendFormat("[{0:0.##} {1:0.##} {1:0.##} {1:0.##}]0 d\n", new object[] { num2, width });
                        break;

                    case XDashStyle.DashDotDot:
                        this.renderer.AppendFormat("[{0:0.##} {1:0.##} {1:0.##} {1:0.##} {1:0.##} {1:0.##}]0 d\n", new object[] { num2, width });
                        break;

                    case XDashStyle.Custom:
                    {
                        StringBuilder builder = new StringBuilder("[", 0x100);
                        int num3 = (pen.dashPattern == null) ? 0 : pen.dashPattern.Length;
                        for (int i = 0; i < num3; i++)
                        {
                            if (i > 0)
                            {
                                builder.Append(' ');
                            }
                            builder.Append(PdfEncoders.ToString((double) (pen.dashPattern[i] * pen.width)));
                        }
                        if ((num3 > 0) && ((num3 % 2) == 1))
                        {
                            builder.Append(' ');
                            builder.Append(PdfEncoders.ToString((double) (0.2 * pen.width)));
                        }
                        builder.AppendFormat(CultureInfo.InvariantCulture, "]{0:0.###} d\n", new object[] { pen.dashOffset * pen.width });
                        string str = builder.ToString();
                        this.realizedDashPattern = str;
                        this.renderer.Append(str);
                        break;
                    }
                }
                this.realizedDashStyle = dashStyle;
            }
            if (colorMode != PdfColorMode.Cmyk)
            {
                if (this.realizedStrokeColor.Rgb != color.Rgb)
                {
                    this.renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Rgb));
                    this.renderer.Append(" RG\n");
                }
            }
            else if (!ColorSpaceHelper.IsEqualCmyk(this.realizedStrokeColor, color))
            {
                this.renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Cmyk));
                this.renderer.Append(" K\n");
            }
            if ((this.renderer.Owner.Version >= 14) && (this.realizedStrokeColor.A != color.A))
            {
                PdfExtGState extGStateStroke = this.renderer.Owner.ExtGStateTable.GetExtGStateStroke(color.A);
                string str2 = this.renderer.Resources.AddExtGState(extGStateStroke);
                this.renderer.AppendFormat("{0} gs\n", new object[] { str2 });
                if ((this.renderer.page != null) && (color.A < 1.0))
                {
                    this.renderer.page.transparencyUsed = true;
                }
            }
            this.realizedStrokeColor = color;
        }

        public void SetAndRealizeClipPath(XGraphicsPath clipPath)
        {
            this.RealizeClipPath(clipPath);
        }

        public void SetAndRealizeClipRect(XRect clipRect)
        {
            XGraphicsPath clipPath = new XGraphicsPath();
            clipPath.AddRectangle(clipRect);
            this.RealizeClipPath(clipPath);
        }

        object ICloneable.Clone() => 
            this.Clone();

        public XMatrix Transform
        {
            get
            {
                if (this.MustRealizeCtm)
                {
                    XMatrix realizedCtm = this.realizedCtm;
                    realizedCtm.Prepend(this.unrealizedCtm);
                    return realizedCtm;
                }
                return this.realizedCtm;
            }
            set
            {
                XMatrix realizedCtm = this.realizedCtm;
                realizedCtm.Invert();
                realizedCtm.Prepend(value);
                this.unrealizedCtm = realizedCtm;
                this.MustRealizeCtm = !this.unrealizedCtm.IsIdentity;
            }
        }
    }
}

