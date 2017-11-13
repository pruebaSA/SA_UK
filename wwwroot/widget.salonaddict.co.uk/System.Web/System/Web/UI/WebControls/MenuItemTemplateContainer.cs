namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MenuItemTemplateContainer : Control, IDataItemContainer, INamingContainer
    {
        private object _dataItem;
        private int _itemIndex;

        public MenuItemTemplateContainer(int itemIndex, MenuItem dataItem)
        {
            this._itemIndex = itemIndex;
            this._dataItem = dataItem;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            CommandEventArgs args = e as CommandEventArgs;
            if (args == null)
            {
                return false;
            }
            if (args is MenuEventArgs)
            {
                base.RaiseBubbleEvent(this, args);
            }
            else
            {
                MenuEventArgs args2 = new MenuEventArgs((MenuItem) this._dataItem, source, args);
                base.RaiseBubbleEvent(this, args2);
            }
            return true;
        }

        public object DataItem
        {
            get => 
                this._dataItem;
            set
            {
                this._dataItem = value;
            }
        }

        public int ItemIndex =>
            this._itemIndex;

        object IDataItemContainer.DataItem =>
            this._dataItem;

        int IDataItemContainer.DataItemIndex =>
            this.ItemIndex;

        int IDataItemContainer.DisplayIndex =>
            this.ItemIndex;
    }
}

