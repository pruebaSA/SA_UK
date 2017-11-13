namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Resources.Design;

    internal class LinqDataSourceConfigureOrderBy : ILinqDataSourceConfigureOrderBy
    {
        private ILinqDataSourceConfigureOrderByForm _form;
        private ILinqDataSourceDesignerHelper _helper;
        private ConfigureOrderByState _state;

        private LinqDataSourceConfigureOrderBy(ILinqDataSourceConfigureOrderByForm form, ILinqDataSourceDesignerHelper helper)
        {
            this._form = form;
            this._form.Register(this);
            this._helper = helper;
        }

        internal LinqDataSourceConfigureOrderBy(ILinqDataSourceConfigureOrderByForm form, ILinqDataSourceDesignerHelper helper, ConfigureOrderByState state) : this(form, helper)
        {
            this._state = state;
        }

        internal LinqDataSourceConfigureOrderBy(ILinqDataSourceConfigureOrderByForm form, ILinqDataSourceDesignerHelper helper, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(form, helper)
        {
            this._state = new ConfigureOrderByState();
            this.LinqDataSourceState = linqDataSourceState;
            this.OrderByNone = new LinqDataSourcePropertyItem(null, AtlasWebDesign.Combo_NoneOption);
            this.SelectedOrderByFields = new OrderByStatement();
        }

        public bool LoadState()
        {
            this._form.SetSelectedOrderByField(0, this.OrderByNone, true);
            string[] strArray = this.LinqDataSourceState.OrderBy.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if ((strArray.Length > 3) || this.LinqDataSourceState.AutoGenerateOrderByClause)
            {
                return false;
            }
            for (int i = 0; i < strArray.Length; i++)
            {
                OrderByExpression item = new OrderByExpression(strArray[i].Trim(), this.OrderByFields, this.OrderByNone);
                if (item.Field != this.OrderByNone)
                {
                    this.SelectedOrderByFields.Add(item);
                    this._form.SetOrderByFieldEnabled(i, true);
                    this._form.SetSelectedOrderByField(i, item.Field, item.IsAsc);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void SaveState()
        {
            this.LinqDataSourceState.OrderBy = this.SelectedOrderByFields.ToString();
            this.LinqDataSourceState.OrderByParameters.Clear();
        }

        public void SelectOrderByDirection(int clauseNumber, bool isAsc)
        {
            if (this.SelectedOrderByFields.Count > clauseNumber)
            {
                this.SelectedOrderByFields[clauseNumber].IsAsc = isAsc;
            }
            this._form.SetPreview(this.SelectedOrderByFields.ToString());
        }

        public void SelectOrderByField(int clauseNumber, ILinqDataSourcePropertyItem field)
        {
            if ((field == null) || (field == this.OrderByNone))
            {
                while (this.SelectedOrderByFields.Count > clauseNumber)
                {
                    this.SelectedOrderByFields.RemoveAt(clauseNumber);
                }
                if (clauseNumber < 2)
                {
                    this._form.SetOrderByFieldEnabled(clauseNumber + 1, false);
                }
                this._form.SetOrderByDirectionEnabled(clauseNumber, false);
            }
            else
            {
                if (clauseNumber < this.SelectedOrderByFields.Count)
                {
                    this.SelectedOrderByFields[clauseNumber].Field = field;
                }
                else
                {
                    this.SelectedOrderByFields.Add(new OrderByExpression(field.Name, true, this.OrderByFields, this.OrderByNone));
                }
                if (clauseNumber < 2)
                {
                    this._form.SetOrderByFieldEnabled(clauseNumber + 1, true);
                }
                this._form.SetOrderByDirectionEnabled(clauseNumber, true);
            }
            this._form.SetPreview(this.SelectedOrderByFields.ToString());
        }

        public void SelectTable(ILinqDataSourcePropertyItem tableProperty)
        {
            if (tableProperty != null)
            {
                this.OrderByFields = this._helper.GetProperties(tableProperty, true, false);
            }
            else
            {
                this.OrderByFields = new List<ILinqDataSourcePropertyItem>();
            }
            this.OrderByFields.Insert(0, this.OrderByNone);
            this._form.SetOrderByFields(this.OrderByFields);
            this.SelectedOrderByFields.Clear();
            this._form.SetOrderByFieldEnabled(2, false);
            this._form.SetOrderByFieldEnabled(1, false);
            this._form.SetSelectedOrderByField(0, this.OrderByNone, true);
        }

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState
        {
            get => 
                this._state.LinqDataSourceState;
            set
            {
                this._state.LinqDataSourceState = value;
            }
        }

        private List<ILinqDataSourcePropertyItem> OrderByFields
        {
            get => 
                this._state.OrderByFields;
            set
            {
                this._state.OrderByFields = value;
            }
        }

        private ILinqDataSourcePropertyItem OrderByNone
        {
            get => 
                this._state.OrderByNone;
            set
            {
                this._state.OrderByNone = value;
            }
        }

        private OrderByStatement SelectedOrderByFields
        {
            get => 
                this._state.SelectedOrderByFields;
            set
            {
                this._state.SelectedOrderByFields = value;
            }
        }

        internal class ConfigureOrderByState
        {
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public List<ILinqDataSourcePropertyItem> OrderByFields;
            public ILinqDataSourcePropertyItem OrderByNone;
            public LinqDataSourceConfigureOrderBy.OrderByStatement SelectedOrderByFields;
        }

        internal sealed class OrderByExpression
        {
            private ILinqDataSourcePropertyItem _field;
            private bool _isAsc;

            public OrderByExpression(string expression, List<ILinqDataSourcePropertyItem> orderByFields, ILinqDataSourcePropertyItem orderByNone)
            {
                expression = expression.Trim();
                int length = 0;
                while ((length < expression.Length) && !char.IsWhiteSpace(expression[length]))
                {
                    length++;
                }
                this.SetField(expression.Substring(0, length), orderByFields, orderByNone);
                if (length < expression.Length)
                {
                    if (string.Equals("asc", expression.Substring(length).Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        this._isAsc = true;
                    }
                    else if (string.Equals("desc", expression.Substring(length).Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        this._isAsc = false;
                    }
                    else
                    {
                        this._field = orderByNone;
                    }
                }
                else
                {
                    this._isAsc = true;
                }
            }

            public OrderByExpression(string fieldName, bool isAsc, List<ILinqDataSourcePropertyItem> orderByFields, ILinqDataSourcePropertyItem orderByNone)
            {
                this.SetField(fieldName, orderByFields, orderByNone);
                this._isAsc = isAsc;
            }

            private void SetField(string fieldName, List<ILinqDataSourcePropertyItem> orderByFields, ILinqDataSourcePropertyItem orderByNone)
            {
                foreach (ILinqDataSourcePropertyItem item in orderByFields)
                {
                    if ((item != orderByNone) && string.Equals(item.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        this._field = item;
                        return;
                    }
                }
                this._field = orderByNone;
            }

            public override string ToString() => 
                (this.Field.Name + (this.IsAsc ? string.Empty : " desc"));

            public ILinqDataSourcePropertyItem Field
            {
                get => 
                    this._field;
                set
                {
                    this._field = value;
                }
            }

            public bool IsAsc
            {
                get => 
                    this._isAsc;
                set
                {
                    this._isAsc = value;
                }
            }
        }

        internal sealed class OrderByStatement : List<LinqDataSourceConfigureOrderBy.OrderByExpression>
        {
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < base.Count; i++)
                {
                    builder.Append(base[i].ToString());
                    if (i < (base.Count - 1))
                    {
                        builder.Append(", ");
                    }
                }
                if (builder.Length <= 0)
                {
                    return null;
                }
                return builder.ToString();
            }
        }
    }
}

