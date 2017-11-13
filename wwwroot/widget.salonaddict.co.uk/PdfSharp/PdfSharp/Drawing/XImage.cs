namespace PdfSharp.Drawing
{
    using PdfSharp;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Drawing;
    using System.IO;

    public class XImage : IDisposable
    {
        private bool disposed;
        private XImageFormat format;
        internal Image gdiImage;
        private bool interpolate;
        internal string path;
        internal PdfImageTable.ImageSelector selector;

        protected XImage()
        {
            this.interpolate = true;
        }

        private XImage(Image image)
        {
            this.interpolate = true;
            this.gdiImage = image;
            this.Initialize();
        }

        private XImage(Stream stream)
        {
            this.interpolate = true;
            this.path = "*" + Guid.NewGuid().ToString("B");
            this.gdiImage = Image.FromStream(stream);
            this.Initialize();
        }

        private XImage(string path)
        {
            this.interpolate = true;
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(PSSR.FileNotFound(path), path);
            }
            this.path = path;
            this.gdiImage = Image.FromFile(path);
            this.Initialize();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }
            if (this.gdiImage != null)
            {
                this.gdiImage.Dispose();
                this.gdiImage = null;
            }
        }

        public static bool ExistsFile(string path) => 
            ((PdfReader.TestPdfFile(path) > 0) || File.Exists(path));

        public static XImage FromFile(string path)
        {
            if (PdfReader.TestPdfFile(path) > 0)
            {
                return new XPdfForm(path);
            }
            return new XImage(path);
        }

        public static XImage FromGdiPlusImage(Image image) => 
            new XImage(image);

        private void Initialize()
        {
            if (this.gdiImage != null)
            {
                switch (this.gdiImage.RawFormat.Guid.ToString("B").ToUpper())
                {
                    case "{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}":
                    case "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}":
                    case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":
                        this.format = XImageFormat.Png;
                        return;

                    case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":
                        this.format = XImageFormat.Jpeg;
                        return;

                    case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":
                        this.format = XImageFormat.Gif;
                        return;

                    case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":
                        this.format = XImageFormat.Tiff;
                        return;

                    case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":
                        this.format = XImageFormat.Icon;
                        return;
                }
                throw new InvalidOperationException("Unsupported image format.");
            }
        }

        public static implicit operator XImage(Image image) => 
            new XImage(image);

        public XImageFormat Format =>
            this.format;

        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelHeight, but will become PointHeight in future releases of PDFsharp.")]
        public virtual double Height =>
            ((double) this.gdiImage.Height);

        public virtual double HorizontalResolution =>
            ((double) this.gdiImage.HorizontalResolution);

        public virtual bool Interpolate
        {
            get => 
                this.interpolate;
            set
            {
                this.interpolate = value;
            }
        }

        public virtual int PixelHeight =>
            this.gdiImage.Height;

        public virtual int PixelWidth =>
            this.gdiImage.Width;

        public virtual double PointHeight =>
            ((double) (((float) (this.gdiImage.Height * 0x48)) / this.gdiImage.HorizontalResolution));

        public virtual double PointWidth =>
            ((double) (((float) (this.gdiImage.Width * 0x48)) / this.gdiImage.HorizontalResolution));

        public virtual XSize Size =>
            new XSize(this.PointWidth, this.PointHeight);

        public virtual double VerticalResolution =>
            ((double) this.gdiImage.VerticalResolution);

        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelWidth, but will become PointWidth in future releases of PDFsharp.")]
        public virtual double Width =>
            ((double) this.gdiImage.Width);
    }
}

