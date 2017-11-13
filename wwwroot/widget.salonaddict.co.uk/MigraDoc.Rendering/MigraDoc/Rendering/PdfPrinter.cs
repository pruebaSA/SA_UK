namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering.Resources;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.IO;
    using System.Reflection;

    [Obsolete("Use class PdfDocumentRenderer.")]
    public class PdfPrinter
    {
        private MigraDoc.DocumentObjectModel.Document document;
        private MigraDoc.Rendering.DocumentRenderer documentRenderer;
        private PdfSharp.Pdf.PdfDocument pdfDocument;
        private string workingDirectory;

        private void PrepareDocumentRenderer()
        {
            if (this.document == null)
            {
                throw new InvalidOperationException(Messages.PropertyNotSetBefore("DocumentRenderer", MethodBase.GetCurrentMethod().Name));
            }
            this.documentRenderer = new MigraDoc.Rendering.DocumentRenderer(this.document);
            this.documentRenderer.WorkingDirectory = this.workingDirectory;
            this.documentRenderer.PrepareDocument();
        }

        public void PrintDocument()
        {
            if (this.documentRenderer == null)
            {
                this.PrepareDocumentRenderer();
            }
            if (this.pdfDocument == null)
            {
                this.pdfDocument = new PdfSharp.Pdf.PdfDocument();
                this.pdfDocument.Info.Creator = "MigraDoc 1.32.3885 (www.migradoc.com)";
            }
            this.WriteDocumentInformation();
            this.PrintPages(1, this.documentRenderer.FormattedDocument.PageCount);
        }

        public void PrintPages(int startPage, int endPage)
        {
            if (startPage < 1)
            {
                throw new ArgumentOutOfRangeException("startPage");
            }
            if (endPage > this.documentRenderer.FormattedDocument.PageCount)
            {
                throw new ArgumentOutOfRangeException("endPage");
            }
            if (this.documentRenderer == null)
            {
                this.PrepareDocumentRenderer();
            }
            if (this.pdfDocument == null)
            {
                this.pdfDocument = new PdfSharp.Pdf.PdfDocument();
                this.pdfDocument.Info.Creator = "MigraDoc 1.32.3885 (www.migradoc.com)";
            }
            this.documentRenderer.printDate = DateTime.Now;
            for (int i = startPage; i <= endPage; i++)
            {
                PdfPage page = this.pdfDocument.AddPage();
                PageInfo pageInfo = this.documentRenderer.FormattedDocument.GetPageInfo(i);
                page.Width = pageInfo.Width;
                page.Height = pageInfo.Height;
                page.Orientation = pageInfo.Orientation;
                this.documentRenderer.RenderPage(XGraphics.FromPdfPage(page), i);
            }
        }

        public void Save(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path == "")
            {
                throw new ArgumentException("PDF file Path must not be empty");
            }
            if (this.workingDirectory != null)
            {
                Path.Combine(this.workingDirectory, path);
            }
            this.pdfDocument.Save(path);
        }

        public void Save(Stream stream, bool closeStream)
        {
            this.pdfDocument.Save(stream, closeStream);
        }

        private void WriteDocumentInformation()
        {
            if (!this.document.IsNull("Info"))
            {
                DocumentInfo info = this.document.Info;
                PdfDocumentInformation information = this.pdfDocument.Info;
                if (!info.IsNull("Author"))
                {
                    information.Author = info.Author;
                }
                if (!info.IsNull("Keywords"))
                {
                    information.Keywords = info.Keywords;
                }
                if (!info.IsNull("Subject"))
                {
                    information.Subject = info.Subject;
                }
                if (!info.IsNull("Title"))
                {
                    information.Title = info.Title;
                }
            }
        }

        public MigraDoc.DocumentObjectModel.Document Document
        {
            set
            {
                this.document = null;
                value.BindToRenderer(this);
                this.document = value;
            }
        }

        public MigraDoc.Rendering.DocumentRenderer DocumentRenderer
        {
            get => 
                this.documentRenderer;
            set
            {
                this.documentRenderer = value;
            }
        }

        public PdfSharp.Pdf.PdfDocument PdfDocument
        {
            get => 
                this.pdfDocument;
            set
            {
                this.pdfDocument = value;
            }
        }

        public string WorkingDirectory
        {
            get => 
                this.workingDirectory;
            set
            {
                this.workingDirectory = value;
            }
        }
    }
}

