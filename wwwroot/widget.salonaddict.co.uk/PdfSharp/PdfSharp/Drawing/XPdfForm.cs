namespace PdfSharp.Drawing
{
    using PdfSharp;
    using PdfSharp.Internal;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class XPdfForm : XForm
    {
        private bool disposed;
        internal PdfDocument externalDocument;
        private int pageCount;
        private int pageNumber;
        private XImage placeHolder;

        internal XPdfForm(Stream stream)
        {
            this.pageCount = -1;
            this.pageNumber = 1;
            base.path = "*" + Guid.NewGuid().ToString("B");
            if (PdfReader.TestPdfFile(stream) == 0)
            {
                throw new ArgumentException("The specified stream has no valid PDF file header.", "stream");
            }
            this.externalDocument = PdfReader.Open(stream);
        }

        internal XPdfForm(string path)
        {
            int num;
            this.pageCount = -1;
            this.pageNumber = 1;
            path = ExtractPageNumber(path, out num);
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(PSSR.FileNotFound(path), path);
            }
            if (PdfReader.TestPdfFile(path) == 0)
            {
                throw new ArgumentException("The specified file has no valid PDF file header.", "path");
            }
            base.path = path;
            if (num != 0)
            {
                this.PageNumber = num;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                try
                {
                    if (this.externalDocument != null)
                    {
                        PdfDocument.Tls.DetachDocument(this.externalDocument.Handle);
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        public static string ExtractPageNumber(string path, out int pageNumber)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            pageNumber = 0;
            int length = path.Length;
            if (length != 0)
            {
                length--;
                if (!char.IsDigit(path, length))
                {
                    return path;
                }
                while (char.IsDigit(path, length) && (length >= 0))
                {
                    length--;
                }
                if (((length > 0) && (path[length] == '#')) && (path.IndexOf('.') != -1))
                {
                    pageNumber = int.Parse(path.Substring(length + 1));
                    path = path.Substring(0, length);
                }
            }
            return path;
        }

        internal override void Finish()
        {
            if ((base.formState != XForm.FormState.NotATemplate) && (base.formState != XForm.FormState.Finished))
            {
                base.Finish();
            }
        }

        public static XPdfForm FromFile(string path) => 
            new XPdfForm(path);

        public static XPdfForm FromStream(Stream stream) => 
            new XPdfForm(stream);

        internal PdfDocument ExternalDocument
        {
            get
            {
                if (base.IsTemplate)
                {
                    throw new InvalidOperationException("This XPdfForm is a template and not an imported PDF page; therefore it has no external document.");
                }
                if (this.externalDocument == null)
                {
                    this.externalDocument = PdfDocument.Tls.GetDocument(base.path);
                }
                return this.externalDocument;
            }
        }

        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF.")]
        public override double Height
        {
            get
            {
                PdfPage page = this.ExternalDocument.Pages[this.pageNumber - 1];
                return (double) page.Height;
            }
        }

        public PdfPage Page
        {
            get
            {
                if (base.IsTemplate)
                {
                    return null;
                }
                return this.ExternalDocument.Pages[this.pageNumber - 1];
            }
        }

        public int PageCount
        {
            get
            {
                if (base.IsTemplate)
                {
                    return 1;
                }
                if (this.pageCount == -1)
                {
                    this.pageCount = this.ExternalDocument.Pages.Count;
                }
                return this.pageCount;
            }
        }

        public int PageIndex
        {
            get => 
                (this.PageNumber - 1);
            set
            {
                this.PageNumber = value + 1;
            }
        }

        public int PageNumber
        {
            get => 
                this.pageNumber;
            set
            {
                if (base.IsTemplate)
                {
                    throw new InvalidOperationException("The page number of an XPdfForm template cannot be modified.");
                }
                if (this.pageNumber != value)
                {
                    this.pageNumber = value;
                    base.pdfForm = null;
                }
            }
        }

        public override int PixelHeight =>
            DoubleUtil.DoubleToInt(this.PointHeight);

        public override int PixelWidth =>
            DoubleUtil.DoubleToInt(this.PointWidth);

        public XImage PlaceHolder
        {
            get => 
                this.placeHolder;
            set
            {
                this.placeHolder = value;
            }
        }

        public override double PointHeight
        {
            get
            {
                PdfPage page = this.ExternalDocument.Pages[this.pageNumber - 1];
                return (double) page.Height;
            }
        }

        public override double PointWidth
        {
            get
            {
                PdfPage page = this.ExternalDocument.Pages[this.pageNumber - 1];
                return (double) page.Width;
            }
        }

        public override XSize Size
        {
            get
            {
                PdfPage page = this.ExternalDocument.Pages[this.pageNumber - 1];
                return new XSize((double) page.Width, (double) page.Height);
            }
        }

        public override XMatrix Transform
        {
            get => 
                base.transform;
            set
            {
                if (base.transform != value)
                {
                    base.pdfForm = null;
                    base.transform = value;
                }
            }
        }

        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF.")]
        public override double Width
        {
            get
            {
                PdfPage page = this.ExternalDocument.Pages[this.pageNumber - 1];
                return (double) page.Width;
            }
        }
    }
}

