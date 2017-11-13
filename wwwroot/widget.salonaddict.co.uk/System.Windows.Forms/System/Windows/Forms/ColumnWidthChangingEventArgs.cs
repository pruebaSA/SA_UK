﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class ColumnWidthChangingEventArgs : CancelEventArgs
    {
        private int columnIndex;
        private int newWidth;

        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth)
        {
            this.columnIndex = columnIndex;
            this.newWidth = newWidth;
        }

        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel) : base(cancel)
        {
            this.columnIndex = columnIndex;
            this.newWidth = newWidth;
        }

        public int ColumnIndex =>
            this.columnIndex;

        public int NewWidth
        {
            get => 
                this.newWidth;
            set
            {
                this.newWidth = value;
            }
        }
    }
}

