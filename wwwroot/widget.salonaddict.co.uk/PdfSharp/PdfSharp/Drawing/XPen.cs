namespace PdfSharp.Drawing
{
    using PdfSharp;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public sealed class XPen
    {
        internal XColor color;
        internal double dashOffset;
        internal double[] dashPattern;
        internal XDashStyle dashStyle;
        private bool dirty;
        private Pen gdiPen;
        private bool immutable;
        internal XLineCap lineCap;
        internal XLineJoin lineJoin;
        internal double miterLimit;
        internal double width;

        public XPen(XColor color) : this(color, 1.0, false)
        {
        }

        public XPen(XPen pen)
        {
            this.dirty = true;
            this.color = pen.color;
            this.width = pen.width;
            this.lineJoin = pen.lineJoin;
            this.lineCap = pen.lineCap;
            this.dashStyle = pen.dashStyle;
            this.dashOffset = pen.dashOffset;
            this.dashPattern = pen.dashPattern;
            if (this.dashPattern != null)
            {
                this.dashPattern = (double[]) this.dashPattern.Clone();
            }
        }

        public XPen(XColor color, double width) : this(color, width, false)
        {
        }

        internal XPen(XColor color, double width, bool immutable)
        {
            this.dirty = true;
            this.color = color;
            this.width = width;
            this.lineJoin = XLineJoin.Miter;
            this.lineCap = XLineCap.Flat;
            this.dashStyle = XDashStyle.Solid;
            this.dashOffset = 0.0;
            this.immutable = immutable;
        }

        public XPen Clone() => 
            new XPen(this);

        public static implicit operator XPen(Pen pen)
        {
            if (pen.PenType != PenType.SolidColor)
            {
                throw new NotImplementedException("Pen type not supported by PDFsharp.");
            }
            XPen pen2 = new XPen(pen.Color, (double) pen.Width) {
                LineJoin = (XLineJoin) pen.LineJoin,
                DashStyle = (XDashStyle) pen.DashStyle,
                miterLimit = pen.MiterLimit
            };
            if (pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
            {
                int length = pen.DashPattern.Length;
                double[] numArray = new double[length];
                for (int i = 0; i < length; i++)
                {
                    numArray[i] = pen.DashPattern[i];
                }
                pen2.DashPattern = numArray;
                pen2.dashOffset = pen.DashOffset;
            }
            return pen2;
        }

        internal Pen RealizeGdiPen()
        {
            if (this.dirty)
            {
                if (this.gdiPen == null)
                {
                    this.gdiPen = new Pen(this.color.ToGdiColor(), (float) this.width);
                }
                else
                {
                    this.gdiPen.Color = this.color.ToGdiColor();
                    this.gdiPen.Width = (float) this.width;
                }
                System.Drawing.Drawing2D.LineCap cap = XConvert.ToLineCap(this.lineCap);
                this.gdiPen.StartCap = cap;
                this.gdiPen.EndCap = cap;
                this.gdiPen.LineJoin = XConvert.ToLineJoin(this.lineJoin);
                this.gdiPen.DashOffset = (float) this.dashOffset;
                if (this.dashStyle == XDashStyle.Custom)
                {
                    int num = (this.dashPattern == null) ? 0 : this.dashPattern.Length;
                    float[] numArray = new float[num];
                    for (int i = 0; i < num; i++)
                    {
                        numArray[i] = (float) this.dashPattern[i];
                    }
                    this.gdiPen.DashPattern = numArray;
                }
                else
                {
                    this.gdiPen.DashStyle = (System.Drawing.Drawing2D.DashStyle) this.dashStyle;
                }
            }
            return this.gdiPen;
        }

        public XColor Color
        {
            get => 
                this.color;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.color != value);
                this.color = value;
            }
        }

        public double DashOffset
        {
            get => 
                this.dashOffset;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.dashOffset != value);
                this.dashOffset = value;
            }
        }

        public double[] DashPattern
        {
            get
            {
                if (this.dashPattern == null)
                {
                    this.dashPattern = new double[0];
                }
                return this.dashPattern;
            }
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                int length = value.Length;
                for (int i = 0; i < length; i++)
                {
                    if (value[i] <= 0.0)
                    {
                        throw new ArgumentException("Dash pattern value must greater than zero.");
                    }
                }
                this.dirty = true;
                this.dashStyle = XDashStyle.Custom;
                this.dashPattern = (double[]) value.Clone();
            }
        }

        public XDashStyle DashStyle
        {
            get => 
                this.dashStyle;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.dashStyle != value);
                this.dashStyle = value;
            }
        }

        public XLineCap LineCap
        {
            get => 
                this.lineCap;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.lineCap != value);
                this.lineCap = value;
            }
        }

        public XLineJoin LineJoin
        {
            get => 
                this.lineJoin;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.lineJoin != value);
                this.lineJoin = value;
            }
        }

        public double MiterLimit
        {
            get => 
                this.miterLimit;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.miterLimit != value);
                this.miterLimit = value;
            }
        }

        public double Width
        {
            get => 
                this.width;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XPen"));
                }
                this.dirty = this.dirty || (this.width != value);
                this.width = value;
            }
        }
    }
}

