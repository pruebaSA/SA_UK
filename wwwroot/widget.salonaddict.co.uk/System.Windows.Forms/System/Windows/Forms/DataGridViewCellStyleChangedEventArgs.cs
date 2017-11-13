namespace System.Windows.Forms
{
    using System;

    internal class DataGridViewCellStyleChangedEventArgs : EventArgs
    {
        private bool changeAffectsPreferredSize;

        internal DataGridViewCellStyleChangedEventArgs()
        {
        }

        internal bool ChangeAffectsPreferredSize
        {
            get => 
                this.changeAffectsPreferredSize;
            set
            {
                this.changeAffectsPreferredSize = value;
            }
        }
    }
}

