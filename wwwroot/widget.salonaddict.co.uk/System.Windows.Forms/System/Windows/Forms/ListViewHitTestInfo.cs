namespace System.Windows.Forms
{
    using System;

    public class ListViewHitTestInfo
    {
        private ListViewItem item;
        private ListViewHitTestLocations loc;
        private ListViewItem.ListViewSubItem subItem;

        public ListViewHitTestInfo(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitLocation)
        {
            this.item = hitItem;
            this.subItem = hitSubItem;
            this.loc = hitLocation;
        }

        public ListViewItem Item =>
            this.item;

        public ListViewHitTestLocations Location =>
            this.loc;

        public ListViewItem.ListViewSubItem SubItem =>
            this.subItem;
    }
}

