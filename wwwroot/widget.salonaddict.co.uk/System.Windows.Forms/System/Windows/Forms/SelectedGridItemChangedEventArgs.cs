namespace System.Windows.Forms
{
    using System;

    public class SelectedGridItemChangedEventArgs : EventArgs
    {
        private GridItem newSelection;
        private GridItem oldSelection;

        public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
        {
            this.oldSelection = oldSel;
            this.newSelection = newSel;
        }

        public GridItem NewSelection =>
            this.newSelection;

        public GridItem OldSelection =>
            this.oldSelection;
    }
}

