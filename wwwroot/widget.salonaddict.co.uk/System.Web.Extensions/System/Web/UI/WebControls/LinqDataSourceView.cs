namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.DynamicData;
    using System.Web.Resources;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceView : DataSourceView, IStateManager
    {
        private bool _autoGenerateOrderByClause;
        private static readonly Regex _autoGenerateOrderByRegex = new Regex(_identifierPattern + @"(\s+(asc|ascending|desc|descending))?\s*$", RegexOptions.IgnoreCase);
        private bool _autoGenerateWhereClause;
        private bool _autoPage;
        private bool _autoSort;
        private HttpContext _context;
        private Type _contextType;
        private string _contextTypeName;
        private ParameterCollection _deleteParameters;
        private IDynamicQueryable _dynamicQueryable;
        private bool _enableDelete;
        private bool _enableInsert;
        private bool _enableObjectTracking;
        private bool _enableUpdate;
        private string _groupBy;
        private ParameterCollection _groupByParameters;
        private static readonly string _identifierPattern = @"^\s*[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}_][\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}\p{Pc}\p{Mn}\p{Mc}\p{Cf}_]*";
        private static readonly Regex _identifierRegex = new Regex(_identifierPattern + @"\s*$");
        private ParameterCollection _insertParameters;
        private ILinqToSql _linqToSql;
        private string _orderBy;
        private ParameterCollection _orderByParameters;
        private string _orderGroupsBy;
        private ParameterCollection _orderGroupsByParameters;
        private Hashtable _originalValues;
        private LinqDataSource _owner;
        private bool _reuseSelectContext;
        private List<LinqDataSourceContextData> _selectContexts;
        private string _selectNew;
        private ParameterCollection _selectNewParameters;
        private bool _storeOriginalValuesInViewState;
        private string _tableName;
        private bool _tracking;
        private ParameterCollection _updateParameters;
        private string _where;
        private ParameterCollection _whereParameters;
        private static readonly object EventContextCreated = new object();
        private static readonly object EventContextCreating = new object();
        private static readonly object EventContextDisposing = new object();
        private static readonly object EventDeleted = new object();
        private static readonly object EventDeleting = new object();
        private static readonly object EventException = new object();
        private static readonly object EventInserted = new object();
        private static readonly object EventInserting = new object();
        private static readonly object EventSelected = new object();
        private static readonly object EventSelecting = new object();
        private static readonly object EventUpdated = new object();
        private static readonly object EventUpdating = new object();

        public event EventHandler<LinqDataSourceStatusEventArgs> ContextCreated
        {
            add
            {
                base.Events.AddHandler(EventContextCreated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventContextCreated, value);
            }
        }

        public event EventHandler<LinqDataSourceContextEventArgs> ContextCreating
        {
            add
            {
                base.Events.AddHandler(EventContextCreating, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventContextCreating, value);
            }
        }

        public event EventHandler<LinqDataSourceDisposeEventArgs> ContextDisposing
        {
            add
            {
                base.Events.AddHandler(EventContextDisposing, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventContextDisposing, value);
            }
        }

        public event EventHandler<LinqDataSourceStatusEventArgs> Deleted
        {
            add
            {
                base.Events.AddHandler(EventDeleted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDeleted, value);
            }
        }

        public event EventHandler<LinqDataSourceDeleteEventArgs> Deleting
        {
            add
            {
                base.Events.AddHandler(EventDeleting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDeleting, value);
            }
        }

        internal event EventHandler<DynamicValidatorEventArgs> Exception
        {
            add
            {
                base.Events.AddHandler(EventException, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventException, value);
            }
        }

        public event EventHandler<LinqDataSourceStatusEventArgs> Inserted
        {
            add
            {
                base.Events.AddHandler(EventInserted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventInserted, value);
            }
        }

        public event EventHandler<LinqDataSourceInsertEventArgs> Inserting
        {
            add
            {
                base.Events.AddHandler(EventInserting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventInserting, value);
            }
        }

        public event EventHandler<LinqDataSourceStatusEventArgs> Selected
        {
            add
            {
                base.Events.AddHandler(EventSelected, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSelected, value);
            }
        }

        public event EventHandler<LinqDataSourceSelectEventArgs> Selecting
        {
            add
            {
                base.Events.AddHandler(EventSelecting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSelecting, value);
            }
        }

        public event EventHandler<LinqDataSourceStatusEventArgs> Updated
        {
            add
            {
                base.Events.AddHandler(EventUpdated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventUpdated, value);
            }
        }

        public event EventHandler<LinqDataSourceUpdateEventArgs> Updating
        {
            add
            {
                base.Events.AddHandler(EventUpdating, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventUpdating, value);
            }
        }

        public LinqDataSourceView(LinqDataSource owner, string name, HttpContext context) : this(owner, name, context, new DynamicQueryableWrapper(), new LinqToSqlWrapper())
        {
        }

        internal LinqDataSourceView(LinqDataSource owner, string name, HttpContext context, IDynamicQueryable dynamicQueryable, ILinqToSql linqToSql) : base(owner, name)
        {
            this._autoPage = true;
            this._autoSort = true;
            this._enableObjectTracking = true;
            this._storeOriginalValuesInViewState = true;
            this._owner = owner;
            this._context = context;
            this._dynamicQueryable = dynamicQueryable;
            this._linqToSql = linqToSql;
        }

        private static IQueryable AsQueryable(object o)
        {
            IQueryable queryable = o as IQueryable;
            if (queryable != null)
            {
                return queryable;
            }
            string str = o as string;
            if (str != null)
            {
                return new string[] { str }.AsQueryable<string>();
            }
            IEnumerable source = o as IEnumerable;
            if (source != null)
            {
                if (LinqDataSourceHelper.FindGenericEnumerableType(o.GetType()) != null)
                {
                    return source.AsQueryable();
                }
                List<object> list = new List<object>();
                foreach (object obj2 in source)
                {
                    list.Add(obj2);
                }
                return list.AsQueryable<object>();
            }
            IList list2 = (IList) CreateObjectInstance(typeof(List<>).MakeGenericType(new Type[] { o.GetType() }));
            list2.Add(o);
            return list2.AsQueryable();
        }

        private object BuildDataObject(Type dataObjectType, IDictionary inputParameters)
        {
            object component = CreateObjectInstance(dataObjectType);
            Dictionary<string, System.Exception> innerExceptions = null;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            foreach (DictionaryEntry entry in inputParameters)
            {
                string name = (entry.Key == null) ? string.Empty : entry.Key.ToString();
                PropertyDescriptor descriptor = properties.Find(name, true);
                if ((descriptor != null) && !descriptor.IsReadOnly)
                {
                    try
                    {
                        object obj3 = BuildObjectValue(entry.Value, descriptor.PropertyType, name);
                        descriptor.SetValue(component, obj3);
                    }
                    catch (System.Exception exception)
                    {
                        if (innerExceptions == null)
                        {
                            innerExceptions = new Dictionary<string, System.Exception>(StringComparer.OrdinalIgnoreCase);
                        }
                        innerExceptions[descriptor.Name] = exception;
                    }
                }
            }
            if (innerExceptions != null)
            {
                throw new LinqDataSourceValidationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ValidationFailed, new object[] { dataObjectType, innerExceptions.Values.First<System.Exception>().Message }), innerExceptions);
            }
            return component;
        }

        private LinqDataSourceEditData BuildDeleteDataObject(object table, IDictionary keys, IDictionary oldValues)
        {
            Type dataObjectType = this.GetDataObjectType(table.GetType());
            IDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            IDictionary originalValues = this.GetOriginalValues(keys);
            ParameterCollection deleteParameters = this.DeleteParameters;
            this.MergeDictionaries(dataObjectType, deleteParameters, keys, destination);
            this.MergeDictionaries(dataObjectType, deleteParameters, oldValues, destination);
            if (originalValues != null)
            {
                this.MergeDictionaries(dataObjectType, deleteParameters, originalValues, destination);
            }
            return new LinqDataSourceEditData { OriginalDataObject = this.BuildDataObject(dataObjectType, destination) };
        }

        private LinqDataSourceEditData BuildInsertDataObject(object table, IDictionary values)
        {
            Type dataObjectType = this.GetDataObjectType(table.GetType());
            IDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            this.MergeDictionaries(dataObjectType, this.InsertParameters, this.InsertParameters.GetValues(this._context, this._owner), destination);
            this.MergeDictionaries(dataObjectType, this.InsertParameters, values, destination);
            return new LinqDataSourceEditData { NewDataObject = this.BuildDataObject(dataObjectType, destination) };
        }

        internal static object BuildObjectValue(object value, Type destinationType, string paramName)
        {
            if ((value != null) && !destinationType.IsInstanceOfType(value))
            {
                Type elementType = destinationType;
                bool flag = false;
                if (destinationType.IsGenericType && (destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    elementType = destinationType.GetGenericArguments()[0];
                    flag = true;
                }
                else if (destinationType.IsByRef)
                {
                    elementType = destinationType.GetElementType();
                }
                value = ConvertType(value, elementType, paramName);
                if (flag)
                {
                    Type type = value.GetType();
                    if (elementType != type)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_CannotConvertType, new object[] { paramName, type.FullName, string.Format(CultureInfo.InvariantCulture, "Nullable<{0}>", new object[] { destinationType.GetGenericArguments()[0].FullName }) }));
                    }
                }
            }
            return value;
        }

        private LinqDataSourceEditData BuildUpdateDataObjects(object table, IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            Type dataObjectType = this.GetDataObjectType(table.GetType());
            IDictionary destinationCopy = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            IDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            IDictionary originalValues = this.GetOriginalValues(keys);
            ParameterCollection updateParameters = this.UpdateParameters;
            MergeDictionaries(dataObjectType, updateParameters, oldValues, destination, destinationCopy);
            MergeDictionaries(dataObjectType, updateParameters, keys, destination, destinationCopy);
            if (originalValues != null)
            {
                MergeDictionaries(dataObjectType, updateParameters, originalValues, destination, destinationCopy);
            }
            this.MergeDictionaries(dataObjectType, updateParameters, values, destinationCopy);
            return new LinqDataSourceEditData { 
                NewDataObject = this.BuildDataObject(dataObjectType, destinationCopy),
                OriginalDataObject = this.BuildDataObject(dataObjectType, destination)
            };
        }

        private static object ConvertType(object value, Type type, string paramName)
        {
            string text = value as string;
            if (text != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter == null)
                {
                    return value;
                }
                try
                {
                    value = converter.ConvertFromString(text);
                }
                catch (NotSupportedException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_CannotConvertType, new object[] { paramName, typeof(string).FullName, type.FullName }));
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_CannotConvertType, new object[] { paramName, typeof(string).FullName, type.FullName }));
                }
            }
            return value;
        }

        protected virtual object CreateContext(Type contextType) => 
            CreateObjectInstance(contextType);

        private LinqDataSourceContextData CreateContextAndTable(DataSourceOperation operation)
        {
            LinqDataSourceContextData data = null;
            bool flag = false;
            try
            {
                LinqDataSourceContextEventArgs e = new LinqDataSourceContextEventArgs(operation);
                this.OnContextCreating(e);
                data = new LinqDataSourceContextData(e.ObjectInstance);
                Type contextType = null;
                MemberInfo member = null;
                if (data.Context == null)
                {
                    contextType = this.ContextType;
                    member = this.GetTableMemberInfo(contextType);
                    if (member != null)
                    {
                        if (MemberIsStatic(member))
                        {
                            if (operation != DataSourceOperation.Select)
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_TableCannotBeStatic, new object[] { this.TableName, contextType.Name, this._owner.ID }));
                            }
                        }
                        else
                        {
                            data.Context = this.CreateContext(contextType);
                            data.IsNewContext = true;
                        }
                    }
                }
                else
                {
                    member = this.GetTableMemberInfo(data.Context.GetType());
                }
                if (member != null)
                {
                    FieldInfo info2 = member as FieldInfo;
                    if (info2 != null)
                    {
                        data.Table = info2.GetValue(data.Context);
                    }
                    PropertyInfo info3 = member as PropertyInfo;
                    if (info3 != null)
                    {
                        data.Table = info3.GetValue(data.Context, null);
                    }
                }
                if (data.Table == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_TableNameNotFound, new object[] { this.TableName, contextType.Name, this._owner.ID }));
                }
            }
            catch (System.Exception exception)
            {
                flag = true;
                LinqDataSourceStatusEventArgs args2 = new LinqDataSourceStatusEventArgs(exception);
                this.OnContextCreated(args2);
                this.OnException(new DynamicValidatorEventArgs(exception, DynamicDataSourceOperation.ContextCreate));
                if (!args2.ExceptionHandled)
                {
                    throw;
                }
            }
            finally
            {
                if (!flag)
                {
                    object context = data?.Context;
                    LinqDataSourceStatusEventArgs args3 = new LinqDataSourceStatusEventArgs(context);
                    this.OnContextCreated(args3);
                }
            }
            return data;
        }

        private LinqDataSourceContextData CreateContextAndTableForEdit(DataSourceOperation operation)
        {
            LinqDataSourceContextData data = this.CreateContextAndTable(operation);
            if (data != null)
            {
                if (data.Context == null)
                {
                    return null;
                }
                if (data.Table == null)
                {
                    this.ReleaseContext(data.Context);
                    return null;
                }
                this.ValidateContextType(data.Context.GetType(), false);
                this.ValidateTableType(data.Table.GetType(), false);
            }
            return data;
        }

        private LinqDataSourceContextData CreateContextAndTableForSelect()
        {
            if (this._selectContexts == null)
            {
                this._selectContexts = new List<LinqDataSourceContextData>();
            }
            else if (this._reuseSelectContext && (this._selectContexts.Count > 0))
            {
                return this._selectContexts[this._selectContexts.Count - 1];
            }
            LinqDataSourceContextData item = this.CreateContextAndTable(DataSourceOperation.Select);
            if (item != null)
            {
                if (item.Context != null)
                {
                    this.ValidateContextType(item.Context.GetType(), true);
                }
                if (item.Table != null)
                {
                    this.ValidateTableType(item.Table.GetType(), true);
                }
                this._selectContexts.Add(item);
                DataContext context = item.Context as DataContext;
                if ((context != null) && item.IsNewContext)
                {
                    context.ObjectTrackingEnabled = this.EnableObjectTracking;
                }
                this._reuseSelectContext = (context == null) || !this.EnableObjectTracking;
            }
            return item;
        }

        private static object CreateObjectInstance(Type type) => 
            HttpRuntime.FastCreatePublicInstance(type);

        public int Delete(IDictionary keys, IDictionary oldValues) => 
            this.ExecuteDelete(keys, oldValues);

        protected virtual void DeleteDataObject(object dataContext, object table, object oldDataObject)
        {
            this._linqToSql.Attach((ITable) table, oldDataObject);
            this._linqToSql.Remove((ITable) table, oldDataObject);
            this._linqToSql.SubmitChanges((DataContext) dataContext);
        }

        private IDictionary<string, object> EscapeParameterKeys(IDictionary<string, object> parameters)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(parameters.Count, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ParametersMustBeNamed, new object[] { this._owner.ID }));
                }
                this.ValidateParameterName(key);
                dictionary.Add('@' + key, pair.Value);
            }
            return dictionary;
        }

        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            this.ValidateDeleteSupported(keys, oldValues);
            LinqDataSourceContextData data = null;
            try
            {
                data = this.CreateContextAndTableForEdit(DataSourceOperation.Delete);
                if (data != null)
                {
                    LinqDataSourceEditData data2 = null;
                    LinqDataSourceDeleteEventArgs e = null;
                    try
                    {
                        data2 = this.BuildDeleteDataObject(data.Table, keys, oldValues);
                    }
                    catch (LinqDataSourceValidationException exception)
                    {
                        e = new LinqDataSourceDeleteEventArgs(exception);
                        this.OnDeleting(e);
                        this.OnException(new DynamicValidatorEventArgs(exception, DynamicDataSourceOperation.Delete));
                        if (!e.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    e = new LinqDataSourceDeleteEventArgs(data2.OriginalDataObject);
                    this.OnDeleting(e);
                    if (e.Cancel)
                    {
                        return -1;
                    }
                    LinqDataSourceStatusEventArgs args2 = null;
                    try
                    {
                        this.DeleteDataObject(data.Context, data.Table, e.OriginalObject);
                    }
                    catch (System.Exception exception2)
                    {
                        args2 = new LinqDataSourceStatusEventArgs(exception2);
                        this.OnDeleted(args2);
                        this.OnException(new DynamicValidatorEventArgs(exception2, DynamicDataSourceOperation.Delete));
                        if (!args2.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    args2 = new LinqDataSourceStatusEventArgs(e.OriginalObject);
                    this.OnDeleted(args2);
                }
            }
            finally
            {
                if (data != null)
                {
                    this.ReleaseContext(data.Context);
                }
            }
            return 1;
        }

        protected override int ExecuteInsert(IDictionary values)
        {
            this.ValidateInsertSupported(values);
            LinqDataSourceContextData data = null;
            try
            {
                data = this.CreateContextAndTableForEdit(DataSourceOperation.Insert);
                if (data != null)
                {
                    LinqDataSourceEditData data2 = null;
                    LinqDataSourceInsertEventArgs e = null;
                    try
                    {
                        data2 = this.BuildInsertDataObject(data.Table, values);
                    }
                    catch (LinqDataSourceValidationException exception)
                    {
                        e = new LinqDataSourceInsertEventArgs(exception);
                        this.OnInserting(e);
                        this.OnException(new DynamicValidatorEventArgs(exception, DynamicDataSourceOperation.Insert));
                        if (!e.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    e = new LinqDataSourceInsertEventArgs(data2.NewDataObject);
                    this.OnInserting(e);
                    if (e.Cancel)
                    {
                        return -1;
                    }
                    LinqDataSourceStatusEventArgs args2 = null;
                    try
                    {
                        this.InsertDataObject(data.Context, data.Table, e.NewObject);
                    }
                    catch (System.Exception exception2)
                    {
                        args2 = new LinqDataSourceStatusEventArgs(exception2);
                        this.OnInserted(args2);
                        this.OnException(new DynamicValidatorEventArgs(exception2, DynamicDataSourceOperation.Insert));
                        if (!args2.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    args2 = new LinqDataSourceStatusEventArgs(e.NewObject);
                    this.OnInserted(args2);
                }
            }
            finally
            {
                if (data != null)
                {
                    this.ReleaseContext(data.Context);
                }
            }
            return 1;
        }

        protected internal override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            this._originalValues = null;
            IDictionary<string, object> parameterValues = this.GetParameterValues(this.WhereParameters);
            IOrderedDictionary orderedParameterValues = this.GetOrderedParameterValues(this.OrderByParameters);
            IDictionary<string, object> groupByParameters = this.GetParameterValues(this.GroupByParameters);
            IDictionary<string, object> orderGroupsByParameters = this.GetParameterValues(this.OrderGroupsByParameters);
            IDictionary<string, object> selectParameters = this.GetParameterValues(this.SelectNewParameters);
            LinqDataSourceSelectEventArgs e = new LinqDataSourceSelectEventArgs(arguments, parameterValues, orderedParameterValues, groupByParameters, orderGroupsByParameters, selectParameters);
            this.OnSelecting(e);
            if (e.Cancel)
            {
                return null;
            }
            object result = e.Result;
            object table = result;
            bool storeOriginalValues = ((this.StoreOriginalValuesInViewState && (this.CanDelete || this.CanUpdate)) && string.IsNullOrEmpty(this.GroupBy)) && string.IsNullOrEmpty(this.SelectNew);
            if (result == null)
            {
                LinqDataSourceContextData data = this.CreateContextAndTableForSelect();
                if (data != null)
                {
                    result = data.Table;
                    table = data.Table;
                }
            }
            else if (!(table is ITable) && storeOriginalValues)
            {
                LinqDataSourceContextData data2 = this.CreateContextAndTableForSelect();
                if (data2 != null)
                {
                    table = data2.Table;
                }
            }
            return this.ExecuteSelectQuery(e, result, table, storeOriginalValues);
        }

        private IQueryable ExecuteSelectAutoSortAndPage(IQueryable source, DataSourceSelectArguments arguments)
        {
            IQueryable queryable = source;
            string sortExpression = arguments.SortExpression;
            if (this.AutoSort && !string.IsNullOrEmpty(sortExpression))
            {
                queryable = this._dynamicQueryable.OrderBy(queryable, sortExpression, new object[0]);
            }
            if (this.AutoPage)
            {
                if (arguments.RetrieveTotalRowCount)
                {
                    arguments.TotalRowCount = this._dynamicQueryable.Count(queryable);
                }
                if ((arguments.MaximumRows > 0) && (arguments.StartRowIndex >= 0))
                {
                    queryable = this._dynamicQueryable.Skip(queryable, arguments.StartRowIndex);
                    queryable = this._dynamicQueryable.Take(queryable, arguments.MaximumRows);
                }
                return queryable;
            }
            if (arguments.RetrieveTotalRowCount && (arguments.TotalRowCount == -1))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_PagingNotHandled, new object[] { this._owner.ID }));
            }
            return queryable;
        }

        private IQueryable ExecuteSelectExpressions(IQueryable source, IDictionary<string, object> whereValues, IOrderedDictionary orderByOrderedValues, IDictionary<string, object> groupByValues, IDictionary<string, object> orderGroupsByValues, IDictionary<string, object> selectNewValues)
        {
            IQueryable queryable = source;
            string str = this.Where;
            if (this.AutoGenerateWhereClause)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_WhereAlreadySpecified, new object[] { this._owner.ID }));
                }
                LinqDataSourceAutoGeneratedWhere where = this.GenerateWhereClauseAndEscapeParameterKeys(whereValues);
                if (!string.IsNullOrEmpty(where.Where))
                {
                    queryable = this._dynamicQueryable.Where(queryable, where.Where, new object[] { where.WhereParameters });
                }
            }
            else if (!string.IsNullOrEmpty(str))
            {
                queryable = this._dynamicQueryable.Where(queryable, str, new object[] { this.EscapeParameterKeys(whereValues) });
            }
            string orderBy = this.OrderBy;
            IDictionary<string, object> parameters = null;
            if (this.AutoGenerateOrderByClause)
            {
                if (!string.IsNullOrEmpty(orderBy))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_OrderByAlreadySpecified, new object[] { this._owner.ID }));
                }
                orderBy = this.GenerateOrderByClause(orderByOrderedValues);
                parameters = new Dictionary<string, object>(0);
            }
            else
            {
                parameters = this.GetParameterValues(orderByOrderedValues);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                queryable = this._dynamicQueryable.OrderBy(queryable, orderBy, new object[] { this.EscapeParameterKeys(parameters) });
            }
            string groupBy = this.GroupBy;
            if (string.IsNullOrEmpty(groupBy))
            {
                if (!string.IsNullOrEmpty(this.OrderGroupsBy))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_OrderGroupsByRequiresGroupBy, new object[] { this._owner.ID }));
                }
            }
            else
            {
                queryable = this._dynamicQueryable.GroupBy(queryable, groupBy, "it", new object[] { this.EscapeParameterKeys(groupByValues) });
                string orderGroupsBy = this.OrderGroupsBy;
                if (!string.IsNullOrEmpty(orderGroupsBy))
                {
                    queryable = this._dynamicQueryable.OrderBy(queryable, orderGroupsBy, new object[] { this.EscapeParameterKeys(orderGroupsByValues) });
                }
            }
            string selectNew = this.SelectNew;
            if (!string.IsNullOrEmpty(selectNew))
            {
                queryable = this._dynamicQueryable.Select(queryable, selectNew, new object[] { this.EscapeParameterKeys(selectNewValues) });
            }
            return queryable;
        }

        private IEnumerable ExecuteSelectQuery(LinqDataSourceSelectEventArgs selectEventArgs, object selectResult, object table, bool storeOriginalValues)
        {
            IList result = null;
            if (selectResult != null)
            {
                try
                {
                    IQueryable source = AsQueryable(selectResult);
                    source = this.ExecuteSelectExpressions(source, selectEventArgs.WhereParameters, selectEventArgs.OrderByParameters, selectEventArgs.GroupByParameters, selectEventArgs.OrderGroupsByParameters, selectEventArgs.SelectParameters);
                    source = this.ExecuteSelectAutoSortAndPage(source, selectEventArgs.Arguments);
                    Type dataObjectType = this.GetDataObjectType(source.GetType());
                    result = this.ToList(source, dataObjectType);
                    if (storeOriginalValues)
                    {
                        ITable table2 = table as ITable;
                        if ((table2 != null) && dataObjectType.Equals(this.GetDataObjectType(table2.GetType())))
                        {
                            this.StoreOriginalValues(table2, dataObjectType, result);
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    result = null;
                    LinqDataSourceStatusEventArgs e = new LinqDataSourceStatusEventArgs(exception);
                    this.OnSelected(e);
                    this.OnException(new DynamicValidatorEventArgs(exception, DynamicDataSourceOperation.Select));
                    if (!e.ExceptionHandled)
                    {
                        throw;
                    }
                }
                finally
                {
                    if (result != null)
                    {
                        int totalRowCount = -1;
                        if (selectEventArgs.Arguments.RetrieveTotalRowCount)
                        {
                            totalRowCount = selectEventArgs.Arguments.TotalRowCount;
                        }
                        else if (!this.AutoPage)
                        {
                            totalRowCount = result.Count;
                        }
                        LinqDataSourceStatusEventArgs args2 = new LinqDataSourceStatusEventArgs(result, totalRowCount);
                        this.OnSelected(args2);
                    }
                }
            }
            return result;
        }

        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            this.ValidateUpdateSupported(keys, values, oldValues);
            LinqDataSourceContextData data = null;
            try
            {
                data = this.CreateContextAndTableForEdit(DataSourceOperation.Update);
                if (data != null)
                {
                    LinqDataSourceEditData data2 = null;
                    LinqDataSourceUpdateEventArgs e = null;
                    try
                    {
                        data2 = this.BuildUpdateDataObjects(data.Table, keys, values, oldValues);
                    }
                    catch (LinqDataSourceValidationException exception)
                    {
                        e = new LinqDataSourceUpdateEventArgs(exception);
                        this.OnUpdating(e);
                        this.OnException(new DynamicValidatorEventArgs(exception, DynamicDataSourceOperation.Update));
                        if (!e.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    e = new LinqDataSourceUpdateEventArgs(data2.OriginalDataObject, data2.NewDataObject);
                    this.OnUpdating(e);
                    if (e.Cancel)
                    {
                        return -1;
                    }
                    LinqDataSourceStatusEventArgs args2 = null;
                    try
                    {
                        this.UpdateDataObject(data.Context, data.Table, e.OriginalObject, e.NewObject);
                    }
                    catch (System.Exception exception2)
                    {
                        this.ResetDataObject(data.Table, e.OriginalObject);
                        args2 = new LinqDataSourceStatusEventArgs(exception2);
                        this.OnUpdated(args2);
                        this.OnException(new DynamicValidatorEventArgs(exception2, DynamicDataSourceOperation.Update));
                        if (!args2.ExceptionHandled)
                        {
                            throw;
                        }
                        return -1;
                    }
                    args2 = new LinqDataSourceStatusEventArgs(e.NewObject);
                    this.OnUpdated(args2);
                }
            }
            finally
            {
                if (data != null)
                {
                    this.ReleaseContext(data.Context);
                }
            }
            return 1;
        }

        private string GenerateOrderByClause(IOrderedDictionary orderByParameters)
        {
            if ((orderByParameters == null) || (orderByParameters.Count <= 0))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (DictionaryEntry entry in orderByParameters)
            {
                string str = (string) entry.Value;
                if (!string.IsNullOrEmpty(str))
                {
                    string key = (string) entry.Key;
                    this.ValidateOrderByParameter(key, str);
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(str);
                }
            }
            return builder.ToString();
        }

        private LinqDataSourceAutoGeneratedWhere GenerateWhereClauseAndEscapeParameterKeys(IDictionary<string, object> whereParameters)
        {
            if ((whereParameters == null) || (whereParameters.Count <= 0))
            {
                return new LinqDataSourceAutoGeneratedWhere(string.Empty, null);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>(whereParameters.Count);
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (KeyValuePair<string, object> pair in whereParameters)
            {
                string key = pair.Key;
                string str2 = pair.Value?.ToString();
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(str2))
                {
                    string str3 = "@p" + num++;
                    if (builder.Length > 0)
                    {
                        builder.Append(" AND ");
                    }
                    builder.Append(key);
                    builder.Append(" == ");
                    builder.Append(str3);
                    dictionary.Add(str3, pair.Value);
                }
            }
            return new LinqDataSourceAutoGeneratedWhere(builder.ToString(), dictionary);
        }

        protected virtual Type GetDataObjectType(Type tableType)
        {
            if (tableType.IsGenericType)
            {
                Type[] genericArguments = tableType.GetGenericArguments();
                if (genericArguments.Length == 1)
                {
                    return genericArguments[0];
                }
            }
            return typeof(object);
        }

        private IOrderedDictionary GetOrderedParameterValues(ParameterCollection parameters)
        {
            IOrderedDictionary dictionary = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in parameters.GetValues(this._context, this._owner))
            {
                dictionary[entry.Key] = entry.Value;
            }
            return dictionary;
        }

        private IDictionary GetOriginalValues(IDictionary keys)
        {
            if (this._originalValues == null)
            {
                return null;
            }
            List<bool> list = new List<bool>();
            foreach (DictionaryEntry entry in keys)
            {
                string key = (string) entry.Key;
                if (this._originalValues.ContainsKey(key))
                {
                    object obj2 = entry.Value;
                    ArrayList list2 = (ArrayList) this._originalValues[key];
                    for (int i = 0; i < list2.Count; i++)
                    {
                        if (list.Count <= i)
                        {
                            list.Add(this.OriginalValueMatches(list2[i], obj2));
                        }
                        else if (list[i])
                        {
                            list[i] = this.OriginalValueMatches(list2[i], obj2);
                        }
                    }
                }
            }
            int index = list.IndexOf(true);
            if ((index < 0) || (list.IndexOf(true, index + 1) >= 0))
            {
                throw new InvalidOperationException(AtlasWeb.LinqDataSourceView_OriginalValuesNotFound);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>(this._originalValues.Count, StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry2 in this._originalValues)
            {
                ArrayList list3 = (ArrayList) entry2.Value;
                dictionary.Add((string) entry2.Key, list3[index]);
            }
            return dictionary;
        }

        private IDictionary<string, object> GetParameterValues(IOrderedDictionary parameterValues)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(parameterValues.Count, StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in parameterValues)
            {
                dictionary[(string) entry.Key] = entry.Value;
            }
            return dictionary;
        }

        private IDictionary<string, object> GetParameterValues(ParameterCollection parameters) => 
            this.GetParameterValues(parameters.GetValues(this._context, this._owner));

        protected virtual MemberInfo GetTableMemberInfo(Type contextType)
        {
            string tableName = this.TableName;
            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_TableNameNotSpecified, new object[] { this._owner.ID }));
            }
            MemberInfo[] infoArray = contextType.FindMembers(MemberTypes.Property | MemberTypes.Field, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null);
            for (int i = 0; i < infoArray.Length; i++)
            {
                if (string.Equals(infoArray[i].Name, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    return infoArray[i];
                }
            }
            return null;
        }

        private ReadOnlyCollection<MetaDataMember> GetTableMetaDataMembers(ITable table, Type dataObjectType) => 
            table.Context.Mapping.GetTable(dataObjectType).RowType.DataMembers;

        public int Insert(IDictionary values) => 
            this.ExecuteInsert(values);

        protected virtual void InsertDataObject(object dataContext, object table, object newDataObject)
        {
            this._linqToSql.Add((ITable) table, newDataObject);
            this._linqToSql.SubmitChanges((DataContext) dataContext);
        }

        protected virtual void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] objArray = (object[]) savedState;
                if (objArray[0] != null)
                {
                    ((IStateManager) this.WhereParameters).LoadViewState(objArray[0]);
                }
                if (objArray[1] != null)
                {
                    ((IStateManager) this.OrderByParameters).LoadViewState(objArray[1]);
                }
                if (objArray[2] != null)
                {
                    ((IStateManager) this.GroupByParameters).LoadViewState(objArray[2]);
                }
                if (objArray[3] != null)
                {
                    ((IStateManager) this.OrderGroupsByParameters).LoadViewState(objArray[3]);
                }
                if (objArray[4] != null)
                {
                    ((IStateManager) this.SelectNewParameters).LoadViewState(objArray[4]);
                }
                if (objArray[5] != null)
                {
                    this._originalValues = new Hashtable((Hashtable) objArray[5], StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        private static bool MemberIsStatic(MemberInfo member)
        {
            FieldInfo info = member as FieldInfo;
            if (info != null)
            {
                return info.IsStatic;
            }
            PropertyInfo info2 = member as PropertyInfo;
            if (info2 == null)
            {
                return false;
            }
            MethodInfo getMethod = info2.GetGetMethod();
            return ((getMethod != null) && getMethod.IsStatic);
        }

        private void MergeDictionaries(object dataObjectType, ParameterCollection referenceValues, IDictionary source, IDictionary destination)
        {
            MergeDictionaries(dataObjectType, referenceValues, source, destination, null);
        }

        private static void MergeDictionaries(object dataObjectType, ParameterCollection reference, IDictionary source, IDictionary destination, IDictionary destinationCopy)
        {
            if (source != null)
            {
                Dictionary<string, System.Exception> innerExceptions = null;
                foreach (DictionaryEntry entry in source)
                {
                    object obj2 = entry.Value;
                    Parameter parameter = null;
                    string key = (string) entry.Key;
                    foreach (Parameter parameter2 in reference)
                    {
                        if (string.Equals(parameter2.Name, key, StringComparison.OrdinalIgnoreCase))
                        {
                            parameter = parameter2;
                            break;
                        }
                    }
                    if (parameter != null)
                    {
                        try
                        {
                            obj2 = parameter.GetValue(obj2, true);
                        }
                        catch (System.Exception exception)
                        {
                            if (innerExceptions == null)
                            {
                                innerExceptions = new Dictionary<string, System.Exception>(StringComparer.OrdinalIgnoreCase);
                            }
                            innerExceptions[parameter.Name] = exception;
                        }
                    }
                    destination[key] = obj2;
                    if (destinationCopy != null)
                    {
                        destinationCopy[key] = obj2;
                    }
                }
                if (innerExceptions != null)
                {
                    throw new LinqDataSourceValidationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ValidationFailed, new object[] { dataObjectType, innerExceptions.Values.First<System.Exception>().Message }), innerExceptions);
                }
            }
        }

        protected virtual void OnContextCreated(LinqDataSourceStatusEventArgs e)
        {
            EventHandler<LinqDataSourceStatusEventArgs> handler = (EventHandler<LinqDataSourceStatusEventArgs>) base.Events[EventContextCreated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnContextCreating(LinqDataSourceContextEventArgs e)
        {
            EventHandler<LinqDataSourceContextEventArgs> handler = (EventHandler<LinqDataSourceContextEventArgs>) base.Events[EventContextCreating];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnContextDisposing(LinqDataSourceDisposeEventArgs e)
        {
            EventHandler<LinqDataSourceDisposeEventArgs> handler = (EventHandler<LinqDataSourceDisposeEventArgs>) base.Events[EventContextDisposing];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDeleted(LinqDataSourceStatusEventArgs e)
        {
            EventHandler<LinqDataSourceStatusEventArgs> handler = (EventHandler<LinqDataSourceStatusEventArgs>) base.Events[EventDeleted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDeleting(LinqDataSourceDeleteEventArgs e)
        {
            EventHandler<LinqDataSourceDeleteEventArgs> handler = (EventHandler<LinqDataSourceDeleteEventArgs>) base.Events[EventDeleting];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnException(DynamicValidatorEventArgs e)
        {
            EventHandler<DynamicValidatorEventArgs> handler = (EventHandler<DynamicValidatorEventArgs>) base.Events[EventException];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnInserted(LinqDataSourceStatusEventArgs e)
        {
            EventHandler<LinqDataSourceStatusEventArgs> handler = (EventHandler<LinqDataSourceStatusEventArgs>) base.Events[EventInserted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnInserting(LinqDataSourceInsertEventArgs e)
        {
            EventHandler<LinqDataSourceInsertEventArgs> handler = (EventHandler<LinqDataSourceInsertEventArgs>) base.Events[EventInserting];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelected(LinqDataSourceStatusEventArgs e)
        {
            EventHandler<LinqDataSourceStatusEventArgs> handler = (EventHandler<LinqDataSourceStatusEventArgs>) base.Events[EventSelected];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelecting(LinqDataSourceSelectEventArgs e)
        {
            EventHandler<LinqDataSourceSelectEventArgs> handler = (EventHandler<LinqDataSourceSelectEventArgs>) base.Events[EventSelecting];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUpdated(LinqDataSourceStatusEventArgs e)
        {
            EventHandler<LinqDataSourceStatusEventArgs> handler = (EventHandler<LinqDataSourceStatusEventArgs>) base.Events[EventUpdated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnUpdating(LinqDataSourceUpdateEventArgs e)
        {
            EventHandler<LinqDataSourceUpdateEventArgs> handler = (EventHandler<LinqDataSourceUpdateEventArgs>) base.Events[EventUpdating];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool OriginalValueMatches(object originalValue, object value)
        {
            IEnumerable enumerableA = originalValue as IEnumerable;
            IEnumerable enumerableB = value as IEnumerable;
            if ((enumerableA != null) && (enumerableB != null))
            {
                return LinqDataSourceHelper.EnumerableContentEquals(enumerableA, enumerableB);
            }
            return originalValue.Equals(value);
        }

        private void ReleaseContext(object dataContext)
        {
            if (dataContext != null)
            {
                LinqDataSourceDisposeEventArgs e = new LinqDataSourceDisposeEventArgs(dataContext);
                this.OnContextDisposing(e);
                if (!e.Cancel)
                {
                    IDisposable disposable = dataContext as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        internal void ReleaseSelectContexts()
        {
            if (this._selectContexts != null)
            {
                foreach (LinqDataSourceContextData data in this._selectContexts)
                {
                    this.ReleaseContext(data.Context);
                }
            }
        }

        protected virtual void ResetDataObject(object table, object dataObject)
        {
        }

        private object SaveParametersViewState(ParameterCollection parameters)
        {
            if ((parameters != null) && (parameters.Count > 0))
            {
                return ((IStateManager) parameters).SaveViewState();
            }
            return null;
        }

        protected virtual object SaveViewState()
        {
            object[] objArray = new object[6];
            objArray[0] = this.SaveParametersViewState(this._whereParameters);
            objArray[1] = this.SaveParametersViewState(this._orderByParameters);
            objArray[2] = this.SaveParametersViewState(this._groupByParameters);
            objArray[3] = this.SaveParametersViewState(this._orderGroupsByParameters);
            objArray[4] = this.SaveParametersViewState(this._selectNewParameters);
            if ((this._originalValues != null) && (this._originalValues.Count > 0))
            {
                objArray[5] = this._originalValues;
            }
            return objArray;
        }

        public IEnumerable Select(DataSourceSelectArguments arguments) => 
            this.ExecuteSelect(arguments);

        private void SelectParametersChangedEventHandler(object o, EventArgs e)
        {
            this.OnDataSourceViewChanged(EventArgs.Empty);
        }

        private Dictionary<string, System.Exception> SetDataObjectProperties(object oldDataObject, object newDataObject)
        {
            Dictionary<string, System.Exception> dictionary = null;
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(oldDataObject))
            {
                if (descriptor.PropertyType.IsSerializable && !descriptor.IsReadOnly)
                {
                    object obj2 = descriptor.GetValue(newDataObject);
                    try
                    {
                        descriptor.SetValue(oldDataObject, obj2);
                    }
                    catch (System.Exception exception)
                    {
                        if (dictionary == null)
                        {
                            dictionary = new Dictionary<string, System.Exception>(StringComparer.OrdinalIgnoreCase);
                        }
                        dictionary[descriptor.Name] = exception;
                    }
                }
            }
            return dictionary;
        }

        private void StoreOriginalValues(ITable table, Type dataObjectType, IList result)
        {
            ReadOnlyCollection<MetaDataMember> tableMetaDataMembers = this.GetTableMetaDataMembers(table, dataObjectType);
            if (tableMetaDataMembers != null)
            {
                int count = result.Count;
                int capacity = tableMetaDataMembers.Count;
                List<MetaDataMember> list = new List<MetaDataMember>(capacity);
                this._originalValues = new Hashtable(capacity, StringComparer.OrdinalIgnoreCase);
                foreach (MetaDataMember member in tableMetaDataMembers)
                {
                    if (member.Type.IsSerializable && ((member.IsPrimaryKey || member.IsVersion) || (member.UpdateCheck != UpdateCheck.Never)))
                    {
                        list.Add(member);
                        this._originalValues[member.Member.Name] = new ArrayList(count);
                    }
                }
                foreach (object obj2 in result)
                {
                    foreach (MetaDataMember member2 in list)
                    {
                        object boxedValue = member2.MemberAccessor.GetBoxedValue(obj2);
                        ((ArrayList) this._originalValues[member2.Member.Name]).Add(boxedValue);
                    }
                }
            }
        }

        void IStateManager.LoadViewState(object savedState)
        {
            this.LoadViewState(savedState);
        }

        object IStateManager.SaveViewState() => 
            this.SaveViewState();

        void IStateManager.TrackViewState()
        {
            this.TrackViewState();
        }

        private IList ToList(IQueryable query, Type dataObjectType) => 
            ((IList) typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new Type[] { dataObjectType }).Invoke(null, new object[] { query }));

        private void TrackParametersViewState(ParameterCollection parameters)
        {
            if ((parameters != null) && (parameters.Count > 0))
            {
                ((IStateManager) parameters).TrackViewState();
            }
        }

        protected virtual void TrackViewState()
        {
            this._tracking = true;
            this.TrackParametersViewState(this._whereParameters);
            this.TrackParametersViewState(this._orderByParameters);
            this.TrackParametersViewState(this._groupByParameters);
            this.TrackParametersViewState(this._orderGroupsByParameters);
            this.TrackParametersViewState(this._selectNewParameters);
        }

        public int Update(IDictionary keys, IDictionary values, IDictionary oldValues) => 
            this.ExecuteUpdate(keys, values, oldValues);

        protected virtual void UpdateDataObject(object dataContext, object table, object oldDataObject, object newDataObject)
        {
            this._linqToSql.Attach((ITable) table, oldDataObject);
            Dictionary<string, System.Exception> innerExceptions = this.SetDataObjectProperties(oldDataObject, newDataObject);
            if (innerExceptions != null)
            {
                throw new LinqDataSourceValidationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ValidationFailed, new object[] { oldDataObject.GetType(), innerExceptions.Values.First<System.Exception>().Message }), innerExceptions);
            }
            this._linqToSql.SubmitChanges((DataContext) dataContext);
        }

        protected virtual void ValidateContextType(Type contextType, bool selecting)
        {
            if (!selecting && !typeof(DataContext).IsAssignableFrom(contextType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InvalidContextType, new object[] { this._owner.ID }));
            }
        }

        protected virtual void ValidateDeleteSupported(IDictionary keys, IDictionary oldValues)
        {
            if (!this.CanDelete)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_DeleteNotSupported, new object[] { this._owner.ID }));
            }
            this.ValidateEditSupported();
        }

        private void ValidateEditSupported()
        {
            if (!string.IsNullOrEmpty(this.GroupBy))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_GroupByNotSupportedOnEdit, new object[] { this._owner.ID }));
            }
            if (!string.IsNullOrEmpty(this.SelectNew))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_SelectNewNotSupportedOnEdit, new object[] { this._owner.ID }));
            }
        }

        protected virtual void ValidateInsertSupported(IDictionary values)
        {
            if (!this.CanInsert)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InsertNotSupported, new object[] { this._owner.ID }));
            }
            this.ValidateEditSupported();
            if ((values == null) || (values.Count == 0))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InsertRequiresValues, new object[] { this._owner.ID }));
            }
        }

        protected virtual void ValidateOrderByParameter(string name, string value)
        {
            if (!_autoGenerateOrderByRegex.IsMatch(value))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InvalidOrderByFieldName, new object[] { value, name }));
            }
        }

        protected virtual void ValidateParameterName(string name)
        {
            if (!_identifierRegex.IsMatch(name))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InvalidParameterName, new object[] { name, this._owner.ID }));
            }
        }

        protected virtual void ValidateTableType(Type tableType, bool selecting)
        {
            if (!selecting && (!tableType.IsGenericType || !tableType.GetGenericTypeDefinition().Equals(typeof(Table<>))))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_InvalidTablePropertyType, new object[] { this._owner.ID }));
            }
        }

        protected virtual void ValidateUpdateSupported(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            if (!this.CanUpdate)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_UpdateNotSupported, new object[] { this._owner.ID }));
            }
            this.ValidateEditSupported();
        }

        public bool AutoGenerateOrderByClause
        {
            get => 
                this._autoGenerateOrderByClause;
            set
            {
                if (this._autoGenerateOrderByClause != value)
                {
                    this._autoGenerateOrderByClause = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public bool AutoGenerateWhereClause
        {
            get => 
                this._autoGenerateWhereClause;
            set
            {
                if (this._autoGenerateWhereClause != value)
                {
                    this._autoGenerateWhereClause = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public bool AutoPage
        {
            get => 
                this._autoPage;
            set
            {
                if (this._autoPage != value)
                {
                    this._autoPage = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public bool AutoSort
        {
            get => 
                this._autoSort;
            set
            {
                if (this._autoSort != value)
                {
                    this._autoSort = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public override bool CanDelete =>
            this.EnableDelete;

        public override bool CanInsert =>
            this.EnableInsert;

        public override bool CanPage =>
            true;

        public override bool CanRetrieveTotalRowCount =>
            true;

        public override bool CanSort =>
            true;

        public override bool CanUpdate =>
            this.EnableUpdate;

        protected internal virtual Type ContextType
        {
            get
            {
                if (this._contextType == null)
                {
                    string contextTypeName = this.ContextTypeName;
                    if (string.IsNullOrEmpty(contextTypeName))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ContextTypeNameNotSpecified, new object[] { this._owner.ID }));
                    }
                    try
                    {
                        this._contextType = BuildManager.GetType(contextTypeName, true, true);
                    }
                    catch (System.Exception exception)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ContextTypeNameNotFound, new object[] { this._owner.ID }), exception);
                    }
                }
                return this._contextType;
            }
        }

        public virtual string ContextTypeName
        {
            get => 
                (this._contextTypeName ?? string.Empty);
            set
            {
                if (this._contextTypeName != value)
                {
                    if (this._reuseSelectContext)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_ContextTypeNameChanged, new object[] { this._owner.ID }));
                    }
                    this._contextTypeName = value;
                    this._contextType = null;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection DeleteParameters
        {
            get
            {
                if (this._deleteParameters == null)
                {
                    this._deleteParameters = new ParameterCollection();
                }
                return this._deleteParameters;
            }
        }

        public bool EnableDelete
        {
            get => 
                this._enableDelete;
            set
            {
                if (this._enableDelete != value)
                {
                    this._enableDelete = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public bool EnableInsert
        {
            get => 
                this._enableInsert;
            set
            {
                if (this._enableInsert != value)
                {
                    this._enableInsert = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public bool EnableObjectTracking
        {
            get => 
                this._enableObjectTracking;
            set
            {
                if (this._enableObjectTracking != value)
                {
                    if (this._reuseSelectContext)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_EnableObjectTrackingChanged, new object[] { this._owner.ID }));
                    }
                    this._enableObjectTracking = value;
                }
            }
        }

        public bool EnableUpdate
        {
            get => 
                this._enableUpdate;
            set
            {
                if (this._enableUpdate != value)
                {
                    this._enableUpdate = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public string GroupBy
        {
            get => 
                (this._groupBy ?? string.Empty);
            set
            {
                if (this._groupBy != value)
                {
                    this._groupBy = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection GroupByParameters
        {
            get
            {
                if (this._groupByParameters == null)
                {
                    this._groupByParameters = new ParameterCollection();
                    this._groupByParameters.ParametersChanged += new EventHandler(this.SelectParametersChangedEventHandler);
                    if (this._tracking)
                    {
                        ((IStateManager) this._groupByParameters).TrackViewState();
                    }
                }
                return this._groupByParameters;
            }
        }

        public ParameterCollection InsertParameters
        {
            get
            {
                if (this._insertParameters == null)
                {
                    this._insertParameters = new ParameterCollection();
                }
                return this._insertParameters;
            }
        }

        protected bool IsTrackingViewState =>
            this._tracking;

        public string OrderBy
        {
            get => 
                (this._orderBy ?? string.Empty);
            set
            {
                if (this._orderBy != value)
                {
                    this._orderBy = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection OrderByParameters
        {
            get
            {
                if (this._orderByParameters == null)
                {
                    this._orderByParameters = new ParameterCollection();
                    this._orderByParameters.ParametersChanged += new EventHandler(this.SelectParametersChangedEventHandler);
                    if (this._tracking)
                    {
                        ((IStateManager) this._orderByParameters).TrackViewState();
                    }
                }
                return this._orderByParameters;
            }
        }

        public string OrderGroupsBy
        {
            get => 
                (this._orderGroupsBy ?? string.Empty);
            set
            {
                if (this._orderGroupsBy != value)
                {
                    this._orderGroupsBy = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection OrderGroupsByParameters
        {
            get
            {
                if (this._orderGroupsByParameters == null)
                {
                    this._orderGroupsByParameters = new ParameterCollection();
                    this._orderGroupsByParameters.ParametersChanged += new EventHandler(this.SelectParametersChangedEventHandler);
                    if (this._tracking)
                    {
                        ((IStateManager) this._orderGroupsByParameters).TrackViewState();
                    }
                }
                return this._orderGroupsByParameters;
            }
        }

        public string SelectNew
        {
            get => 
                (this._selectNew ?? string.Empty);
            set
            {
                if (this._selectNew != value)
                {
                    this._selectNew = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection SelectNewParameters
        {
            get
            {
                if (this._selectNewParameters == null)
                {
                    this._selectNewParameters = new ParameterCollection();
                    this._selectNewParameters.ParametersChanged += new EventHandler(this.SelectParametersChangedEventHandler);
                    if (this._tracking)
                    {
                        ((IStateManager) this._selectNewParameters).TrackViewState();
                    }
                }
                return this._selectNewParameters;
            }
        }

        public bool StoreOriginalValuesInViewState
        {
            get => 
                this._storeOriginalValuesInViewState;
            set
            {
                if (this._storeOriginalValuesInViewState != value)
                {
                    this._storeOriginalValuesInViewState = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        bool IStateManager.IsTrackingViewState =>
            this.IsTrackingViewState;

        public string TableName
        {
            get => 
                (this._tableName ?? string.Empty);
            set
            {
                if (this._tableName != value)
                {
                    if (this._reuseSelectContext)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSourceView_TableNameChanged, new object[] { this._owner.ID }));
                    }
                    this._tableName = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection UpdateParameters
        {
            get
            {
                if (this._updateParameters == null)
                {
                    this._updateParameters = new ParameterCollection();
                }
                return this._updateParameters;
            }
        }

        public string Where
        {
            get => 
                (this._where ?? string.Empty);
            set
            {
                if (this._where != value)
                {
                    this._where = value;
                    this.OnDataSourceViewChanged(EventArgs.Empty);
                }
            }
        }

        public ParameterCollection WhereParameters
        {
            get
            {
                if (this._whereParameters == null)
                {
                    this._whereParameters = new ParameterCollection();
                    this._whereParameters.ParametersChanged += new EventHandler(this.SelectParametersChangedEventHandler);
                    if (this._tracking)
                    {
                        ((IStateManager) this._whereParameters).TrackViewState();
                    }
                }
                return this._whereParameters;
            }
        }

        private class LinqDataSourceAutoGeneratedWhere
        {
            public IDictionary<string, object> WhereParameters;

            public LinqDataSourceAutoGeneratedWhere(string Where, IDictionary<string, object> WhereParameters)
            {
                this.Where = Where;
                this.WhereParameters = WhereParameters;
            }

            public string Where { get; set; }
        }
    }
}

