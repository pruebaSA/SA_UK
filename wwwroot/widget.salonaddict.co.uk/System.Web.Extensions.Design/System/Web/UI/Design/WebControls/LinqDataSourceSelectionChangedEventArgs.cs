namespace System.Web.UI.Design.WebControls
{
    using System;

    internal class LinqDataSourceSelectionChangedEventArgs : EventArgs
    {
        private bool _hasSelect;
        private LinqDataSourceGroupByMode _mode;

        public LinqDataSourceSelectionChangedEventArgs(LinqDataSourceGroupByMode mode, bool hasSelect)
        {
            this._mode = mode;
            this._hasSelect = hasSelect;
        }

        public LinqDataSourceGroupByMode GroupByMode =>
            this._mode;

        public bool HasSelect =>
            this._hasSelect;
    }
}

