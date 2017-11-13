namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class ListViewItemMouseHoverEventArgs : EventArgs
    {
        private readonly ListViewItem item;

        public ListViewItemMouseHoverEventArgs(ListViewItem item)
        {
            this.item = item;
        }

        public ListViewItem Item =>
            this.item;
    }
}

