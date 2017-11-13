namespace System.Windows.Forms
{
    using System;

    public class DataGridViewColumnStateChangedEventArgs : EventArgs
    {
        private DataGridViewColumn dataGridViewColumn;
        private DataGridViewElementStates stateChanged;

        public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
        {
            this.dataGridViewColumn = dataGridViewColumn;
            this.stateChanged = stateChanged;
        }

        public DataGridViewColumn Column =>
            this.dataGridViewColumn;

        public DataGridViewElementStates StateChanged =>
            this.stateChanged;
    }
}

