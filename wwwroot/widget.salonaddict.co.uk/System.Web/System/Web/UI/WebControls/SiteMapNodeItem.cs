namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ToolboxItem(false), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SiteMapNodeItem : WebControl, IDataItemContainer, INamingContainer
    {
        private int _itemIndex;
        private SiteMapNodeItemType _itemType;
        private System.Web.SiteMapNode _siteMapNode;

        public SiteMapNodeItem(int itemIndex, SiteMapNodeItemType itemType)
        {
            this._itemIndex = itemIndex;
            this._itemType = itemType;
        }

        protected internal virtual void SetItemType(SiteMapNodeItemType itemType)
        {
            this._itemType = itemType;
        }

        public virtual int ItemIndex =>
            this._itemIndex;

        public virtual SiteMapNodeItemType ItemType =>
            this._itemType;

        public virtual System.Web.SiteMapNode SiteMapNode
        {
            get => 
                this._siteMapNode;
            set
            {
                this._siteMapNode = value;
            }
        }

        object IDataItemContainer.DataItem =>
            this.SiteMapNode;

        int IDataItemContainer.DataItemIndex =>
            this.ItemIndex;

        int IDataItemContainer.DisplayIndex =>
            this.ItemIndex;
    }
}

