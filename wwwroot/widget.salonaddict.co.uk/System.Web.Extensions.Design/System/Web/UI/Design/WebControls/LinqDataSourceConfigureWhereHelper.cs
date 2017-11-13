namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceConfigureWhereHelper
    {
        private ILinqDataSourceConfigureWhereForm _form;
        private LinqDataSourceConfigureWhere.ConfigureWhereState _state;
        private ILinqDataSourceConfigureWhere _where;

        internal LinqDataSourceConfigureWhereHelper()
        {
        }

        public LinqDataSourceConfigureWhereHelper(ILinqDataSourceConfigureWhere where, ILinqDataSourceConfigureWhereForm form, LinqDataSourceConfigureWhere.ConfigureWhereState state)
        {
            this._where = where;
            this._form = form;
            this._state = state;
        }

        internal virtual string GetParameterExpression(IServiceProvider serviceProvider, Parameter parameter, Control control, out bool isHelperText)
        {
            MethodInfo method = typeof(ParameterEditorUserControl).GetMethod("GetParameterExpression", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = new object[4];
            parameters[0] = serviceProvider;
            parameters[1] = parameter;
            parameters[2] = control;
            string str = (string) method.Invoke(null, parameters);
            isHelperText = (bool) parameters[3];
            return str;
        }

        public virtual void UpdateAvailableOperators()
        {
            this.UpdateAvailableOperatorsOnly();
            this.UpdateParameterConfigArea();
        }

        internal void UpdateAvailableOperatorsOnly()
        {
            this._state.AvailableOperators.Clear();
            if (this._state.SelectedWhereField != null)
            {
                this._state.AvailableOperators.Add(LinqDataSourceOperators.EQ);
                this._state.AvailableOperators.Add(LinqDataSourceOperators.NE);
                Type type = this._state.SelectedWhereField.Type;
                if ((type != typeof(bool)) && !type.IsSubclassOf(typeof(bool)))
                {
                    this._state.AvailableOperators.Add(LinqDataSourceOperators.GT);
                    this._state.AvailableOperators.Add(LinqDataSourceOperators.GE);
                    this._state.AvailableOperators.Add(LinqDataSourceOperators.LT);
                    this._state.AvailableOperators.Add(LinqDataSourceOperators.LE);
                }
            }
            this._form.SetOperatorsEnabled(this._state.SelectedWhereField != null);
            this._form.SetOperators(this._state.AvailableOperators);
        }

        public virtual void UpdateCanAdd()
        {
            if ((this._state.SelectedWhereField == null) || (this._state.SelectedOperator == LinqDataSourceOperators.None))
            {
                this._form.SetCanAddSelectExpression(false);
            }
            else
            {
                this._form.SetCanAddSelectExpression((this._state.SelectedParameterEditor != null) && this._where.GetParameterEditorHasCompleteInformation(this._state.SelectedParameterEditor));
            }
        }

        public virtual void UpdateCanRemove()
        {
            this._form.SetCanRemoveSelectExpression(this._state.SelectedWhereExpression != null);
        }

        public virtual void UpdateExpression(string paramPreview, bool isHelperText)
        {
            if ((this._state.SelectedOperator != LinqDataSourceOperators.None) && (this._state.SelectedParameterEditor != null))
            {
                LinqDataSourceWhereExpression currentWhereExpression = this._where.GetCurrentWhereExpression();
                if (currentWhereExpression != null)
                {
                    this._form.SetWhereExpressionPreview(currentWhereExpression.ToString());
                }
                else
                {
                    this._form.SetWhereExpressionPreview(string.Empty);
                }
                if (this._where.GetParameterEditorParameter(this._state.SelectedParameterEditor) == null)
                {
                    this._form.SetParameterValuePreview(string.Empty);
                }
                else if (isHelperText)
                {
                    this._form.SetParameterValuePreview(string.Empty);
                }
                else
                {
                    this._form.SetParameterValuePreview(paramPreview);
                }
            }
            else
            {
                this._form.SetWhereExpressionPreview(string.Empty);
                this._form.SetParameterValuePreview(string.Empty);
            }
        }

        public virtual void UpdateParameterConfigArea()
        {
            this.UpdateParameterConfigAreaOnly();
            string paramPreview = null;
            bool isHelperText = false;
            if (this._state.SelectedParameterEditor != null)
            {
                paramPreview = this.GetParameterExpression(this._state.ServiceProvider, this._where.GetParameterEditorParameter(this._state.SelectedParameterEditor), this._state.DataSource, out isHelperText);
            }
            this.UpdateExpression(paramPreview, isHelperText);
            this.UpdateCanAdd();
        }

        internal void UpdateParameterConfigAreaOnly()
        {
            this._form.SetParametersEnabled(this._state.SelectedOperator != LinqDataSourceOperators.None);
            this._form.SetParameterEditorToShow(this._state.SelectedParameterEditor);
            this._form.SetExpressionPreviewEnabled(this._state.SelectedOperator != LinqDataSourceOperators.None);
        }
    }
}

