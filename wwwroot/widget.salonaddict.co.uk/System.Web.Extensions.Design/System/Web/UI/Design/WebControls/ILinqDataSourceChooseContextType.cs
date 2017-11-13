namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Runtime.CompilerServices;

    internal interface ILinqDataSourceChooseContextType
    {
        event LinqDataSourceContextChangedEventHandler ContextChanged;

        void LoadState();
        bool OnNext();
        void SaveState();
        void SelectContextType(ILinqDataSourceContextTypeItem selectedContextType);
        void SelectShowDataContextsOnly(bool showOnlyDataContexts);
        void UpdateWizardState(ILinqDataSourceWizardForm parentWizard);
    }
}

