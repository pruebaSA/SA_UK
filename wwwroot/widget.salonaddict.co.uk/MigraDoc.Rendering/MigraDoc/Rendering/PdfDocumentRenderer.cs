namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering.Resources;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.IO;
    using System.Reflection;

    public class PdfDocumentRenderer
    {
        private MigraDoc.DocumentObjectModel.Document document;
        private MigraDoc.Rendering.DocumentRenderer documentRenderer;
        private PdfFontEmbedding fontEmbedding;
        private string language;
        private PdfSharp.Pdf.PdfDocument pdfDocument;
        private bool unicode;
        private string workingDirectory;

        public PdfDocumentRenderer()
        {
            this.language = string.Empty;
        }

        public PdfDocumentRenderer(bool unicode)
        {
            this.language = string.Empty;
            this.unicode = unicode;
        }

        public PdfDocumentRenderer(bool unicode, PdfFontEmbedding fontEmbedding)
        {
            this.language = string.Empty;
            this.unicode = unicode;
            this.fontEmbedding = fontEmbedding;
        }

        private PdfSharp.Pdf.PdfDocument CreatePdfDocument()
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument {
                Info = { Creator = "MigraDoc 1.32.3885 (www.migradoc.com)" }
            };
            if ((this.language != null) && (this.language.Length != 0))
            {
                document.Language = this.language;
            }
            return document;
        }

        private void PrepareDocumentRenderer()
        {
            this.PrepareDocumentRenderer(false);
        }

        private void PrepareDocumentRenderer(bool prepareCompletely)
        {
            if (this.document == null)
            {
                throw new InvalidOperationException(Messages.PropertyNotSetBefore("DocumentRenderer", MethodBase.GetCurrentMethod().Name));
            }
            if (this.documentRenderer == null)
            {
                this.documentRenderer = new MigraDoc.Rendering.DocumentRenderer(this.document);
                this.documentRenderer.WorkingDirectory = this.workingDirectory;
            }
            if (prepareCompletely && (this.documentRenderer.formattedDocument == null))
            {
                this.documentRenderer.PrepareDocument();
            }
        }

        public void PrepareRenderPages()
        {
            this.PrepareDocumentRenderer(true);
            if (this.pdfDocument == null)
            {
                this.pdfDocument = this.CreatePdfDocument();
                if (this.document.UseCmykColor)
                {
                    this.pdfDocument.Options.ColorMode = PdfColorMode.Cmyk;
                }
            }
            this.WriteDocumentInformation();
        }

        public void RenderDocument()
        {
            this.PrepareRenderPages();
            this.RenderPages(1, this.documentRenderer.FormattedDocument.PageCount);
        }

        public void RenderPages(int startPage, int endPage)
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
                this.pdfDocument = this.CreatePdfDocument();
            }
            this.documentRenderer.printDate = DateTime.Now;
            for (int i = startPage; i <= endPage; i++)
            {
                PdfPage page = this.pdfDocument.AddPage();
                PageInfo pageInfo = this.documentRenderer.FormattedDocument.GetPageInfo(i);
                page.Width = pageInfo.Width;
                page.Height = pageInfo.Height;
                page.Orientation = pageInfo.Orientation;
                using (XGraphics graphics = XGraphics.FromPdfPage(page))
                {
                    graphics.MUH = this.unicode ? PdfFontEncoding.Unicode : PdfFontEncoding.WinAnsi;
                    graphics.MFEH = this.fontEmbedding;
                    this.documentRenderer.RenderPage(graphics, i);
                }
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

        public void WriteDocumentInformation()
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
            get
            {
                if (this.documentRenderer == null)
                {
                    this.PrepareDocumentRenderer();
                }
                return this.documentRenderer;
            }
            set
            {
                this.documentRenderer = value;
            }
        }

        public PdfFontEmbedding FontEmbedding =>
            this.fontEmbedding;

        public string Language
        {
            get => 
                this.language;
            set
            {
                this.language = value;
            }
        }

        public int PageCount =>
            this.documentRenderer.FormattedDocument.PageCount;

        public PdfSharp.Pdf.PdfDocument PdfDocument
        {
            get => 
                this.pdfDocument;
            set
            {
                this.pdfDocument = value;
            }
        }

        public bool Unicode =>
            this.unicode;

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

