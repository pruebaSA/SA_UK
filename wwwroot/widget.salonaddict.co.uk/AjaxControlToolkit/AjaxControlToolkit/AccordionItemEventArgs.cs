namespace AjaxControlToolkit
{
    using System;

    public class AccordionItemEventArgs : EventArgs
    {
        private AccordionContentPanel _item;
        private AccordionItemType _type;

        public AccordionItemEventArgs(AccordionContentPanel item, AccordionItemType type)
        {
            this._item = item;
            this._type = type;
        }

        public AccordionContentPanel AccordionItem =>
            this._item;

        public object Item =>
            this._item.DataItem;

        public int ItemIndex =>
            this._item.DataItemIndex;

        public AccordionItemType ItemType =>
            this._type;
    }
}

