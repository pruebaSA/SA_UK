namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI.WebControls;

    public class ComboBoxItemInsertEventArgs : CancelEventArgs
    {
        private ComboBoxItemInsertLocation _insertLocation;
        private ListItem _listItem;

        internal ComboBoxItemInsertEventArgs(string text, ComboBoxItemInsertLocation location)
        {
            this._listItem = new ListItem(text);
            this._insertLocation = location;
        }

        public ComboBoxItemInsertLocation InsertLocation
        {
            get => 
                this._insertLocation;
            set
            {
                this._insertLocation = value;
            }
        }

        public ListItem Item
        {
            get => 
                this._listItem;
            set
            {
                this._listItem = value;
            }
        }
    }
}

