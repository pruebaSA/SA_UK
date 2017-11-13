namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class ToolStripArrowRenderEventArgs : EventArgs
    {
        private Color arrowColor = Color.Empty;
        private bool arrowColorChanged;
        private ArrowDirection arrowDirection = ArrowDirection.Down;
        private Rectangle arrowRect = Rectangle.Empty;
        private Color defaultArrowColor = Color.Empty;
        private System.Drawing.Graphics graphics;
        private ToolStripItem item;

        public ToolStripArrowRenderEventArgs(System.Drawing.Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
        {
            this.item = toolStripItem;
            this.graphics = g;
            this.arrowRect = arrowRectangle;
            this.defaultArrowColor = arrowColor;
            this.arrowDirection = arrowDirection;
        }

        public Color ArrowColor
        {
            get
            {
                if (this.arrowColorChanged)
                {
                    return this.arrowColor;
                }
                return this.DefaultArrowColor;
            }
            set
            {
                this.arrowColor = value;
                this.arrowColorChanged = true;
            }
        }

        public Rectangle ArrowRectangle
        {
            get => 
                this.arrowRect;
            set
            {
                this.arrowRect = value;
            }
        }

        internal Color DefaultArrowColor
        {
            get => 
                this.defaultArrowColor;
            set
            {
                this.defaultArrowColor = value;
            }
        }

        public ArrowDirection Direction
        {
            get => 
                this.arrowDirection;
            set
            {
                this.arrowDirection = value;
            }
        }

        public System.Drawing.Graphics Graphics =>
            this.graphics;

        public ToolStripItem Item =>
            this.item;
    }
}

