namespace MigraDoc.Rendering.Printing
{
    using MigraDoc.Rendering;
    using PdfSharp;
    using PdfSharp.Drawing;
    using System;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Runtime.InteropServices;

    public class MigraDocPrintDocument : PrintDocument
    {
        private int pageCount;
        private int pageNumber;
        private const int PHYSICALOFFSETX = 0x70;
        private const int PHYSICALOFFSETY = 0x71;
        private DocumentRenderer renderer;
        private int selectedPage;

        public MigraDocPrintDocument()
        {
            this.pageNumber = -1;
            base.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            base.OriginAtMargins = false;
        }

        public MigraDocPrintDocument(DocumentRenderer renderer)
        {
            this.pageNumber = -1;
            this.renderer = renderer;
            base.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            base.OriginAtMargins = false;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int capability);
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            if (!e.Cancel)
            {
                switch (base.PrinterSettings.PrintRange)
                {
                    case PrintRange.AllPages:
                        this.pageNumber = 1;
                        this.pageCount = this.renderer.FormattedDocument.PageCount;
                        return;

                    case PrintRange.Selection:
                        this.pageNumber = this.selectedPage;
                        this.pageCount = 1;
                        return;

                    case PrintRange.SomePages:
                        this.pageNumber = base.PrinterSettings.FromPage;
                        this.pageCount = (base.PrinterSettings.ToPage - base.PrinterSettings.FromPage) + 1;
                        return;
                }
                e.Cancel = true;
            }
        }

        protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);
            this.pageNumber = -1;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            if (!e.Cancel)
            {
                PageSettings pageSettings = e.PageSettings;
                try
                {
                    Graphics graphics = e.Graphics;
                    IntPtr hdc = graphics.GetHdc();
                    int deviceCaps = GetDeviceCaps(hdc, 0x70);
                    int num2 = GetDeviceCaps(hdc, 0x71);
                    graphics.ReleaseHdc(hdc);
                    graphics.TranslateTransform(((float) (-deviceCaps * 100)) / graphics.DpiX, ((float) (-num2 * 100)) / graphics.DpiY);
                    XSize size = new XSize((((double) e.PageSettings.Bounds.Width) / 100.0) * 72.0, (((double) e.PageSettings.Bounds.Height) / 100.0) * 72.0);
                    float sx = 1.388889f;
                    graphics.ScaleTransform(sx, sx);
                    XGraphics gfx = XGraphics.FromGraphics(graphics, size);
                    this.renderer.RenderPage(gfx, this.pageNumber);
                }
                catch
                {
                    e.Cancel = true;
                }
                this.pageNumber++;
                this.pageCount--;
                e.HasMorePages = this.pageCount > 0;
            }
        }

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            base.OnQueryPageSettings(e);
            if (!e.Cancel)
            {
                PageSettings pageSettings = e.PageSettings;
                PageInfo pageInfo = this.renderer.FormattedDocument.GetPageInfo(this.pageNumber);
                pageSettings.Landscape = pageInfo.Orientation == PageOrientation.Landscape;
            }
        }

        public DocumentRenderer Renderer
        {
            get => 
                this.renderer;
            set
            {
                this.renderer = value;
            }
        }

        public int SelectedPage
        {
            get => 
                this.selectedPage;
            set
            {
                this.selectedPage = value;
            }
        }
    }
}

