namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DetailsViewRow : TableRow
    {
        private int _rowIndex;
        private DataControlRowState _rowState;
        private DataControlRowType _rowType;

        public DetailsViewRow(int rowIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            this._rowIndex = rowIndex;
            this._rowType = rowType;
            this._rowState = rowState;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                DetailsViewCommandEventArgs args = new DetailsViewCommandEventArgs(source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }

        public virtual int RowIndex =>
            this._rowIndex;

        public virtual DataControlRowState RowState =>
            this._rowState;

        public virtual DataControlRowType RowType =>
            this._rowType;
    }
}

