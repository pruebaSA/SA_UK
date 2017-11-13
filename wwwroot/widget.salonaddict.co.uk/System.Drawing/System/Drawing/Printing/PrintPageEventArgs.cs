namespace System.Drawing.Printing
{
    using System;
    using System.Drawing;

    public class PrintPageEventArgs : EventArgs
    {
        private bool cancel;
        private System.Drawing.Graphics graphics;
        private bool hasMorePages;
        private readonly Rectangle marginBounds;
        private readonly Rectangle pageBounds;
        private readonly System.Drawing.Printing.PageSettings pageSettings;

        public PrintPageEventArgs(System.Drawing.Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, System.Drawing.Printing.PageSettings pageSettings)
        {
            this.graphics = graphics;
            this.marginBounds = marginBounds;
            this.pageBounds = pageBounds;
            this.pageSettings = pageSettings;
        }

        internal void Dispose()
        {
            this.graphics.Dispose();
        }

        internal void SetGraphics(System.Drawing.Graphics value)
        {
            this.graphics = value;
        }

        public bool Cancel
        {
            get => 
                this.cancel;
            set
            {
                this.cancel = value;
            }
        }

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public bool HasMorePages
        {
            get => 
                this.hasMorePages;
            set
            {
                this.hasMorePages = value;
            }
        }

        public Rectangle MarginBounds =>
            this.marginBounds;

        public Rectangle PageBounds =>
            this.pageBounds;

        public System.Drawing.Printing.PageSettings PageSettings =>
            this.pageSettings;
    }
}

