namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourceConfigureAdvancedForm
    {
        void Register(LinqDataSourceConfigureAdvanced configureAdvanced);
        void SetEnableDelete(bool isChecked);
        void SetEnableInsert(bool isChecked);
        void SetEnableUpdate(bool isChecked);
    }
}

