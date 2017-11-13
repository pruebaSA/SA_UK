namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class MouseEventArgs : EventArgs
    {
        private readonly MouseButtons button;
        private readonly int clicks;
        private readonly int delta;
        private readonly int x;
        private readonly int y;

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }

        public MouseButtons Button =>
            this.button;

        public int Clicks =>
            this.clicks;

        public int Delta =>
            this.delta;

        public Point Location =>
            new Point(this.x, this.y);

        public int X =>
            this.x;

        public int Y =>
            this.y;
    }
}

