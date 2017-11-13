namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal class LinqDataSourceConfigureSelect : ILinqDataSourceConfigureSelect
    {
        private LinqDataSourceSelectBuilder _builder;
        private ILinqDataSourceDesignerHelper _helper;
        private ILinqDataSourceConfigureSelectPanel _panel;
        private ConfigureSelectState _state;
        private ILinqDataSourceWizardForm _wizard;

        public event SelectionChangedEventHandler SelectionChanged;

        private LinqDataSourceConfigureSelect(ILinqDataSourceConfigureSelectPanel form, ILinqDataSourceWizardForm wizard, ILinqDataSourceDesignerHelper helper)
        {
            this._panel = form;
            this._panel.Register(this);
            this._helper = helper;
            this._wizard = wizard;
            this._builder = new LinqDataSourceSelectBuilder();
        }

        internal LinqDataSourceConfigureSelect(ILinqDataSourceConfigureSelectPanel form, ILinqDataSourceWizardForm wizard, ILinqDataSourceDesignerHelper helper, ConfigureSelectState state) : this(form, wizard, helper)
        {
            this._state = state;
        }

        internal LinqDataSourceConfigureSelect(ILinqDataSourceConfigureSelectPanel form, ILinqDataSourceWizardForm wizard, ILinqDataSourceDesignerHelper helper, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(form, wizard, helper)
        {
            this._state = new ConfigureSelectState();
            this.LinqDataSourceState = linqDataSourceState;
            this._state.UngroupedProjections = new List<ILinqDataSourceProjection>();
            this._state.GroupedProjections = new List<ILinqDataSourceProjection>();
            this._state.KeyProjection = new LinqDataSourceProjection(LinqDataSourceSelectBuilder.KeyField, LinqDataSourceAggregateFunctions.None, true, LinqDataSourceSelectBuilder.KeyField.Name);
            this._state.ItProjection = new LinqDataSourceProjection(LinqDataSourceSelectBuilder.ItField, LinqDataSourceAggregateFunctions.None, true, LinqDataSourceSelectBuilder.ItField.Name);
        }

        public void AddField(int fieldIndex)
        {
            if (fieldIndex == -1)
            {
                this._panel.UncheckFieldCheckboxes();
                this.UngroupedProjections.Clear();
            }
            else
            {
                this._panel.UncheckStarCheckbox();
                this.UngroupedProjections.Add(new LinqDataSourceProjection(this.Fields[fieldIndex], LinqDataSourceAggregateFunctions.None, false, null));
            }
            this._wizard.SetCanFinish(true);
            this.OnSelectionChanged();
        }

        public void ChangeProjectionAggregateFunction(int projectionIndex, string newFunction)
        {
            if (this.GroupedProjections.Count == projectionIndex)
            {
                LinqDataSourceAggregateFunctions aggregateFunction = LinqDataSourceAggregateFunctions.FromString(newFunction);
                ILinqDataSourceProjection item = new LinqDataSourceProjection(LinqDataSourceSelectBuilder.CountField, aggregateFunction, true);
                this.GroupedProjections.Add(item);
                this._panel.SetProjectionField(projectionIndex, LinqDataSourceSelectBuilder.CountField.ToString());
                this._panel.SetProjectionAlias(projectionIndex, item.Alias);
                this._panel.EnableAlias(projectionIndex);
            }
            else
            {
                this.GroupedProjections[projectionIndex].AggregateFunction = LinqDataSourceAggregateFunctions.FromString(newFunction);
            }
            this._panel.SetProjectionAlias(projectionIndex, this.GroupedProjections[projectionIndex].Alias);
            this.OnSelectionChanged();
        }

        public void ChangeProjectionAlias(int projectionIndex, string newAlias)
        {
            newAlias = (newAlias == null) ? string.Empty : newAlias.Trim();
            if (!this.GroupedProjections[projectionIndex].IsAliasMandatory || !string.IsNullOrEmpty(newAlias))
            {
                bool flag = true;
                foreach (char ch in newAlias)
                {
                    if (char.IsWhiteSpace(ch))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    this.GroupedProjections[projectionIndex].Alias = newAlias;
                }
            }
            this._panel.SetProjectionAlias(projectionIndex, this.GroupedProjections[projectionIndex].Alias);
            this.OnSelectionChanged();
        }

        public void ChangeProjectionField(int projectionIndex, string newFieldName)
        {
            ILinqDataSourcePropertyItem field = LinqDataSourceProjection.GetField(newFieldName, this.Fields);
            if (field != null)
            {
                if (this.GroupedProjections.Count == projectionIndex)
                {
                    List<LinqDataSourceAggregateFunctions> aggregates = this.GetAggregates(field);
                    LinqDataSourceAggregateFunctions aggregateFunction = aggregates[0];
                    ILinqDataSourceProjection item = new LinqDataSourceProjection(field, aggregateFunction, true);
                    this.GroupedProjections.Add(item);
                    this._panel.SetAggregateFunctions(projectionIndex, aggregates);
                    this._panel.SetProjectionAggregateFunction(projectionIndex, item.AggregateFunction);
                    this._panel.SetProjectionAlias(projectionIndex, item.Alias);
                    this._panel.EnableAlias(projectionIndex);
                }
                else
                {
                    ILinqDataSourceProjection projection2 = this.GroupedProjections[projectionIndex];
                    projection2.Column = field;
                    List<LinqDataSourceAggregateFunctions> list2 = this.GetAggregates(field);
                    this._panel.SetAggregateFunctions(projectionIndex, list2);
                    if (!list2.Contains(projection2.AggregateFunction))
                    {
                        projection2.AggregateFunction = list2[0];
                        this._panel.SetProjectionAggregateFunction(projectionIndex, projection2.AggregateFunction);
                    }
                    this._panel.SetProjectionAlias(projectionIndex, projection2.Alias);
                }
                this.OnSelectionChanged();
            }
        }

        private List<LinqDataSourceAggregateFunctions> GetAggregates(ILinqDataSourcePropertyItem field)
        {
            if (field == LinqDataSourceSelectBuilder.CountField)
            {
                return LinqDataSourceProjection.GetValidAggregates(null);
            }
            return LinqDataSourceProjection.GetValidAggregates(field.Type);
        }

        public void GroupByChangedHandler(bool isNone, bool isCustom, ILinqDataSourcePropertyItem groupByField)
        {
            if (isNone)
            {
                this.GroupByMode = LinqDataSourceGroupByMode.GroupByNone;
            }
            else if (isCustom)
            {
                this.GroupByMode = LinqDataSourceGroupByMode.GroupByCustom;
            }
            else
            {
                this.GroupByMode = LinqDataSourceGroupByMode.GroupByField;
                LinqDataSourceSelectBuilder.KeyField.DisplayName = string.Format(CultureInfo.InvariantCulture, "key ({0})", new object[] { groupByField.ToString() });
                this.KeyProjection.Alias = groupByField.Name;
                this._panel.SetProjectionField(this.GroupedProjections.IndexOf(this.KeyProjection), this.KeyProjection.Column.ToString());
                this._panel.SetProjectionAlias(this.GroupedProjections.IndexOf(this.KeyProjection), this.KeyProjection.Alias);
            }
            this._panel.SetSelectMode(this.GroupByMode);
            this.OnSelectionChanged();
        }

        public void LoadState()
        {
            LinqDataSourceSelectBuilder.ParseResult result = this._builder.CreateProjection(this._state.LinqDataSourceState, this._state.Fields);
            this.GroupByMode = result.Mode;
            if (this.GroupByMode == LinqDataSourceGroupByMode.GroupByNone)
            {
                this.UngroupedProjections.Clear();
                foreach (ILinqDataSourceProjection projection in result.Projections)
                {
                    this.UngroupedProjections.Add(projection);
                }
                if (this.UngroupedProjections.Count > 0)
                {
                    this._panel.UncheckStarCheckbox();
                }
                foreach (LinqDataSourceProjection projection2 in this.UngroupedProjections)
                {
                    this._panel.AddField(projection2.Column);
                }
            }
            else if (this.GroupByMode == LinqDataSourceGroupByMode.GroupByField)
            {
                this.GroupedProjections.Clear();
                foreach (ILinqDataSourceProjection projection3 in result.Projections)
                {
                    this.GroupedProjections.Add(projection3);
                }
                this._panel.ClearGridProjections();
                foreach (LinqDataSourceProjection projection4 in this.GroupedProjections)
                {
                    if (projection4.Column == LinqDataSourceSelectBuilder.ItField)
                    {
                        this._state.ItProjection = projection4;
                        this._panel.AddProjection(projection4.Column, null, projection4.AggregateFunction, projection4.Alias, true);
                    }
                    else if (projection4.Column == LinqDataSourceSelectBuilder.KeyField)
                    {
                        this._state.KeyProjection = projection4;
                        this._panel.AddProjection(projection4.Column, null, projection4.AggregateFunction, projection4.Alias, true);
                    }
                    else
                    {
                        this._panel.AddProjection(projection4.Column, this.GetAggregates(projection4.Column), projection4.AggregateFunction, projection4.Alias, false);
                    }
                }
            }
            else
            {
                this.CustomGroupBy = this.LinqDataSourceState.GroupBy;
                this.CustomSelect = this.LinqDataSourceState.Select;
                this._panel.SetCustomGroupBy(this.CustomGroupBy);
                this._panel.SetCustomSelect(this.CustomSelect);
            }
            this.OnSelectionChanged();
            this._panel.SetSelectMode(this.GroupByMode);
        }

        public void MoveProjection(int oldIndex, int newIndex)
        {
            ILinqDataSourceProjection item = this.GroupedProjections[oldIndex];
            this.GroupedProjections.RemoveAt(oldIndex);
            this.GroupedProjections.Insert(newIndex, item);
            this._panel.MoveProjection(oldIndex, newIndex);
            this.OnSelectionChanged();
        }

        public void OnSelectionChanged()
        {
            bool hasSelect = false;
            switch (this.GroupByMode)
            {
                case LinqDataSourceGroupByMode.GroupByNone:
                    hasSelect = this.UngroupedProjections.Count > 0;
                    break;

                case LinqDataSourceGroupByMode.GroupByField:
                    hasSelect = this.GroupedProjections.Count > 0;
                    break;

                case LinqDataSourceGroupByMode.GroupByCustom:
                    hasSelect = !string.IsNullOrEmpty(this.CustomSelect);
                    break;
            }
            this._selectionChanged(new LinqDataSourceSelectionChangedEventArgs(this.GroupByMode, hasSelect));
        }

        public void RemoveField(int fieldIndex)
        {
            if (fieldIndex > -1)
            {
                for (int i = 0; i < this.UngroupedProjections.Count; i++)
                {
                    if (this.Fields[fieldIndex] == this.UngroupedProjections[i].Column)
                    {
                        this.UngroupedProjections.RemoveAt(i);
                    }
                }
            }
            if (this.UngroupedProjections.Count == 0)
            {
                this._wizard.SetCanFinish(false);
            }
            this.OnSelectionChanged();
        }

        public void RemoveProjectionDeleteButton(int projectionIndex)
        {
            this._panel.RemoveProjection(projectionIndex);
            this.RemoveProjectionDeleteKey(projectionIndex);
        }

        public void RemoveProjectionDeleteKey(int projectionIndex)
        {
            this.GroupedProjections.RemoveAt(projectionIndex);
            if (projectionIndex == this.GroupedProjections.Count)
            {
                this.SelectProjection(projectionIndex - 1);
            }
            else
            {
                this.SelectProjection(projectionIndex);
            }
            this.OnSelectionChanged();
        }

        private void SaveProjections(List<ILinqDataSourceProjection> projections)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = projections.Count > 0;
            if (flag)
            {
                builder.Append("new (");
            }
            int num = 0;
            foreach (LinqDataSourceProjection projection in projections)
            {
                builder.Append(projection.ToString());
                if (num < (projections.Count - 1))
                {
                    builder.Append(", ");
                    num++;
                }
            }
            if (flag)
            {
                builder.Append(")");
            }
            this.LinqDataSourceState.Select = builder.ToString();
        }

        public void SaveState()
        {
            if (this.GroupByMode == LinqDataSourceGroupByMode.GroupByNone)
            {
                this.SaveProjections(this.UngroupedProjections);
                this.LinqDataSourceState.SelectParameters.Clear();
            }
            else if (this.GroupByMode == LinqDataSourceGroupByMode.GroupByField)
            {
                this.SaveProjections(this.GroupedProjections);
                this.LinqDataSourceState.SelectParameters.Clear();
            }
            else
            {
                this.LinqDataSourceState.GroupBy = this.CustomGroupBy;
                this.LinqDataSourceState.Select = this.CustomSelect;
            }
        }

        public void SelectProjection(int projectionIndex)
        {
            if (projectionIndex < this.GroupedProjections.Count)
            {
                this._panel.SetCanMoveUp(projectionIndex > 0);
                this._panel.SetCanMoveDown(projectionIndex < (this.GroupedProjections.Count - 1));
                this._panel.SetCanRemove((this.GroupedProjections[projectionIndex] != this.ItProjection) && (this.GroupedProjections[projectionIndex] != this.KeyProjection));
            }
            else
            {
                this._panel.SetCanMoveUp(false);
                this._panel.SetCanMoveDown(false);
                this._panel.SetCanRemove(false);
            }
        }

        public void SelectTable(ILinqDataSourcePropertyItem table)
        {
            this.Table = table;
            if (this.Table != null)
            {
                this.Fields = this._helper.GetProperties(this.Table, false, true);
                List<ILinqDataSourcePropertyItem> fields = new List<ILinqDataSourcePropertyItem>();
                foreach (ILinqDataSourcePropertyItem item in this.Fields)
                {
                    fields.Add(item);
                }
                fields.Sort();
                fields.Add(LinqDataSourceSelectBuilder.CountField);
                this._panel.SetCheckBoxFields(this.Fields);
                this._panel.SetGridFields(fields);
                List<LinqDataSourceAggregateFunctions> functions = new List<LinqDataSourceAggregateFunctions> {
                    LinqDataSourceAggregateFunctions.Count
                };
                this._panel.SetGridAggregateFunctions(functions);
                LinqDataSourceSelectBuilder.ItField.DisplayName = string.Format(CultureInfo.InvariantCulture, "it ({0})", new object[] { LinqDataSourceTablePropertyItem.GetTypeDisplay(table.Type) });
                this.ItProjection.Alias = table.Name;
            }
            else
            {
                this._panel.SetCheckBoxFields(null);
            }
            this.UngroupedProjections.Clear();
            this._panel.UncheckFieldCheckboxes();
            this.GroupedProjections.Clear();
            this.GroupedProjections.Add(this.KeyProjection);
            this.GroupedProjections.Add(this.ItProjection);
            this._panel.ClearGridProjections();
            this._panel.AddProjection(this.KeyProjection.Column, null, this.KeyProjection.AggregateFunction, this.KeyProjection.Alias, true);
            this._panel.AddProjection(this.ItProjection.Column, null, this.ItProjection.AggregateFunction, this.ItProjection.Alias, true);
            this.CustomGroupBy = null;
            this.CustomSelect = null;
            this._panel.SetCustomGroupBy(null);
            this._panel.SetCustomSelect(null);
            this.GroupByMode = LinqDataSourceGroupByMode.GroupByNone;
            this.OnSelectionChanged();
            this._panel.SetSelectMode(this.GroupByMode);
        }

        public void SetCustomGroupBy(string newGroupBy)
        {
            this.CustomGroupBy = newGroupBy;
            this.OnSelectionChanged();
        }

        public void SetCustomSelect(string newSelect)
        {
            this.CustomSelect = newSelect;
            this.OnSelectionChanged();
        }

        private string CustomGroupBy
        {
            get => 
                this._state.CustomGroupBy;
            set
            {
                this._state.CustomGroupBy = value;
            }
        }

        private string CustomSelect
        {
            get => 
                this._state.CustomSelect;
            set
            {
                this._state.CustomSelect = value;
            }
        }

        private List<ILinqDataSourcePropertyItem> Fields
        {
            get => 
                this._state.Fields;
            set
            {
                this._state.Fields = value;
            }
        }

        private LinqDataSourceGroupByMode GroupByMode
        {
            get => 
                this._state.SelectMode;
            set
            {
                this._state.SelectMode = value;
            }
        }

        private List<ILinqDataSourceProjection> GroupedProjections =>
            this._state.GroupedProjections;

        private ILinqDataSourceProjection ItProjection =>
            this._state.ItProjection;

        private ILinqDataSourceProjection KeyProjection =>
            this._state.KeyProjection;

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState
        {
            get => 
                this._state.LinqDataSourceState;
            set
            {
                this._state.LinqDataSourceState = value;
            }
        }

        private ILinqDataSourcePropertyItem Table
        {
            get => 
                this._state.TableProperty;
            set
            {
                this._state.TableProperty = value;
            }
        }

        private List<ILinqDataSourceProjection> UngroupedProjections =>
            this._state.UngroupedProjections;

        internal class ConfigureSelectState
        {
            public string CustomGroupBy;
            public string CustomSelect;
            public List<ILinqDataSourcePropertyItem> Fields;
            public List<ILinqDataSourceProjection> GroupedProjections;
            public ILinqDataSourceProjection ItProjection;
            public ILinqDataSourceProjection KeyProjection;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public LinqDataSourceGroupByMode SelectMode;
            public ILinqDataSourcePropertyItem TableProperty;
            public List<ILinqDataSourceProjection> UngroupedProjections;
        }
    }
}

