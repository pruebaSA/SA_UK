namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    internal class LinqDataSourceChooseContextType : ILinqDataSourceChooseContextType
    {
        private ILinqDataSourceDesignerHelper _helper;
        private ILinqDataSourceChooseContextTypePanel _panel;
        private static ILinqDataSourceContextTypeItem _placeholderContextType;
        private IServiceProvider _serviceProvider;
        private ChooseContextTypeState _state;

        public event LinqDataSourceContextChangedEventHandler ContextChanged;

        private LinqDataSourceChooseContextType(ILinqDataSourceChooseContextTypePanel panel, ILinqDataSourceDesignerHelper helper, IServiceProvider serviceProvider)
        {
            this._panel = panel;
            this._helper = helper;
            this._serviceProvider = serviceProvider;
            this._panel.Register(this);
            _placeholderContextType = new LinqDataSourceContextTypeItem(null);
        }

        public LinqDataSourceChooseContextType(ILinqDataSourceChooseContextTypePanel panel, ILinqDataSourceDesignerHelper helper, IServiceProvider serviceProvider, ChooseContextTypeState chooseContextTypeState) : this(panel, helper, serviceProvider)
        {
            this._state = chooseContextTypeState;
        }

        public LinqDataSourceChooseContextType(ILinqDataSourceChooseContextTypePanel panel, ILinqDataSourceWizardForm wizardForm, ILinqDataSourceDesignerHelper helper, IServiceProvider serviceProvider, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(panel, helper, serviceProvider)
        {
            this._state = new ChooseContextTypeState();
            this._state.WizardForm = wizardForm;
            this._state.LinqDataSourceState = linqDataSourceState;
            this._state.SelectedContextType = null;
        }

        internal void LoadContextTypes(string contextName)
        {
            this.ContextTypes = this._helper.GetContextTypes(typeof(DataContext), this._serviceProvider);
            this.ShowOnlyDataContexts = true;
            bool flag = (this.ContextTypes != null) && (this.ContextTypes.Count > 0);
            if (flag)
            {
                if (!string.IsNullOrEmpty(contextName))
                {
                    flag = this.MatchContextType(contextName);
                }
                else
                {
                    this.SelectedContextType = this.ContextTypes[0];
                }
            }
            if (!flag)
            {
                this.ContextTypes = this._helper.GetContextTypes(typeof(object), this._serviceProvider);
                this.ShowOnlyDataContexts = false;
                flag = (this.ContextTypes != null) && (this.ContextTypes.Count > 0);
                if (flag)
                {
                    if (!string.IsNullOrEmpty(contextName))
                    {
                        flag = this.MatchContextType(contextName);
                    }
                    else
                    {
                        this.SelectedContextType = this.ContextTypes[0];
                    }
                }
            }
            if (this.ContextTypes == null)
            {
                this.ContextTypes = new List<ILinqDataSourceContextTypeItem>();
            }
            if (!flag && !string.IsNullOrEmpty(contextName))
            {
                this.HasUnrecognizedContext = true;
                this.SelectedContextType = PlaceholderContextType;
                PlaceholderContextType.DisplayName = contextName;
                this.ContextTypes.Add(PlaceholderContextType);
                this.ContextTypes.Sort();
            }
            this._panel.SetContextTypes(this.ContextTypes);
            this._panel.SetShowOnlyDataContexts(this.ShowOnlyDataContexts);
            this._panel.SetSelectedContextType(this.SelectedContextType);
            this.OnContextChanged(this.SelectedContextType);
        }

        internal void LoadContextTypes(string contextName, bool showOnlyDataContexts)
        {
            if (showOnlyDataContexts)
            {
                this.ContextTypes = this._helper.GetContextTypes(typeof(DataContext), this._serviceProvider);
            }
            else
            {
                this.ContextTypes = this._helper.GetContextTypes(typeof(object), this._serviceProvider);
            }
            if (this.ContextTypes == null)
            {
                this.ContextTypes = new List<ILinqDataSourceContextTypeItem>();
            }
            if (this.HasUnrecognizedContext)
            {
                this.ContextTypes.Add(PlaceholderContextType);
                this.ContextTypes.Sort();
            }
            if (!string.IsNullOrEmpty(contextName))
            {
                this.MatchContextType(contextName);
            }
            else if (this.ContextTypes.Count > 0)
            {
                this.SelectedContextType = this.ContextTypes[0];
            }
            else
            {
                this.SelectedContextType = null;
            }
            this._panel.SetContextTypes(this.ContextTypes);
            this._panel.SetSelectedContextType(this.SelectedContextType);
            this.OnContextChanged(this.SelectedContextType);
        }

        public void LoadState()
        {
            string contextTypeName = this.LinqDataSourceState.ContextTypeName;
            if (!string.IsNullOrEmpty(contextTypeName))
            {
                this.LoadContextTypes(contextTypeName);
            }
            else
            {
                this.LoadContextTypes(null);
            }
        }

        internal bool MatchContextType(string contextName)
        {
            foreach (ILinqDataSourceContextTypeItem item in this.ContextTypes)
            {
                if ((item.Type != null) && string.Equals(item.Type.FullName, contextName, StringComparison.Ordinal))
                {
                    this.SelectedContextType = item;
                    return true;
                }
            }
            this.SelectedContextType = PlaceholderContextType;
            return false;
        }

        private void OnContextChanged(ILinqDataSourceContextTypeItem selectedContextType)
        {
            if (this._contextChanged != null)
            {
                this._contextChanged(this, selectedContextType);
            }
        }

        public bool OnNext() => 
            ((this.SelectedContextType != null) && (this.SelectedContextType != PlaceholderContextType));

        public void SaveState()
        {
            if (this.SelectedContextType != PlaceholderContextType)
            {
                this.LinqDataSourceState.ContextTypeName = this.SelectedContextType.Type.FullName;
            }
            else
            {
                this.LinqDataSourceState.ContextTypeName = string.Empty;
            }
        }

        public void SelectContextType(ILinqDataSourceContextTypeItem selectedContextType)
        {
            this.SelectedContextType = selectedContextType;
            this.SetNextEnabled();
            this.OnContextChanged(selectedContextType);
        }

        public void SelectShowDataContextsOnly(bool showOnlyDataContexts)
        {
            this.ShowOnlyDataContexts = showOnlyDataContexts;
            if (this.SelectedContextType == PlaceholderContextType)
            {
                this.LoadContextTypes(PlaceholderContextType.DisplayName, showOnlyDataContexts);
            }
            else if (this.SelectedContextType != null)
            {
                this.LoadContextTypes(this.SelectedContextType.Type.FullName, showOnlyDataContexts);
                if ((this.SelectedContextType == PlaceholderContextType) && (this.ContextTypes.Count > 0))
                {
                    this.SelectContextType(this.ContextTypes[0]);
                    this._panel.SetSelectedContextType(this.ContextTypes[0]);
                }
            }
            else
            {
                this.LoadContextTypes(null, showOnlyDataContexts);
            }
        }

        internal void SetNextEnabled()
        {
            if (((this.ContextTypes == null) || (this.ContextTypes.Count == 0)) && !this.HasUnrecognizedContext)
            {
                this.WizardForm.SetCanNext(false);
            }
            else
            {
                this.WizardForm.SetCanNext(true);
            }
        }

        public void UpdateWizardState(ILinqDataSourceWizardForm parentWizard)
        {
            this.SetNextEnabled();
            parentWizard.SetCanFinish(false);
        }

        private List<ILinqDataSourceContextTypeItem> ContextTypes
        {
            get => 
                this._state.ContextTypes;
            set
            {
                this._state.ContextTypes = value;
            }
        }

        private bool HasUnrecognizedContext
        {
            get => 
                this._state.HasUnrecognizedContext;
            set
            {
                this._state.HasUnrecognizedContext = value;
            }
        }

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState =>
            this._state.LinqDataSourceState;

        internal static ILinqDataSourceContextTypeItem PlaceholderContextType =>
            _placeholderContextType;

        private ILinqDataSourceContextTypeItem SelectedContextType
        {
            get => 
                this._state.SelectedContextType;
            set
            {
                this._state.SelectedContextType = value;
            }
        }

        private bool ShowOnlyDataContexts
        {
            get => 
                this._state.ShowOnlyDataContexts;
            set
            {
                this._state.ShowOnlyDataContexts = value;
            }
        }

        private ILinqDataSourceWizardForm WizardForm =>
            this._state.WizardForm;

        internal class ChooseContextTypeState
        {
            public List<ILinqDataSourceContextTypeItem> ContextTypes;
            public bool HasUnrecognizedContext;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public ILinqDataSourceContextTypeItem SelectedContextType;
            public bool ShowOnlyDataContexts;
            public ILinqDataSourceWizardForm WizardForm;
        }
    }
}

