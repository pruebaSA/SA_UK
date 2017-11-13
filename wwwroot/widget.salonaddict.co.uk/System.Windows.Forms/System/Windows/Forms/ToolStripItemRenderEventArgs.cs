namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class ToolStripItemRenderEventArgs : EventArgs
    {
        private System.Drawing.Graphics graphics;
        private ToolStripItem item;

        public ToolStripItemRenderEventArgs(System.Drawing.Graphics g, ToolStripItem item)
        {
            this.item = item;
            this.graphics = g;
        }

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public ToolStripItem Item =>
            this.item;

        public System.Windows.Forms.ToolStrip ToolStrip =>
            this.item.ParentInternal;
    }
}

