namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(CommonToolkitScripts)), Designer(typeof(TabPanelDesigner)), ToolboxItem(false), RequiredScript(typeof(DynamicPopulateExtender)), RequiredScript(typeof(TabContainer)), ClientScriptResource("Sys.Extended.UI.TabPanel", "Tabs.Tabs.js"), ClientCssResource("Tabs.Tabs_resource.css"), ParseChildren(true)]
    public class TabPanel : ScriptControlBase
    {
        private bool _active;
        private ITemplate _contentTemplate;
        private Control _headerControl;
        private ITemplate _headerTemplate;
        private TabContainer _owner;

        public TabPanel() : base(false, HtmlTextWriterTag.Div)
        {
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddElementProperty("headerTab", "__tab_" + this.ClientID);
            if (this._owner != null)
            {
                descriptor.AddComponentProperty("owner", this._owner.ClientID);
                descriptor.AddProperty("ownerID", this._owner.ClientID);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this._headerTemplate != null)
            {
                this._headerControl = new Control();
                this._headerTemplate.InstantiateIn(this._headerControl);
                this.Controls.Add(this._headerControl);
            }
            if (this._contentTemplate != null)
            {
                Control container = new Control();
                this._contentTemplate.InstantiateIn(container);
                if (this._owner.OnDemand && (this.OnDemandMode != AjaxControlToolkit.OnDemandMode.None))
                {
                    string str = this.ClientID + "_onDemandPanel";
                    Panel child = new Panel {
                        ID = str,
                        Visible = false
                    };
                    child.Controls.Add(container);
                    UpdatePanel panel2 = new UpdatePanel {
                        ID = this.ClientID + "_updatePanel",
                        UpdateMode = UpdatePanelUpdateMode.Conditional
                    };
                    panel2.Load += new EventHandler(this.UpdatePanelOnLoad);
                    panel2.ContentTemplateContainer.Controls.Add(child);
                    this.Controls.Add(panel2);
                    this.UpdatePanelID = panel2.ClientID;
                }
                else
                {
                    this.Controls.Add(container);
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this._headerControl != null)
            {
                this._headerControl.Visible = false;
            }
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_panel");
            if (!this.Active || !this.Enabled)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            this.RenderChildren(writer);
            writer.RenderEndTag();
            base.ScriptManager.RegisterScriptDescriptors(this);
        }

        private void RenderBeginTag(HtmlTextWriter writer)
        {
            if (this._owner.UseVerticalStripPlacement)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
        }

        protected internal virtual void RenderHeader(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_tab");
            this.RenderBeginTag(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_outer");
            this.RenderBeginTag(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_inner");
            this.RenderBeginTag(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_tab");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "__tab_" + this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "none");
            if (this._owner.UseVerticalStripPlacement)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            this.RenderBeginTag(writer);
            if (this._headerControl != null)
            {
                this._headerControl.Visible = true;
                this._headerControl.RenderControl(writer);
                this._headerControl.Visible = false;
            }
            else
            {
                writer.Write(this.HeaderText);
            }
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        internal void SetOwner(TabContainer owner)
        {
            this._owner = owner;
        }

        private void UpdatePanelOnLoad(object sender, EventArgs e)
        {
            if (sender is UpdatePanel)
            {
                string iD = (sender as UpdatePanel).ID;
                string str2 = iD.Substring(0, iD.Length - 12);
                if (this.Active)
                {
                    Control control = this.FindControl(str2 + "_onDemandPanel");
                    if ((control != null) && (control is Panel))
                    {
                        control.Visible = true;
                    }
                }
            }
        }

        internal bool Active
        {
            get => 
                this._active;
            set
            {
                this._active = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateInstance(TemplateInstance.Single), MergableProperty(false)]
        public ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
            }
        }

        [ClientPropertyName("dynamicContextKey"), Category("Behavior"), DefaultValue(""), ExtenderControlProperty]
        public string DynamicContextKey
        {
            get => 
                (this.ViewState["DynamicContextKey"] ?? string.Empty);
            set
            {
                this.ViewState["DynamicContextKey"] = value;
            }
        }

        [ClientPropertyName("dynamicServiceMethod"), DefaultValue(""), Category("Behavior"), ExtenderControlProperty]
        public string DynamicServiceMethod
        {
            get => 
                (this.ViewState["DynamicServiceMethod"] ?? string.Empty);
            set
            {
                this.ViewState["DynamicServiceMethod"] = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("dynamicServicePath"), UrlProperty, DefaultValue(""), Category("Behavior")]
        public string DynamicServicePath
        {
            get => 
                (this.ViewState["DynamicServicePath"] ?? string.Empty);
            set
            {
                this.ViewState["DynamicServicePath"] = value;
            }
        }

        [DefaultValue(true), ClientPropertyName("enabled"), Category("Behavior"), ExtenderControlProperty]
        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [Browsable(false), MergableProperty(false), TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate
        {
            get => 
                this._headerTemplate;
            set
            {
                this._headerTemplate = value;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public string HeaderText
        {
            get => 
                (this.ViewState["HeaderText"] ?? string.Empty);
            set
            {
                this.ViewState["HeaderText"] = value;
            }
        }

        [Category("Behavior"), ClientPropertyName("click"), DefaultValue(""), ExtenderControlEvent]
        public string OnClientClick
        {
            get => 
                (this.ViewState["OnClientClick"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientClick"] = value;
            }
        }

        [ExtenderControlEvent, ClientPropertyName("populated"), Category("Behavior"), DefaultValue("")]
        public string OnClientPopulated
        {
            get => 
                (this.ViewState["OnClientPopulated"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientPopulated"] = value;
            }
        }

        [ClientPropertyName("populating"), DefaultValue(""), Category("Behavior"), ExtenderControlEvent]
        public string OnClientPopulating
        {
            get => 
                (this.ViewState["OnClientPopulating"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientPopulating"] = value;
            }
        }

        [DefaultValue(1), Category("Behavior"), ClientPropertyName("onDemandMode"), ExtenderControlProperty]
        public AjaxControlToolkit.OnDemandMode OnDemandMode
        {
            get => 
                (this.ViewState["OnDemandMode"] ?? AjaxControlToolkit.OnDemandMode.Always);
            set
            {
                this.ViewState["OnDemandMode"] = value;
            }
        }

        [ClientPropertyName("scrollBars"), Category("Behavior"), ExtenderControlProperty, DefaultValue(0)]
        public System.Web.UI.WebControls.ScrollBars ScrollBars
        {
            get => 
                (this.ViewState["ScrollBars"] ?? System.Web.UI.WebControls.ScrollBars.None);
            set
            {
                this.ViewState["ScrollBars"] = value;
            }
        }

        [ClientPropertyName("updatePanelID"), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UpdatePanelID { get; set; }
    }
}

