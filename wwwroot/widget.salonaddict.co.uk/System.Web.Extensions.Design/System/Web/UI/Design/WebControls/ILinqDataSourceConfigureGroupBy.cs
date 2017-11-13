namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Runtime.CompilerServices;

    internal interface ILinqDataSourceConfigureGroupBy
    {
        event LinqDataSourceGroupByChangedEventHandler GroupByChanged;

        void LoadState();
        void SaveState();
        void SelectionChangedHandler(LinqDataSourceSelectionChangedEventArgs e);
        void SelectTable(ILinqDataSourcePropertyItem tableProperty);
    }
}

