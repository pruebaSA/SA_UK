namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public class LinqDataSourceDesigner : DataSourceDesigner
    {
        private ILinqDataSourceDesignerHelper _helper;
        internal const string DefaultViewName = "DefaultView";

        public LinqDataSourceDesigner()
        {
        }

        internal LinqDataSourceDesigner(ILinqDataSourceDesignerHelper helper)
        {
            this._helper = helper;
        }

        public override void Configure()
        {
            ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.ConfigureDataSourceChangeCallback), null, AtlasWebDesign.LinqDataSourceDesigner_WizardTransactionDescription);
        }

        private bool ConfigureDataSourceChangeCallback(object context)
        {
            bool flag;
            try
            {
                this.SuppressDataSourceEvents();
                LinqDataSourceWizardForm form = new LinqDataSourceWizardForm(this.ServiceProvider, this._helper.GetLinqDataSourceState(), this);
                if (UIServiceHelper.ShowDialog(base.Component.Site, form) == DialogResult.OK)
                {
                    this._helper.SetLinqDataSourceState(form.LinqDataSourceState);
                    this.OnDataSourceChanged(EventArgs.Empty);
                    this.RefreshSchema(true);
                    return true;
                }
                flag = false;
            }
            finally
            {
                this.ResumeDataSourceEvents();
            }
            return flag;
        }

        internal void FireOnDataSourceChanged(EventArgs e)
        {
            this.OnDataSourceChanged(e);
        }

        internal void FireOnSchemaRefreshed(EventArgs e)
        {
            this.OnSchemaRefreshed(e);
        }

        public override DesignerDataSourceView GetView(string viewName) => 
            this._helper.GetView(viewName);

        public override string[] GetViewNames() => 
            this._helper.GetViewNames();

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            this._helper = new LinqDataSourceDesignerHelper(this);
            this._helper.SetWrapper(new LinqDataSourceWrapper(component as LinqDataSource, this._helper));
        }

        internal bool InternalViewSchemasEquivalent(IDataSourceViewSchema viewSchema1, IDataSourceViewSchema viewSchema2) => 
            DataSourceDesigner.ViewSchemasEquivalent(viewSchema1, viewSchema2);

        internal virtual object LoadFromDesignerState(string key) => 
            base.DesignerState[key];

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            this._helper.PreFilterProperties(properties, base.GetType());
        }

        public override void RefreshSchema(bool preferSilent)
        {
            this._helper.RefreshSchema(this._helper, preferSilent);
        }

        internal virtual void SaveDesignerState(string key, object value)
        {
            base.DesignerState[key] = value;
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new LinqDataSourceDesignerActionList(this, this._helper));
                return lists;
            }
        }

        public override bool CanConfigure =>
            this._helper.CanConfigure(base.Component.Site);

        public override bool CanRefreshSchema =>
            this._helper.CanRefreshSchema(base.Component.Site);

        [DefaultValue((string) null), Category("Data"), MergableProperty(false), TypeConverter(typeof(LinqDataSourceContextTypeConverter)), ResourceDescription("LinqDataSource_ContextTypeName")]
        public string ContextTypeName
        {
            get => 
                this._helper.ContextTypeName;
            set
            {
                this._helper.ContextTypeName = value;
            }
        }

        [MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), Category("Data"), TypeConverter(typeof(LinqDataSourceInsertUpdateDeleteStatementConverter)), ResourceDescription("LinqDataSource_DeleteParameters"), DefaultValue((string) null)]
        public string Delete
        {
            get => 
                null;
            set
            {
            }
        }

        public bool EnableDelete
        {
            get => 
                this._helper.EnableDelete;
            set
            {
                this._helper.EnableDelete = value;
            }
        }

        public bool EnableInsert
        {
            get => 
                this._helper.EnableInsert;
            set
            {
                this._helper.EnableInsert = value;
            }
        }

        public bool EnableUpdate
        {
            get => 
                this._helper.EnableUpdate;
            set
            {
                this._helper.EnableUpdate = value;
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_GroupBy"), DefaultValue(""), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), MergableProperty(false)]
        public string GroupBy
        {
            get => 
                this._helper.GroupBy;
            set
            {
                this._helper.GroupBy = value;
            }
        }

        internal ILinqDataSourceDesignerHelper Helper =>
            this._helper;

        [ResourceDescription("LinqDataSource_InsertParameters"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), MergableProperty(false), Category("Data"), DefaultValue((string) null), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), TypeConverter(typeof(LinqDataSourceInsertUpdateDeleteStatementConverter))]
        public string Insert
        {
            get => 
                null;
            set
            {
            }
        }

        [MergableProperty(false), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), Category("Data"), ResourceDescription("LinqDataSource_OrderBy"), DefaultValue("")]
        public string OrderBy
        {
            get => 
                this._helper.OrderBy;
            set
            {
                this._helper.OrderBy = value;
            }
        }

        [Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), MergableProperty(false), ResourceDescription("LinqDataSource_OrderGroupsBy"), DefaultValue(""), Category("Data")]
        public string OrderGroupsBy
        {
            get => 
                this._helper.OrderGroupsBy;
            set
            {
                this._helper.OrderGroupsBy = value;
            }
        }

        [DefaultValue(""), MergableProperty(false), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), Category("Data"), ResourceDescription("LinqDataSource_Select")]
        public string Select
        {
            get => 
                this._helper.Select;
            set
            {
                this._helper.Select = value;
            }
        }

        public IServiceProvider ServiceProvider =>
            this._helper.ServiceProvider;

        [Category("Data"), DefaultValue((string) null), TypeConverter(typeof(LinqDataSourceTableConverter)), MergableProperty(false), ResourceDescription("LinqDataSource_TableName")]
        public string TableName
        {
            get => 
                this._helper.TableName;
            set
            {
                this._helper.TableName = value;
            }
        }

        [MergableProperty(false), TypeConverter(typeof(LinqDataSourceInsertUpdateDeleteStatementConverter)), Category("Data"), ResourceDescription("LinqDataSource_UpdateParameters"), DefaultValue((string) null), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Update
        {
            get => 
                null;
            set
            {
            }
        }

        [Category("Data"), ResourceDescription("LinqDataSource_Where"), Editor(typeof(LinqDataSourceStatementEditor), typeof(UITypeEditor)), DefaultValue(""), MergableProperty(false)]
        public string Where
        {
            get => 
                this._helper.Where;
            set
            {
                this._helper.Where = value;
            }
        }

        internal class LinqDataSourceDesignerActionList : DesignerActionList
        {
            private ILinqDataSourceDesignerHelper _helper;

            public LinqDataSourceDesignerActionList(LinqDataSourceDesigner parent, ILinqDataSourceDesignerHelper helper) : base(parent.Component)
            {
                this._helper = helper;
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();
                if (this._helper.CanInsertUpdateDelete(this._helper))
                {
                    items.Add(new DesignerActionPropertyItem("EnableDelete", AtlasWebDesign.LinqDataSourceDesignerActionList_EnableDelete));
                    items.Add(new DesignerActionPropertyItem("EnableInsert", AtlasWebDesign.LinqDataSourceDesignerActionList_EnableInsert));
                    items.Add(new DesignerActionPropertyItem("EnableUpdate", AtlasWebDesign.LinqDataSourceDesignerActionList_EnableUpdate));
                }
                return items;
            }

            public bool EnableDelete
            {
                get => 
                    this._helper.EnableDelete;
                set
                {
                    this._helper.EnableDelete = value;
                }
            }

            public bool EnableInsert
            {
                get => 
                    this._helper.EnableInsert;
                set
                {
                    this._helper.EnableInsert = value;
                }
            }

            public bool EnableUpdate
            {
                get => 
                    this._helper.EnableUpdate;
                set
                {
                    this._helper.EnableUpdate = value;
                }
            }
        }
    }
}

