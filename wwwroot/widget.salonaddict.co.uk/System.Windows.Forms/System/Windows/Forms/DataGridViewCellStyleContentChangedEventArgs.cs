namespace System.Windows.Forms
{
    using System;

    public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
    {
        private bool changeAffectsPreferredSize;
        private DataGridViewCellStyle dataGridViewCellStyle;

        internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle dataGridViewCellStyle, bool changeAffectsPreferredSize)
        {
            this.dataGridViewCellStyle = dataGridViewCellStyle;
            this.changeAffectsPreferredSize = changeAffectsPreferredSize;
        }

        public DataGridViewCellStyle CellStyle =>
            this.dataGridViewCellStyle;

        public DataGridViewCellStyleScopes CellStyleScope =>
            this.dataGridViewCellStyle.Scope;

        internal bool ChangeAffectsPreferredSize =>
            this.changeAffectsPreferredSize;
    }
}

