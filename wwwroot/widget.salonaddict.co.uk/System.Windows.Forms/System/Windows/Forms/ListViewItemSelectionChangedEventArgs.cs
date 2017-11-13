namespace System.Windows.Forms
{
    using System;

    public class ListViewItemSelectionChangedEventArgs : EventArgs
    {
        private bool isSelected;
        private ListViewItem item;
        private int itemIndex;

        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
        {
            this.item = item;
            this.itemIndex = itemIndex;
            this.isSelected = isSelected;
        }

        public bool IsSelected =>
            this.isSelected;

        public ListViewItem Item =>
            this.item;

        public int ItemIndex =>
            this.itemIndex;
    }
}

