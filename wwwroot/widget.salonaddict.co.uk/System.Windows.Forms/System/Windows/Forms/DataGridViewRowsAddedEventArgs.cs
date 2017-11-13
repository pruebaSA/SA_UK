﻿namespace System.Windows.Forms
{
    using System;

    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        private int rowCount;
        private int rowIndex;

        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
        {
            this.rowIndex = rowIndex;
            this.rowCount = rowCount;
        }

        public int RowCount =>
            this.rowCount;

        public int RowIndex =>
            this.rowIndex;
    }
}

