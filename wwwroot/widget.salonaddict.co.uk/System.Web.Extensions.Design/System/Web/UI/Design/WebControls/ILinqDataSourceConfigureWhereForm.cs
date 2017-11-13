namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    internal interface ILinqDataSourceConfigureWhereForm
    {
        void AddNewWhereExpressionItem(LinqDataSourceWhereExpression whereExpression, LinqDataSourceWhereStatement whereStatement, IServiceProvider serviceProvider, Control dataSource);
        void Register(ILinqDataSourceConfigureWhere configureWhere, Control dataSource);
        void RemoveWhereExpressionItem(LinqDataSourceWhereExpression whereExpression);
        void SetCanAddSelectExpression(bool enabled);
        void SetCanRemoveSelectExpression(bool enabled);
        void SetExpressionPreviewEnabled(bool enabled);
        void SetOperators(List<LinqDataSourceOperators> operators);
        void SetOperatorsEnabled(bool enabled);
        void SetParameterEditorToShow(object editor);
        void SetParametersEnabled(bool enabled);
        void SetParameterValuePreview(string preview);
        void SetSelectedWhereField(ILinqDataSourcePropertyItem selected);
        void SetWhereExpressionPreview(string preview);
        void SetWhereFields(List<ILinqDataSourcePropertyItem> fields);
        void SetWhereStatement(LinqDataSourceWhereStatement whereStatement, IServiceProvider serviceProvider, Control dataSource);
    }
}

