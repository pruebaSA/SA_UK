namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;

    [DefaultEvent("Selecting"), ToolboxBitmap(typeof(SqlDataSource)), WebSysDisplayName("SqlDataSource_DisplayName"), Designer("System.Web.UI.Design.WebControls.SqlDataSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ParseChildren(true), PersistChildren(false), DefaultProperty("SelectQuery"), WebSysDescription("SqlDataSource_Description"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SqlDataSource : DataSourceControl
    {
        private DataSourceCache _cache;
        private string _cachedSelectCommand;
        private string _connectionString;
        private SqlDataSourceMode _dataSourceMode;
        private DbProviderFactory _providerFactory;
        private string _providerName;
        private SqlDataSourceView _view;
        private ICollection _viewNames;
        private const string DefaultProviderName = "System.Data.SqlClient";
        private const string DefaultViewName = "DefaultView";

        [WebCategory("Data"), WebSysDescription("DataSource_Deleted")]
        public event SqlDataSourceStatusEventHandler Deleted
        {
            add
            {
                this.GetView().Deleted += value;
            }
            remove
            {
                this.GetView().Deleted -= value;
            }
        }

        [WebSysDescription("DataSource_Deleting"), WebCategory("Data")]
        public event SqlDataSourceCommandEventHandler Deleting
        {
            add
            {
                this.GetView().Deleting += value;
            }
            remove
            {
                this.GetView().Deleting -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("DataSource_Filtering")]
        public event SqlDataSourceFilteringEventHandler Filtering
        {
            add
            {
                this.GetView().Filtering += value;
            }
            remove
            {
                this.GetView().Filtering -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("DataSource_Inserted")]
        public event SqlDataSourceStatusEventHandler Inserted
        {
            add
            {
                this.GetView().Inserted += value;
            }
            remove
            {
                this.GetView().Inserted -= value;
            }
        }

        [WebSysDescription("DataSource_Inserting"), WebCategory("Data")]
        public event SqlDataSourceCommandEventHandler Inserting
        {
            add
            {
                this.GetView().Inserting += value;
            }
            remove
            {
                this.GetView().Inserting -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_Selected")]
        public event SqlDataSourceStatusEventHandler Selected
        {
            add
            {
                this.GetView().Selected += value;
            }
            remove
            {
                this.GetView().Selected -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_Selecting")]
        public event SqlDataSourceSelectingEventHandler Selecting
        {
            add
            {
                this.GetView().Selecting += value;
            }
            remove
            {
                this.GetView().Selecting -= value;
            }
        }

        [WebSysDescription("DataSource_Updated"), WebCategory("Data")]
        public event SqlDataSourceStatusEventHandler Updated
        {
            add
            {
                this.GetView().Updated += value;
            }
            remove
            {
                this.GetView().Updated -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("DataSource_Updating")]
        public event SqlDataSourceCommandEventHandler Updating
        {
            add
            {
                this.GetView().Updating += value;
            }
            remove
            {
                this.GetView().Updating -= value;
            }
        }

        public SqlDataSource()
        {
            this._dataSourceMode = SqlDataSourceMode.DataSet;
        }

        public SqlDataSource(string connectionString, string selectCommand)
        {
            this._dataSourceMode = SqlDataSourceMode.DataSet;
            this._connectionString = connectionString;
            this._cachedSelectCommand = selectCommand;
        }

        public SqlDataSource(string providerName, string connectionString, string selectCommand) : this(connectionString, selectCommand)
        {
            this._providerName = providerName;
        }

        internal string CreateCacheKey(int startRowIndex, int maximumRows)
        {
            StringBuilder builder = this.CreateRawCacheKey();
            builder.Append(startRowIndex.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(maximumRows.ToString(CultureInfo.InvariantCulture));
            return builder.ToString();
        }

        internal DbCommand CreateCommand(string commandText, DbConnection connection)
        {
            DbCommand command = this.GetDbProviderFactorySecure().CreateCommand();
            command.CommandText = commandText;
            command.Connection = connection;
            return command;
        }

        internal DbConnection CreateConnection(string connectionString)
        {
            DbConnection connection = this.GetDbProviderFactorySecure().CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        internal DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            DbDataAdapter adapter = this.GetDbProviderFactorySecure().CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        protected virtual SqlDataSourceView CreateDataSourceView(string viewName) => 
            new SqlDataSourceView(this, viewName, this.Context);

        internal string CreateMasterCacheKey() => 
            this.CreateRawCacheKey().ToString();

        internal DbParameter CreateParameter(string parameterName, object parameterValue)
        {
            DbParameter parameter = this.GetDbProviderFactorySecure().CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }

        private StringBuilder CreateRawCacheKey()
        {
            StringBuilder builder = new StringBuilder("u", 0x400);
            builder.Append(base.GetType().GetHashCode().ToString(CultureInfo.InvariantCulture));
            builder.Append(this.CacheDuration.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(((int) this.CacheExpirationPolicy).ToString(CultureInfo.InvariantCulture));
            System.Web.UI.SqlDataSourceCache cache = this.Cache as System.Web.UI.SqlDataSourceCache;
            if (cache != null)
            {
                builder.Append(":");
                builder.Append(cache.SqlCacheDependency);
            }
            builder.Append(":");
            builder.Append(this.ConnectionString);
            builder.Append(":");
            builder.Append(this.SelectCommand);
            if (this.SelectParameters.Count > 0)
            {
                builder.Append("?");
                foreach (DictionaryEntry entry in this.SelectParameters.GetValues(this.Context, this))
                {
                    builder.Append(entry.Key.ToString());
                    if ((entry.Value != null) && (entry.Value != DBNull.Value))
                    {
                        builder.Append("=");
                        builder.Append(entry.Value.ToString());
                    }
                    else if (entry.Value == DBNull.Value)
                    {
                        builder.Append("(dbnull)");
                    }
                    else
                    {
                        builder.Append("(null)");
                    }
                    builder.Append("&");
                }
            }
            return builder;
        }

        public int Delete() => 
            this.GetView().Delete(null, null);

        protected virtual DbProviderFactory GetDbProviderFactory()
        {
            string providerName = this.ProviderName;
            if (string.IsNullOrEmpty(providerName))
            {
                return SqlClientFactory.Instance;
            }
            return DbProviderFactories.GetFactory(providerName);
        }

        private DbProviderFactory GetDbProviderFactorySecure()
        {
            if (this._providerFactory == null)
            {
                this._providerFactory = this.GetDbProviderFactory();
                if (!HttpRuntime.ProcessRequestInApplicationTrust && !HttpRuntime.HasDbPermission(this._providerFactory))
                {
                    throw new HttpException(System.Web.SR.GetString("SqlDataSource_NoDbPermission", new object[] { this._providerFactory.GetType().Name, this.ID }));
                }
            }
            return this._providerFactory;
        }

        private SqlDataSourceView GetView()
        {
            if (this._view == null)
            {
                this._view = this.CreateDataSourceView("DefaultView");
                if (this._cachedSelectCommand != null)
                {
                    this._view.SelectCommand = this._cachedSelectCommand;
                }
                if (base.IsTrackingViewState)
                {
                    ((IStateManager) this._view).TrackViewState();
                }
            }
            return this._view;
        }

        protected override DataSourceView GetView(string viewName)
        {
            if ((viewName == null) || ((viewName.Length != 0) && !string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(System.Web.SR.GetString("DataSource_InvalidViewName", new object[] { this.ID, "DefaultView" }), "viewName");
            }
            return this.GetView();
        }

        protected override ICollection GetViewNames()
        {
            if (this._viewNames == null)
            {
                this._viewNames = new string[] { "DefaultView" };
            }
            return this._viewNames;
        }

        public int Insert() => 
            this.GetView().Insert(null);

        internal void InvalidateCacheEntry()
        {
            string key = this.CreateMasterCacheKey();
            this.Cache.Invalidate(key);
        }

        private void LoadCompleteEventHandler(object sender, EventArgs e)
        {
            this.SelectParameters.UpdateValues(this.Context, this);
            this.FilterParameters.UpdateValues(this.Context, this);
        }

        internal object LoadDataFromCache(int startRowIndex, int maximumRows)
        {
            string key = this.CreateCacheKey(startRowIndex, maximumRows);
            return this.Cache.LoadDataFromCache(key);
        }

        internal int LoadTotalRowCountFromCache()
        {
            string key = this.CreateMasterCacheKey();
            object obj2 = this.Cache.LoadDataFromCache(key);
            if (obj2 is int)
            {
                return (int) obj2;
            }
            return -1;
        }

        protected override void LoadViewState(object savedState)
        {
            Pair pair = (Pair) savedState;
            if (savedState == null)
            {
                base.LoadViewState(null);
            }
            else
            {
                base.LoadViewState(pair.First);
                if (pair.Second != null)
                {
                    ((IStateManager) this.GetView()).LoadViewState(pair.Second);
                }
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.Page != null)
            {
                this.Page.LoadComplete += new EventHandler(this.LoadCompleteEventHandler);
            }
        }

        internal virtual void SaveDataToCache(int startRowIndex, int maximumRows, object data, CacheDependency dependency)
        {
            string key = this.CreateCacheKey(startRowIndex, maximumRows);
            string str2 = this.CreateMasterCacheKey();
            if (this.Cache.LoadDataFromCache(str2) == null)
            {
                this.Cache.SaveDataToCache(str2, -1, dependency);
            }
            CacheDependency dependency2 = new CacheDependency(0, new string[0], new string[] { str2 });
            this.Cache.SaveDataToCache(key, data, dependency2);
        }

        protected override object SaveViewState()
        {
            Pair pair = new Pair {
                First = base.SaveViewState()
            };
            if (this._view != null)
            {
                pair.Second = ((IStateManager) this._view).SaveViewState();
            }
            if ((pair.First == null) && (pair.Second == null))
            {
                return null;
            }
            return pair;
        }

        public IEnumerable Select(DataSourceSelectArguments arguments) => 
            this.GetView().Select(arguments);

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._view != null)
            {
                ((IStateManager) this._view).TrackViewState();
            }
        }

        public int Update() => 
            this.GetView().Update(null, null, null);

        internal virtual DataSourceCache Cache
        {
            get
            {
                if (this._cache == null)
                {
                    this._cache = new System.Web.UI.SqlDataSourceCache();
                }
                return this._cache;
            }
        }

        [DefaultValue(0), WebSysDescription("DataSourceCache_Duration"), WebCategory("Cache"), TypeConverter(typeof(DataSourceCacheDurationConverter))]
        public virtual int CacheDuration
        {
            get => 
                this.Cache.Duration;
            set
            {
                this.Cache.Duration = value;
            }
        }

        [DefaultValue(0), WebSysDescription("DataSourceCache_ExpirationPolicy"), WebCategory("Cache")]
        public virtual DataSourceCacheExpiry CacheExpirationPolicy
        {
            get => 
                this.Cache.ExpirationPolicy;
            set
            {
                this.Cache.ExpirationPolicy = value;
            }
        }

        [WebSysDescription("DataSourceCache_KeyDependency"), DefaultValue(""), WebCategory("Cache")]
        public virtual string CacheKeyDependency
        {
            get => 
                this.Cache.KeyDependency;
            set
            {
                this.Cache.KeyDependency = value;
            }
        }

        [WebCategory("Data"), DefaultValue(true), WebSysDescription("SqlDataSource_CancelSelectOnNullParameter")]
        public virtual bool CancelSelectOnNullParameter
        {
            get => 
                this.GetView().CancelSelectOnNullParameter;
            set
            {
                this.GetView().CancelSelectOnNullParameter = value;
            }
        }

        [WebCategory("Data"), DefaultValue(0), WebSysDescription("SqlDataSource_ConflictDetection")]
        public ConflictOptions ConflictDetection
        {
            get => 
                this.GetView().ConflictDetection;
            set
            {
                this.GetView().ConflictDetection = value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_ConnectionString"), Editor("System.Web.UI.Design.WebControls.SqlDataSourceConnectionStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue("")]
        public virtual string ConnectionString
        {
            get
            {
                if (this._connectionString != null)
                {
                    return this._connectionString;
                }
                return string.Empty;
            }
            set
            {
                if (this.ConnectionString != value)
                {
                    this._connectionString = value;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(1), WebSysDescription("SqlDataSource_DataSourceMode"), WebCategory("Behavior")]
        public SqlDataSourceMode DataSourceMode
        {
            get => 
                this._dataSourceMode;
            set
            {
                if ((value < SqlDataSourceMode.DataReader) || (value > SqlDataSourceMode.DataSet))
                {
                    throw new ArgumentOutOfRangeException(System.Web.SR.GetString("SqlDataSource_InvalidMode", new object[] { this.ID }));
                }
                if (this.DataSourceMode != value)
                {
                    this._dataSourceMode = value;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        [WebSysDescription("SqlDataSource_DeleteCommand"), DefaultValue(""), WebCategory("Data")]
        public string DeleteCommand
        {
            get => 
                this.GetView().DeleteCommand;
            set
            {
                this.GetView().DeleteCommand = value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_DeleteCommandType"), DefaultValue(0)]
        public SqlDataSourceCommandType DeleteCommandType
        {
            get => 
                this.GetView().DeleteCommandType;
            set
            {
                this.GetView().DeleteCommandType = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), WebSysDescription("SqlDataSource_DeleteParameters"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue((string) null), MergableProperty(false)]
        public ParameterCollection DeleteParameters =>
            this.GetView().DeleteParameters;

        [WebSysDescription("DataSourceCache_Enabled"), DefaultValue(false), WebCategory("Cache")]
        public virtual bool EnableCaching
        {
            get => 
                this.Cache.Enabled;
            set
            {
                this.Cache.Enabled = value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_FilterExpression"), DefaultValue("")]
        public string FilterExpression
        {
            get => 
                this.GetView().FilterExpression;
            set
            {
                this.GetView().FilterExpression = value;
            }
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue((string) null), WebSysDescription("SqlDataSource_FilterParameters")]
        public ParameterCollection FilterParameters =>
            this.GetView().FilterParameters;

        [WebSysDescription("SqlDataSource_InsertCommand"), DefaultValue(""), WebCategory("Data")]
        public string InsertCommand
        {
            get => 
                this.GetView().InsertCommand;
            set
            {
                this.GetView().InsertCommand = value;
            }
        }

        [WebCategory("Data"), DefaultValue(0), WebSysDescription("SqlDataSource_InsertCommandType")]
        public SqlDataSourceCommandType InsertCommandType
        {
            get => 
                this.GetView().InsertCommandType;
            set
            {
                this.GetView().InsertCommandType = value;
            }
        }

        [MergableProperty(false), DefaultValue((string) null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Data"), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("SqlDataSource_InsertParameters")]
        public ParameterCollection InsertParameters =>
            this.GetView().InsertParameters;

        [WebSysDescription("DataSource_OldValuesParameterFormatString"), DefaultValue("{0}"), WebCategory("Data")]
        public string OldValuesParameterFormatString
        {
            get => 
                this.GetView().OldValuesParameterFormatString;
            set
            {
                this.GetView().OldValuesParameterFormatString = value;
            }
        }

        [WebCategory("Data"), WebSysDescription("SqlDataSource_ProviderName"), DefaultValue(""), TypeConverter("System.Web.UI.Design.WebControls.DataProviderNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public virtual string ProviderName
        {
            get
            {
                if (this._providerName != null)
                {
                    return this._providerName;
                }
                return string.Empty;
            }
            set
            {
                if (this.ProviderName != value)
                {
                    this._providerFactory = null;
                    this._providerName = value;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        [WebSysDescription("SqlDataSource_SelectCommand"), DefaultValue(""), WebCategory("Data")]
        public string SelectCommand
        {
            get => 
                this.GetView().SelectCommand;
            set
            {
                this.GetView().SelectCommand = value;
            }
        }

        [DefaultValue(0), WebSysDescription("SqlDataSource_SelectCommandType"), WebCategory("Data")]
        public SqlDataSourceCommandType SelectCommandType
        {
            get => 
                this.GetView().SelectCommandType;
            set
            {
                this.GetView().SelectCommandType = value;
            }
        }

        [WebSysDescription("SqlDataSource_SelectParameters"), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false), DefaultValue((string) null)]
        public ParameterCollection SelectParameters =>
            this.GetView().SelectParameters;

        [WebCategory("Data"), DefaultValue(""), WebSysDescription("SqlDataSource_SortParameterName")]
        public string SortParameterName
        {
            get => 
                this.GetView().SortParameterName;
            set
            {
                this.GetView().SortParameterName = value;
            }
        }

        [WebCategory("Cache"), WebSysDescription("SqlDataSourceCache_SqlCacheDependency"), DefaultValue("")]
        public virtual string SqlCacheDependency
        {
            get => 
                this.SqlDataSourceCache.SqlCacheDependency;
            set
            {
                this.SqlDataSourceCache.SqlCacheDependency = value;
            }
        }

        private System.Web.UI.SqlDataSourceCache SqlDataSourceCache
        {
            get
            {
                System.Web.UI.SqlDataSourceCache cache = this.Cache as System.Web.UI.SqlDataSourceCache;
                if (cache == null)
                {
                    throw new NotSupportedException(System.Web.SR.GetString("SqlDataSource_SqlCacheDependencyNotSupported", new object[] { this.ID }));
                }
                return cache;
            }
        }

        [WebCategory("Data"), DefaultValue(""), WebSysDescription("SqlDataSource_UpdateCommand")]
        public string UpdateCommand
        {
            get => 
                this.GetView().UpdateCommand;
            set
            {
                this.GetView().UpdateCommand = value;
            }
        }

        [WebCategory("Data"), DefaultValue(0), WebSysDescription("SqlDataSource_UpdateCommandType")]
        public SqlDataSourceCommandType UpdateCommandType
        {
            get => 
                this.GetView().UpdateCommandType;
            set
            {
                this.GetView().UpdateCommandType = value;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), WebSysDescription("SqlDataSource_UpdateParameters"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection UpdateParameters =>
            this.GetView().UpdateParameters;
    }
}

