namespace PdfSharp.Drawing
{
    using PdfSharp.Fonts;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;

    [DebuggerDisplay("'{Name}', {Size}")]
    public class XFont
    {
        internal int cellAscent;
        internal int cellDescent;
        internal int cellSpace;
        private double emSize;
        internal string familyName;
        internal Font font;
        private XFontFamily fontFamily;
        private XFontMetrics fontMetrics;
        private System.Drawing.FontFamily gdifamily;
        private bool isVertical;
        private XPdfFontOptions pdfOptions;
        internal PdfFontTable.FontSelector selector;
        private XFontStyle style;
        internal int unitsPerEm;

        public XFont(Font font, XPdfFontOptions pdfOptions)
        {
            if (font.Unit != GraphicsUnit.World)
            {
                throw new ArgumentException("Font must use GraphicsUnit.World.");
            }
            this.font = font;
            this.familyName = font.Name;
            this.emSize = font.Size;
            this.style = FontStyleFrom(font);
            this.pdfOptions = pdfOptions;
            this.Initialize();
        }

        public XFont(string familyName, double emSize)
        {
            this.familyName = familyName;
            this.emSize = emSize;
            this.style = XFontStyle.Regular;
            this.pdfOptions = new XPdfFontOptions();
            this.Initialize();
        }

        public XFont(string familyName, double emSize, XFontStyle style)
        {
            this.familyName = familyName;
            this.emSize = emSize;
            this.style = style;
            this.pdfOptions = new XPdfFontOptions();
            this.Initialize();
        }

        public XFont(System.Drawing.FontFamily family, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
            this.familyName = null;
            this.gdifamily = family;
            this.emSize = emSize;
            this.style = style;
            this.pdfOptions = pdfOptions;
            this.Initialize();
        }

        public XFont(string familyName, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
            this.familyName = familyName;
            this.emSize = emSize;
            this.style = style;
            this.pdfOptions = pdfOptions;
            this.Initialize();
        }

        internal static XFontStyle FontStyleFrom(Font font) => 
            ((((font.Bold ? XFontStyle.Bold : XFontStyle.Regular) | (font.Italic ? XFontStyle.Italic : XFontStyle.Regular)) | (font.Strikeout ? XFontStyle.Strikeout : XFontStyle.Regular)) | (font.Underline ? XFontStyle.Underline : XFontStyle.Regular));

        public double GetHeight()
        {
            this.RealizeGdiFont();
            return (double) this.font.GetHeight();
        }

        public double GetHeight(XGraphics graphics)
        {
            this.RealizeGdiFont();
            this.font.GetHeight(graphics.gfx);
            double num1 = (this.cellSpace * this.emSize) / ((double) this.unitsPerEm);
            return (double) this.font.GetHeight(graphics.gfx);
        }

        private void Initialize()
        {
            XFontMetrics metrics = null;
            if (this.font == null)
            {
                if (this.gdifamily != null)
                {
                    this.font = new Font(this.gdifamily, (float) this.emSize, (FontStyle) this.style, GraphicsUnit.World);
                    this.familyName = this.gdifamily.Name;
                }
                else
                {
                    this.font = XPrivateFontCollection.TryFindPrivateFont(this.familyName, this.emSize, (FontStyle) this.style) ?? new Font(this.familyName, (float) this.emSize, (FontStyle) this.style, GraphicsUnit.World);
                }
            }
            metrics = this.Metrics;
            System.Drawing.FontFamily fontFamily = this.font.FontFamily;
            this.unitsPerEm = metrics.UnitsPerEm;
            this.cellSpace = this.font.FontFamily.GetLineSpacing(this.font.Style);
            this.cellAscent = fontFamily.GetCellAscent(this.font.Style);
            this.cellDescent = fontFamily.GetCellDescent(this.font.Style);
        }

        public static implicit operator XFont(Font font) => 
            new XFont(font, null);

        internal Font RealizeGdiFont() => 
            this.font;

        public bool Bold =>
            ((this.style & XFontStyle.Bold) == XFontStyle.Bold);

        [Browsable(false)]
        public XFontFamily FontFamily
        {
            get
            {
                if (this.fontFamily == null)
                {
                    this.RealizeGdiFont();
                    this.fontFamily = new XFontFamily(this.font.FontFamily);
                }
                return this.fontFamily;
            }
        }

        public System.Drawing.FontFamily GdiFamily =>
            this.gdifamily;

        [Browsable(false)]
        public int Height =>
            ((int) Math.Ceiling(this.GetHeight()));

        internal bool IsVertical
        {
            get => 
                this.isVertical;
            set
            {
                this.isVertical = value;
            }
        }

        public bool Italic =>
            ((this.style & XFontStyle.Italic) == XFontStyle.Italic);

        public XFontMetrics Metrics
        {
            get
            {
                if (this.fontMetrics == null)
                {
                    FontDescriptor descriptor = FontDescriptorStock.Global.CreateDescriptor(this);
                    this.fontMetrics = descriptor.FontMetrics;
                }
                return this.fontMetrics;
            }
        }

        public string Name
        {
            get
            {
                this.RealizeGdiFont();
                return this.font.Name;
            }
        }

        public XPdfFontOptions PdfOptions
        {
            get
            {
                if (this.pdfOptions == null)
                {
                    this.pdfOptions = new XPdfFontOptions();
                }
                return this.pdfOptions;
            }
        }

        public double Size =>
            this.emSize;

        public bool Strikeout =>
            ((this.style & XFontStyle.Strikeout) == XFontStyle.Strikeout);

        [Browsable(false)]
        public XFontStyle Style =>
            this.style;

        public bool Underline =>
            ((this.style & XFontStyle.Underline) == XFontStyle.Underline);

        internal bool Unicode =>
            (this.pdfOptions?.FontEncoding == PdfFontEncoding.Unicode);
    }
}

