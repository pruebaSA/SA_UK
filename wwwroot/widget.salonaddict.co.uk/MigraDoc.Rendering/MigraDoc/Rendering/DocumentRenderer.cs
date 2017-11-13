namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using MigraDoc.Rendering.Resources;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class DocumentRenderer
    {
        private Document document;
        internal MigraDoc.Rendering.FormattedDocument formattedDocument;
        private ListInfo previousListInfo;
        private Hashtable previousListNumbers;
        internal DateTime printDate = DateTime.MinValue;
        internal XPrivateFontCollection privateFonts;
        internal int ProgressCompleted;
        internal int ProgressMaximum;
        private string workingDirectory;

        public event PrepareDocumentProgressEventHandler PrepareDocumentProgress;

        public DocumentRenderer(Document document)
        {
            this.document = document;
        }

        internal void AddOutline(int level, string title, PdfPage destinationPage)
        {
            if ((level >= 1) && (destinationPage != null))
            {
                PdfDocument owner = destinationPage.Owner;
                if (owner != null)
                {
                    PdfOutline.PdfOutlineCollection outlines = owner.Outlines;
                    while (--level > 0)
                    {
                        int count = outlines.Count;
                        if (count == 0)
                        {
                            outlines = outlines.Add(" ", destinationPage, true).Outlines;
                        }
                        else
                        {
                            outlines = outlines[count - 1].Outlines;
                        }
                    }
                    outlines.Add(title, destinationPage, true);
                }
            }
        }

        public DocumentObject[] GetDocumentObjectsFromPage(int page)
        {
            RenderInfo[] renderInfos = this.formattedDocument.GetRenderInfos(page);
            int num = (renderInfos != null) ? renderInfos.Length : 0;
            DocumentObject[] objArray = new DocumentObject[num];
            for (int i = 0; i < num; i++)
            {
                objArray[i] = renderInfos[i].DocumentObject;
            }
            return objArray;
        }

        internal int NextListNumber(ListInfo listInfo)
        {
            ListType listType = listInfo.ListType;
            bool flag = ((listType == ListType.NumberList1) || (listType == ListType.NumberList2)) || (listType == ListType.NumberList3);
            int num = -2147483648;
            if (listInfo == this.previousListInfo)
            {
                if (flag)
                {
                    return (int) this.previousListNumbers[listType];
                }
                return num;
            }
            if (flag)
            {
                num = 1;
                if (listInfo.IsNull("ContinuePreviousList") || listInfo.ContinuePreviousList)
                {
                    num = ((int) this.previousListNumbers[listType]) + 1;
                }
                this.previousListNumbers[listType] = num;
            }
            this.previousListInfo = listInfo;
            return num;
        }

        internal virtual void OnPrepareDocumentProgress(int value, int maximum)
        {
            if (this.PrepareDocumentProgress != null)
            {
                PrepareDocumentProgressEventArgs e = new PrepareDocumentProgressEventArgs(value, maximum);
                this.PrepareDocumentProgress(this, e);
            }
        }

        public void PrepareDocument()
        {
            new PdfFlattenVisitor().Visit(this.document);
            this.previousListNumbers = new Hashtable(3);
            this.previousListNumbers[ListType.NumberList1] = 0;
            this.previousListNumbers[ListType.NumberList2] = 0;
            this.previousListNumbers[ListType.NumberList3] = 0;
            this.formattedDocument = new MigraDoc.Rendering.FormattedDocument(this.document, this);
            XGraphics gfx = XGraphics.CreateMeasureContext(new XSize(2000.0, 2000.0), XGraphicsUnit.Point, XPageDirection.Downwards);
            this.previousListInfo = null;
            this.formattedDocument.Format(gfx);
        }

        private void RenderFooter(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedFooter = this.formattedDocument.GetFormattedFooter(page);
            if (formattedFooter != null)
            {
                Rectangle footerArea = this.formattedDocument.GetFooterArea(page);
                RenderInfo[] renderInfos = formattedFooter.GetRenderInfos();
                XUnit unit = (((double) footerArea.Y) + ((double) footerArea.Height)) - ((double) RenderInfo.GetTotalHeight(renderInfos));
                FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);
                foreach (RenderInfo info in renderInfos)
                {
                    Renderer renderer = Renderer.Create(graphics, this, info, fieldInfos);
                    XUnit y = renderer.RenderInfo.LayoutInfo.ContentArea.Y;
                    renderer.RenderInfo.LayoutInfo.ContentArea.Y = unit;
                    renderer.Render();
                    renderer.RenderInfo.LayoutInfo.ContentArea.Y = y;
                }
            }
        }

        private void RenderHeader(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedHeader = this.formattedDocument.GetFormattedHeader(page);
            if (formattedHeader != null)
            {
                this.formattedDocument.GetHeaderArea(page);
                RenderInfo[] renderInfos = formattedHeader.GetRenderInfos();
                FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);
                foreach (RenderInfo info in renderInfos)
                {
                    Renderer.Create(graphics, this, info, fieldInfos).Render();
                }
            }
        }

        public void RenderObject(XGraphics graphics, XUnit xPosition, XUnit yPosition, XUnit width, DocumentObject documentObject)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            if (documentObject == null)
            {
                throw new ArgumentNullException("documentObject");
            }
            if ((!(documentObject is Shape) && !(documentObject is Table)) && !(documentObject is Paragraph))
            {
                throw new ArgumentException(Messages.ObjectNotRenderable, "documentObject");
            }
            Renderer renderer = Renderer.Create(graphics, this, documentObject, null);
            renderer.Format(new Rectangle(xPosition, yPosition, width, 1.7976931348623157E+308), null);
            RenderInfo renderInfo = renderer.RenderInfo;
            renderInfo.LayoutInfo.ContentArea.X = xPosition;
            renderInfo.LayoutInfo.ContentArea.Y = yPosition;
            Renderer.Create(graphics, this, renderer.RenderInfo, null).Render();
        }

        public void RenderPage(XGraphics gfx, int page)
        {
            this.RenderPage(gfx, page, PageRenderOptions.All);
        }

        public void RenderPage(XGraphics gfx, int page, PageRenderOptions options)
        {
            if (!this.formattedDocument.IsEmptyPage(page))
            {
                FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);
                if (this.printDate != DateTime.MinValue)
                {
                    fieldInfos.date = this.printDate;
                }
                else
                {
                    fieldInfos.date = DateTime.Now;
                }
                if ((options & PageRenderOptions.RenderHeader) == PageRenderOptions.RenderHeader)
                {
                    this.RenderHeader(gfx, page);
                }
                if ((options & PageRenderOptions.RenderFooter) == PageRenderOptions.RenderFooter)
                {
                    this.RenderFooter(gfx, page);
                }
                if ((options & PageRenderOptions.RenderContent) == PageRenderOptions.RenderContent)
                {
                    RenderInfo[] renderInfos = this.formattedDocument.GetRenderInfos(page);
                    int length = renderInfos.Length;
                    for (int i = 0; i < length; i++)
                    {
                        RenderInfo renderInfo = renderInfos[i];
                        Renderer.Create(gfx, this, renderInfo, fieldInfos).Render();
                    }
                }
            }
        }

        public MigraDoc.Rendering.FormattedDocument FormattedDocument =>
            this.formattedDocument;

        public bool HasPrepareDocumentProgress =>
            (this.PrepareDocumentProgress != null);

        public XPrivateFontCollection PrivateFonts
        {
            get => 
                this.privateFonts;
            set
            {
                this.privateFonts = value;
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

        public class PrepareDocumentProgressEventArgs : EventArgs
        {
            public int Maximum;
            public int Value;

            public PrepareDocumentProgressEventArgs(int value, int maximum)
            {
                this.Value = value;
                this.Maximum = maximum;
            }
        }

        public delegate void PrepareDocumentProgressEventHandler(object sender, DocumentRenderer.PrepareDocumentProgressEventArgs e);
    }
}

