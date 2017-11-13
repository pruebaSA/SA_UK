namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Resources.Design;

    internal class LinqDataSourceConfigureGroupBy : ILinqDataSourceConfigureGroupBy
    {
        private ILinqDataSourceConfigureGroupByPanel _form;
        private ILinqDataSourceDesignerHelper _helper;
        private ConfigureGroupByState _state;

        private event LinqDataSourceGroupByChangedEventHandler _groupByChanged;

        public event LinqDataSourceGroupByChangedEventHandler GroupByChanged;

        private LinqDataSourceConfigureGroupBy(ILinqDataSourceConfigureGroupByPanel form, ILinqDataSourceDesignerHelper helper)
        {
            this._form = form;
            this._form.Register(this);
            this._helper = helper;
        }

        internal LinqDataSourceConfigureGroupBy(ILinqDataSourceConfigureGroupByPanel form, ILinqDataSourceDesignerHelper helper, ConfigureGroupByState state) : this(form, helper)
        {
            this._state = state;
        }

        internal LinqDataSourceConfigureGroupBy(ILinqDataSourceConfigureGroupByPanel form, ILinqDataSourceDesignerHelper helper, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(form, helper)
        {
            this._state = new ConfigureGroupByState();
            this.LinqDataSourceState = linqDataSourceState;
            this.GroupByNone = new LinqDataSourcePropertyItem(null, AtlasWebDesign.Combo_NoneOption);
            this.GroupByCustom = new LinqDataSourcePropertyItem(null, AtlasWebDesign.LinqDataSourceConfigureGroupBy_GroupByCustomOption);
        }

        public void LoadState()
        {
            bool flag = false;
            if (this.LinqDataSourceState.GroupByParameters.Count == 0)
            {
                string groupBy = this.LinqDataSourceState.GroupBy;
                foreach (ILinqDataSourcePropertyItem item in this.TableFields)
                {
                    if ((item != this.GroupByNone) && string.Equals(item.Name, groupBy, StringComparison.OrdinalIgnoreCase))
                    {
                        this.SelectedGroupByField = item;
                        this._form.SetSelectedGroupByField(item);
                        this._form.ShowOrderGroupsBy(true);
                        this.OrderGroupsBy = this.LinqDataSourceState.OrderGroupsBy;
                        this._form.OrderGroupsBy = this.OrderGroupsBy;
                        return;
                    }
                }
                if (string.IsNullOrEmpty(groupBy))
                {
                    this.SelectedGroupByField = null;
                    this._form.SetSelectedGroupByField(this.GroupByNone);
                    flag = true;
                }
                else
                {
                    this.SelectedGroupByField = this.GroupByCustom;
                    this._form.SetSelectedGroupByField(this.GroupByCustom);
                }
            }
            else
            {
                this.SelectedGroupByField = this.GroupByCustom;
                this._form.SetSelectedGroupByField(this.GroupByCustom);
            }
            if (flag)
            {
                this._form.ShowOrderGroupsBy(false);
            }
            this.OrderGroupsBy = this.LinqDataSourceState.OrderGroupsBy;
            this._form.OrderGroupsBy = this.OrderGroupsBy;
        }

        public void SaveState()
        {
            if (this.SelectedGroupByField == null)
            {
                this.LinqDataSourceState.GroupBy = null;
                this.LinqDataSourceState.GroupByParameters.Clear();
            }
            else if (this.SelectedGroupByField != this.GroupByCustom)
            {
                this.LinqDataSourceState.GroupBy = this.SelectedGroupByField.Name;
                this.LinqDataSourceState.GroupByParameters.Clear();
            }
            this.OrderGroupsBy = this._form.OrderGroupsBy;
            this.LinqDataSourceState.OrderGroupsBy = this.OrderGroupsBy;
            if (string.IsNullOrEmpty(this.OrderGroupsBy))
            {
                this.LinqDataSourceState.OrderGroupsByParameters.Clear();
            }
        }

        public void SelectGroupBy(ILinqDataSourcePropertyItem selected)
        {
            bool flag;
            bool flag2;
            ILinqDataSourcePropertyItem selectedGroupByField;
            if ((selected == null) || (selected == this.GroupByNone))
            {
                this.SelectedGroupByField = null;
                flag = true;
                flag2 = false;
                selectedGroupByField = null;
            }
            else if (selected == this.GroupByCustom)
            {
                this.SelectedGroupByField = this.GroupByCustom;
                flag = false;
                flag2 = true;
                selectedGroupByField = null;
            }
            else
            {
                this.SelectedGroupByField = selected;
                flag = false;
                flag2 = false;
                selectedGroupByField = this.SelectedGroupByField;
            }
            this._form.ShowOrderGroupsBy(!flag);
            if (flag)
            {
                this._form.OrderGroupsBy = string.Empty;
            }
            this._groupByChanged(flag, flag2, selectedGroupByField);
        }

        public void SelectionChangedHandler(LinqDataSourceSelectionChangedEventArgs e)
        {
            if (e.GroupByMode == LinqDataSourceGroupByMode.GroupByCustom)
            {
                this.SelectedGroupByField = this.GroupByCustom;
                this._form.SetGroupBy(this.GroupByCustom);
            }
        }

        public void SelectTable(ILinqDataSourcePropertyItem tableProperty)
        {
            if (tableProperty != null)
            {
                this.TableFields = this._helper.GetProperties(tableProperty, true, false);
                this.TableFields.Insert(0, this.GroupByNone);
                this.TableFields.Add(this.GroupByCustom);
            }
            else
            {
                this.TableFields = new List<ILinqDataSourcePropertyItem>();
                this.TableFields.Add(this.GroupByNone);
            }
            this._form.SetGroupByFields(this.TableFields);
            this.SelectedGroupByField = null;
            this._form.SetSelectedGroupByField(this.GroupByNone);
        }

        private ILinqDataSourcePropertyItem GroupByCustom
        {
            get => 
                this._state.GroupByCustom;
            set
            {
                this._state.GroupByCustom = value;
            }
        }

        private ILinqDataSourcePropertyItem GroupByNone
        {
            get => 
                this._state.GroupByNone;
            set
            {
                this._state.GroupByNone = value;
            }
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

        private string OrderGroupsBy
        {
            get => 
                this._state.OrderGroupsBy;
            set
            {
                this._state.OrderGroupsBy = value;
            }
        }

        private ILinqDataSourcePropertyItem SelectedGroupByField
        {
            get => 
                this._state.SelectedGroupByField;
            set
            {
                this._state.SelectedGroupByField = value;
            }
        }

        private List<ILinqDataSourcePropertyItem> TableFields
        {
            get => 
                this._state.GroupByFields;
            set
            {
                this._state.GroupByFields = value;
            }
        }

        internal class ConfigureGroupByState
        {
            public ILinqDataSourcePropertyItem GroupByCustom;
            public List<ILinqDataSourcePropertyItem> GroupByFields;
            public ILinqDataSourcePropertyItem GroupByNone;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
            public string OrderGroupsBy;
            public ILinqDataSourcePropertyItem SelectedGroupByField;
        }
    }
}

