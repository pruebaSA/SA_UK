namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ToolboxBitmap(typeof(Accordion), "Accordion.Accordion.ico"), ParseChildren(true), Designer("AjaxControlToolkit.AccordionDesigner, AjaxControlToolkit"), ToolboxData("<{0}:Accordion runat=server></{0}:Accordion>")]
    public class Accordion : WebControl
    {
        private DataSourceSelectArguments _arguments;
        private ITemplate _contentTemplate;
        private DataSourceView _currentView;
        private bool _currentViewIsFromDataSourceID;
        private bool _currentViewValid;
        private object _dataSource;
        private AjaxControlToolkit.AccordionExtender _extender;
        private ITemplate _headerTemplate;
        private bool _initialized;
        private bool _pagePreLoadFired;
        private AccordionPaneCollection _panes;
        private bool _requiresDataBinding;
        private IEnumerable _selectResult;
        private EventWaitHandle _selectWait;
        private bool _throwOnDataPropertyChange;
        internal const string ItemCountViewStateKey = "_!ItemCount";

        public event CommandEventHandler ItemCommand;

        public event EventHandler<AccordionItemEventArgs> ItemCreated;

        public event EventHandler<AccordionItemEventArgs> ItemDataBound;

        public Accordion() : base(HtmlTextWriterTag.Div)
        {
        }

        internal void ClearPanes()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is AccordionPane)
                {
                    this.Controls.RemoveAt(i);
                }
            }
        }

        private DataSourceView ConnectToDataSourceView()
        {
            if (!this._currentViewValid || base.DesignMode)
            {
                if ((this._currentView != null) && this._currentViewIsFromDataSourceID)
                {
                    this._currentView.DataSourceViewChanged -= new EventHandler(this.OnDataSourceViewChanged);
                }
                IDataSource source = null;
                string dataSourceID = this.DataSourceID;
                if (!string.IsNullOrEmpty(dataSourceID))
                {
                    Control control = this.NamingContainer.FindControl(dataSourceID);
                    if (control == null)
                    {
                        throw new HttpException(string.Format(CultureInfo.CurrentCulture, "DataSource '{1}' for control '{0}' doesn't exist", new object[] { this.ID, dataSourceID }));
                    }
                    source = control as IDataSource;
                    if (source == null)
                    {
                        throw new HttpException(string.Format(CultureInfo.CurrentCulture, "'{1}' is not a data source for control '{0}'.", new object[] { this.ID, dataSourceID }));
                    }
                }
                if (source == null)
                {
                    return null;
                }
                if (this.DataSource != null)
                {
                    throw new InvalidOperationException("DataSourceID and DataSource can't be set at the same time.");
                }
                DataSourceView view = source.GetView(this.DataMember);
                if (view == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "DataSourceView not found for control '{0}'", new object[] { this.ID }));
                }
                this._currentViewIsFromDataSourceID = this.IsBoundUsingDataSourceID;
                this._currentView = view;
                if ((this._currentView != null) && this._currentViewIsFromDataSourceID)
                {
                    this._currentView.DataSourceViewChanged += new EventHandler(this.OnDataSourceViewChanged);
                }
                this._currentViewValid = true;
            }
            return this._currentView;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if ((this.AccordionExtender != null) && (this.ViewState["_!ItemCount"] != null))
            {
                this.CreateControlHierarchy(false);
            }
            base.ClearChildViewState();
            foreach (AccordionPane pane in this.Panes)
            {
                ControlCollection controls = pane.Controls;
            }
        }

        protected virtual void CreateControlHierarchy(bool useDataSource)
        {
            int capacity = -1;
            IEnumerable data = null;
            List<AccordionPane> list = new List<AccordionPane>();
            if (!useDataSource)
            {
                object obj2 = this.ViewState["_!ItemCount"];
                if (obj2 != null)
                {
                    capacity = (int) obj2;
                    if (capacity != -1)
                    {
                        List<object> list2 = new List<object>(capacity);
                        for (int i = 0; i < capacity; i++)
                        {
                            list2.Add(null);
                        }
                        data = list2;
                        list.Capacity = capacity;
                    }
                }
            }
            else
            {
                data = this.GetData();
                capacity = 0;
                ICollection is2 = data as ICollection;
                if (is2 != null)
                {
                    list.Capacity = is2.Count;
                }
            }
            if (data != null)
            {
                int index = 0;
                foreach (object obj3 in data)
                {
                    AccordionPane child = new AccordionPane {
                        ID = string.Format(CultureInfo.InvariantCulture, "{0}_Pane_{1}", new object[] { 
                            this.ID,
                            index.ToString(CultureInfo.InvariantCulture)
                        })
                    };
                    this.Controls.Add(child);
                    this.CreateItem(obj3, index, AccordionItemType.Header, child.HeaderContainer, this.HeaderTemplate, useDataSource);
                    this.CreateItem(obj3, index, AccordionItemType.Content, child.ContentContainer, this.ContentTemplate, useDataSource);
                    list.Add(child);
                    capacity++;
                    index++;
                }
            }
            if (useDataSource)
            {
                this.ViewState["_!ItemCount"] = (data != null) ? capacity : -1;
            }
        }

        protected virtual DataSourceSelectArguments CreateDataSourceSelectArguments() => 
            DataSourceSelectArguments.Empty;

        private void CreateItem(object dataItem, int index, AccordionItemType itemType, AccordionContentPanel container, ITemplate template, bool dataBind)
        {
            if (template != null)
            {
                AccordionItemEventArgs args = new AccordionItemEventArgs(container, itemType);
                this.OnItemCreated(args);
                container.SetDataItemProperties(dataItem, index, itemType);
                template.InstantiateIn(container);
                if (dataBind)
                {
                    container.DataBind();
                    this.OnItemDataBound(args);
                }
            }
        }

        public override void DataBind()
        {
            if ((!this.IsBoundUsingDataSourceID || !base.DesignMode) || (base.Site != null))
            {
                this.RequiresDataBinding = false;
                this.OnDataBinding(EventArgs.Empty);
            }
        }

        private void DoSelect(IEnumerable data)
        {
            this._selectResult = data;
            this._selectWait.Set();
        }

        protected void EnsureDataBound()
        {
            try
            {
                this._throwOnDataPropertyChange = true;
                if (this.RequiresDataBinding && !string.IsNullOrEmpty(this.DataSourceID))
                {
                    this.DataBind();
                }
            }
            finally
            {
                this._throwOnDataPropertyChange = false;
            }
        }

        public override Control FindControl(string id)
        {
            Control control = base.FindControl(id);
            if (control == null)
            {
                foreach (AccordionPane pane in this.Panes)
                {
                    control = pane.FindControl(id);
                    if (control != null)
                    {
                        return control;
                    }
                }
            }
            return control;
        }

        protected virtual IEnumerable GetData()
        {
            this._selectResult = null;
            DataSourceView view = this.ConnectToDataSourceView();
            if (view != null)
            {
                this._selectWait = new EventWaitHandle(false, EventResetMode.AutoReset);
                view.Select(this.SelectArguments, new DataSourceViewSelectCallback(this.DoSelect));
                this._selectWait.WaitOne();
            }
            else if (this.DataSource != null)
            {
                this._selectResult = this.DataSource as IEnumerable;
            }
            return this._selectResult;
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool flag = false;
            AccordionCommandEventArgs args2 = args as AccordionCommandEventArgs;
            if (args2 != null)
            {
                this.OnItemCommand(args2);
                flag = true;
            }
            return flag;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            if ((this.DataSource != null) || this.IsBoundUsingDataSourceID)
            {
                this.ClearPanes();
                base.ClearChildViewState();
                this.CreateControlHierarchy(true);
                base.ChildControlsCreated = true;
            }
        }

        protected virtual void OnDataPropertyChanged()
        {
            if (this._throwOnDataPropertyChange)
            {
                throw new HttpException("Invalid data property change");
            }
            if (this._initialized)
            {
                this.RequiresDataBinding = true;
            }
            this._currentViewValid = false;
        }

        protected virtual void OnDataSourceViewChanged(object sender, EventArgs args)
        {
            this.RequiresDataBinding = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.Page != null)
            {
                this.Page.PreLoad += new EventHandler(this.OnPagePreLoad);
                if (!base.IsViewStateEnabled && this.Page.IsPostBack)
                {
                    this.RequiresDataBinding = true;
                }
            }
        }

        protected virtual void OnItemCommand(AccordionCommandEventArgs args)
        {
            if (this.ItemCommand != null)
            {
                this.ItemCommand(this, args);
            }
        }

        protected virtual void OnItemCreated(AccordionItemEventArgs args)
        {
            if (this.ItemCreated != null)
            {
                this.ItemCreated(this, args);
            }
        }

        protected virtual void OnItemDataBound(AccordionItemEventArgs args)
        {
            if (this.ItemDataBound != null)
            {
                this.ItemDataBound(this, args);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            this._initialized = true;
            this.ConnectToDataSourceView();
            if (((this.Page != null) && !this._pagePreLoadFired) && (this.ViewState["_!ItemCount"] == null))
            {
                if (!this.Page.IsPostBack)
                {
                    this.RequiresDataBinding = true;
                }
                else if (base.IsViewStateEnabled)
                {
                    this.RequiresDataBinding = true;
                }
            }
            base.OnLoad(e);
        }

        private void OnPagePreLoad(object sender, EventArgs e)
        {
            this._initialized = true;
            if (this.Page != null)
            {
                this.Page.PreLoad -= new EventHandler(this.OnPagePreLoad);
                if (!this.Page.IsPostBack)
                {
                    this.RequiresDataBinding = true;
                }
                if ((this.Page.IsPostBack && base.IsViewStateEnabled) && (this.ViewState["_!ItemCount"] == null))
                {
                    this.RequiresDataBinding = true;
                }
                this._pagePreLoadFired = true;
            }
            this.EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.EnsureDataBound();
            base.OnPreRender(e);
            if (this.AutoSize != AjaxControlToolkit.AutoSize.None)
            {
                base.Style[HtmlTextWriterStyle.Overflow] = "hidden";
                base.Style[HtmlTextWriterStyle.OverflowX] = "auto";
            }
            foreach (AccordionPane pane in this.Panes)
            {
                if (pane.HeaderCssClass == this.HeaderSelectedCssClass)
                {
                    pane.HeaderCssClass = string.Empty;
                }
                if (!string.IsNullOrEmpty(this.HeaderCssClass) && string.IsNullOrEmpty(pane.HeaderCssClass))
                {
                    pane.HeaderCssClass = this.HeaderCssClass;
                }
                if (!string.IsNullOrEmpty(this.ContentCssClass) && string.IsNullOrEmpty(pane.ContentCssClass))
                {
                    pane.ContentCssClass = this.ContentCssClass;
                }
            }
            int selectedIndex = this.AccordionExtender.SelectedIndex;
            selectedIndex = (((selectedIndex < 0) || (selectedIndex >= this.Panes.Count)) && this.AccordionExtender.RequireOpenedPane) ? 0 : selectedIndex;
            if ((selectedIndex >= 0) && (selectedIndex < this.Panes.Count))
            {
                AccordionContentPanel contentContainer = this.Panes[selectedIndex].ContentContainer;
                if (contentContainer != null)
                {
                    contentContainer.Collapsed = false;
                }
                if (!string.IsNullOrEmpty(this.HeaderSelectedCssClass))
                {
                    this.Panes[selectedIndex].HeaderCssClass = this.HeaderSelectedCssClass;
                }
            }
        }

        private AjaxControlToolkit.AccordionExtender AccordionExtender
        {
            get
            {
                if (this._extender == null)
                {
                    this._extender = new AjaxControlToolkit.AccordionExtender();
                    this._extender.ID = this.ID + "_AccordionExtender";
                    this._extender.TargetControlID = this.ID;
                    this.Controls.AddAt(0, this._extender);
                }
                return this._extender;
            }
        }

        [Browsable(true), Description("Determine how the growth of the Accordion will be controlled"), Category("Behavior"), DefaultValue(0)]
        public AjaxControlToolkit.AutoSize AutoSize
        {
            get => 
                this.AccordionExtender.AutoSize;
            set
            {
                this.AccordionExtender.AutoSize = value;
            }
        }

        [Category("Appearance"), Description("Default CSS class for Accordion Pane Content"), Browsable(true)]
        public string ContentCssClass
        {
            get => 
                this.AccordionExtender.ContentCssClass;
            set
            {
                this.AccordionExtender.ContentCssClass = value;
            }
        }

        [Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(AccordionContentPanel))]
        public virtual ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [DefaultValue(""), Category("Data")]
        public virtual string DataMember
        {
            get => 
                ((this.ViewState["DataMember"] as string) ?? string.Empty);
            set
            {
                this.ViewState["DataMember"] = value;
                this.OnDataPropertyChanged();
            }
        }

        [DefaultValue((string) null), Bindable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Data")]
        public virtual object DataSource
        {
            get => 
                this._dataSource;
            set
            {
                if (((value != null) && !(value is IListSource)) && !(value is IEnumerable))
                {
                    throw new ArgumentException("Can't bind to value that is not an IListSource or an IEnumerable.");
                }
                this._dataSource = value;
                this.OnDataPropertyChanged();
            }
        }

        [Category("Data"), DefaultValue(""), IDReferenceProperty(typeof(DataSourceControl))]
        public virtual string DataSourceID
        {
            get => 
                ((this.ViewState["DataSourceID"] as string) ?? string.Empty);
            set
            {
                this.ViewState["DataSourceID"] = value;
                this.OnDataPropertyChanged();
            }
        }

        [DefaultValue(false), Category("Behavior"), Browsable(true), Description("Whether or not to use a fade effect in the transition animations")]
        public bool FadeTransitions
        {
            get => 
                this.AccordionExtender.FadeTransitions;
            set
            {
                this.AccordionExtender.FadeTransitions = value;
            }
        }

        [DefaultValue(15), Browsable(true), Category("Behavior"), Description("Number of frames per second used in the transition animation")]
        public int FramesPerSecond
        {
            get => 
                this.AccordionExtender.FramesPerSecond;
            set
            {
                this.AccordionExtender.FramesPerSecond = value;
            }
        }

        [Description("Default CSS class for Accordion Pane Headers"), Browsable(true), Category("Appearance")]
        public string HeaderCssClass
        {
            get => 
                this.AccordionExtender.HeaderCssClass;
            set
            {
                this.AccordionExtender.HeaderCssClass = value;
            }
        }

        [Description("Default CSS class for the selected Accordion Pane Headers"), Category("Appearance"), Browsable(true)]
        public string HeaderSelectedCssClass
        {
            get => 
                this.AccordionExtender.HeaderSelectedCssClass;
            set
            {
                this.AccordionExtender.HeaderSelectedCssClass = value;
            }
        }

        [Browsable(false), TemplateContainer(typeof(AccordionContentPanel)), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate HeaderTemplate
        {
            get => 
                this._headerTemplate;
            set
            {
                this._headerTemplate = value;
            }
        }

        protected bool IsBoundUsingDataSourceID =>
            !string.IsNullOrEmpty(this.DataSourceID);

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AccordionPaneCollection Panes
        {
            get
            {
                if (this._panes == null)
                {
                    this._panes = new AccordionPaneCollection(this);
                }
                return this._panes;
            }
        }

        [Category("Behavior"), DefaultValue(true), Browsable(true), Description("Whether or not clicking the header will close the currently opened pane (leaving all the Accordion's panes closed)")]
        public bool RequireOpenedPane
        {
            get => 
                this.AccordionExtender.RequireOpenedPane;
            set
            {
                this.AccordionExtender.RequireOpenedPane = value;
            }
        }

        protected bool RequiresDataBinding
        {
            get => 
                this._requiresDataBinding;
            set
            {
                this._requiresDataBinding = value;
            }
        }

        protected DataSourceSelectArguments SelectArguments
        {
            get
            {
                if (this._arguments == null)
                {
                    this._arguments = this.CreateDataSourceSelectArguments();
                }
                return this._arguments;
            }
        }

        [DefaultValue(0), Browsable(true), Category("Behavior"), Description("Index of the AccordionPane to be displayed")]
        public int SelectedIndex
        {
            get => 
                this.AccordionExtender.SelectedIndex;
            set
            {
                this.AccordionExtender.SelectedIndex = value;
            }
        }

        [Description("Whether or not we suppress the client-side click handlers of any elements in the header sections"), DefaultValue(false), Browsable(true), Category("Behavior")]
        public bool SuppressHeaderPostbacks
        {
            get => 
                this.AccordionExtender.SuppressHeaderPostbacks;
            set
            {
                this.AccordionExtender.SuppressHeaderPostbacks = value;
            }
        }

        [Browsable(true), DefaultValue(500), Category("Behavior"), Description("Length of the transition animation in milliseconds")]
        public int TransitionDuration
        {
            get => 
                this.AccordionExtender.TransitionDuration;
            set
            {
                this.AccordionExtender.TransitionDuration = value;
            }
        }
    }
}

