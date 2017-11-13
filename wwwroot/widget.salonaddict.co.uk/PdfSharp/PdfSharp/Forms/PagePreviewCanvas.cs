namespace PdfSharp.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal class PagePreviewCanvas : Control
    {
        private PagePreview preview;

        public PagePreviewCanvas(PagePreview preview)
        {
            this.preview = preview;
            base.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.preview.showPage)
            {
                bool flag;
                Graphics gfx = e.Graphics;
                this.preview.CalculatePreviewDimension(out flag);
                this.preview.RenderPage(gfx);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!this.preview.showPage)
            {
                e.Graphics.Clear(this.preview.desktopColor);
            }
            else
            {
                bool flag;
                this.preview.CalculatePreviewDimension(out flag);
                this.preview.PaintBackground(e.Graphics);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.Invalidate();
        }
    }
}

