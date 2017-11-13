﻿namespace System.Windows.Forms
{
    using System;

    public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
    {
        private DataGridViewColumn dataGridViewColumn;
        private DataGridViewAutoSizeColumnMode previousMode;

        public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            this.dataGridViewColumn = dataGridViewColumn;
            this.previousMode = previousMode;
        }

        public DataGridViewColumn Column =>
            this.dataGridViewColumn;

        public DataGridViewAutoSizeColumnMode PreviousMode =>
            this.previousMode;
    }
}

