namespace System.Windows.Forms
{
    using System;

    public class RetrieveVirtualItemEventArgs : EventArgs
    {
        private ListViewItem item;
        private int itemIndex;

        public RetrieveVirtualItemEventArgs(int itemIndex)
        {
            this.itemIndex = itemIndex;
        }

        public ListViewItem Item
        {
            get => 
                this.item;
            set
            {
                this.item = value;
            }
        }

        public int ItemIndex =>
            this.itemIndex;
    }
}

