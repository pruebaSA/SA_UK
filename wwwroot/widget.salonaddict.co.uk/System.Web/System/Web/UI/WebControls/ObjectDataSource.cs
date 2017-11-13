namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;

    [WebSysDisplayName("ObjectDataSource_DisplayName"), ToolboxBitmap(typeof(ObjectDataSource)), WebSysDescription("ObjectDataSource_Description"), DefaultEvent("Selecting"), DefaultProperty("TypeName"), Designer("System.Web.UI.Design.WebControls.ObjectDataSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ParseChildren(true), PersistChildren(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSource : DataSourceControl
    {
        private SqlDataSourceCache _cache;
        private ObjectDataSourceView _view;
        private ICollection _viewNames;
        private const string DefaultViewName = "DefaultView";

        [WebSysDescription("DataSource_Deleted"), WebCategory("Data")]
        public event ObjectDataSourceStatusEventHandler Deleted
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

        [WebCategory("Data"), WebSysDescription("DataSource_Deleting")]
        public event ObjectDataSourceMethodEventHandler Deleting
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
        public event ObjectDataSourceFilteringEventHandler Filtering
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

        [WebSysDescription("DataSource_Inserted"), WebCategory("Data")]
        public event ObjectDataSourceStatusEventHandler Inserted
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

        [WebCategory("Data"), WebSysDescription("DataSource_Inserting")]
        public event ObjectDataSourceMethodEventHandler Inserting
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

        [WebSysDescription("ObjectDataSource_ObjectCreated"), WebCategory("Data")]
        public event ObjectDataSourceObjectEventHandler ObjectCreated
        {
            add
            {
                this.GetView().ObjectCreated += value;
            }
            remove
            {
                this.GetView().ObjectCreated -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("ObjectDataSource_ObjectCreating")]
        public event ObjectDataSourceObjectEventHandler ObjectCreating
        {
            add
            {
                this.GetView().ObjectCreating += value;
            }
            remove
            {
                this.GetView().ObjectCreating -= value;
            }
        }

        [WebSysDescription("ObjectDataSource_ObjectDisposing"), WebCategory("Data")]
        public event ObjectDataSourceDisposingEventHandler ObjectDisposing
        {
            add
            {
                this.GetView().ObjectDisposing += value;
            }
            remove
            {
                this.GetView().ObjectDisposing -= value;
            }
        }

        [WebCategory("Data"), WebSysDescription("ObjectDataSource_Selected")]
        public event ObjectDataSourceStatusEventHandler Selected
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

        [WebSysDescription("ObjectDataSource_Selecting"), WebCategory("Data")]
        public event ObjectDataSourceSelectingEventHandler Selecting
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
        public event ObjectDataSourceStatusEventHandler Updated
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
        public event ObjectDataSourceMethodEventHandler Updating
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

        public ObjectDataSource()
        {
        }

        public ObjectDataSource(string typeName, string selectMethod)
        {
            this.TypeName = typeName;
            this.SelectMethod = selectMethod;
        }

        internal string CreateCacheKey(int startRowIndex, int maximumRows)
        {
            StringBuilder builder = this.CreateRawCacheKey();
            builder.Append(':');
            builder.Append(startRowIndex.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(maximumRows.ToString(CultureInfo.InvariantCulture));
            return builder.ToString();
        }

        internal string CreateMasterCacheKey() => 
            this.CreateRawCacheKey().ToString();

        private StringBuilder CreateRawCacheKey()
        {
            StringBuilder builder = new StringBuilder("u", 0x400);
            builder.Append(base.GetType().GetHashCode().ToString(CultureInfo.InvariantCulture));
            builder.Append(":");
            builder.Append(this.CacheDuration.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(((int) this.CacheExpirationPolicy).ToString(CultureInfo.InvariantCulture));
            builder.Append(":");
            builder.Append(this.SqlCacheDependency);
            builder.Append(":");
            builder.Append(this.TypeName);
            builder.Append(":");
            builder.Append(this.SelectMethod);
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

        private ObjectDataSourceView GetView()
        {
            if (this._view == null)
            {
                this._view = new ObjectDataSourceView(this, "DefaultView", this.Context);
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

        internal void SaveDataToCache(int startRowIndex, int maximumRows, object data)
        {
            string key = this.CreateCacheKey(startRowIndex, maximumRows);
            string str2 = this.CreateMasterCacheKey();
            if (this.Cache.LoadDataFromCache(str2) == null)
            {
                this.Cache.SaveDataToCache(str2, -1);
            }
            CacheDependency dependency = new CacheDependency(0, new string[0], new string[] { str2 });
            this.Cache.SaveDataToCache(key, data, dependency);
        }

        internal void SaveTotalRowCountToCache(int totalRowCount)
        {
            string key = this.CreateMasterCacheKey();
            this.Cache.SaveDataToCache(key, totalRowCount);
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

        public IEnumerable Select() => 
            this.GetView().Select(DataSourceSelectArguments.Empty);

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

        internal SqlDataSourceCache Cache
        {
            get
            {
                if (this._cache == null)
                {
                    this._cache = new SqlDataSourceCache();
                }
                return this._cache;
            }
        }

        [TypeConverter(typeof(DataSourceCacheDurationConverter)), DefaultValue(0), WebCategory("Cache"), WebSysDescription("DataSourceCache_Duration")]
        public virtual int CacheDuration
        {
            get => 
                this.Cache.Duration;
            set
            {
                this.Cache.Duration = value;
            }
        }

        [DefaultValue(0), WebCategory("Cache"), WebSysDescription("DataSourceCache_ExpirationPolicy")]
        public virtual DataSourceCacheExpiry CacheExpirationPolicy
        {
            get => 
                this.Cache.ExpirationPolicy;
            set
            {
                this.Cache.ExpirationPolicy = value;
            }
        }

        [DefaultValue(""), WebCategory("Cache"), WebSysDescription("DataSourceCache_KeyDependency")]
        public virtual string CacheKeyDependency
        {
            get => 
                this.Cache.KeyDependency;
            set
            {
                this.Cache.KeyDependency = value;
            }
        }

        [WebSysDescription("ObjectDataSource_ConflictDetection"), DefaultValue(0), WebCategory("Data")]
        public ConflictOptions ConflictDetection
        {
            get => 
                this.GetView().ConflictDetection;
            set
            {
                this.GetView().ConflictDetection = value;
            }
        }

        [DefaultValue(false), WebSysDescription("ObjectDataSource_ConvertNullToDBNull"), WebCategory("Data")]
        public bool ConvertNullToDBNull
        {
            get => 
                this.GetView().ConvertNullToDBNull;
            set
            {
                this.GetView().ConvertNullToDBNull = value;
            }
        }

        [DefaultValue(""), WebSysDescription("ObjectDataSource_DataObjectTypeName"), WebCategory("Data")]
        public string DataObjectTypeName
        {
            get => 
                this.GetView().DataObjectTypeName;
            set
            {
                this.GetView().DataObjectTypeName = value;
            }
        }

        [WebSysDescription("ObjectDataSource_DeleteMethod"), DefaultValue(""), WebCategory("Data")]
        public string DeleteMethod
        {
            get => 
                this.GetView().DeleteMethod;
            set
            {
                this.GetView().DeleteMethod = value;
            }
        }

        [Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Data"), DefaultValue((string) null), WebSysDescription("ObjectDataSource_DeleteParameters"), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false)]
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

        [WebSysDescription("ObjectDataSource_EnablePaging"), WebCategory("Paging"), DefaultValue(false)]
        public bool EnablePaging
        {
            get => 
                this.GetView().EnablePaging;
            set
            {
                this.GetView().EnablePaging = value;
            }
        }

        [WebCategory("Data"), DefaultValue(""), WebSysDescription("ObjectDataSource_FilterExpression")]
        public string FilterExpression
        {
            get => 
                this.GetView().FilterExpression;
            set
            {
                this.GetView().FilterExpression = value;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), WebSysDescription("ObjectDataSource_FilterParameters"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection FilterParameters =>
            this.GetView().FilterParameters;

        [WebSysDescription("ObjectDataSource_InsertMethod"), DefaultValue(""), WebCategory("Data")]
        public string InsertMethod
        {
            get => 
                this.GetView().InsertMethod;
            set
            {
                this.GetView().InsertMethod = value;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), WebSysDescription("ObjectDataSource_InsertParameters"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection InsertParameters =>
            this.GetView().InsertParameters;

        [WebSysDescription("ObjectDataSource_MaximumRowsParameterName"), DefaultValue("maximumRows"), WebCategory("Paging")]
        public string MaximumRowsParameterName
        {
            get => 
                this.GetView().MaximumRowsParameterName;
            set
            {
                this.GetView().MaximumRowsParameterName = value;
            }
        }

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

        [WebSysDescription("ObjectDataSource_SelectCountMethod"), DefaultValue(""), WebCategory("Paging")]
        public string SelectCountMethod
        {
            get => 
                this.GetView().SelectCountMethod;
            set
            {
                this.GetView().SelectCountMethod = value;
            }
        }

        [DefaultValue(""), WebSysDescription("ObjectDataSource_SelectMethod"), WebCategory("Data")]
        public string SelectMethod
        {
            get => 
                this.GetView().SelectMethod;
            set
            {
                this.GetView().SelectMethod = value;
            }
        }

        [WebSysDescription("ObjectDataSource_SelectParameters"), DefaultValue((string) null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data")]
        public ParameterCollection SelectParameters =>
            this.GetView().SelectParameters;

        [DefaultValue(""), WebSysDescription("ObjectDataSource_SortParameterName"), WebCategory("Data")]
        public string SortParameterName
        {
            get => 
                this.GetView().SortParameterName;
            set
            {
                this.GetView().SortParameterName = value;
            }
        }

        [WebSysDescription("SqlDataSourceCache_SqlCacheDependency"), DefaultValue(""), WebCategory("Cache")]
        public virtual string SqlCacheDependency
        {
            get => 
                this.Cache.SqlCacheDependency;
            set
            {
                this.Cache.SqlCacheDependency = value;
            }
        }

        [DefaultValue("startRowIndex"), WebSysDescription("ObjectDataSource_StartRowIndexParameterName"), WebCategory("Paging")]
        public string StartRowIndexParameterName
        {
            get => 
                this.GetView().StartRowIndexParameterName;
            set
            {
                this.GetView().StartRowIndexParameterName = value;
            }
        }

        [WebSysDescription("ObjectDataSource_TypeName"), DefaultValue(""), WebCategory("Data")]
        public string TypeName
        {
            get => 
                this.GetView().TypeName;
            set
            {
                this.GetView().TypeName = value;
            }
        }

        [WebCategory("Data"), WebSysDescription("ObjectDataSource_UpdateMethod"), DefaultValue("")]
        public string UpdateMethod
        {
            get => 
                this.GetView().UpdateMethod;
            set
            {
                this.GetView().UpdateMethod = value;
            }
        }

        [Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Data"), WebSysDescription("ObjectDataSource_UpdateParameters")]
        public ParameterCollection UpdateParameters =>
            this.GetView().UpdateParameters;
    }
}

