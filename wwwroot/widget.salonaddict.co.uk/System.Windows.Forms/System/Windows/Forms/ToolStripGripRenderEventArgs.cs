namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs
    {
        public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip)
        {
        }

        public Rectangle GripBounds =>
            base.ToolStrip.GripRectangle;

        public ToolStripGripDisplayStyle GripDisplayStyle =>
            base.ToolStrip.GripDisplayStyle;

        public ToolStripGripStyle GripStyle =>
            base.ToolStrip.GripStyle;
    }
}

