namespace System.Windows.Forms
{
    using System;

    public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
    {
        private DataGridViewCellStyle cellStyle;
        private int columnIndex;
        private bool formattingApplied;
        private int rowIndex;

        public DataGridViewCellFormattingEventArgs(int columnIndex, int rowIndex, object value, System.Type desiredType, DataGridViewCellStyle cellStyle) : base(value, desiredType)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }
            this.columnIndex = columnIndex;
            this.rowIndex = rowIndex;
            this.cellStyle = cellStyle;
        }

        public DataGridViewCellStyle CellStyle
        {
            get => 
                this.cellStyle;
            set
            {
                this.cellStyle = value;
            }
        }

        public int ColumnIndex =>
            this.columnIndex;

        public bool FormattingApplied
        {
            get => 
                this.formattingApplied;
            set
            {
                this.formattingApplied = value;
            }
        }

        public int RowIndex =>
            this.rowIndex;
    }
}

