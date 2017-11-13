namespace PdfSharp.Pdf
{
    using PdfSharp.Drawing;
    using System;

    public sealed class PdfDocumentSettings
    {
        private XPrivateFontCollection privateFontCollection;
        private PdfSharp.Pdf.TrimMargins trimMargins = new PdfSharp.Pdf.TrimMargins();

        internal PdfDocumentSettings(PdfDocument document)
        {
        }

        public XPrivateFontCollection PrivateFontCollection
        {
            internal get => 
                this.privateFontCollection;
            set
            {
                if (this.privateFontCollection != null)
                {
                    throw new InvalidOperationException("PrivateFontCollection can only be set once.");
                }
                this.privateFontCollection = value;
            }
        }

        public PdfSharp.Pdf.TrimMargins TrimMargins
        {
            get
            {
                if (this.trimMargins == null)
                {
                    this.trimMargins = new PdfSharp.Pdf.TrimMargins();
                }
                return this.trimMargins;
            }
            set
            {
                if (this.trimMargins == null)
                {
                    this.trimMargins = new PdfSharp.Pdf.TrimMargins();
                }
                if (value != null)
                {
                    this.trimMargins.Left = value.Left;
                    this.trimMargins.Right = value.Right;
                    this.trimMargins.Top = value.Top;
                    this.trimMargins.Bottom = value.Bottom;
                }
                else
                {
                    this.trimMargins.All = 0;
                }
            }
        }
    }
}

