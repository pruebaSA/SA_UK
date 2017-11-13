namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ToolboxItem(false), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RepeaterItem : Control, IDataItemContainer, INamingContainer
    {
        private object dataItem;
        private int itemIndex;
        private ListItemType itemType;

        public RepeaterItem(int itemIndex, ListItemType itemType)
        {
            this.itemIndex = itemIndex;
            this.itemType = itemType;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                RepeaterCommandEventArgs args = new RepeaterCommandEventArgs(this, source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
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

        public virtual int ItemIndex =>
            this.itemIndex;

        public virtual ListItemType ItemType =>
            this.itemType;

        int IDataItemContainer.DataItemIndex =>
            this.ItemIndex;

        int IDataItemContainer.DisplayIndex =>
            this.ItemIndex;
    }
}

