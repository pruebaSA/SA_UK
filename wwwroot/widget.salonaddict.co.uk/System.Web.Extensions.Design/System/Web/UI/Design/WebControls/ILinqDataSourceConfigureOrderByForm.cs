namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal interface ILinqDataSourceConfigureOrderByForm
    {
        void Register(LinqDataSourceConfigureOrderBy _configureOrderBy);
        void SetOrderByDirectionEnabled(int clauseNumber, bool enabled);
        void SetOrderByFieldEnabled(int clauseNumber, bool enabled);
        void SetOrderByFields(List<ILinqDataSourcePropertyItem> fields);
        void SetPreview(string preview);
        void SetSelectedOrderByField(int clauseNumber, ILinqDataSourcePropertyItem field, bool isAsc);
    }
}

