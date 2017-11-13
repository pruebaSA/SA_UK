namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Data;
    using System.Data.Linq;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Resources.Design;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal class LinqDataSourceDesignerHelper : ILinqDataSourceDesignerHelper
    {
        private bool _forceSchemaRetrieval;
        private ILinqDataSourceWrapper _linqDataSourceWrapper;
        private LinqDataSourceDesigner _owner;
        private LinqDesignerDataSourceView _view;
        public const string DefaultViewName = "DefaultView";
        public const string DesignerStateDataSourceContextTypeNameKey = "LinqDataSourceContextTypeName";
        public const string DesignerStateDataSourceSchemaKey = "LinqDataSourceSchema";
        public const string DesignerStateDataSourceTableNameKey = "LinqDataSourceTableName";

        internal LinqDataSourceDesignerHelper()
        {
        }

        public LinqDataSourceDesignerHelper(LinqDataSourceDesigner owner)
        {
            this._owner = owner;
        }

        internal static void AddAllColumnsSelectEmpty(DataTable dataTable, List<ILinqDataSourcePropertyItem> columns)
        {
            foreach (ILinqDataSourcePropertyItem item in columns)
            {
                if (item.IsProperty)
                {
                    AddDataColumn(dataTable, item, item.Name, false);
                }
            }
        }

        internal static void AddDataColumn(DataTable table, ILinqDataSourcePropertyItem column, string columnName, bool isSelectNew)
        {
            DataColumn column2 = new DataColumn();
            table.Columns.Add(column2);
            column2.ColumnName = columnName;
            column2.DataType = column.Type;
            column2.AllowDBNull = column.IsNullable;
            column2.Unique = column.IsUnique;
            if (isSelectNew)
            {
                column2.ReadOnly = true;
                column2.AutoIncrement = false;
            }
            else
            {
                column2.ReadOnly = column.IsReadOnly;
                column2.AutoIncrement = column.IsIdentity;
                if (column.IsPrimaryKey)
                {
                    table.PrimaryKey = new List<DataColumn>(table.PrimaryKey) { column2 }.ToArray();
                }
            }
        }

        public bool CanConfigure(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return false;
            }
            ITypeDiscoveryService service = (ITypeDiscoveryService) serviceProvider.GetService(typeof(ITypeDiscoveryService));
            return (service != null);
        }

        public bool CanInsertUpdateDelete(ILinqDataSourceDesignerHelper helperHelper) => 
            helperHelper.GetView("DefaultView").CanModify;

        public bool CanRefreshSchema(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(this.ContextTypeName) || string.IsNullOrEmpty(this.TableName))
            {
                return false;
            }
            if (serviceProvider == null)
            {
                return false;
            }
            ITypeDiscoveryService service = (ITypeDiscoveryService) serviceProvider.GetService(typeof(ITypeDiscoveryService));
            return (service != null);
        }

        public ParameterCollection CloneDeleteParameters() => 
            this._linqDataSourceWrapper.CloneDeleteParameters();

        public ParameterCollection CloneGroupByParameters() => 
            this._linqDataSourceWrapper.CloneGroupByParameters();

        public ParameterCollection CloneInsertParameters() => 
            this._linqDataSourceWrapper.CloneInsertParameters();

        public ParameterCollection CloneOrderByParameters() => 
            this._linqDataSourceWrapper.CloneOrderByParameters();

        public ParameterCollection CloneOrderGroupsByParameters() => 
            this._linqDataSourceWrapper.CloneOrderGroupsByParameters();

        public ParameterCollection CloneParameters(ParameterCollection original) => 
            this._linqDataSourceWrapper.CloneParameters(original);

        public ParameterCollection CloneSelectParameters() => 
            this._linqDataSourceWrapper.CloneSelectParameters();

        public ParameterCollection CloneUpdateParameters() => 
            this._linqDataSourceWrapper.CloneUpdateParameters();

        public ParameterCollection CloneWhereParameters() => 
            this._linqDataSourceWrapper.CloneWhereParameters();

        public LinqDesignerDataSourceView CreateView()
        {
            LinqDesignerDataSourceView view = new LinqDesignerDataSourceView(this._owner);
            view.SetHelper(this);
            return view;
        }

        public ILinqDataSourceContextTypeItem GetContextType(ILinqDataSourceDesignerHelper helperHelper)
        {
            System.Type type = helperHelper.GetType(this.ContextTypeName, this.ServiceProvider);
            if (type == null)
            {
                return null;
            }
            return new LinqDataSourceContextTypeItem(type);
        }

        public List<ILinqDataSourceContextTypeItem> GetContextTypes(System.Type baseType, IServiceProvider serviceProvider)
        {
            ITypeDiscoveryService service = serviceProvider.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
            if (service == null)
            {
                return null;
            }
            ICollection types = service.GetTypes(baseType, true);
            List<ILinqDataSourceContextTypeItem> list = new List<ILinqDataSourceContextTypeItem>();
            foreach (System.Type type in types)
            {
                if ((!type.IsAbstract && !type.IsInterface) && !type.IsGenericTypeDefinition)
                {
                    list.Add(new LinqDataSourceContextTypeItem(type));
                }
            }
            list.Sort();
            return list;
        }

        internal System.Type GetItType(ILinqDataSourcePropertyItem table, bool isGrouped)
        {
            if (!isGrouped)
            {
                return table.Type;
            }
            return typeof(object);
        }

        internal ILinqDataSourcePropertyItem GetKeyColumn(List<ILinqDataSourcePropertyItem> columns, string groupBy)
        {
            foreach (ILinqDataSourcePropertyItem item in columns)
            {
                if (string.Equals(item.Name, groupBy, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        public LinqDataSourceState GetLinqDataSourceState() => 
            this._linqDataSourceWrapper.GetState();

        public virtual List<ILinqDataSourcePropertyItem> GetProperties(System.Type tableType, bool isSorted, bool includeEnumerables)
        {
            FieldInfo[] fields = tableType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<ILinqDataSourcePropertyItem> list = new List<ILinqDataSourcePropertyItem>();
            foreach (FieldInfo info in fields)
            {
                if (this.IncludeProperty(includeEnumerables, info.FieldType))
                {
                    list.Add(new LinqDataSourcePropertyItem(info));
                }
            }
            foreach (PropertyInfo info2 in properties)
            {
                if (info2.CanRead && this.IncludeProperty(includeEnumerables, info2.PropertyType))
                {
                    list.Add(new LinqDataSourcePropertyItem(info2));
                }
            }
            if (isSorted)
            {
                list.Sort();
            }
            return list;
        }

        public List<ILinqDataSourcePropertyItem> GetProperties(ILinqDataSourcePropertyItem table, bool isSorted, bool includeEnumerables) => 
            this.GetProperties(table.Type, isSorted, includeEnumerables);

        public ILinqDataSourcePropertyItem GetTable(ILinqDataSourceDesignerHelper helperHelper)
        {
            ILinqDataSourceContextTypeItem contextType = helperHelper.GetContextType(helperHelper);
            if (contextType != null)
            {
                foreach (ILinqDataSourcePropertyItem item2 in helperHelper.GetTables(contextType, false))
                {
                    if (string.Equals(this.TableName, item2.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return item2;
                    }
                }
            }
            return null;
        }

        public virtual List<ILinqDataSourcePropertyItem> GetTables(System.Type contextType, bool isSorted)
        {
            if (contextType == null)
            {
                return new List<ILinqDataSourcePropertyItem>();
            }
            System.Type type = null;
            if (typeof(DataContext).IsAssignableFrom(contextType))
            {
                type = typeof(ITable);
            }
            else
            {
                type = typeof(IEnumerable);
            }
            List<ILinqDataSourcePropertyItem> list = new List<ILinqDataSourcePropertyItem>();
            FieldInfo[] fields = contextType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            System.Type o = typeof(string);
            foreach (FieldInfo info in fields)
            {
                System.Type fieldType = info.FieldType;
                if (type.IsAssignableFrom(fieldType) && !fieldType.Equals(o))
                {
                    list.Add(new LinqDataSourceTablePropertyItem(info));
                }
            }
            foreach (PropertyInfo info2 in contextType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                System.Type propertyType = info2.PropertyType;
                if ((info2.CanRead && type.IsAssignableFrom(propertyType)) && !propertyType.Equals(o))
                {
                    list.Add(new LinqDataSourceTablePropertyItem(info2));
                }
            }
            if (isSorted)
            {
                list.Sort();
            }
            return list;
        }

        public List<ILinqDataSourcePropertyItem> GetTables(ILinqDataSourceContextTypeItem context, bool isSorted) => 
            this.GetTables(context.Type, isSorted);

        public System.Type GetType(string typeName, IServiceProvider serviceProvider)
        {
            ITypeResolutionService service = serviceProvider.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
            if ((service != null) && !string.IsNullOrEmpty(typeName))
            {
                return service.GetType(typeName, false, true);
            }
            return null;
        }

        public LinqDesignerDataSourceView GetView(string viewName)
        {
            if (!string.IsNullOrEmpty(viewName) && !string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            if (this.View == null)
            {
                this.View = this.CreateView();
            }
            return this.View;
        }

        public string[] GetViewNames() => 
            new string[] { "DefaultView" };

        private bool IncludeProperty(bool includeEnumerables, System.Type type)
        {
            if (!includeEnumerables && !typeof(string).Equals(type))
            {
                return !typeof(IEnumerable).IsAssignableFrom(type);
            }
            return true;
        }

        public bool IsDataContext(ILinqDataSourceDesignerHelper helperHelper)
        {
            ILinqDataSourceContextTypeItem contextType = helperHelper.GetContextType(helperHelper);
            if ((contextType == null) || ((contextType.Type != typeof(DataContext)) && !contextType.Type.IsSubclassOf(typeof(DataContext))))
            {
                return false;
            }
            return true;
        }

        internal static bool IsPrimitiveType(System.Type t)
        {
            System.Type underlyingType = Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
            {
                t = underlyingType;
            }
            if ((!t.IsPrimitive && (t != typeof(string))) && (t != typeof(DateTime)))
            {
                return (t == typeof(decimal));
            }
            return true;
        }

        public bool IsTableTypeTable(ILinqDataSourceDesignerHelper helperHelper) => 
            this.IsTableTypeTable(helperHelper, this.ContextTypeName, this.TableName, this.ServiceProvider);

        public bool IsTableTypeTable(ILinqDataSourceDesignerHelper helperHelper, string contextTypeName, string tableName, IServiceProvider serviceProvider)
        {
            System.Type type = this.TableType(helperHelper, contextTypeName, tableName, serviceProvider);
            return ((type?.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Table<>))) && !type.GetGenericArguments()[0].IsGenericParameter);
        }

        protected virtual object LoadFromDesignerState(string key) => 
            this._owner.LoadFromDesignerState(key);

        public DataTable LoadSchema()
        {
            if (!this.ForceSchemaRetrieval)
            {
                string a = this.LoadFromDesignerState("LinqDataSourceContextTypeName") as string;
                string str2 = this.LoadFromDesignerState("LinqDataSourceTableName") as string;
                if (!string.Equals(str2, this.TableName, StringComparison.OrdinalIgnoreCase) || !string.Equals(a, this.ContextTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }
            return (this.LoadFromDesignerState("LinqDataSourceSchema") as DataTable);
        }

        internal static DataColumn MakeAggregateColumn(System.Type dataType, bool isNullable, string columnName) => 
            new DataColumn { 
                ColumnName = columnName,
                DataType = dataType,
                ReadOnly = true,
                AllowDBNull = isNullable,
                Unique = false,
                AutoIncrement = false
            };

        public DataTable MakeDataTable(ILinqDataSourceDesignerHelper helperHelper, ILinqDataSourcePropertyItem table)
        {
            DataTable dataTable = new DataTable(table.Name) {
                Locale = CultureInfo.InvariantCulture
            };
            List<ILinqDataSourcePropertyItem> tableFields = helperHelper.GetProperties(table.Type, false, true);
            bool flag = IsPrimitiveType(table.Type);
            bool isSelectNew = false;
            if (this.Select.Trim().StartsWith("new", StringComparison.OrdinalIgnoreCase))
            {
                string str = this.Select.Trim().Substring(3).Trim();
                isSelectNew = str.StartsWith("(", StringComparison.Ordinal) && str.EndsWith(")", StringComparison.Ordinal);
            }
            bool isGrouped = !string.IsNullOrEmpty(this.GroupBy);
            LinqDataSourceSelectBuilder.ParseResult result = new LinqDataSourceSelectBuilder().CreateProjection(this._linqDataSourceWrapper.GetState(), tableFields);
            if (result.Projections == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(this.Select.Trim()))
            {
                if (flag)
                {
                    return null;
                }
                AddAllColumnsSelectEmpty(dataTable, tableFields);
                return dataTable;
            }
            if (!isSelectNew)
            {
                return null;
            }
            ILinqDataSourcePropertyItem keyColumn = this.GetKeyColumn(tableFields, this.GroupBy);
            ILinqDataSourcePropertyItem column = table;
            this.GetItType(table, isGrouped);
            foreach (ILinqDataSourceProjection projection in result.Projections)
            {
                string alias;
                if (!string.IsNullOrEmpty(projection.Alias))
                {
                    alias = projection.Alias;
                }
                else
                {
                    alias = projection.Column.Name;
                }
                if (projection.AggregateFunction == LinqDataSourceAggregateFunctions.None)
                {
                    if (projection.Column == LinqDataSourceSelectBuilder.KeyField)
                    {
                        AddDataColumn(dataTable, keyColumn, alias, isSelectNew);
                    }
                    else if (projection.Column == LinqDataSourceSelectBuilder.ItField)
                    {
                        AddDataColumn(dataTable, column, alias, isSelectNew);
                    }
                    else
                    {
                        AddDataColumn(dataTable, projection.Column, alias, isSelectNew);
                    }
                }
                else if (projection.AggregateFunction == LinqDataSourceAggregateFunctions.Count)
                {
                    dataTable.Columns.Add(MakeAggregateColumn(typeof(int), false, alias));
                }
                else
                {
                    System.Type type;
                    bool isNullable;
                    if (projection.Column == LinqDataSourceSelectBuilder.ItField)
                    {
                        type = column.Type;
                        isNullable = false;
                    }
                    else
                    {
                        type = projection.Column.Type;
                        isNullable = projection.Column.IsNullable;
                    }
                    if (projection.AggregateFunction == LinqDataSourceAggregateFunctions.Sum)
                    {
                        if (type == typeof(float))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(float), isNullable, alias));
                        }
                        else if (type == typeof(double))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(double), isNullable, alias));
                        }
                        else if (type == typeof(decimal))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(decimal), isNullable, alias));
                        }
                        else if ((type == typeof(long)) || (type == typeof(uint)))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(long), isNullable, alias));
                        }
                        else
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(int), isNullable, alias));
                        }
                    }
                    else if (projection.AggregateFunction == LinqDataSourceAggregateFunctions.Average)
                    {
                        if (type == typeof(float))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(float), isNullable, alias));
                        }
                        else if (type == typeof(decimal))
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(decimal), isNullable, alias));
                        }
                        else
                        {
                            dataTable.Columns.Add(MakeAggregateColumn(typeof(double), isNullable, alias));
                        }
                    }
                    else
                    {
                        dataTable.Columns.Add(MakeAggregateColumn(type, isNullable, alias));
                    }
                }
            }
            return dataTable;
        }

        public void PreFilterProperties(IDictionary properties, System.Type designerType)
        {
            properties["ContextTypeName"] = TypeDescriptor.CreateProperty(designerType, "ContextTypeName", typeof(string), new Attribute[0]);
            properties["TableName"] = TypeDescriptor.CreateProperty(designerType, "TableName", typeof(string), new Attribute[0]);
            properties["Where"] = TypeDescriptor.CreateProperty(designerType, "Where", typeof(string), new Attribute[0]);
            properties["OrderBy"] = TypeDescriptor.CreateProperty(designerType, "OrderBy", typeof(string), new Attribute[0]);
            properties["GroupBy"] = TypeDescriptor.CreateProperty(designerType, "GroupBy", typeof(string), new Attribute[0]);
            properties["OrderGroupsBy"] = TypeDescriptor.CreateProperty(designerType, "OrderGroupsBy", typeof(string), new Attribute[0]);
            properties["Select"] = TypeDescriptor.CreateProperty(designerType, "Select", typeof(string), new Attribute[0]);
            properties["Insert"] = TypeDescriptor.CreateProperty(designerType, "Insert", typeof(string), new Attribute[0]);
            properties["Update"] = TypeDescriptor.CreateProperty(designerType, "Update", typeof(string), new Attribute[0]);
            properties["Delete"] = TypeDescriptor.CreateProperty(designerType, "Delete", typeof(string), new Attribute[0]);
        }

        public void RefreshSchema(ILinqDataSourceDesignerHelper helperHelper, bool preferSilent)
        {
            try
            {
                this._owner.SuppressDataSourceEvents();
                Cursor current = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (helperHelper.GetContextType(helperHelper) == null)
                    {
                        if (!preferSilent)
                        {
                            UIServiceHelper.ShowError(this.ServiceProvider, string.Format(CultureInfo.InvariantCulture, AtlasWebDesign.LinqDataSourceDesigner_CannotGetType, new object[] { this.ContextTypeName }));
                        }
                    }
                    else
                    {
                        ILinqDataSourcePropertyItem table = helperHelper.GetTable(helperHelper);
                        if (table == null)
                        {
                            if (!preferSilent)
                            {
                                UIServiceHelper.ShowError(this.ServiceProvider, string.Format(CultureInfo.InvariantCulture, AtlasWebDesign.LinqDataSourceDesigner_CannotGetType, new object[] { this.TableName }));
                            }
                        }
                        else
                        {
                            DesignerDataSourceView view = helperHelper.GetView("DefaultView");
                            IDataSourceViewSchema schema = view.Schema;
                            bool flag = false;
                            if (schema == null)
                            {
                                this.ForceSchemaRetrieval = true;
                                schema = view.Schema;
                                this.ForceSchemaRetrieval = false;
                                flag = true;
                            }
                            helperHelper.SaveSchema(helperHelper.GetContextType(helperHelper).Type.FullName, table.Name, helperHelper.MakeDataTable(helperHelper, table));
                            IDataSourceViewSchema schema2 = view.Schema;
                            if (flag && this._owner.InternalViewSchemasEquivalent(schema, schema2))
                            {
                                this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                            }
                            else if (!this._owner.InternalViewSchemasEquivalent(schema, schema2))
                            {
                                this._owner.FireOnSchemaRefreshed(EventArgs.Empty);
                            }
                        }
                    }
                }
                finally
                {
                    Cursor.Current = current;
                }
            }
            finally
            {
                this._owner.ResumeDataSourceEvents();
            }
        }

        public void RegisterClone(object original, object clone)
        {
            this._owner.RegisterClone(original, clone);
        }

        public void SaveSchema(string contextTypeName, string tableName, DataTable schema)
        {
            this.SaveToDesignerState("LinqDataSourceSchema", schema);
            this.SaveToDesignerState("LinqDataSourceContextTypeName", contextTypeName);
            this.SaveToDesignerState("LinqDataSourceTableName", tableName);
        }

        protected virtual void SaveToDesignerState(string key, object value)
        {
            this._owner.SaveDesignerState(key, value);
        }

        public void SetDeleteParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetDeleteParameterContents(newParams);
        }

        public void SetGroupByParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetGroupByParameterContents(newParams);
        }

        public void SetInsertParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetInsertParameterContents(newParams);
        }

        public void SetLinqDataSourceState(LinqDataSourceState state)
        {
            this._linqDataSourceWrapper.SetState(state);
        }

        public void SetOrderByParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetOrderByParameterContents(newParams);
        }

        public void SetOrderGroupsByParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetOrderGroupsByParameterContents(newParams);
        }

        public void SetSelectParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetSelectParameterContents(newParams);
        }

        public void SetUpdateParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetUpdateParameterContents(newParams);
        }

        public void SetWhereParameterContents(ParameterCollection newParams)
        {
            this._linqDataSourceWrapper.SetWhereParameterContents(newParams);
        }

        public void SetWrapper(ILinqDataSourceWrapper wrapper)
        {
            this._linqDataSourceWrapper = wrapper;
        }

        public System.Type TableType(ILinqDataSourceDesignerHelper helperHelper, string contextTypeName, string tableName, IServiceProvider serviceProvider)
        {
            System.Type type = helperHelper.GetType(contextTypeName, serviceProvider);
            if (type != null)
            {
                PropertyInfo property = type.GetProperty(tableName);
                if ((property != null) && property.CanRead)
                {
                    return property.PropertyType;
                }
                FieldInfo field = type.GetField(tableName);
                if (field != null)
                {
                    return field.FieldType;
                }
            }
            return null;
        }

        public bool AutoGenerateOrderByClause =>
            this._linqDataSourceWrapper.AutoGenerateOrderByClause;

        public bool AutoGenerateWhereClause =>
            this._linqDataSourceWrapper.AutoGenerateWhereClause;

        public bool CanPage =>
            this._linqDataSourceWrapper.CanPage;

        public bool CanSort =>
            this._linqDataSourceWrapper.CanSort;

        public string ContextTypeName
        {
            get => 
                this._linqDataSourceWrapper.ContextTypeName;
            set
            {
                if (value != this.ContextTypeName)
                {
                    this._linqDataSourceWrapper.ContextTypeName = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public bool EnableDelete
        {
            get => 
                this._linqDataSourceWrapper.EnableDelete;
            set
            {
                if (value != this.EnableDelete)
                {
                    this._linqDataSourceWrapper.EnableDelete = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public bool EnableInsert
        {
            get => 
                this._linqDataSourceWrapper.EnableInsert;
            set
            {
                if (value != this.EnableInsert)
                {
                    this._linqDataSourceWrapper.EnableInsert = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public bool EnableUpdate
        {
            get => 
                this._linqDataSourceWrapper.EnableUpdate;
            set
            {
                if (value != this.EnableUpdate)
                {
                    this._linqDataSourceWrapper.EnableUpdate = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        internal bool ForceSchemaRetrieval
        {
            get => 
                this._forceSchemaRetrieval;
            set
            {
                this._forceSchemaRetrieval = value;
            }
        }

        public string GroupBy
        {
            get => 
                this._linqDataSourceWrapper.GroupBy;
            set
            {
                if (!string.Equals(this.GroupBy, value, StringComparison.OrdinalIgnoreCase))
                {
                    this._linqDataSourceWrapper.GroupBy = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public string OrderBy
        {
            get => 
                this._linqDataSourceWrapper.OrderBy;
            set
            {
                if (!string.Equals(this.OrderBy, value, StringComparison.OrdinalIgnoreCase))
                {
                    this._linqDataSourceWrapper.OrderBy = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public string OrderGroupsBy
        {
            get => 
                this._linqDataSourceWrapper.OrderGroupsBy;
            set
            {
                if (!string.Equals(this.OrderGroupsBy, value, StringComparison.OrdinalIgnoreCase))
                {
                    this._linqDataSourceWrapper.OrderGroupsBy = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public string Select
        {
            get => 
                this._linqDataSourceWrapper.Select;
            set
            {
                if (!string.Equals(this.Select, value, StringComparison.OrdinalIgnoreCase))
                {
                    this._linqDataSourceWrapper.Select = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        public IServiceProvider ServiceProvider =>
            this._linqDataSourceWrapper.ServiceProvider;

        public string TableName
        {
            get => 
                this._linqDataSourceWrapper.TableName;
            set
            {
                if (value != this.TableName)
                {
                    this._linqDataSourceWrapper.TableName = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        internal LinqDesignerDataSourceView View
        {
            get => 
                this._view;
            set
            {
                this._view = value;
            }
        }

        public string Where
        {
            get => 
                this._linqDataSourceWrapper.Where;
            set
            {
                if (!string.Equals(this.Where, value, StringComparison.OrdinalIgnoreCase))
                {
                    this._linqDataSourceWrapper.Where = value;
                    this._owner.FireOnDataSourceChanged(EventArgs.Empty);
                }
            }
        }
    }
}

