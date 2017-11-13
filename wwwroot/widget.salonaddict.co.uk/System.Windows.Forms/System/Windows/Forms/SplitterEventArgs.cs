﻿namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class SplitterEventArgs : EventArgs
    {
        private int splitX;
        private int splitY;
        private readonly int x;
        private readonly int y;

        public SplitterEventArgs(int x, int y, int splitX, int splitY)
        {
            this.x = x;
            this.y = y;
            this.splitX = splitX;
            this.splitY = splitY;
        }

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

        public int X =>
            this.x;

        public int Y =>
            this.y;
    }
}

