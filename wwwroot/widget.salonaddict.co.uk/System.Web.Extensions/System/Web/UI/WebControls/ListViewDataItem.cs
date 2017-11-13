namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewDataItem : ListViewItem, IDataItemContainer, INamingContainer
    {
        private object _dataItem;
        private int _dataItemIndex;
        private int _displayIndex;

        public ListViewDataItem(int dataItemIndex, int displayIndex) : base(ListViewItemType.DataItem)
        {
            this._dataItemIndex = dataItemIndex;
            this._displayIndex = displayIndex;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                ListViewCommandEventArgs args = new ListViewCommandEventArgs(this, source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }

        public virtual object DataItem
        {
            get => 
                this._dataItem;
            set
            {
                this._dataItem = value;
            }
        }

        public virtual int DataItemIndex =>
            this._dataItemIndex;

        public virtual int DisplayIndex =>
            this._displayIndex;
    }
}

