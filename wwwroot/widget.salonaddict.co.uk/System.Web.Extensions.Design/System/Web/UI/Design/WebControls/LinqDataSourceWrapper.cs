namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceWrapper : ILinqDataSourceWrapper
    {
        private ILinqDataSourceDesignerHelper _designerHelper;
        private LinqDataSource _linqDataSource;

        public LinqDataSourceWrapper(LinqDataSource linqDataSource, ILinqDataSourceDesignerHelper designerHelper)
        {
            this._linqDataSource = linqDataSource;
            this._designerHelper = designerHelper;
        }

        public ParameterCollection CloneDeleteParameters() => 
            this.CloneParameters(this._linqDataSource.DeleteParameters);

        public ParameterCollection CloneGroupByParameters() => 
            this.CloneParameters(this._linqDataSource.GroupByParameters);

        public ParameterCollection CloneInsertParameters() => 
            this.CloneParameters(this._linqDataSource.InsertParameters);

        public ParameterCollection CloneOrderByParameters() => 
            this.CloneParameters(this._linqDataSource.OrderByParameters);

        public ParameterCollection CloneOrderGroupsByParameters() => 
            this.CloneParameters(this._linqDataSource.OrderGroupsByParameters);

        public ParameterCollection CloneParameters(ParameterCollection original)
        {
            ParameterCollection parameters = new ParameterCollection();
            foreach (ICloneable cloneable in original)
            {
                Parameter clone = (Parameter) cloneable.Clone();
                this.RegisterClone(cloneable, clone);
                parameters.Add(clone);
            }
            return parameters;
        }

        public ParameterCollection CloneSelectParameters() => 
            this.CloneParameters(this._linqDataSource.SelectParameters);

        public ParameterCollection CloneUpdateParameters() => 
            this.CloneParameters(this._linqDataSource.UpdateParameters);

        public ParameterCollection CloneWhereParameters() => 
            this.CloneParameters(this._linqDataSource.WhereParameters);

        public LinqDataSourceState GetState() => 
            new LinqDataSourceState(this.CloneWhereParameters(), this.CloneOrderByParameters(), this.CloneGroupByParameters(), this.CloneOrderGroupsByParameters(), this.CloneSelectParameters(), this.CloneInsertParameters(), this.CloneUpdateParameters(), this.CloneDeleteParameters()) { 
                ContextTypeName = this._linqDataSource.ContextTypeName,
                TableName = this._linqDataSource.TableName,
                Where = this._linqDataSource.Where,
                OrderBy = this._linqDataSource.OrderBy,
                GroupBy = this._linqDataSource.GroupBy,
                OrderGroupsBy = this._linqDataSource.OrderGroupsBy,
                Select = this._linqDataSource.Select,
                EnableDelete = this._linqDataSource.EnableDelete,
                EnableInsert = this._linqDataSource.EnableInsert,
                EnableUpdate = this._linqDataSource.EnableUpdate,
                AutoGenerateOrderByClause = this._linqDataSource.AutoGenerateOrderByClause,
                AutoGenerateWhereClause = this._linqDataSource.AutoGenerateWhereClause
            };

        public void RegisterClone(object original, object clone)
        {
            this._designerHelper.RegisterClone(original, clone);
        }

        public void SetDeleteParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.DeleteParameters, newParams);
        }

        public void SetGroupByParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.GroupByParameters, newParams);
        }

        public void SetInsertParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.InsertParameters, newParams);
        }

        public void SetOrderByParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.OrderByParameters, newParams);
        }

        public void SetOrderGroupsByParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.OrderGroupsByParameters, newParams);
        }

        private void SetParameters(ParameterCollection original, ParameterCollection newParams)
        {
            original.Clear();
            foreach (Parameter parameter in newParams)
            {
                original.Add(parameter);
            }
        }

        private void SetProperty(string propertyName, object newValue)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this._linqDataSource)[propertyName];
            descriptor.ResetValue(this._linqDataSource);
            descriptor.SetValue(this._linqDataSource, newValue);
        }

        private void SetProperty(string propertyName, bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                this.SetProperty(propertyName, newValue);
            }
        }

        private void SetProperty(string propertyName, string oldValue, string newValue)
        {
            if (string.Compare(oldValue, newValue, StringComparison.Ordinal) != 0)
            {
                this.SetProperty(propertyName, newValue);
            }
        }

        public void SetSelectParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.SelectParameters, newParams);
        }

        public void SetState(LinqDataSourceState state)
        {
            this.SetProperty("AutoGenerateWhereClause", state.AutoGenerateWhereClause);
            this.SetProperty("AutoGenerateOrderByClause", state.AutoGenerateOrderByClause);
            this.SetProperty("ContextTypeName", this._linqDataSource.ContextTypeName, state.ContextTypeName);
            this.SetProperty("TableName", this._linqDataSource.TableName, state.TableName);
            this.SetProperty("Where", this._linqDataSource.Where, state.Where);
            this._linqDataSource.WhereParameters.Clear();
            foreach (Parameter parameter in state.WhereParameters)
            {
                this._linqDataSource.WhereParameters.Add(parameter);
            }
            this.SetProperty("OrderBy", this._linqDataSource.OrderBy, state.OrderBy);
            this._linqDataSource.OrderByParameters.Clear();
            foreach (Parameter parameter2 in state.OrderByParameters)
            {
                this._linqDataSource.OrderByParameters.Add(parameter2);
            }
            this.SetProperty("GroupBy", this._linqDataSource.GroupBy, state.GroupBy);
            this._linqDataSource.GroupByParameters.Clear();
            foreach (Parameter parameter3 in state.GroupByParameters)
            {
                this._linqDataSource.GroupByParameters.Add(parameter3);
            }
            this.SetProperty("OrderGroupsBy", this._linqDataSource.OrderGroupsBy, state.OrderGroupsBy);
            this._linqDataSource.OrderGroupsByParameters.Clear();
            foreach (Parameter parameter4 in state.OrderGroupsByParameters)
            {
                this._linqDataSource.OrderGroupsByParameters.Add(parameter4);
            }
            this.SetProperty("Select", this._linqDataSource.Select, state.Select);
            this._linqDataSource.SelectParameters.Clear();
            foreach (Parameter parameter5 in state.SelectParameters)
            {
                this._linqDataSource.SelectParameters.Add(parameter5);
            }
            this.SetProperty("EnableDelete", this._linqDataSource.EnableDelete, state.EnableDelete);
            this.SetProperty("EnableInsert", this._linqDataSource.EnableInsert, state.EnableInsert);
            this.SetProperty("EnableUpdate", this._linqDataSource.EnableUpdate, state.EnableUpdate);
        }

        public void SetUpdateParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.UpdateParameters, newParams);
        }

        public void SetWhereParameterContents(ParameterCollection newParams)
        {
            this.SetParameters(this._linqDataSource.WhereParameters, newParams);
        }

        public bool AutoGenerateOrderByClause =>
            this._linqDataSource.AutoGenerateOrderByClause;

        public bool AutoGenerateWhereClause =>
            this._linqDataSource.AutoGenerateWhereClause;

        public bool CanPage =>
            this._linqDataSource.View.CanPage;

        public bool CanSort =>
            this._linqDataSource.View.CanSort;

        public Control Component =>
            this._linqDataSource;

        public string ContextTypeName
        {
            get => 
                this._linqDataSource.ContextTypeName;
            set
            {
                this._linqDataSource.ContextTypeName = value;
            }
        }

        public bool EnableDelete
        {
            get => 
                this._linqDataSource.EnableDelete;
            set
            {
                this.SetProperty("EnableDelete", value);
            }
        }

        public bool EnableInsert
        {
            get => 
                this._linqDataSource.EnableInsert;
            set
            {
                this.SetProperty("EnableInsert", value);
            }
        }

        public bool EnableUpdate
        {
            get => 
                this._linqDataSource.EnableUpdate;
            set
            {
                this.SetProperty("EnableUpdate", value);
            }
        }

        public string GroupBy
        {
            get => 
                this._linqDataSource.GroupBy;
            set
            {
                this._linqDataSource.GroupBy = value;
            }
        }

        public string OrderBy
        {
            get => 
                this._linqDataSource.OrderBy;
            set
            {
                this._linqDataSource.OrderBy = value;
            }
        }

        public string OrderGroupsBy
        {
            get => 
                this._linqDataSource.OrderGroupsBy;
            set
            {
                this._linqDataSource.OrderGroupsBy = value;
            }
        }

        public string Select
        {
            get => 
                this._linqDataSource.Select;
            set
            {
                this._linqDataSource.Select = value;
            }
        }

        public IServiceProvider ServiceProvider =>
            this._linqDataSource.Site;

        public string TableName
        {
            get => 
                this._linqDataSource.TableName;
            set
            {
                this._linqDataSource.TableName = value;
            }
        }

        public string Where
        {
            get => 
                this._linqDataSource.Where;
            set
            {
                this._linqDataSource.Where = value;
            }
        }
    }
}

