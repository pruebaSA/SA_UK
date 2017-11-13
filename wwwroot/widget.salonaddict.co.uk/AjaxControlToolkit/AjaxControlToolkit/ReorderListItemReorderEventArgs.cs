namespace AjaxControlToolkit
{
    using System;

    public class ReorderListItemReorderEventArgs : ReorderListItemEventArgs
    {
        private int _newIndex;
        private int _oldIndex;

        internal ReorderListItemReorderEventArgs(ReorderListItem item, int oldIndex, int newIndex) : base(item)
        {
            this._oldIndex = oldIndex;
            this._newIndex = newIndex;
        }

        public int NewIndex
        {
            get => 
                this._newIndex;
            set
            {
                this._newIndex = value;
            }
        }

        public int OldIndex
        {
            get => 
                this._oldIndex;
            set
            {
                this._oldIndex = value;
            }
        }
    }
}

