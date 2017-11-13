namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal interface ILinqDataSourceConfigureSelectPanel
    {
        void AddField(ILinqDataSourcePropertyItem field);
        void AddProjection(ILinqDataSourcePropertyItem field, List<LinqDataSourceAggregateFunctions> aggregates, LinqDataSourceAggregateFunctions aggregateFunction, string alias, bool isDefaultProjection);
        void ClearGridProjections();
        void EnableAlias(int rowIndex);
        void MoveProjection(int oldIndex, int newIndex);
        void Register(LinqDataSourceConfigureSelect configureSelect);
        void RemoveProjection(int index);
        void SetAggregateFunctions(int rowIndex, List<LinqDataSourceAggregateFunctions> aggregates);
        void SetCanMoveDown(bool enabled);
        void SetCanMoveUp(bool enabled);
        void SetCanRemove(bool enabled);
        void SetCheckBoxFields(List<ILinqDataSourcePropertyItem> fields);
        void SetCustomGroupBy(string newGroupBy);
        void SetCustomSelect(string newSelect);
        void SetGridAggregateFunctions(List<LinqDataSourceAggregateFunctions> functions);
        void SetGridFields(List<ILinqDataSourcePropertyItem> fields);
        void SetProjectionAggregateFunction(int rowIndex, LinqDataSourceAggregateFunctions function);
        void SetProjectionAlias(int rowIndex, string alias);
        void SetProjectionField(int rowIndex, string field);
        void SetSelectMode(LinqDataSourceGroupByMode SelectMode);
        void UncheckFieldCheckboxes();
        void UncheckStarCheckbox();
    }
}

