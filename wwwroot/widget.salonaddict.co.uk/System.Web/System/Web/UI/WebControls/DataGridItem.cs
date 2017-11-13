namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridItem : TableRow, IDataItemContainer, INamingContainer
    {
        private object dataItem;
        private int dataSetIndex;
        private int itemIndex;
        private ListItemType itemType;

        public DataGridItem(int itemIndex, int dataSetIndex, ListItemType itemType)
        {
            this.itemIndex = itemIndex;
            this.dataSetIndex = dataSetIndex;
            this.itemType = itemType;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                DataGridCommandEventArgs args = new DataGridCommandEventArgs(this, source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }

        protected internal virtual void SetItemType(ListItemType itemType)
        {
            this.itemType = itemType;
        }

        public virtual object DataItem
        {
            get => 
                this.dataItem;
            set
            {
                this.dataItem = value;
            }
        }

        public virtual int DataSetIndex =>
            this.dataSetIndex;

        public virtual int ItemIndex =>
            this.itemIndex;

        public virtual ListItemType ItemType =>
            this.itemType;

        object IDataItemContainer.DataItem =>
            this.DataItem;

        int IDataItemContainer.DataItemIndex =>
            this.DataSetIndex;

        int IDataItemContainer.DisplayIndex =>
            this.ItemIndex;
    }
}

