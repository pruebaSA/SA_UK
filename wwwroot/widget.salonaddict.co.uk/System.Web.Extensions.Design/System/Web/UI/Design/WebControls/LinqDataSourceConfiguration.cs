namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal class LinqDataSourceConfiguration : ILinqDataSourceConfiguration
    {
        private ILinqDataSourceDesignerHelper _designerHelper;
        private LinqDataSourceConfigurationHelper _helper;
        private System.Web.UI.Control _linqDataSource;
        private ILinqDataSourceConfigurationPanel _panel;
        private ConfigureState _state;

        private LinqDataSourceConfiguration(ILinqDataSourceConfigurationPanel panel, ILinqDataSourceDesignerHelper designerHelper)
        {
            this._panel = panel;
            this._panel.Register(this);
            this._designerHelper = designerHelper;
        }

        public LinqDataSourceConfiguration(ILinqDataSourceConfigurationPanel panel, ILinqDataSourceDesignerHelper designerHelper, LinqDataSourceConfigurationHelper helper, ConfigureState configureState) : this(panel, designerHelper)
        {
            this._state = configureState;
            this._helper = helper;
            this.LoadState();
        }

        public LinqDataSourceConfiguration(ILinqDataSourceConfigurationPanel panel, ILinqDataSourceWizardForm wizard, ILinqDataSourceDesignerHelper designerHelper, System.Web.UI.Control linqDataSource, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState, IServiceProvider serviceProvider) : this(panel, wizard, designerHelper, linqDataSource, linqDataSourceState, serviceProvider, false)
        {
        }

        internal LinqDataSourceConfiguration(ILinqDataSourceConfigurationPanel panel, ILinqDataSourceWizardForm wizard, ILinqDataSourceDesignerHelper designerHelper, System.Web.UI.Control linqDataSource, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState, IServiceProvider serviceProvider, bool isDebug) : this(panel, designerHelper)
        {
            this._state = new ConfigureState();
            this._linqDataSource = linqDataSource;
            this._state.ServiceProvider = serviceProvider;
            this._state.LinqDataSourceState = linqDataSourceState;
            this._helper = new LinqDataSourceConfigurationHelper(designerHelper, linqDataSource, linqDataSourceState, serviceProvider);
            ILinqDataSourceConfigureGroupByPanel form = new LinqDataSourceConfigureGroupByPanel();
            this.ConfigureGroupBy = new LinqDataSourceConfigureGroupBy(form, this._designerHelper, this.LinqDataSourceState);
            this._panel.SetConfigureGroupByForm(form);
            ILinqDataSourceConfigureSelectPanel panel3 = new LinqDataSourceConfigureSelectPanel(isDebug);
            this.ConfigureSelect = new LinqDataSourceConfigureSelect(panel3, wizard, this._designerHelper, this.LinqDataSourceState);
            this._panel.SetConfigureSelectForm(panel3);
            ILinqDataSourceConfigureSelect configureSelect = this.ConfigureSelect;
            this.ConfigureGroupBy.GroupByChanged += new LinqDataSourceGroupByChangedEventHandler(configureSelect.GroupByChangedHandler);
            ILinqDataSourceConfigureGroupBy configureGroupBy = this.ConfigureGroupBy;
            this.ConfigureSelect.SelectionChanged += new SelectionChangedEventHandler(configureGroupBy.SelectionChangedHandler);
            this.ConfigureSelect.SelectionChanged += new SelectionChangedEventHandler(this.SelectionChangedHandler);
            this.LoadState();
        }

        public void ContextChangedHandler(object sender, ILinqDataSourceContextTypeItem newContext)
        {
            ILinqDataSourcePropertyItem selectedTable = null;
            this._panel.SetTableComboEnabled(newContext != null);
            if (newContext != null)
            {
                this.Tables = this._designerHelper.GetTables(newContext, true);
                this._panel.SetTables(this.Tables);
                if (this.Tables.Count > 0)
                {
                    selectedTable = this.Tables[0];
                }
                if (newContext.Type != null)
                {
                    this.ContextTypeName = newContext.Type.FullName;
                    this.IsDataContext = (newContext.Type == typeof(DataContext)) || newContext.Type.IsSubclassOf(typeof(DataContext));
                }
                else
                {
                    this.ContextTypeName = null;
                    this.IsDataContext = false;
                }
                this.IsAdvancedEnabled = this.IsDataContext;
                this._panel.SetAdvancedEnabled(this.IsAdvancedEnabled);
            }
            this.SelectTable(selectedTable);
            this._panel.SetSelectedTable(selectedTable);
        }

        public void LoadState()
        {
            if (this.Tables != null)
            {
                foreach (ILinqDataSourcePropertyItem item in this.Tables)
                {
                    if (string.Equals(item.Name, this.LinqDataSourceState.TableName, StringComparison.Ordinal))
                    {
                        this.SelectTable(item);
                        this._panel.SetSelectedTable(item);
                        break;
                    }
                }
            }
        }

        public void SaveState()
        {
            this.LinqDataSourceState.TableName = this.SelectedTable.Name;
            this.ConfigureGroupBy.SaveState();
            this.ConfigureSelect.SaveState();
            if (!this.IsAdvancedEnabled)
            {
                this._state.LinqDataSourceState.EnableDelete = false;
                this._state.LinqDataSourceState.EnableInsert = false;
                this._state.LinqDataSourceState.EnableUpdate = false;
            }
        }

        public void SelectionChangedHandler(LinqDataSourceSelectionChangedEventArgs e)
        {
            if (this.IsDataContext && this.IsTableTypeTable)
            {
                this.IsAdvancedEnabled = (e.GroupByMode == LinqDataSourceGroupByMode.GroupByNone) && !e.HasSelect;
                this._panel.SetAdvancedEnabled(this.IsAdvancedEnabled);
            }
        }

        public void SelectTable(ILinqDataSourcePropertyItem selectedTable)
        {
            if (selectedTable != this.SelectedTable)
            {
                this.SelectedTable = selectedTable;
                this.ConfigureGroupBy.SelectTable(selectedTable);
                this.ConfigureSelect.SelectTable(selectedTable);
                if ((selectedTable != null) && string.Equals(this.SelectedTable.Name, this.LinqDataSourceState.TableName, StringComparison.Ordinal))
                {
                    this.ConfigureGroupBy.LoadState();
                    this.ConfigureSelect.LoadState();
                }
                this._panel.SetPanelEnabled(selectedTable != null);
                if (selectedTable != null)
                {
                    this.IsTableTypeTable = this._designerHelper.IsTableTypeTable(this._designerHelper, this.ContextTypeName, selectedTable.Name, this.ServiceProvider);
                    if (this.IsDataContext)
                    {
                        this.IsAdvancedEnabled = this.IsTableTypeTable;
                        this._panel.SetAdvancedEnabled(this.IsAdvancedEnabled);
                    }
                }
            }
        }

        public void ShowAdvanced()
        {
            ILinqDataSourceConfigureAdvancedForm advancedForm = this._helper.MakeAdvancedForm();
            this._helper.MakeAdvanced(advancedForm).LoadState();
            this._helper.ShowDialog(advancedForm);
        }

        public void ShowOrderBy()
        {
            ILinqDataSourceConfigureOrderByForm orderByForm = this._helper.MakeOrderByForm();
            ILinqDataSourceConfigureOrderBy by = this._helper.MakeOrderBy(orderByForm);
            by.SelectTable(this.SelectedTable);
            if (!this.IsOrderByCustomSet)
            {
                this.IsOrderByCustomSet = true;
                this.IsOrderByCustom = !by.LoadState();
            }
            if (!this.IsOrderByCustom)
            {
                by.LoadState();
                this._helper.ShowDialog(orderByForm);
            }
            else
            {
                ILinqDataSourceStatementEditorForm form = this._helper.MakeStatementEditorForm(this.LinqDataSourceState.AutoGenerateOrderByClause, this.LinqDataSourceState.OrderBy, this.LinqDataSourceState.OrderByParameters, "OrderBy");
                if (this._helper.ShowDialog(form) == DialogResult.OK)
                {
                    this.LinqDataSourceState.AutoGenerateOrderByClause = form.AutoGen;
                    this.LinqDataSourceState.OrderBy = form.Statement;
                    ParameterCollection parameters = form.Parameters;
                    this.LinqDataSourceState.OrderByParameters.Clear();
                    foreach (Parameter parameter in parameters)
                    {
                        this.LinqDataSourceState.OrderByParameters.Add(parameter);
                    }
                }
            }
        }

        public void ShowWhere()
        {
            ILinqDataSourceConfigureWhereForm whereForm = this._helper.MakeWhereForm();
            ILinqDataSourceConfigureWhere where = this._helper.MakeWhere(whereForm);
            where.SelectTable(this.SelectedTable);
            if (!this.IsWhereCustomSet)
            {
                this.IsWhereCustomSet = true;
                this.IsWhereCustom = !where.LoadState();
            }
            if (!this.IsWhereCustom)
            {
                where.LoadState();
                this._helper.ShowDialog(whereForm);
            }
            else
            {
                ILinqDataSourceStatementEditorForm form = this._helper.MakeStatementEditorForm(this.LinqDataSourceState.AutoGenerateWhereClause, this.LinqDataSourceState.Where, this._state.LinqDataSourceState.WhereParameters, "Where");
                if (this._helper.ShowDialog(form) == DialogResult.OK)
                {
                    this.LinqDataSourceState.AutoGenerateWhereClause = form.AutoGen;
                    this.LinqDataSourceState.Where = form.Statement;
                    ParameterCollection parameters = form.Parameters;
                    this.LinqDataSourceState.WhereParameters.Clear();
                    foreach (Parameter parameter in parameters)
                    {
                        this.LinqDataSourceState.WhereParameters.Add(parameter);
                    }
                }
            }
        }

        public void UpdateWizardState(ILinqDataSourceWizardForm wizard)
        {
            wizard.SetCanFinish(this.SelectedTable != null);
        }

        private ILinqDataSourceConfigureGroupBy ConfigureGroupBy
        {
            get => 
                this._state.ConfigureGroupBy;
            set
            {
                this._state.ConfigureGroupBy = value;
            }
        }

        private ILinqDataSourceConfigureSelect ConfigureSelect
        {
            get => 
                this._state.ConfigureSelect;
            set
            {
                this._state.ConfigureSelect = value;
            }
        }

        private string ContextTypeName
        {
            get => 
                this._state.ContextTypeName;
            set
            {
                this._state.ContextTypeName = value;
            }
        }

        private bool IsAdvancedEnabled
        {
            get => 
                this._state.IsAdvancedEnabled;
            set
            {
                this._state.IsAdvancedEnabled = value;
            }
        }

        private bool IsDataContext
        {
            get => 
                this._state.IsDataContext;
            set
            {
                this._state.IsDataContext = value;
            }
        }

        private bool IsOrderByCustom
        {
            get => 
                this._state.IsOrderByCustom;
            set
            {
                this._state.IsOrderByCustom = value;
            }
        }

        private bool IsOrderByCustomSet
        {
            get => 
                this._state.IsOrderByCustomSet;
            set
            {
                this._state.IsOrderByCustomSet = value;
            }
        }

        private bool IsTableTypeTable
        {
            get => 
                this._state.IsTableTypeTable;
            set
            {
                this._state.IsTableTypeTable = value;
            }
        }

        private bool IsWhereCustom
        {
            get => 
                this._state.IsWhereCustom;
            set
            {
                this._state.IsWhereCustom = value;
            }
        }

        private bool IsWhereCustomSet
        {
            get => 
                this._state.IsWhereCustomSet;
            set
            {
                this._state.IsWhereCustomSet = value;
            }
        }

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState =>
            this._state.LinqDataSourceState;

        private ILinqDataSourcePropertyItem SelectedTable
        {
            get => 
                this._state.SelectedTable;
            set
            {
                this._state.SelectedTable = value;
            }
        }

        private IServiceProvider ServiceProvider =>
            this._state.ServiceProvider;

        private List<ILinqDataSourcePropertyItem> Tables
        {
            get => 
                this._state.Tables;
            set
            {
                this._state.Tables = value;
            }
        }

        internal class ConfigureState
        {
            public ILinqDataSourceConfigureGroupBy ConfigureGroupBy;
            public ILinqDataSourceConfigureSelect ConfigureSelect;
            public string ContextTypeName;
            public bool IsAdvancedEnabled;
            public bool IsDataContext;
            public bool IsOrderByCustom;
            public bool IsOrderByCustomSet;
            public bool IsTableTypeTable;
            public bool IsWhereCustom;
            public bool IsWhereCustomSet;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public ILinqDataSourcePropertyItem SelectedTable;
            public IServiceProvider ServiceProvider;
            public List<ILinqDataSourcePropertyItem> Tables;
        }
    }
}

