namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal interface ILinqDataSourceChooseContextTypePanel
    {
        void Register(ILinqDataSourceChooseContextType chooseContextType);
        void SetContextTypes(List<ILinqDataSourceContextTypeItem> contextTypes);
        void SetSelectedContextType(ILinqDataSourceContextTypeItem selected);
        void SetShowOnlyDataContexts(bool showOnlyDataContexts);
    }
}

