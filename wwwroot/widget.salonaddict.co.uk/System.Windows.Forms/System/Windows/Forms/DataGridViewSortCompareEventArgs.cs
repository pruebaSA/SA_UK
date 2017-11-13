namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class DataGridViewSortCompareEventArgs : HandledEventArgs
    {
        private object cellValue1;
        private object cellValue2;
        private DataGridViewColumn dataGridViewColumn;
        private int rowIndex1;
        private int rowIndex2;
        private int sortResult;

        public DataGridViewSortCompareEventArgs(DataGridViewColumn dataGridViewColumn, object cellValue1, object cellValue2, int rowIndex1, int rowIndex2)
        {
            this.dataGridViewColumn = dataGridViewColumn;
            this.cellValue1 = cellValue1;
            this.cellValue2 = cellValue2;
            this.rowIndex1 = rowIndex1;
            this.rowIndex2 = rowIndex2;
        }

        public object CellValue1 =>
            this.cellValue1;

        public object CellValue2 =>
            this.cellValue2;

        public DataGridViewColumn Column =>
            this.dataGridViewColumn;

        public int RowIndex1 =>
            this.rowIndex1;

        public int RowIndex2 =>
            this.rowIndex2;

        public int SortResult
        {
            get => 
                this.sortResult;
            set
            {
                this.sortResult = value;
            }
        }
    }
}

