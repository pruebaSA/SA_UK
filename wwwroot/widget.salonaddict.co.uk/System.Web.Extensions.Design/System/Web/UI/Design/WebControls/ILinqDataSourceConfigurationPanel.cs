namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal interface ILinqDataSourceConfigurationPanel
    {
        void Register(ILinqDataSourceConfiguration _configuration);
        void SetAdvancedEnabled(bool enabled);
        void SetConfigureGroupByForm(ILinqDataSourceConfigureGroupByPanel form);
        void SetConfigureSelectForm(ILinqDataSourceConfigureSelectPanel form);
        void SetPanelEnabled(bool enabled);
        void SetSelectedTable(ILinqDataSourcePropertyItem selected);
        void SetTableComboEnabled(bool enabled);
        void SetTables(List<ILinqDataSourcePropertyItem> tableProperties);
    }
}

