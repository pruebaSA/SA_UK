namespace AjaxControlToolkit
{
    using System;

    public class ReorderListItemEventArgs : EventArgs
    {
        private ReorderListItem _item;

        public ReorderListItemEventArgs(ReorderListItem item)
        {
            this._item = item;
        }

        public ReorderListItem Item
        {
            get => 
                this._item;
            set
            {
                this._item = value;
            }
        }
    }
}

