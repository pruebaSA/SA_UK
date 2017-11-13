namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class PageEventArgs : EventArgs
    {
        private int _maximumRows;
        private int _startRowIndex;
        private int _totalRowCount;

        public PageEventArgs(int startRowIndex, int maximumRows, int totalRowCount)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
            this._totalRowCount = totalRowCount;
        }

        public int MaximumRows =>
            this._maximumRows;

        public int StartRowIndex =>
            this._startRowIndex;

        public int TotalRowCount =>
            this._totalRowCount;
    }
}

