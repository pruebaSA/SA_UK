namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourceConfiguration
    {
        void ContextChangedHandler(object sender, ILinqDataSourceContextTypeItem newContext);
        void LoadState();
        void SaveState();
        void SelectTable(ILinqDataSourcePropertyItem selectedTable);
        void ShowAdvanced();
        void ShowOrderBy();
        void ShowWhere();
        void UpdateWizardState(ILinqDataSourceWizardForm wizard);
    }
}

