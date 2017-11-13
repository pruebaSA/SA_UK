namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataPagerCommandEventArgs : CommandEventArgs
    {
        private DataPagerFieldItem _item;
        private int _newMaximumRows;
        private int _newStartRowIndex;
        private DataPagerField _pagerField;
        private int _totalRowCount;

        public DataPagerCommandEventArgs(DataPagerField pagerField, int totalRowCount, CommandEventArgs originalArgs, DataPagerFieldItem item) : base(originalArgs)
        {
            this._newMaximumRows = -1;
            this._newStartRowIndex = -1;
            this._pagerField = pagerField;
            this._totalRowCount = totalRowCount;
            this._item = item;
        }

        public DataPagerFieldItem Item =>
            this._item;

        public int NewMaximumRows
        {
            get => 
                this._newMaximumRows;
            set
            {
                this._newMaximumRows = value;
            }
        }

        public int NewStartRowIndex
        {
            get => 
                this._newStartRowIndex;
            set
            {
                this._newStartRowIndex = value;
            }
        }

        public DataPagerField PagerField =>
            this._pagerField;

        public int TotalRowCount =>
            this._totalRowCount;
    }
}

