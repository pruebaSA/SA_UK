namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class SplitterCancelEventArgs : CancelEventArgs
    {
        private readonly int mouseCursorX;
        private readonly int mouseCursorY;
        private int splitX;
        private int splitY;

        public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY) : base(false)
        {
            this.mouseCursorX = mouseCursorX;
            this.mouseCursorY = mouseCursorY;
            this.splitX = splitX;
            this.splitY = splitY;
        }

        public int MouseCursorX =>
            this.mouseCursorX;

        public int MouseCursorY =>
            this.mouseCursorY;

        public int SplitX
        {
            get => 
                this.splitX;
            set
            {
                this.splitX = value;
            }
        }

        public int SplitY
        {
            get => 
                this.splitY;
            set
            {
                this.splitY = value;
            }
        }
    }
}

