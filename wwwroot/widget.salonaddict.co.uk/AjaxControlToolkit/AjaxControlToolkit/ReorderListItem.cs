namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false)]
    public class ReorderListItem : WebControl, IDataItemContainer, INamingContainer
    {
        private ReorderListItem _baseItem;
        private object _dataItem;
        private bool _isAddItem;
        private int _itemIndex;
        private ListItemType _itemType;
        private HtmlTextWriterTag _tag;
        internal const string ItemBaseName = "_rli";

        public ReorderListItem(int index) : this(index, false)
        {
        }

        internal ReorderListItem(ReorderListItem baseItem, HtmlTextWriterTag tag)
        {
            this._tag = HtmlTextWriterTag.Li;
            this._baseItem = baseItem;
            this._tag = tag;
        }

        public ReorderListItem(int index, bool isAddItem)
        {
            this._tag = HtmlTextWriterTag.Li;
            this._itemIndex = index;
            if (!isAddItem)
            {
                this.ID = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { "_rli", index });
            }
            else
            {
                this.ID = string.Format(CultureInfo.InvariantCulture, "{0}Insert", new object[] { "_rli" });
            }
            base.Style["vertical-align"] = "middle";
            this._isAddItem = isAddItem;
        }

        public ReorderListItem(object dataItem, int index, ListItemType itemType) : this(index)
        {
            this._dataItem = dataItem;
            this._itemType = itemType;
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            CommandEventArgs ce = args as CommandEventArgs;
            if (ce != null)
            {
                ReorderListCommandEventArgs args3 = new ReorderListCommandEventArgs(ce, source, this);
                base.RaiseBubbleEvent(this, args3);
                return true;
            }
            return true;
        }

        public object DataItem
        {
            get
            {
                if (this._baseItem != null)
                {
                    return this._baseItem.DataItem;
                }
                return this._dataItem;
            }
            set
            {
                this._dataItem = value;
            }
        }

        public int DataItemIndex =>
            this.ItemIndex;

        public int DisplayIndex =>
            this.ItemIndex;

        public bool IsAddItem
        {
            get
            {
                if (this._baseItem != null)
                {
                    return this._baseItem.IsAddItem;
                }
                return this._isAddItem;
            }
        }

        public int ItemIndex
        {
            get
            {
                if (this._baseItem != null)
                {
                    return this._baseItem.ItemIndex;
                }
                return this._itemIndex;
            }
            set
            {
                this._itemIndex = value;
            }
        }

        public ListItemType ItemType
        {
            get
            {
                if (this._baseItem != null)
                {
                    return this._baseItem.ItemType;
                }
                if (this._isAddItem)
                {
                    throw new InvalidOperationException("Item type isn't valid for Add items.");
                }
                return this._itemType;
            }
            set
            {
                this._itemType = value;
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            this._tag;
    }
}

