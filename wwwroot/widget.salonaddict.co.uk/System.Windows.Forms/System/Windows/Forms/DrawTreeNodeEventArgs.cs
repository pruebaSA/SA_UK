namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class DrawTreeNodeEventArgs : EventArgs
    {
        private readonly Rectangle bounds;
        private bool drawDefault;
        private readonly System.Drawing.Graphics graphics;
        private readonly TreeNode node;
        private readonly TreeNodeStates state;

        public DrawTreeNodeEventArgs(System.Drawing.Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
        {
            this.graphics = graphics;
            this.node = node;
            this.bounds = bounds;
            this.state = state;
            this.drawDefault = false;
        }

        public Rectangle Bounds =>
            this.bounds;

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

        public TreeNode Node =>
            this.node;

        public TreeNodeStates State =>
            this.state;
    }
}

