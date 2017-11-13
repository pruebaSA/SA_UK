namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal interface ILinqDataSourceConfigureGroupByPanel
    {
        void Register(LinqDataSourceConfigureGroupBy configureGroupBy);
        void SetGroupBy(ILinqDataSourcePropertyItem field);
        void SetGroupByFields(List<ILinqDataSourcePropertyItem> fields);
        void SetSelectedGroupByField(ILinqDataSourcePropertyItem selected);
        void ShowOrderGroupsBy(bool show);

        string OrderGroupsBy { get; set; }
    }
}

