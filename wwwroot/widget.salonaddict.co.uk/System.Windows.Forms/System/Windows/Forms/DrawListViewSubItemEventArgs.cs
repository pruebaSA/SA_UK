namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class DrawListViewSubItemEventArgs : EventArgs
    {
        private readonly Rectangle bounds;
        private readonly int columnIndex;
        private bool drawDefault;
        private readonly System.Drawing.Graphics graphics;
        private readonly ColumnHeader header;
        private readonly ListViewItem item;
        private readonly int itemIndex;
        private readonly ListViewItemStates itemState;
        private readonly ListViewItem.ListViewSubItem subItem;

        public DrawListViewSubItemEventArgs(System.Drawing.Graphics graphics, Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
        {
            this.graphics = graphics;
            this.bounds = bounds;
            this.item = item;
            this.subItem = subItem;
            this.itemIndex = itemIndex;
            this.columnIndex = columnIndex;
            this.header = header;
            this.itemState = itemState;
        }

        public void DrawBackground()
        {
            Color color = (this.itemIndex == -1) ? this.item.BackColor : this.subItem.BackColor;
            using (Brush brush = new SolidBrush(color))
            {
                this.Graphics.FillRectangle(brush, this.bounds);
            }
        }

        public void DrawFocusRectangle(Rectangle bounds)
        {
            if ((this.itemState & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(this.graphics, Rectangle.Inflate(bounds, -1, -1), this.item.ForeColor, this.item.BackColor);
            }
        }

        public void DrawText()
        {
            HorizontalAlignment textAlign = this.header.TextAlign;
            TextFormatFlags flags = (textAlign == HorizontalAlignment.Left) ? TextFormatFlags.Default : ((textAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;
            this.DrawText(flags);
        }

        public void DrawText(TextFormatFlags flags)
        {
            string text = (this.itemIndex == -1) ? this.item.Text : this.subItem.Text;
            Font font = (this.itemIndex == -1) ? this.item.Font : this.subItem.Font;
            Color foreColor = (this.itemIndex == -1) ? this.item.ForeColor : this.subItem.ForeColor;
            int width = TextRenderer.MeasureText(" ", font).Width;
            Rectangle bounds = Rectangle.Inflate(this.bounds, -width, 0);
            TextRenderer.DrawText(this.graphics, text, font, bounds, foreColor, flags);
        }

        public Rectangle Bounds =>
            this.bounds;

        public int ColumnIndex =>
            this.columnIndex;

        public bool DrawDefault
        {
            get => 
                this.drawDefault;
            set
            {
                this.drawDefault = value;
            }
        }

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public ColumnHeader Header =>
            this.header;

        public ListViewItem Item =>
            this.item;

        public int ItemIndex =>
            this.itemIndex;

        public ListViewItemStates ItemState =>
            this.itemState;

        public ListViewItem.ListViewSubItem SubItem =>
            this.subItem;
    }
}

