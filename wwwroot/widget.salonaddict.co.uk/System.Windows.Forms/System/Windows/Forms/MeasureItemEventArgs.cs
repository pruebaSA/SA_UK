namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class MeasureItemEventArgs : EventArgs
    {
        private readonly System.Drawing.Graphics graphics;
        private int index;
        private int itemHeight;
        private int itemWidth;

        public MeasureItemEventArgs(System.Drawing.Graphics graphics, int index)
        {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = 0;
            this.itemWidth = 0;
        }

        public MeasureItemEventArgs(System.Drawing.Graphics graphics, int index, int itemHeight)
        {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = itemHeight;
            this.itemWidth = 0;
        }

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public int Index =>
            this.index;

        public int ItemHeight
        {
            get => 
                this.itemHeight;
            set
            {
                this.itemHeight = value;
            }
        }

        public int ItemWidth
        {
            get => 
                this.itemWidth;
            set
            {
                this.itemWidth = value;
            }
        }
    }
}

