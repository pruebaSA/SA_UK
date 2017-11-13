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
    using System.Web.Resources;
    using System.Web.UI;

    [PersistChildren(false), Themeable(true), SupportsEventValidation, Designer("System.Web.UI.Design.WebControls.DataPagerDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), ToolboxBitmap(typeof(DataPager), "DataPager.ico"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataPager : Control, IAttributeAccessor, INamingContainer, ICompositeControlDesignerAccessor
    {
        private System.Web.UI.AttributeCollection _attributes;
        private bool _creatingPagerFields;
        private DataPagerFieldCollection _fields;
        private bool _initialized;
        private int _maximumRows;
        private readonly System.Web.UI.IPage _page;
        private IPageableItemContainer _pageableItemContainer;
        private bool _queryStringHandled;
        private string _queryStringNavigateUrl;
        private bool _setPageProperties;
        private int _startRowIndex;
        private int _totalRowCount;

        public DataPager()
        {
            this._maximumRows = 10;
        }

        internal DataPager(System.Web.UI.IPage page)
        {
            this._maximumRows = 10;
            this._page = page;
        }

        protected virtual void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.ID != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            }
            if (this._attributes != null)
            {
                System.Web.UI.AttributeCollection attributes = this.Attributes;
                IEnumerator enumerator = attributes.Keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string current = (string) enumerator.Current;
                    writer.AddAttribute(current, attributes[current]);
                }
            }
        }

        protected virtual void ConnectToEvents(IPageableItemContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this._pageableItemContainer.TotalRowCountAvailable += new EventHandler<PageEventArgs>(this.OnTotalRowCountAvailable);
        }

        protected virtual void CreatePagerFields()
        {
            this._creatingPagerFields = true;
            this.Controls.Clear();
            if (this._fields != null)
            {
                foreach (DataPagerField field in this._fields)
                {
                    DataPagerFieldItem child = new DataPagerFieldItem(field, this);
                    this.Controls.Add(child);
                    if (field.Visible)
                    {
                        field.CreateDataPagers(child, this._startRowIndex, this._maximumRows, this._totalRowCount, this._fields.IndexOf(field));
                        child.DataBind();
                    }
                }
            }
            this._creatingPagerFields = false;
        }

        public override void DataBind()
        {
            this.OnDataBinding(EventArgs.Empty);
            this.EnsureChildControls();
            this.DataBindChildren();
        }

        protected virtual IPageableItemContainer FindPageableItemContainer()
        {
            if (!string.IsNullOrEmpty(this.PagedControlID))
            {
                Control control = DataBoundControlHelper.FindControl(this, this.PagedControlID);
                if (control == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.DataPager_PageableItemContainerNotFound, new object[] { this.PagedControlID }));
                }
                IPageableItemContainer container = control as IPageableItemContainer;
                if (container == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.DataPager_ControlIsntPageable, new object[] { this.PagedControlID }));
                }
                return container;
            }
            Control namingContainer = this.NamingContainer;
            IPageableItemContainer container2 = null;
            while ((container2 == null) && (namingContainer != this.Page))
            {
                if (namingContainer == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.DataPager_NoNamingContainer, new object[] { this.ID }));
                }
                container2 = namingContainer as IPageableItemContainer;
                namingContainer = namingContainer.NamingContainer;
            }
            return container2;
        }

        internal string GetQueryStringNavigateUrl(int pageNumber)
        {
            if (this._queryStringNavigateUrl == null)
            {
                string queryStringField = this.QueryStringField;
                StringBuilder builder = new StringBuilder();
                if (base.DesignMode)
                {
                    builder.Append("?");
                }
                else
                {
                    bool flag = (this.IPage.Form != null) && this.IPage.Form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);
                    HttpRequestBase request = this.IPage.Request;
                    builder.Append(request.Path);
                    builder.Append("?");
                    foreach (string str2 in request.QueryString.AllKeys)
                    {
                        if ((!flag || !ControlUtil.IsBuiltInHiddenField(str2)) && !str2.Equals(queryStringField, StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Append(HttpUtility.UrlEncode(str2));
                            builder.Append("=");
                            builder.Append(HttpUtility.UrlEncode(request.QueryString[str2]));
                            builder.Append("&");
                        }
                    }
                }
                builder.Append(queryStringField);
                builder.Append("=");
                this._queryStringNavigateUrl = builder.ToString();
            }
            return (this._queryStringNavigateUrl + pageNumber.ToString(CultureInfo.InvariantCulture));
        }

        protected internal override void LoadControlState(object savedState)
        {
            this._startRowIndex = 0;
            this._maximumRows = 10;
            this._totalRowCount = -1;
            object[] objArray = savedState as object[];
            if (objArray != null)
            {
                base.LoadControlState(objArray[0]);
                if (objArray[1] != null)
                {
                    this._startRowIndex = (int) objArray[1];
                }
                if (objArray[2] != null)
                {
                    this._maximumRows = (int) objArray[2];
                }
                if (objArray[3] != null)
                {
                    this._totalRowCount = (int) objArray[3];
                }
            }
            else
            {
                base.LoadControlState(null);
            }
            if (this._pageableItemContainer == null)
            {
                this._pageableItemContainer = this.FindPageableItemContainer();
                if (this._pageableItemContainer == null)
                {
                    throw new InvalidOperationException(AtlasWeb.DataPager_NoPageableItemContainer);
                }
                this.ConnectToEvents(this._pageableItemContainer);
            }
            this._pageableItemContainer.SetPageProperties(this._startRowIndex, this._maximumRows, false);
            this._setPageProperties = true;
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] objArray = (object[]) savedState;
                base.LoadViewState(objArray[0]);
                if (objArray[1] != null)
                {
                    ((IStateManager) this.Fields).LoadViewState(objArray[1]);
                }
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            DataPagerFieldCommandEventArgs args = e as DataPagerFieldCommandEventArgs;
            bool flag = false;
            if (args != null)
            {
                DataPagerFieldItem item = args.Item;
                if ((item != null) && (item.PagerField != null))
                {
                    item.PagerField.HandleEvent(args);
                    flag = true;
                }
            }
            return flag;
        }

        private void OnFieldsChanged(object source, EventArgs e)
        {
            if (this._initialized)
            {
                this.SetPageProperties(this._startRowIndex, this._maximumRows, true);
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!base.DesignMode)
            {
                this._pageableItemContainer = this.FindPageableItemContainer();
                if (this._pageableItemContainer != null)
                {
                    this.ConnectToEvents(this._pageableItemContainer);
                    this._pageableItemContainer.SetPageProperties(this._startRowIndex, this._maximumRows, false);
                    this._setPageProperties = true;
                }
                if (this.Page != null)
                {
                    this.Page.RegisterRequiresControlState(this);
                }
            }
            this._initialized = true;
        }

        protected internal override void OnLoad(EventArgs e)
        {
            if (this._pageableItemContainer == null)
            {
                this._pageableItemContainer = this.FindPageableItemContainer();
            }
            if (this._pageableItemContainer == null)
            {
                throw new InvalidOperationException(AtlasWeb.DataPager_NoPageableItemContainer);
            }
            if (!this._setPageProperties)
            {
                this.ConnectToEvents(this._pageableItemContainer);
                this._pageableItemContainer.SetPageProperties(this._startRowIndex, this._maximumRows, false);
                this._setPageProperties = true;
            }
            base.OnLoad(e);
        }

        protected virtual void OnTotalRowCountAvailable(object sender, PageEventArgs e)
        {
            this._totalRowCount = e.TotalRowCount;
            this._startRowIndex = e.StartRowIndex;
            this._maximumRows = e.MaximumRows;
            if ((this._totalRowCount <= this._startRowIndex) && (this._totalRowCount > 0))
            {
                int startRowIndex = this._startRowIndex - this._maximumRows;
                if (startRowIndex < 0)
                {
                    startRowIndex = 0;
                }
                if (startRowIndex >= this._totalRowCount)
                {
                    startRowIndex = 0;
                }
                this._pageableItemContainer.SetPageProperties(startRowIndex, this._maximumRows, true);
            }
            else if (!this._creatingPagerFields)
            {
                this.CreatePagerFields();
            }
        }

        protected virtual void RecreateChildControls()
        {
            base.ChildControlsCreated = false;
            this.EnsureChildControls();
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                this.EnsureChildControls();
                this.OnTotalRowCountAvailable(null, new PageEventArgs(0, this.PageSize, 0x65));
            }
            this.RenderBeginTag(writer);
            this.RenderContents(writer);
            writer.RenderEndTag();
        }

        public virtual void RenderBeginTag(HtmlTextWriter writer)
        {
            this.AddAttributesToRender(writer);
            writer.RenderBeginTag(this.TagKey);
        }

        protected virtual void RenderContents(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        protected internal override object SaveControlState()
        {
            object obj2 = base.SaveControlState();
            if (((obj2 != null) || (this._startRowIndex != 0)) || ((this._maximumRows != 10) || (this._totalRowCount != -1)))
            {
                return new object[] { obj2, ((this._startRowIndex == 0) ? null : ((object) this._startRowIndex)), ((this._maximumRows == 10) ? null : ((object) this._maximumRows)), ((this._totalRowCount == -1) ? null : ((object) this._totalRowCount)) };
            }
            return null;
        }

        protected override object SaveViewState()
        {
            object obj2 = base.SaveViewState();
            object obj3 = (this._fields != null) ? ((IStateManager) this._fields).SaveViewState() : null;
            return new object[] { obj2, obj3 };
        }

        public virtual void SetPageProperties(int startRowIndex, int maximumRows, bool databind)
        {
            if (!base.DesignMode)
            {
                if (this._pageableItemContainer == null)
                {
                    throw new InvalidOperationException(AtlasWeb.DataPager_PagePropertiesCannotBeSet);
                }
                this._startRowIndex = startRowIndex;
                this._maximumRows = maximumRows;
                this._pageableItemContainer.SetPageProperties(startRowIndex, maximumRows, databind);
            }
        }

        string IAttributeAccessor.GetAttribute(string name) => 
            this.Attributes[name];

        void IAttributeAccessor.SetAttribute(string name, string value)
        {
            this.Attributes[name] = value;
        }

        void ICompositeControlDesignerAccessor.RecreateChildControls()
        {
            this.RecreateChildControls();
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._fields != null)
            {
                ((IStateManager) this._fields).TrackViewState();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new System.Web.UI.AttributeCollection(new StateBag(true));
                }
                return this._attributes;
            }
        }

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        [Editor("System.Web.UI.Design.WebControls.DataPagerFieldTypeEditor, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor)), MergableProperty(false), ResourceDescription("DataPager_Fields"), PersistenceMode(PersistenceMode.InnerProperty), Category("Default"), DefaultValue((string) null)]
        public virtual DataPagerFieldCollection Fields
        {
            get
            {
                if (this._fields == null)
                {
                    this._fields = new DataPagerFieldCollection(this);
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._fields).TrackViewState();
                    }
                    this._fields.FieldsChanged += new EventHandler(this.OnFieldsChanged);
                }
                return this._fields;
            }
        }

        private bool HasAttributes =>
            ((this._attributes != null) && (this._attributes.Count > 0));

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public int MaximumRows =>
            this._maximumRows;

        [Themeable(false), ResourceDescription("DataPager_PagedControlID"), DefaultValue(""), IDReferenceProperty(typeof(IPageableItemContainer)), WebCategory("Paging")]
        public virtual string PagedControlID
        {
            get
            {
                object obj2 = this.ViewState["PagedControlID"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["PagedControlID"] = value;
            }
        }

        [DefaultValue(10), ResourceDescription("DataPager_PageSize"), WebCategory("Paging")]
        public int PageSize
        {
            get => 
                this._maximumRows;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._maximumRows)
                {
                    this._maximumRows = value;
                    if (this._initialized)
                    {
                        this.CreatePagerFields();
                        this.SetPageProperties(this._startRowIndex, this._maximumRows, true);
                    }
                }
            }
        }

        [DefaultValue(""), WebCategory("Paging"), ResourceDescription("DataPager_QueryStringField")]
        public string QueryStringField
        {
            get
            {
                object obj2 = this.ViewState["QueryStringField"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["QueryStringField"] = value;
            }
        }

        internal bool QueryStringHandled
        {
            get => 
                this._queryStringHandled;
            set
            {
                this._queryStringHandled = value;
            }
        }

        internal string QueryStringValue
        {
            get
            {
                if (base.DesignMode)
                {
                    return string.Empty;
                }
                return this.IPage.Request.QueryString[this.QueryStringField];
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StartRowIndex =>
            this._startRowIndex;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        protected virtual HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Span;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TotalRowCount =>
            this._totalRowCount;
    }
}

