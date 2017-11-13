namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class AccordionContentPanel : Panel, IDataItemContainer, INamingContainer
    {
        private bool _collapsed;
        private int _dataIndex;
        private object _dataItem;
        private AccordionItemType _type;

        internal AccordionContentPanel()
        {
        }

        internal AccordionContentPanel(object dataItem, int dataIndex, AccordionItemType type) : this()
        {
            this.SetDataItemProperties(dataItem, dataIndex, type);
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            CommandEventArgs args2 = args as CommandEventArgs;
            if (args2 != null)
            {
                AccordionCommandEventArgs args3 = new AccordionCommandEventArgs(this, args2.CommandName, args2.CommandArgument);
                base.RaiseBubbleEvent(this, args3);
                return true;
            }
            return false;
        }

        internal void SetDataItemProperties(object dataItem, int dataIndex, AccordionItemType type)
        {
            this._dataItem = dataItem;
            this._dataIndex = dataIndex;
            this._type = type;
        }

        public bool Collapsed
        {
            get => 
                this._collapsed;
            set
            {
                this._collapsed = value;
                base.Style[HtmlTextWriterStyle.Display] = this._collapsed ? "none" : "block";
            }
        }

        public object DataItem =>
            this._dataItem;

        public int DataItemIndex =>
            this._dataIndex;

        public int DisplayIndex =>
            this._dataIndex;

        public AccordionItemType ItemType =>
            this._type;
    }
}

