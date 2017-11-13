namespace PdfSharp.Drawing
{
    using PdfSharp;
    using System;
    using System.Drawing;

    public class XSolidBrush : XBrush
    {
        internal XColor color;
        private SolidBrush gdiBrush;
        private bool gdiDirty;
        private bool immutable;

        public XSolidBrush()
        {
            this.gdiDirty = true;
        }

        public XSolidBrush(XColor color) : this(color, false)
        {
        }

        public XSolidBrush(XSolidBrush brush)
        {
            this.gdiDirty = true;
            this.color = brush.Color;
        }

        internal XSolidBrush(XColor color, bool immutable)
        {
            this.gdiDirty = true;
            this.color = color;
            this.immutable = immutable;
        }

        internal override Brush RealizeGdiBrush()
        {
            if (this.gdiDirty)
            {
                if (this.gdiBrush == null)
                {
                    this.gdiBrush = new SolidBrush(this.color.ToGdiColor());
                }
                else
                {
                    this.gdiBrush.Color = this.color.ToGdiColor();
                }
                this.gdiDirty = false;
            }
            return this.gdiBrush;
        }

        public XColor Color
        {
            get => 
                this.color;
            set
            {
                if (this.immutable)
                {
                    throw new ArgumentException(PSSR.CannotChangeImmutableObject("XSolidBrush"));
                }
                this.color = value;
                this.gdiDirty = this.gdiDirty || (this.color != value);
            }
        }
    }
}

