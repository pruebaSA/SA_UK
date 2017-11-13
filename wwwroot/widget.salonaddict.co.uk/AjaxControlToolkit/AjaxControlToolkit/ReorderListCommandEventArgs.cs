namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI.WebControls;

    public class ReorderListCommandEventArgs : CommandEventArgs
    {
        private ReorderListItem _item;
        private object _source;

        internal ReorderListCommandEventArgs(CommandEventArgs ce, object source, ReorderListItem item) : base(ce)
        {
            this._item = item;
            this._source = source;
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

        public object Source
        {
            get => 
                this._source;
            set
            {
                this._source = value;
            }
        }
    }
}

