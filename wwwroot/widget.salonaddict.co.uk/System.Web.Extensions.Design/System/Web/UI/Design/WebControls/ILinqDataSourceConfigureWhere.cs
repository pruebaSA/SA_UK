namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Web.UI.WebControls;

    internal interface ILinqDataSourceConfigureWhere
    {
        void AddCurrentWhereExpression();
        void AttachParameterEditorChangedHandler(object parameterEditor, Delegate handler);
        LinqDataSourceWhereExpression GetCurrentWhereExpression();
        bool GetParameterEditorHasCompleteInformation(object parameterEditor);
        Parameter GetParameterEditorParameter(object parameterEditor);
        void InitializeParameterEditor(object parameterEditor);
        void InvalidateSelectedParameter();
        bool LoadState();
        void RemoveCurrentWhereExpression();
        void SaveState();
        void SelectOperator(LinqDataSourceOperators selected);
        void SelectParameterEditor(object selected);
        void SelectTable(ILinqDataSourcePropertyItem table);
        void SelectWhereExpression(LinqDataSourceWhereExpression selected);
        void SelectWhereField(ILinqDataSourcePropertyItem selected);
        void SetParameterEditorVisible(object parameterEditor, bool value);
    }
}

