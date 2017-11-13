namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
    {
        private int height;
        private int minimumHeight;
        private int rowIndex;

        internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight) : base(false)
        {
            this.rowIndex = rowIndex;
            this.height = height;
            this.minimumHeight = minimumHeight;
        }

        public int Height =>
            this.height;

        public int MinimumHeight =>
            this.minimumHeight;

        public int RowIndex =>
            this.rowIndex;
    }
}

