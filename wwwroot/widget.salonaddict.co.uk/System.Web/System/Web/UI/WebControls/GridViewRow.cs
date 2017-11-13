namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewRow : TableRow, IDataItemContainer, INamingContainer
    {
        private object _dataItem;
        private int _dataItemIndex;
        private int _rowIndex;
        private DataControlRowState _rowState;
        private DataControlRowType _rowType;

        public GridViewRow(int rowIndex, int dataItemIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            this._rowIndex = rowIndex;
            this._dataItemIndex = dataItemIndex;
            this._rowType = rowType;
            this._rowState = rowState;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                GridViewCommandEventArgs args = new GridViewCommandEventArgs(this, source, (CommandEventArgs) e);
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

        public virtual int RowIndex =>
            this._rowIndex;

        public virtual DataControlRowState RowState
        {
            get => 
                this._rowState;
            set
            {
                this._rowState = value;
            }
        }

        public virtual DataControlRowType RowType
        {
            get => 
                this._rowType;
            set
            {
                this._rowType = value;
            }
        }

        object IDataItemContainer.DataItem =>
            this.DataItem;

        int IDataItemContainer.DataItemIndex =>
            this.DataItemIndex;

        int IDataItemContainer.DisplayIndex =>
            this.RowIndex;
    }
}

