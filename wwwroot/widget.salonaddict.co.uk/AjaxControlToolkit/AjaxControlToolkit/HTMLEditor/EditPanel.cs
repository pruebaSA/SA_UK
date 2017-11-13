namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.EditPanel", "HTMLEditor.EditPanel.js"), RequiredScript(typeof(Enums)), RequiredScript(typeof(AjaxControlToolkit.HTMLEditor.HTMLEditor)), ParseChildren(true), PersistChildren(false), ValidationProperty("Content"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(AjaxControlToolkit.HTMLEditor.Events))]
    public abstract class EditPanel : ScriptControlBase, IPostBackEventHandler
    {
        private bool _contentChanged;
        private ControlDesigner _designer;
        private Collection<Toolbar> _toolbars;
        public static readonly object EventContentChanged = new object();
        private readonly ModePanel[] ModePanels;

        [Category("Behavior")]
        public event ContentChangedEventHandler ContentChanged
        {
            add
            {
                this.Events.AddHandler(EventContentChanged, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventContentChanged, value);
            }
        }

        protected EditPanel() : base(false, HtmlTextWriterTag.Div)
        {
            this.ModePanels = new ModePanel[] { new DesignPanel(), new HtmlPanel(), new PreviewPanel() };
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddElementProperty("contentChangedElement", this.ContentChangedId);
            descriptor.AddElementProperty("contentForceElement", this.ContentForceId);
            descriptor.AddElementProperty("contentElement", this.ContentId);
            descriptor.AddElementProperty("activeModeElement", this.ActiveModeId);
        }

        internal string getClientCSSPath(string pathN, string name)
        {
            bool flag = false;
            string path = (pathN.Length > 0) ? this.LocalResolveUrl(pathN) : "";
            if (path.Length > 0)
            {
                try
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(path)))
                    {
                        flag = true;
                    }
                }
                catch
                {
                }
            }
            if (flag)
            {
                return path;
            }
            return this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor." + name + ".css");
        }

        internal static bool IE(Page page)
        {
            try
            {
                return (page.Request.Browser.Browser.IndexOf("IE", StringComparison.OrdinalIgnoreCase) > -1);
            }
            catch
            {
                return false;
            }
        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            base.LoadPostData(postDataKey, postCollection);
            bool flag = false;
            string str = postCollection[this.ContentForceId];
            if (!string.IsNullOrEmpty(str))
            {
                flag = true;
            }
            str = postCollection[this.ActiveModeId];
            if (!string.IsNullOrEmpty(str))
            {
                this.ActiveMode = (ActiveModeType) ((int) long.Parse(str, CultureInfo.InvariantCulture));
            }
            this._contentChanged = false;
            str = postCollection[this.ContentId];
            if ((str != null) && flag)
            {
                string str2 = str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&amp;", "&");
                if (str2 == "<br />")
                {
                    str2 = "";
                }
                this._contentChanged = this.Content.Replace("\n", "").Replace("\r", "") != str2.Replace("\n", "").Replace("\r", "");
                this.Content = str2;
            }
            str = postCollection[this.ContentChangedId];
            if (!string.IsNullOrEmpty(str))
            {
                this._contentChanged = true;
            }
            return this._contentChanged;
        }

        protected string LocalResolveUrl(string path)
        {
            string input = base.ResolveUrl(path);
            Regex regex = new Regex(@"(\(S\([A-Za-z0-9_]+\)\)/)", RegexOptions.Compiled);
            return regex.Replace(input, "");
        }

        protected override bool OnBubbleEvent(object source, EventArgs args) => 
            true;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.Style.Add(HtmlTextWriterStyle.Height, Unit.Percentage(100.0).ToString());
            base.Style.Add(HtmlTextWriterStyle.Width, Unit.Percentage(100.0).ToString());
            if (!this.isDesign)
            {
                for (int i = 0; i < this.ModePanels.Length; i++)
                {
                    this.ModePanels[i].setEditPanel(this);
                    this.Controls.Add(this.ModePanels[i]);
                }
            }
            else
            {
                this.Controls.Add(this.ModePanels[0]);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ScriptManager.RegisterHiddenField(this, this.ContentChangedId, string.Empty);
            ScriptManager.RegisterHiddenField(this, this.ContentForceId, "1");
            ScriptManager.RegisterHiddenField(this, this.ContentId, this.Content.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"));
            ScriptManager.RegisterHiddenField(this, this.ActiveModeId, ((int) this.ActiveMode).ToString(CultureInfo.InvariantCulture));
            this.Page.RegisterRequiresPostBack(this);
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.IgnoreTab)
                {
                    ModePanel panel = this.Controls[i] as ModePanel;
                    panel.Attributes.Add("tabindex", "-1");
                }
                if (this.Controls[i].GetType() == typeof(HtmlPanel))
                {
                    (this.Controls[i] as HtmlPanel).CssClass = this.HtmlPanelCssClass;
                }
            }
        }

        protected virtual void OnRaiseContentChanged(EventArgs e)
        {
            ContentChangedEventHandler handler = (ContentChangedEventHandler) this.Events[EventContentChanged];
            if (handler != null)
            {
                handler(this, e);
            }
            else
            {
                base.RaiseBubbleEvent(this, new CommandEventArgs("contentchanged", ""));
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
        }

        protected override void RaisePostDataChangedEvent()
        {
            if (this._contentChanged)
            {
                this.OnRaiseContentChanged(EventArgs.Empty);
                this._contentChanged = false;
            }
        }

        protected void RefreshDesigner()
        {
            if ((this._designer != null) && this.isDesign)
            {
                this._designer.UpdateDesignTimeHtml();
            }
        }

        public void SetDesigner(ControlDesigner designer)
        {
            this._designer = designer;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeClientDesignPanelCssPath() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeClientDocumentCssPath() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeClientModePanelIds() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImagePath_1X1() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImagePath_Anchor() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImagePath_Flash() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImagePath_Media() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeImagePath_Placeholder() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeToolbarIds() => 
            base.IsRenderingScript;

        [Category("Behavior"), DefaultValue(0)]
        public ActiveModeType ActiveMode
        {
            get => 
                (this.ViewState["ActiveMode"] ?? ActiveModeType.Design);
            set
            {
                this.ViewState["ActiveMode"] = value;
                if ((this._designer != null) && this.isDesign)
                {
                    this.RefreshDesigner();
                }
            }
        }

        protected string ActiveModeId =>
            ("_activeMode_" + this.ClientID);

        [Category("Behavior"), DefaultValue(true), ClientPropertyName("autofocus"), ExtenderControlProperty]
        public bool AutoFocus
        {
            get => 
                (this.ViewState["AutoFocus"] ?? true);
            set
            {
                this.ViewState["AutoFocus"] = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("designPanelCssPath"), Browsable(false)]
        public string ClientDesignPanelCssPath =>
            this.getClientCSSPath(this.DesignPanelCssPath, "DesignPanel");

        [ClientPropertyName("documentCssPath"), ExtenderControlProperty, Browsable(false)]
        public string ClientDocumentCssPath =>
            this.getClientCSSPath(this.DocumentCssPath, "Document");

        [Browsable(false), ExtenderControlProperty, ClientPropertyName("modePanelIds")]
        public string ClientModePanelIds
        {
            get
            {
                string str = "";
                for (int i = 0; i < this.ModePanels.Length; i++)
                {
                    if (i > 0)
                    {
                        str = str + ";";
                    }
                    str = str + this.ModePanels[i].ClientID;
                }
                return str;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public string Content
        {
            get => 
                (this.ViewState["Content"] ?? string.Empty);
            set
            {
                this.ViewState["Content"] = value;
            }
        }

        protected string ContentChangedId =>
            ("_contentChanged_" + this.ClientID);

        protected string ContentForceId =>
            ("_contentForce_" + this.ClientID);

        protected string ContentId =>
            ("_content_" + this.ClientID);

        [DefaultValue(""), Category("Appearance")]
        public string DesignPanelCssPath
        {
            get => 
                (this.ViewState["DesignPanelCssPath"] ?? string.Empty);
            set
            {
                this.ViewState["DesignPanelCssPath"] = value;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public string DocumentCssPath
        {
            get => 
                (this.ViewState["DocumentCssPath"] ?? string.Empty);
            set
            {
                this.ViewState["DocumentCssPath"] = value;
            }
        }

        internal EventHandlerList Events =>
            base.Events;

        [DefaultValue(typeof(Unit), "100%"), Category("Appearance")]
        public override Unit Height =>
            Unit.Percentage(100.0);

        [DefaultValue("ajax__htmleditor_htmlpanel_default"), Category("Appearance")]
        public string HtmlPanelCssClass
        {
            get => 
                (this.ViewState["HtmlPanelCssClass"] ?? "ajax__htmleditor_htmlpanel_default");
            set
            {
                this.ViewState["HtmlPanelCssClass"] = value;
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public bool IgnoreTab
        {
            get => 
                (this.ViewState["IgnoreTab"] ?? false);
            set
            {
                this.ViewState["IgnoreTab"] = value;
            }
        }

        [Browsable(false), ClientPropertyName("imagePath_1x1"), ExtenderControlProperty]
        public string ImagePath_1X1 =>
            this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor.Images.ed_1x1.gif");

        [ExtenderControlProperty, ClientPropertyName("imagePath_anchor"), Browsable(false)]
        public string ImagePath_Anchor =>
            this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor.Images.ed_anchor.gif");

        [Browsable(false), ClientPropertyName("imagePath_flash"), ExtenderControlProperty]
        public string ImagePath_Flash =>
            this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor.Images.ed_flash.gif");

        [ClientPropertyName("imagePath_media"), Browsable(false), ExtenderControlProperty]
        public string ImagePath_Media =>
            this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor.Images.ed_media.gif");

        [ExtenderControlProperty, ClientPropertyName("imagePath_placeHolder"), Browsable(false)]
        public string ImagePath_Placeholder =>
            this.Page.ClientScript.GetWebResourceUrl(typeof(EditPanel), "HTMLEditor.Images.ed_placeHolder.gif");

        [ExtenderControlProperty, DefaultValue(false), Category("Behavior"), ClientPropertyName("initialCleanUp")]
        public bool InitialCleanUp
        {
            get => 
                (this.ViewState["InitialCleanUp"] ?? false);
            set
            {
                this.ViewState["InitialCleanUp"] = value;
            }
        }

        private bool isDesign
        {
            get
            {
                try
                {
                    bool designMode = false;
                    if (this.Context == null)
                    {
                        designMode = true;
                    }
                    else if (base.Site != null)
                    {
                        designMode = base.Site.DesignMode;
                    }
                    else
                    {
                        designMode = false;
                    }
                    return designMode;
                }
                catch
                {
                    return true;
                }
            }
        }

        [DefaultValue(false), Category("Behavior"), ClientPropertyName("noScript"), ExtenderControlProperty]
        public bool NoScript
        {
            get => 
                (this.ViewState["NoScript"] ?? false);
            set
            {
                this.ViewState["NoScript"] = value;
            }
        }

        [DefaultValue(false), ClientPropertyName("noUnicode"), Category("Behavior"), ExtenderControlProperty]
        public bool NoUnicode
        {
            get => 
                (this.ViewState["NoUnicode"] ?? false);
            set
            {
                this.ViewState["NoUnicode"] = value;
            }
        }

        [ClientPropertyName("activeModeChanged"), Category("Behavior"), DefaultValue(""), ExtenderControlEvent]
        public string OnClientActiveModeChanged
        {
            get => 
                (this.ViewState["OnClientActiveModeChanged"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientActiveModeChanged"] = value;
            }
        }

        [ExtenderControlEvent, ClientPropertyName("beforeActiveModeChanged"), Category("Behavior"), DefaultValue("")]
        public string OnClientBeforeActiveModeChanged
        {
            get => 
                (this.ViewState["OnClientBeforeActiveModeChanged"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientBeforeActiveModeChanged"] = value;
            }
        }

        [ExtenderControlProperty, Category("Behavior"), DefaultValue(false), ClientPropertyName("suppressTabInDesignMode")]
        public bool SuppressTabInDesignMode
        {
            get => 
                (this.ViewState["SuppressTabInDesignMode"] ?? false);
            set
            {
                this.ViewState["SuppressTabInDesignMode"] = value;
            }
        }

        [ExtenderControlProperty, Browsable(false), ClientPropertyName("toolbarIds")]
        public string ToolbarIds
        {
            get
            {
                string str = "";
                for (int i = 0; i < this.Toolbars.Count; i++)
                {
                    if (i > 0)
                    {
                        str = str + ";";
                    }
                    str = str + this.Toolbars[i].ClientID;
                }
                return str;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        internal Collection<Toolbar> Toolbars
        {
            get
            {
                if (this._toolbars == null)
                {
                    this._toolbars = new Collection<Toolbar>();
                }
                return this._toolbars;
            }
            set
            {
                this._toolbars = value;
            }
        }

        [DefaultValue(typeof(Unit), "100%"), Category("Appearance")]
        public override Unit Width =>
            Unit.Percentage(100.0);
    }
}

