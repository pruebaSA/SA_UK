namespace PdfSharp.Drawing
{
    using PdfSharp.Drawing.Pdf;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Filters;
    using System;
    using System.Runtime.InteropServices;

    public class XForm : XImage, IContentStream
    {
        private XRect boundingBox;
        private PdfDocument document;
        internal FormState formState;
        internal XGraphics gfx;
        internal PdfFormXObject pdfForm;
        internal XGraphicsPdfRenderer pdfRenderer;
        internal XMatrix transform;
        private XRect viewBox;

        protected XForm()
        {
            this.transform = new XMatrix();
        }

        public XForm(XGraphics gfx, XSize size)
        {
            this.transform = new XMatrix();
            if (gfx == null)
            {
                throw new ArgumentNullException("gfx");
            }
            if ((size.width < 1.0) || (size.height < 1.0))
            {
                throw new ArgumentNullException("size", "The size of the XPdfForm is to small.");
            }
            this.formState = FormState.Created;
            this.viewBox.width = size.width;
            this.viewBox.height = size.height;
            if (gfx.PdfPage != null)
            {
                this.document = gfx.PdfPage.Owner;
                this.pdfForm = new PdfFormXObject(this.document, this);
                PdfRectangle rect = new PdfRectangle(new XPoint(), size);
                this.pdfForm.Elements.SetRectangle("/BBox", rect);
            }
        }

        public XForm(PdfDocument document, XRect viewBox)
        {
            this.transform = new XMatrix();
            if ((viewBox.width < 1.0) || (viewBox.height < 1.0))
            {
                throw new ArgumentNullException("viewBox", "The size of the XPdfForm is to small.");
            }
            if (document == null)
            {
                throw new ArgumentNullException("document", "An XPdfForm template must be associated with a document at creation time.");
            }
            this.formState = FormState.Created;
            this.document = document;
            this.pdfForm = new PdfFormXObject(document, this);
            this.viewBox = viewBox;
            PdfRectangle rect = new PdfRectangle(viewBox);
            this.pdfForm.Elements.SetRectangle("/BBox", rect);
        }

        public XForm(PdfDocument document, XSize size) : this(document, new XRect(0.0, 0.0, size.width, size.height))
        {
        }

        public XForm(XGraphics gfx, XUnit width, XUnit height) : this(gfx, new XSize((double) width, (double) height))
        {
        }

        public XForm(PdfDocument document, XUnit width, XUnit height) : this(document, new XRect(0.0, 0.0, (double) width, (double) height))
        {
        }

        internal void AssociateGraphics(XGraphics gfx)
        {
            if (this.formState == FormState.NotATemplate)
            {
                throw new NotImplementedException("The current version of PDFsharp cannot draw on an imported page.");
            }
            if (this.formState == FormState.UnderConstruction)
            {
                throw new InvalidOperationException("An XGraphics object already exists for this form.");
            }
            if (this.formState == FormState.Finished)
            {
                throw new InvalidOperationException("After drawing a form it cannot be modified anymore.");
            }
            this.formState = FormState.UnderConstruction;
            this.gfx = gfx;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public void DrawingFinished()
        {
            if (this.formState != FormState.Finished)
            {
                if (this.formState == FormState.NotATemplate)
                {
                    throw new InvalidOperationException("This object is an imported PDF page and you cannot finish drawing on it because you must not draw on it at all.");
                }
                this.Finish();
            }
        }

        internal virtual void Finish()
        {
            if ((this.formState != FormState.NotATemplate) && (this.formState != FormState.Finished))
            {
                if (this.gfx.metafile != null)
                {
                    base.gdiImage = this.gfx.metafile;
                }
                this.formState = FormState.Finished;
                this.gfx.Dispose();
                this.gfx = null;
                if (this.pdfRenderer != null)
                {
                    this.pdfRenderer.Close();
                    if (this.document.Options.CompressContentStreams)
                    {
                        this.pdfForm.Stream.Value = Filtering.FlateDecode.Encode(this.pdfForm.Stream.Value);
                        this.pdfForm.Elements["/Filter"] = new PdfName("/FlateDecode");
                    }
                    int length = this.pdfForm.Stream.Length;
                    this.pdfForm.Elements.SetInteger("/Length", length);
                }
            }
        }

        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            pdfFont = this.document.FontTable.GetFont(font);
            return this.Resources.AddFont(pdfFont);
        }

        internal string GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            pdfFont = this.document.FontTable.GetFont(idName, fontData);
            return this.Resources.AddFont(pdfFont);
        }

        internal string GetFormName(XForm form)
        {
            PdfFormXObject obj2 = this.document.FormTable.GetForm(form);
            return this.Resources.AddForm(obj2);
        }

        internal string GetImageName(XImage image)
        {
            PdfImage image2 = this.document.ImageTable.GetImage(image);
            return this.Resources.AddImage(image2);
        }

        string IContentStream.GetFontName(XFont font, out PdfFont pdfFont) => 
            this.GetFontName(font, out pdfFont);

        string IContentStream.GetFontName(string idName, byte[] fontData, out PdfFont pdfFont) => 
            this.GetFontName(idName, fontData, out pdfFont);

        string IContentStream.GetFormName(XForm form) => 
            this.GetFormName(form);

        string IContentStream.GetImageName(XImage image) => 
            this.GetImageName(image);

        internal string TryGetFontName(string idName, out PdfFont pdfFont)
        {
            pdfFont = this.document.FontTable.TryGetFont(idName);
            string str = null;
            if (pdfFont != null)
            {
                str = this.Resources.AddFont(pdfFont);
            }
            return str;
        }

        public XRect BoundingBox
        {
            get => 
                this.boundingBox;
            set
            {
                this.boundingBox = value;
            }
        }

        internal PdfColorMode ColorMode =>
            this.document?.Options.ColorMode;

        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelHeight, but will become PointHeight in future releases of PDFsharp.")]
        public override double Height =>
            this.viewBox.height;

        public override double HorizontalResolution =>
            72.0;

        internal bool IsTemplate =>
            (this.formState != FormState.NotATemplate);

        internal PdfDocument Owner =>
            this.document;

        internal PdfFormXObject PdfForm
        {
            get
            {
                if (this.pdfForm.Reference == null)
                {
                    this.document.irefTable.Add(this.pdfForm);
                }
                return this.pdfForm;
            }
        }

        PdfResources IContentStream.Resources =>
            this.Resources;

        public override int PixelHeight =>
            ((int) this.viewBox.height);

        public override int PixelWidth =>
            ((int) this.viewBox.width);

        public override double PointHeight =>
            this.viewBox.height;

        public override double PointWidth =>
            this.viewBox.width;

        internal PdfResources Resources =>
            this.PdfForm.Resources;

        public override XSize Size =>
            this.viewBox.Size;

        public virtual XMatrix Transform
        {
            get => 
                this.transform;
            set
            {
                if (this.formState == FormState.Finished)
                {
                    throw new InvalidOperationException("After a XPdfForm was once drawn it must not be modified.");
                }
                this.transform = value;
            }
        }

        public override double VerticalResolution =>
            72.0;

        public XRect ViewBox =>
            this.viewBox;

        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelWidth, but will become PointWidth in future releases of PDFsharp.")]
        public override double Width =>
            this.viewBox.Width;

        internal enum FormState
        {
            NotATemplate,
            Created,
            UnderConstruction,
            Finished
        }
    }
}

