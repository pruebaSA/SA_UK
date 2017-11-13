namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourceConfigureOrderBy
    {
        bool LoadState();
        void SaveState();
        void SelectTable(ILinqDataSourcePropertyItem tableProperty);
    }
}

