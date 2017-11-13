namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class InvalidateEventArgs : EventArgs
    {
        private readonly Rectangle invalidRect;

        public InvalidateEventArgs(Rectangle invalidRect)
        {
            this.invalidRect = invalidRect;
        }

        public Rectangle InvalidRect =>
            this.invalidRect;
    }
}

