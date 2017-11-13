namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class TableLayoutCellPaintEventArgs : PaintEventArgs
    {
        private Rectangle bounds;
        private int column;
        private int row;

        public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row) : base(g, clipRectangle)
        {
            this.bounds = cellBounds;
            this.row = row;
            this.column = column;
        }

        public Rectangle CellBounds =>
            this.bounds;

        public int Column =>
            this.column;

        public int Row =>
            this.row;
    }
}

