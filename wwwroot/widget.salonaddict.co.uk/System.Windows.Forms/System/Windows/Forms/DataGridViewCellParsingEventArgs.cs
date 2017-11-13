namespace System.Windows.Forms
{
    using System;

    public class DataGridViewCellParsingEventArgs : ConvertEventArgs
    {
        private int columnIndex;
        private DataGridViewCellStyle inheritedCellStyle;
        private bool parsingApplied;
        private int rowIndex;

        public DataGridViewCellParsingEventArgs(int rowIndex, int columnIndex, object value, System.Type desiredType, DataGridViewCellStyle inheritedCellStyle) : base(value, desiredType)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.inheritedCellStyle = inheritedCellStyle;
        }

        public int ColumnIndex =>
            this.columnIndex;

        public DataGridViewCellStyle InheritedCellStyle
        {
            get => 
                this.inheritedCellStyle;
            set
            {
                this.inheritedCellStyle = value;
            }
        }

        public bool ParsingApplied
        {
            get => 
                this.parsingApplied;
            set
            {
                this.parsingApplied = value;
            }
        }

        public int RowIndex =>
            this.rowIndex;
    }
}

