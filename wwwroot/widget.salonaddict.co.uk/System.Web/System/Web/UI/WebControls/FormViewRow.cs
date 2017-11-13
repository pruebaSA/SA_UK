namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class FormViewRow : TableRow
    {
        private int _itemIndex;
        private DataControlRowState _rowState;
        private DataControlRowType _rowType;

        public FormViewRow(int itemIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            this._itemIndex = itemIndex;
            this._rowType = rowType;
            this._rowState = rowState;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                FormViewCommandEventArgs args = new FormViewCommandEventArgs(source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }

        public virtual int ItemIndex =>
            this._itemIndex;

        public virtual DataControlRowState RowState =>
            this._rowState;

        public virtual DataControlRowType RowType =>
            this._rowType;
    }
}

