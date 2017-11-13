namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Resources;
    using System.Web.UI;

    [DefaultProperty("ContextTypeName"), Designer("System.Web.UI.Design.WebControls.LinqDataSourceDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), ResourceDescription("LinqDataSource_Description"), ResourceDisplayName("LinqDataSource_DisplayName"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), PersistChildren(false), ToolboxBitmap(typeof(LinqDataSource), "LinqDataSource.ico"), DefaultEvent("Selecting"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSource : DataSourceControl, IDynamicDataSource, IDataSource
    {
        private readonly System.Web.UI.IPage _page;
        private LinqDataSourceView _view;
        private ICollection _viewNames;
        private const string DefaultViewName = "DefaultView";

        [ResourceDescription("LinqDataSource_ContextCreated"), Category("Data")]
        public event EventHandler<LinqDataSourceStatusEventArgs> ContextCreated
        {
            add
            {
                this.View.ContextCreated += value;
            }
            remove
            {
                this.View.ContextCreated -= value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_ContextCreating")]
        public event EventHandler<LinqDataSourceContextEventArgs> ContextCreating
        {
            add
            {
                this.View.ContextCreating += value;
            }
            remove
            {
                this.View.ContextCreating -= value;
            }
        }

        [ResourceDescription("LinqDataSource_ContextDisposing"), Category("Data")]
        public event EventHandler<LinqDataSourceDisposeEventArgs> ContextDisposing
        {
            add
            {
                this.View.ContextDisposing += value;
            }
            remove
            {
                this.View.ContextDisposing -= value;
            }
        }

        [ResourceDescription("LinqDataSource_Deleted"), Category("Data")]
        public event EventHandler<LinqDataSourceStatusEventArgs> Deleted
        {
            add
            {
                this.View.Deleted += value;
            }
            remove
            {
                this.View.Deleted -= value;
            }
        }

        [ResourceDescription("LinqDataSource_Deleting"), Category("Data")]
        public event EventHandler<LinqDataSourceDeleteEventArgs> Deleting
        {
            add
            {
                this.View.Deleting += value;
            }
            remove
            {
                this.View.Deleting -= value;
            }
        }

        [ResourceDescription("LinqDataSource_Inserted"), Category("Data")]
        public event EventHandler<LinqDataSourceStatusEventArgs> Inserted
        {
            add
            {
                this.View.Inserted += value;
            }
            remove
            {
                this.View.Inserted -= value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_Inserting")]
        public event EventHandler<LinqDataSourceInsertEventArgs> Inserting
        {
            add
            {
                this.View.Inserting += value;
            }
            remove
            {
                this.View.Inserting -= value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_Selected")]
        public event EventHandler<LinqDataSourceStatusEventArgs> Selected
        {
            add
            {
                this.View.Selected += value;
            }
            remove
            {
                this.View.Selected -= value;
            }
        }

        [ResourceDescription("LinqDataSource_Selecting"), Category("Data")]
        public event EventHandler<LinqDataSourceSelectEventArgs> Selecting
        {
            add
            {
                this.View.Selecting += value;
            }
            remove
            {
                this.View.Selecting -= value;
            }
        }

        event EventHandler<DynamicValidatorEventArgs> IDynamicDataSource.Exception
        {
            add
            {
                this.View.Exception += value;
            }
            remove
            {
                this.View.Exception -= value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_Updated")]
        public event EventHandler<LinqDataSourceStatusEventArgs> Updated
        {
            add
            {
                this.View.Updated += value;
            }
            remove
            {
                this.View.Updated -= value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_Updating")]
        public event EventHandler<LinqDataSourceUpdateEventArgs> Updating
        {
            add
            {
                this.View.Updating += value;
            }
            remove
            {
                this.View.Updating -= value;
            }
        }

        public LinqDataSource()
        {
        }

        internal LinqDataSource(System.Web.UI.IPage page)
        {
            this._page = page;
        }

        internal LinqDataSource(IDynamicQueryable dynamicQueryable, ILinqToSql linqToSql)
        {
            this.View = new LinqDataSourceView(this, "DefaultView", this.Context, dynamicQueryable, linqToSql);
        }

        protected virtual LinqDataSourceView CreateView() => 
            new LinqDataSourceView(this, "DefaultView", this.Context);

        public int Delete(IDictionary keys, IDictionary oldValues) => 
            this.View.Delete(keys, oldValues);

        protected override DataSourceView GetView(string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException("viewName");
            }
            if ((viewName.Length != 0) && !string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.LinqDataSource_InvalidViewName, new object[] { this.ID, "DefaultView" }), "viewName");
            }
            return this.View;
        }

        protected override ICollection GetViewNames()
        {
            if (this._viewNames == null)
            {
                this._viewNames = new ReadOnlyCollection<string>(new string[] { "DefaultView" });
            }
            return this._viewNames;
        }

        public int Insert(IDictionary values) => 
            this.View.Insert(values);

        private void LoadCompleteEventHandler(object sender, EventArgs e)
        {
            this.WhereParameters.UpdateValues(this.Context, this);
            this.OrderByParameters.UpdateValues(this.Context, this);
            this.GroupByParameters.UpdateValues(this.Context, this);
            this.OrderGroupsByParameters.UpdateValues(this.Context, this);
            this.SelectParameters.UpdateValues(this.Context, this);
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
            {
                base.LoadViewState(null);
            }
            else
            {
                Pair pair = (Pair) savedState;
                base.LoadViewState(pair.First);
                if (pair.Second != null)
                {
                    ((IStateManager) this.View).LoadViewState(pair.Second);
                }
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.IPage.LoadComplete += new EventHandler(this.LoadCompleteEventHandler);
            if (this.StoreOriginalValuesInViewState && (this.EnableUpdate || this.EnableDelete))
            {
                this.IPage.RegisterRequiresViewStateEncryption();
            }
        }

        protected internal override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            if (this._view != null)
            {
                this._view.ReleaseSelectContexts();
            }
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

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._view != null)
            {
                ((IStateManager) this._view).TrackViewState();
            }
        }

        public int Update(IDictionary keys, IDictionary values, IDictionary oldValues) => 
            this.View.Update(keys, values, oldValues);

        [ResourceDescription("LinqDataSource_AutoGenerateOrderByClause"), DefaultValue(false), Category("Behavior")]
        public bool AutoGenerateOrderByClause
        {
            get => 
                this.View.AutoGenerateOrderByClause;
            set
            {
                this.View.AutoGenerateOrderByClause = value;
            }
        }

        [DefaultValue(false), Category("Behavior"), ResourceDescription("LinqDataSource_AutoGenerateWhereClause")]
        public bool AutoGenerateWhereClause
        {
            get => 
                this.View.AutoGenerateWhereClause;
            set
            {
                this.View.AutoGenerateWhereClause = value;
            }
        }

        [Category("Behavior"), ResourceDescription("LinqDataSource_AutoPage"), DefaultValue(true)]
        public bool AutoPage
        {
            get => 
                this.View.AutoPage;
            set
            {
                this.View.AutoPage = value;
            }
        }

        [ResourceDescription("LinqDataSource_AutoSort"), DefaultValue(true), Category("Behavior")]
        public bool AutoSort
        {
            get => 
                this.View.AutoSort;
            set
            {
                this.View.AutoSort = value;
            }
        }

        [ResourceDescription("LinqDataSource_ContextTypeName"), Category("Data"), DefaultValue("")]
        public string ContextTypeName
        {
            get => 
                this.View.ContextTypeName;
            set
            {
                this.View.ContextTypeName = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Category("Data"), ResourceDescription("LinqDataSource_DeleteParameters"), Browsable(false), MergableProperty(false), DefaultValue((string) null)]
        public ParameterCollection DeleteParameters =>
            this.View.DeleteParameters;

        [DefaultValue(false), Category("Behavior"), ResourceDescription("LinqDataSource_EnableDelete")]
        public bool EnableDelete
        {
            get => 
                this.View.EnableDelete;
            set
            {
                this.View.EnableDelete = value;
            }
        }

        [ResourceDescription("LinqDataSource_EnableInsert"), DefaultValue(false), Category("Behavior")]
        public bool EnableInsert
        {
            get => 
                this.View.EnableInsert;
            set
            {
                this.View.EnableInsert = value;
            }
        }

        [DefaultValue(true), Category("Behavior"), ResourceDescription("LinqDataSource_EnableObjectTracking")]
        public bool EnableObjectTracking
        {
            get => 
                this.View.EnableObjectTracking;
            set
            {
                this.View.EnableObjectTracking = value;
            }
        }

        [DefaultValue(false), Category("Behavior"), ResourceDescription("LinqDataSource_EnableUpdate")]
        public bool EnableUpdate
        {
            get => 
                this.View.EnableUpdate;
            set
            {
                this.View.EnableUpdate = value;
            }
        }

        [DefaultValue(""), Category("Data"), ResourceDescription("LinqDataSource_GroupBy")]
        public string GroupBy
        {
            get => 
                this.View.GroupBy;
            set
            {
                this.View.GroupBy = value;
            }
        }

        [MergableProperty(false), Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), ResourceDescription("LinqDataSource_GroupByParameters"), Browsable(false)]
        public ParameterCollection GroupByParameters =>
            this.View.GroupByParameters;

        [Browsable(false), DefaultValue((string) null), MergableProperty(false), Category("Data"), ResourceDescription("LinqDataSource_InsertParameters"), PersistenceMode(PersistenceMode.InnerProperty)]
        public ParameterCollection InsertParameters =>
            this.View.InsertParameters;

        internal System.Web.UI.IPage IPage
        {
            get
            {
                if (this._page != null)
                {
                    return this._page;
                }
                Page page = this.Page;
                if (page == null)
                {
                    throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                }
                return new PageWrapper(page);
            }
        }

        [DefaultValue(""), ResourceDescription("LinqDataSource_OrderBy"), Category("Data")]
        public string OrderBy
        {
            get => 
                this.View.OrderBy;
            set
            {
                this.View.OrderBy = value;
            }
        }

        [ResourceDescription("LinqDataSource_OrderByParameters"), MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Category("Data"), DefaultValue((string) null), Browsable(false)]
        public ParameterCollection OrderByParameters =>
            this.View.OrderByParameters;

        [Category("Data"), ResourceDescription("LinqDataSource_OrderGroupsBy"), DefaultValue("")]
        public string OrderGroupsBy
        {
            get => 
                this.View.OrderGroupsBy;
            set
            {
                this.View.OrderGroupsBy = value;
            }
        }

        [ResourceDescription("LinqDataSource_OrderGroupsByParameters"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), MergableProperty(false), Browsable(false), Category("Data")]
        public ParameterCollection OrderGroupsByParameters =>
            this.View.OrderGroupsByParameters;

        [ResourceDescription("LinqDataSource_Select"), DefaultValue(""), Category("Data")]
        public string Select
        {
            get => 
                this.View.SelectNew;
            set
            {
                this.View.SelectNew = value;
            }
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), Category("Data"), ResourceDescription("LinqDataSource_SelectParameters"), Browsable(false)]
        public ParameterCollection SelectParameters =>
            this.View.SelectNewParameters;

        [ResourceDescription("LinqDataSource_StoreOriginalValuesInViewState"), Category("Behavior"), DefaultValue(true)]
        public bool StoreOriginalValuesInViewState
        {
            get => 
                this.View.StoreOriginalValuesInViewState;
            set
            {
                this.View.StoreOriginalValuesInViewState = value;
            }
        }

        Type IDynamicDataSource.ContextType
        {
            get
            {
                if (string.IsNullOrEmpty(this.ContextTypeName))
                {
                    return null;
                }
                return this._view.ContextType;
            }
            set
            {
                this._view.ContextTypeName = value.AssemblyQualifiedName;
            }
        }

        string IDynamicDataSource.EntitySetName
        {
            get => 
                this.TableName;
            set
            {
                this.TableName = value;
            }
        }

        [ResourceDescription("LinqDataSource_TableName"), Category("Data"), DefaultValue("")]
        public string TableName
        {
            get => 
                this.View.TableName;
            set
            {
                this.View.TableName = value;
            }
        }

        [ResourceDescription("LinqDataSource_UpdateParameters"), Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), Browsable(false), MergableProperty(false)]
        public ParameterCollection UpdateParameters =>
            this.View.UpdateParameters;

        internal LinqDataSourceView View
        {
            get
            {
                if (this._view == null)
                {
                    this.View = this.CreateView();
                }
                return this._view;
            }
            private set
            {
                this._view = value;
                if ((this._view != null) && base.IsTrackingViewState)
                {
                    ((IStateManager) this._view).TrackViewState();
                }
            }
        }

        [ResourceDescription("LinqDataSource_Where"), Category("Data"), DefaultValue("")]
        public string Where
        {
            get => 
                this.View.Where;
            set
            {
                this.View.Where = value;
            }
        }

        [Category("Data"), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("LinqDataSource_WhereParameters"), MergableProperty(false), DefaultValue((string) null)]
        public ParameterCollection WhereParameters =>
            this.View.WhereParameters;
    }
}

