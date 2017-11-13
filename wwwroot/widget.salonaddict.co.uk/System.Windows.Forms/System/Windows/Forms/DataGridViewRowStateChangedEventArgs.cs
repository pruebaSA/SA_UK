namespace System.Windows.Forms
{
    using System;

    public class DataGridViewRowStateChangedEventArgs : EventArgs
    {
        private DataGridViewRow dataGridViewRow;
        private DataGridViewElementStates stateChanged;

        public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
        {
            this.dataGridViewRow = dataGridViewRow;
            this.stateChanged = stateChanged;
        }

        public DataGridViewRow Row =>
            this.dataGridViewRow;

        public DataGridViewElementStates StateChanged =>
            this.stateChanged;
    }
}

