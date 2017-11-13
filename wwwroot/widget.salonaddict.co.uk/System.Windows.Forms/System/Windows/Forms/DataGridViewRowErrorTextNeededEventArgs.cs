namespace System.Windows.Forms
{
    using System;

    public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
    {
        private string errorText;
        private int rowIndex;

        internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
        {
            this.rowIndex = rowIndex;
            this.errorText = errorText;
        }

        public string ErrorText
        {
            get => 
                this.errorText;
            set
            {
                this.errorText = value;
            }
        }

        public int RowIndex =>
            this.rowIndex;
    }
}

