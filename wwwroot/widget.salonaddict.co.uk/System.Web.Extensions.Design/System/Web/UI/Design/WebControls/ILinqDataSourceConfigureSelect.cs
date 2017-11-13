namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Runtime.CompilerServices;

    internal interface ILinqDataSourceConfigureSelect
    {
        event SelectionChangedEventHandler SelectionChanged;

        void GroupByChangedHandler(bool isNone, bool isCustom, ILinqDataSourcePropertyItem groupByField);
        void LoadState();
        void SaveState();
        void SelectTable(ILinqDataSourcePropertyItem selectedTable);
    }
}

