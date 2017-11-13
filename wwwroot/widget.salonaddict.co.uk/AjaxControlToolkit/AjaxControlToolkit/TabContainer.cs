namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientCssResource("Tabs.Tabs_resource.css"), ClientScriptResource("Sys.Extended.UI.TabContainer", "Tabs.Tabs.js"), ToolboxBitmap(typeof(TabContainer), "Tabs.Tabs.ico"), Designer("AjaxControlToolkit.TabContainerDesigner, AjaxControlToolkit"), ParseChildren(typeof(TabPanel)), RequiredScript(typeof(CommonToolkitScripts))]
    public class TabContainer : ScriptControlBase, IPostBackEventHandler
    {
        private int _activeTabIndex;
        private bool _autoPostBack;
        private int _cachedActiveTabIndex;
        private bool _initialized;
        private bool _onDemand;
        private AjaxControlToolkit.TabStripPlacement _tabStripPlacement;
        private bool _useVerticalStripPlacement;
        private Unit _verticalStripWidth;
        private static readonly object EventActiveTabChanged = new object();

        [Category("Behavior")]
        public event EventHandler ActiveTabChanged
        {
            add
            {
                base.Events.AddHandler(EventActiveTabChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventActiveTabChanged, value);
            }
        }

        public TabContainer() : base(true, HtmlTextWriterTag.Div)
        {
            this._activeTabIndex = -1;
            this._cachedActiveTabIndex = -1;
            this._verticalStripWidth = new Unit(120.0, UnitType.Pixel);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.Style.Remove(HtmlTextWriterStyle.Visibility);
            if (!base.ControlStyleCreated)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_xp");
            }
            if (this._useVerticalStripPlacement)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            if (!this.Height.IsEmpty && (this.Height.Type == UnitType.Percentage))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
            }
            base.AddAttributesToRender(writer);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
        }

        protected override void AddedControl(Control control, int index)
        {
            ((TabPanel) control).SetOwner(this);
            base.AddedControl(control, index);
        }

        protected override void AddParsedSubObject(object obj)
        {
            TabPanel child = obj as TabPanel;
            if (child != null)
            {
                this.Controls.Add(child);
            }
            else if (!(obj is LiteralControl))
            {
                throw new HttpException(string.Format(CultureInfo.CurrentCulture, "TabContainer cannot have children of type '{0}'.", new object[] { obj.GetType() }));
            }
        }

        protected override ControlCollection CreateControlCollection() => 
            new TabPanelCollection(this);

        protected override Style CreateControlStyle() => 
            new TabContainerStyle(this.ViewState) { CssClass = "ajax__tab_xp" };

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddProperty("tabStripPlacement", this.TabStripPlacement);
            descriptor.AddProperty("useVerticalStripPlacement", this.UseVerticalStripPlacement);
            descriptor.AddProperty("onDemand", this.OnDemand);
        }

        private void EnsureActiveTab()
        {
            if ((this._activeTabIndex < 0) || (this._activeTabIndex >= this.Tabs.Count))
            {
                this._activeTabIndex = 0;
            }
            for (int i = 0; i < this.Tabs.Count; i++)
            {
                if (i == this.ActiveTabIndex)
                {
                    this.Tabs[i].Active = true;
                }
                else
                {
                    this.Tabs[i].Active = false;
                }
            }
        }

        private int getServerActiveTabIndex(int clientActiveTabIndex)
        {
            int num = -1;
            int num2 = clientActiveTabIndex;
            for (int i = 0; i < this.Tabs.Count; i++)
            {
                if (this.Tabs[i].Visible)
                {
                    num++;
                }
                if (num == clientActiveTabIndex)
                {
                    return i;
                }
            }
            return num2;
        }

        private string GetSuffixTabStripPlacementCss()
        {
            string str = "";
            if (this._useVerticalStripPlacement)
            {
                str = str + "_vertical";
                switch (this._tabStripPlacement)
                {
                    case AjaxControlToolkit.TabStripPlacement.Top:
                    case AjaxControlToolkit.TabStripPlacement.Bottom:
                        return (str + "left");

                    case AjaxControlToolkit.TabStripPlacement.TopRight:
                    case AjaxControlToolkit.TabStripPlacement.BottomRight:
                        return (str + "right");
                }
                return str;
            }
            switch (this._tabStripPlacement)
            {
                case AjaxControlToolkit.TabStripPlacement.Bottom:
                case AjaxControlToolkit.TabStripPlacement.BottomRight:
                    return "_bottom";

                case AjaxControlToolkit.TabStripPlacement.TopRight:
                    return str;
            }
            return str;
        }

        protected override void LoadClientState(string clientState)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>) new JavaScriptSerializer().DeserializeObject(clientState);
            if (dictionary != null)
            {
                this.ActiveTabIndex = (int) dictionary["ActiveTabIndex"];
                this.ActiveTabIndex = this.getServerActiveTabIndex(this.ActiveTabIndex);
                object[] objArray = (object[]) dictionary["TabState"];
                for (int i = 0; i < objArray.Length; i++)
                {
                    int num2 = this.getServerActiveTabIndex(i);
                    if (num2 < this.Tabs.Count)
                    {
                        this.Tabs[num2].Enabled = (bool) objArray[i];
                    }
                }
            }
        }

        protected override void LoadControlState(object savedState)
        {
            Pair pair = (Pair) savedState;
            if (pair != null)
            {
                base.LoadControlState(pair.First);
                this.ActiveTabIndex = (int) pair.Second;
            }
            else
            {
                base.LoadControlState(null);
            }
        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            int activeTabIndex = this.ActiveTabIndex;
            bool flag = base.LoadPostData(postDataKey, postCollection);
            if ((this.ActiveTabIndex != 0) && (activeTabIndex == this.ActiveTabIndex))
            {
                return flag;
            }
            return true;
        }

        protected virtual void OnActiveTabChanged(EventArgs e)
        {
            EventHandler handler = base.Events[EventActiveTabChanged] as EventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.RegisterRequiresControlState(this);
            this._initialized = true;
            if (this._cachedActiveTabIndex > -1)
            {
                this.ActiveTabIndex = this._cachedActiveTabIndex;
                if (this.ActiveTabIndex < this.Tabs.Count)
                {
                    this.Tabs[this.ActiveTabIndex].Active = true;
                }
            }
            else if (this.Tabs.Count > 0)
            {
                this.ActiveTabIndex = 0;
            }
        }

        protected override void RaisePostDataChangedEvent()
        {
            this.OnActiveTabChanged(EventArgs.Empty);
        }

        protected override void RemovedControl(Control control)
        {
            TabPanel panel = control as TabPanel;
            if (((control != null) && panel.Active) && (this.ActiveTabIndex < this.Tabs.Count))
            {
                this.EnsureActiveTab();
            }
            panel.SetOwner(null);
            base.RemovedControl(control);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            this.Page.VerifyRenderingInServerForm(this);
            if ((((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Top) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.TopRight)) || ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Bottom) && this._useVerticalStripPlacement)) || ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.BottomRight) && this._useVerticalStripPlacement))
            {
                this.RenderHeader(writer);
            }
            if (!this.Height.IsEmpty)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_body");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_body" + this.GetSuffixTabStripPlacementCss());
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            this.RenderChildren(writer);
            writer.RenderEndTag();
            if (((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Bottom) && !this._useVerticalStripPlacement) || ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.BottomRight) && !this._useVerticalStripPlacement))
            {
                this.RenderHeader(writer);
            }
        }

        protected virtual void RenderHeader(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_header");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_header" + this.GetSuffixTabStripPlacementCss());
            if ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.BottomRight) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.TopRight))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Direction, "rtl");
            }
            if (this._useVerticalStripPlacement)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
                if ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Bottom) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Top))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "float:left");
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "float:right");
                }
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this._verticalStripWidth.ToString());
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Bottom) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.BottomRight))
            {
                this.RenderSpannerForVerticalTabs(writer);
            }
            if (!this._useVerticalStripPlacement && ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.BottomRight) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.TopRight)))
            {
                for (int i = this.Tabs.Count - 1; i >= 0; i--)
                {
                    TabPanel panel = this.Tabs[i];
                    if (panel.Visible)
                    {
                        panel.RenderHeader(writer);
                    }
                }
            }
            else
            {
                foreach (TabPanel panel2 in this.Tabs)
                {
                    if (panel2.Visible)
                    {
                        panel2.RenderHeader(writer);
                    }
                }
            }
            if ((this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.Top) || (this._tabStripPlacement == AjaxControlToolkit.TabStripPlacement.TopRight))
            {
                this.RenderSpannerForVerticalTabs(writer);
            }
            writer.RenderEndTag();
        }

        private void RenderSpannerForVerticalTabs(HtmlTextWriter writer)
        {
            if (this._useVerticalStripPlacement)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_headerSpannerHeight");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag();
            }
        }

        protected override string SaveClientState()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object> {
                ["ActiveTabIndex"] = this.ActiveTabIndex
            };
            List<object> list = new List<object>();
            foreach (TabPanel panel in this.Tabs)
            {
                list.Add(panel.Enabled);
            }
            dictionary["TabState"] = list;
            return new JavaScriptSerializer().Serialize(dictionary);
        }

        protected override object SaveControlState()
        {
            Pair pair = new Pair {
                First = base.SaveControlState(),
                Second = this.ActiveTabIndex
            };
            if ((pair.First == null) && (pair.Second == null))
            {
                return null;
            }
            return pair;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeActiveTabIndexForClient() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeUniqueID() => 
            (base.IsRenderingScript && this.AutoPostBack);

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("activeTabChanged", StringComparison.Ordinal))
            {
                int index = eventArgument.IndexOf(":", StringComparison.Ordinal);
                if (index != -1)
                {
                    string s = eventArgument.Substring(index + 1);
                    if (int.TryParse(s, out index))
                    {
                        index = this.getServerActiveTabIndex(index);
                        if (index != this.ActiveTabIndex)
                        {
                            this.ActiveTabIndex = index;
                            this.OnActiveTabChanged(EventArgs.Empty);
                        }
                    }
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabPanel ActiveTab
        {
            get
            {
                int activeTabIndex = this.ActiveTabIndex;
                if ((activeTabIndex < 0) || (activeTabIndex >= this.Tabs.Count))
                {
                    return null;
                }
                this.EnsureActiveTab();
                return this.Tabs[activeTabIndex];
            }
            set
            {
                int index = this.Tabs.IndexOf(value);
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ActiveTabIndex = index;
            }
        }

        [Category("Behavior"), DefaultValue(-1)]
        public virtual int ActiveTabIndex
        {
            get
            {
                if (this._cachedActiveTabIndex > -1)
                {
                    return this._cachedActiveTabIndex;
                }
                if (this.Tabs.Count == 0)
                {
                    return -1;
                }
                return this._activeTabIndex;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if ((this.Tabs.Count == 0) && !this._initialized)
                {
                    this._cachedActiveTabIndex = value;
                }
                else if (this.ActiveTabIndex != value)
                {
                    if ((this.ActiveTabIndex != -1) && (this.ActiveTabIndex < this.Tabs.Count))
                    {
                        this.Tabs[this.ActiveTabIndex].Active = false;
                    }
                    if (value >= this.Tabs.Count)
                    {
                        this._activeTabIndex = this.Tabs.Count - 1;
                        this._cachedActiveTabIndex = value;
                    }
                    else
                    {
                        this._activeTabIndex = value;
                        this._cachedActiveTabIndex = -1;
                    }
                    if ((this.ActiveTabIndex != -1) && (this.ActiveTabIndex < this.Tabs.Count))
                    {
                        this.Tabs[this.ActiveTabIndex].Active = true;
                    }
                }
            }
        }

        [DefaultValue(-1), EditorBrowsable(EditorBrowsableState.Never), ClientPropertyName("activeTabIndex"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Behavior"), ExtenderControlProperty]
        public int ActiveTabIndexForClient
        {
            get
            {
                int activeTabIndex = this.ActiveTabIndex;
                for (int i = 0; (i <= this.ActiveTabIndex) && (i < this.Tabs.Count); i++)
                {
                    if (!this.Tabs[i].Visible)
                    {
                        activeTabIndex--;
                    }
                }
                if (activeTabIndex < 0)
                {
                    activeTabIndex = 0;
                }
                return activeTabIndex;
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public bool AutoPostBack
        {
            get => 
                this._autoPostBack;
            set
            {
                this._autoPostBack = value;
            }
        }

        [DefaultValue("ajax__tab_xp"), Category("Appearance")]
        public override string CssClass
        {
            get => 
                base.CssClass;
            set
            {
                base.CssClass = value;
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Unit), "")]
        public override Unit Height
        {
            get => 
                base.Height;
            set
            {
                base.Height = value;
            }
        }

        [ClientPropertyName("activeTabChanged"), ExtenderControlEvent, DefaultValue(""), Category("Behavior")]
        public string OnClientActiveTabChanged
        {
            get => 
                (this.ViewState["OnClientActiveTabChanged"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientActiveTabChanged"] = value;
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public bool OnDemand
        {
            get => 
                this._onDemand;
            set
            {
                this._onDemand = value;
            }
        }

        [ClientPropertyName("scrollBars"), Category("Behavior"), DefaultValue(0), ExtenderControlProperty]
        public System.Web.UI.WebControls.ScrollBars ScrollBars
        {
            get => 
                (this.ViewState["ScrollBars"] ?? System.Web.UI.WebControls.ScrollBars.None);
            set
            {
                this.ViewState["ScrollBars"] = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabPanelCollection Tabs =>
            ((TabPanelCollection) this.Controls);

        [Category("Appearance"), DefaultValue(0)]
        public AjaxControlToolkit.TabStripPlacement TabStripPlacement
        {
            get => 
                this._tabStripPlacement;
            set
            {
                this._tabStripPlacement = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("autoPostBackId")]
        public string UniqueID
        {
            get => 
                base.UniqueID;
            set
            {
            }
        }

        [Description("Change tab header placement vertically when value set to true"), DefaultValue(false), Category("Appearance")]
        public bool UseVerticalStripPlacement
        {
            get => 
                this._useVerticalStripPlacement;
            set
            {
                this._useVerticalStripPlacement = value;
            }
        }

        [Description("Set width of tab strips when UseVerticalStripPlacement is set to true. Size must be in pixel"), Category("Appearance"), DefaultValue(typeof(Unit), "120px")]
        public Unit VerticalStripWidth
        {
            get => 
                this._verticalStripWidth;
            set
            {
                if (!value.IsEmpty && (value.Type != UnitType.Pixel))
                {
                    throw new ArgumentOutOfRangeException("value", "VerticalStripWidth must be set in pixels only, or Empty.");
                }
                this._verticalStripWidth = value;
            }
        }

        [DefaultValue(typeof(Unit), ""), Category("Appearance")]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }

        private sealed class TabContainerStyle : Style
        {
            public TabContainerStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Remove(HtmlTextWriterStyle.Height);
                attributes.Remove(HtmlTextWriterStyle.BackgroundImage);
            }
        }
    }
}

