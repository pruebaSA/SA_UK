namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceConfigureWhere : ILinqDataSourceConfigureWhere
    {
        private LinqDataSourceWhereBuilder _builder;
        private ILinqDataSourceDesignerHelper _designerHelper;
        private ILinqDataSourceConfigureWhereForm _form;
        private Type _parameterEditorType;
        private ConfigureWhereState _state;
        private LinqDataSourceConfigureWhereHelper _whereHelper;

        private LinqDataSourceConfigureWhere(ILinqDataSourceConfigureWhereForm form, ILinqDataSourceDesignerHelper helper, Control dataSource, IServiceProvider serviceProvider)
        {
            this._form = form;
            this._form.Register(this, dataSource);
            this._designerHelper = helper;
        }

        internal LinqDataSourceConfigureWhere(ILinqDataSourceConfigureWhereForm form, ILinqDataSourceDesignerHelper helper, Control dataSource, IServiceProvider serviceProvider, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(form, helper, dataSource, serviceProvider)
        {
            this._state = new ConfigureWhereState();
            this._state.LinqDataSourceState = linqDataSourceState;
            this._state.AvailableOperators = new List<LinqDataSourceOperators>();
            this._state.DataSource = dataSource;
            this._state.ServiceProvider = serviceProvider;
            this._builder = new LinqDataSourceWhereBuilder();
            this._whereHelper = new LinqDataSourceConfigureWhereHelper(this, this._form, this._state);
        }

        internal LinqDataSourceConfigureWhere(ILinqDataSourceConfigureWhereForm form, ILinqDataSourceDesignerHelper helper, Control dataSource, IServiceProvider serviceProvider, ConfigureWhereState state, LinqDataSourceConfigureWhereHelper whereHelper, LinqDataSourceWhereBuilder builder) : this(form, helper, dataSource, serviceProvider)
        {
            this._state = state;
            this._whereHelper = whereHelper;
            this._builder = builder;
        }

        public void AddCurrentWhereExpression()
        {
            LinqDataSourceWhereExpression currentWhereExpression = this.GetCurrentWhereExpression();
            this.WhereStatement.Add(currentWhereExpression);
            Parameter parameterEditorParameter = this.GetParameterEditorParameter(this.SelectedParameterEditor);
            parameterEditorParameter.Name = currentWhereExpression.ParameterName;
            ObjectDataSourceDesigner.SetParameterType(parameterEditorParameter, this.SelectedWhereField.Type);
            this.WhereStatement.Parameters.Add(parameterEditorParameter);
            this._form.AddNewWhereExpressionItem(currentWhereExpression, this.WhereStatement, this.ServiceProvider, this.DataSource);
            this.SelectedWhereField = null;
            this.SelectedOperator = LinqDataSourceOperators.None;
            this.SelectedParameterEditor = null;
            this._form.SetSelectedWhereField(null);
            this._form.SetOperatorsEnabled(false);
            this._form.SetParametersEnabled(false);
        }

        public virtual void AttachParameterEditorChangedHandler(object parameterEditor, Delegate handler)
        {
            this.GetParameterEditorType().GetEvent("ParameterChanged").AddEventHandler(parameterEditor, handler);
        }

        public LinqDataSourceWhereExpression GetCurrentWhereExpression()
        {
            if (((this.SelectedWhereField != null) && (this.SelectedOperator != LinqDataSourceOperators.None)) && (this.SelectedParameterEditor != null))
            {
                return new LinqDataSourceWhereExpression(this.SelectedWhereField.Name, this.SelectedOperator.ToString(), this.GetNextParameterName(this.SelectedWhereField.Name));
            }
            return null;
        }

        internal string GetNextParameterName(string proposedName)
        {
            List<string> list = new List<string>();
            bool flag = false;
            foreach (Parameter parameter in this.WhereStatement.Parameters)
            {
                if (parameter.Name.StartsWith(proposedName, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(parameter.Name, proposedName, StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                    }
                    else
                    {
                        list.Add(parameter.Name.Substring(proposedName.Length));
                    }
                }
            }
            if (!flag)
            {
                return proposedName;
            }
            for (int i = 1; i <= list.Count; i++)
            {
                if (!list.Contains(i.ToString(CultureInfo.InvariantCulture)))
                {
                    return (proposedName + i.ToString(CultureInfo.InvariantCulture));
                }
            }
            int num2 = list.Count + 1;
            return (proposedName + num2.ToString(CultureInfo.InvariantCulture));
        }

        public virtual bool GetParameterEditorHasCompleteInformation(object parameterEditor) => 
            ((bool) this.GetParameterEditorType().GetProperty("HasCompleteInformation").GetValue(parameterEditor, null));

        public virtual Parameter GetParameterEditorParameter(object parameterEditor) => 
            ((Parameter) this.GetParameterEditorType().GetProperty("Parameter").GetValue(parameterEditor, null));

        internal virtual Type GetParameterEditorType()
        {
            if (this._parameterEditorType == null)
            {
                this._parameterEditorType = typeof(SqlDataSourceDesigner).Assembly.GetType("System.Web.UI.Design.WebControls.SqlDataSourceConfigureFilterForm+ParameterEditor");
            }
            return this._parameterEditorType;
        }

        public virtual void InitializeParameterEditor(object parameterEditor)
        {
            this.GetParameterEditorType().GetMethod("Initialize").Invoke(parameterEditor, null);
        }

        public void InvalidateSelectedParameter()
        {
            string paramPreview = null;
            bool isHelperText = false;
            if (this.SelectedParameterEditor != null)
            {
                paramPreview = this._whereHelper.GetParameterExpression(this.ServiceProvider, this.GetParameterEditorParameter(this.SelectedParameterEditor), this.DataSource, out isHelperText);
            }
            this._whereHelper.UpdateExpression(paramPreview, isHelperText);
            this._whereHelper.UpdateCanAdd();
        }

        public bool LoadState()
        {
            bool flag = true;
            string where = this.LinqDataSourceState.Where;
            if (this.LinqDataSourceState.AutoGenerateWhereClause)
            {
                flag = false;
            }
            if (!string.IsNullOrEmpty(where))
            {
                ParameterCollection clonedWhereParameters = this._designerHelper.CloneParameters(this.LinqDataSourceState.WhereParameters);
                this.WhereStatement = this._builder.MakeWhereStatement(this.LinqDataSourceState.Where, clonedWhereParameters);
                if (this.WhereStatement == null)
                {
                    this.WhereStatement = new LinqDataSourceWhereStatement();
                    flag = false;
                }
            }
            else
            {
                this.WhereStatement = new LinqDataSourceWhereStatement();
            }
            this.SelectedWhereField = null;
            this.SelectedOperator = LinqDataSourceOperators.None;
            this.SelectedParameterEditor = null;
            this._form.SetSelectedWhereField(null);
            this._form.SetWhereStatement(this.WhereStatement, this.ServiceProvider, this.DataSource);
            return flag;
        }

        public void RemoveCurrentWhereExpression()
        {
            this.WhereStatement.RemoveExpression(this.SelectedWhereExpression);
            this._form.RemoveWhereExpressionItem(this.SelectedWhereExpression);
        }

        public void SaveState()
        {
            this.LinqDataSourceState.Where = this.WhereStatement.ToString();
            this.LinqDataSourceState.WhereParameters.Clear();
            foreach (Parameter parameter in this.WhereStatement.Parameters)
            {
                this.LinqDataSourceState.WhereParameters.Add(parameter);
            }
        }

        public void SelectOperator(LinqDataSourceOperators selected)
        {
            this.SelectedOperator = selected;
            this._whereHelper.UpdateParameterConfigArea();
        }

        public void SelectParameterEditor(object selected)
        {
            this.SelectedParameterEditor = selected;
            this._whereHelper.UpdateParameterConfigArea();
        }

        public void SelectTable(ILinqDataSourcePropertyItem table)
        {
            if (table != null)
            {
                this.WhereFields = this._designerHelper.GetProperties(table, true, false);
            }
            else
            {
                this.WhereFields = new List<ILinqDataSourcePropertyItem>();
            }
            this.WhereStatement = new LinqDataSourceWhereStatement();
            this.SelectedWhereField = null;
            this.SelectedOperator = LinqDataSourceOperators.None;
            this.SelectedParameterEditor = null;
            this._form.SetWhereFields(this.WhereFields);
            this._whereHelper.UpdateCanRemove();
            this._whereHelper.UpdateAvailableOperators();
            this._form.SetWhereStatement(this.WhereStatement, this.ServiceProvider, this.DataSource);
        }

        public void SelectWhereExpression(LinqDataSourceWhereExpression selected)
        {
            this.SelectedWhereExpression = selected;
            this._whereHelper.UpdateCanRemove();
        }

        public void SelectWhereField(ILinqDataSourcePropertyItem selected)
        {
            this.SelectedWhereField = selected;
            this._whereHelper.UpdateAvailableOperators();
        }

        public virtual void SetParameterEditorVisible(object parameterEditor, bool value)
        {
            this.GetParameterEditorType().GetProperty("Visible").SetValue(parameterEditor, value, null);
        }

        private Control DataSource =>
            this._state.DataSource;

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState =>
            this._state.LinqDataSourceState;

        private LinqDataSourceOperators SelectedOperator
        {
            get => 
                this._state.SelectedOperator;
            set
            {
                this._state.SelectedOperator = value;
            }
        }

        private object SelectedParameterEditor
        {
            get => 
                this._state.SelectedParameterEditor;
            set
            {
                this._state.SelectedParameterEditor = value;
            }
        }

        private LinqDataSourceWhereExpression SelectedWhereExpression
        {
            get => 
                this._state.SelectedWhereExpression;
            set
            {
                this._state.SelectedWhereExpression = value;
            }
        }

        private ILinqDataSourcePropertyItem SelectedWhereField
        {
            get => 
                this._state.SelectedWhereField;
            set
            {
                this._state.SelectedWhereField = value;
            }
        }

        private IServiceProvider ServiceProvider =>
            this._state.ServiceProvider;

        private List<ILinqDataSourcePropertyItem> WhereFields
        {
            get => 
                this._state.WhereFields;
            set
            {
                this._state.WhereFields = value;
            }
        }

        private LinqDataSourceWhereStatement WhereStatement
        {
            get => 
                this._state.WhereStatement;
            set
            {
                this._state.WhereStatement = value;
            }
        }

        internal class ConfigureWhereState
        {
            public List<LinqDataSourceOperators> AvailableOperators;
            public Control DataSource;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public LinqDataSourceOperators SelectedOperator;
            public object SelectedParameterEditor;
            public LinqDataSourceWhereExpression SelectedWhereExpression;
            public ILinqDataSourcePropertyItem SelectedWhereField;
            public IServiceProvider ServiceProvider;
            public List<ILinqDataSourcePropertyItem> WhereFields;
            public LinqDataSourceWhereStatement WhereStatement;
        }
    }
}

