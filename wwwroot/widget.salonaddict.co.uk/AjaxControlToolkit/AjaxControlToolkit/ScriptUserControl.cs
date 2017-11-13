namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientScriptResource(null, "ExtenderBase.BaseScripts.js")]
    public class ScriptUserControl : UserControl, IScriptControl, IControlResolver, IPostBackDataHandler, ICallbackEventHandler, IClientStateManager
    {
        private string _cachedClientStateFieldID;
        private string _callbackArgument;
        private System.Web.UI.WebControls.Style _controlStyle;
        private bool _enableClientState;
        private System.Web.UI.ScriptManager _scriptManager;
        private HtmlTextWriterTag _tagKey;
        private string _tagName;

        protected ScriptUserControl() : this(false)
        {
        }

        protected ScriptUserControl(bool enableClientState)
        {
            this._enableClientState = enableClientState;
        }

        protected ScriptUserControl(string tag) : this(false, tag)
        {
        }

        public ScriptUserControl(HtmlTextWriterTag tag) : this(false, tag)
        {
        }

        protected ScriptUserControl(bool enableClientState, string tag)
        {
            this._enableClientState = enableClientState;
            this._tagKey = HtmlTextWriterTag.Unknown;
            this._tagName = tag;
        }

        protected ScriptUserControl(bool enableClientState, HtmlTextWriterTag tag)
        {
            this._enableClientState = enableClientState;
            this._tagKey = tag;
        }

        protected virtual void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.ID != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            }
            if ((this.TagKey == HtmlTextWriterTag.Span) || (this.TagKey == HtmlTextWriterTag.A))
            {
                this.AddDisplayInlineBlockIfNeeded(writer);
            }
            if (this.ControlStyleCreated && !this.ControlStyle.IsEmpty)
            {
                this.ControlStyle.AddAttributesToRender(writer);
            }
            foreach (string str in base.Attributes.Keys)
            {
                writer.AddAttribute(str, base.Attributes[str]);
            }
        }

        private void AddDisplayInlineBlockIfNeeded(HtmlTextWriter writer)
        {
            if (((this.BorderStyle != System.Web.UI.WebControls.BorderStyle.NotSet) || !this.BorderWidth.IsEmpty) || (!this.Height.IsEmpty || !this.Width.IsEmpty))
            {
                if (base.Request.Browser.IsBrowser("FireFox") || base.Request.Browser.IsBrowser("Mozilla"))
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "-moz-inline-box");
                }
                else
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
                }
            }
        }

        void IClientStateManager.LoadClientState(string clientState)
        {
            this.LoadClientState(clientState);
        }

        string IClientStateManager.SaveClientState() => 
            this.SaveClientState();

        protected virtual System.Web.UI.WebControls.Style CreateControlStyle() => 
            new System.Web.UI.WebControls.Style(this.ViewState);

        protected virtual void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            ScriptObjectBuilder.DescribeComponent(this, descriptor, this, this);
            if (this.SupportsClientState)
            {
                descriptor.AddElementProperty("clientStateField", this.ClientStateFieldID);
            }
        }

        private void EnsureScriptManager()
        {
            if (this._scriptManager == null)
            {
                this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(this.Page);
                if (this._scriptManager == null)
                {
                    throw new HttpException(Resources.E_NoScriptManager);
                }
            }
        }

        public override Control FindControl(string id)
        {
            Control control = base.FindControl(id);
            if (control != null)
            {
                return control;
            }
            for (Control control2 = this.NamingContainer; control2 != null; control2 = control2.NamingContainer)
            {
                control = control2.FindControl(id);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }

        protected virtual string GetCallbackResult()
        {
            string callbackArgument = this._callbackArgument;
            this._callbackArgument = null;
            return ScriptObjectBuilder.ExecuteCallbackMethod(this, callbackArgument);
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (!this.Visible)
            {
                return null;
            }
            base.EnsureID();
            List<ScriptDescriptor> list = new List<ScriptDescriptor>();
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor(this.ClientControlType, this.ClientID);
            this.DescribeComponent(descriptor);
            list.Add(descriptor);
            return list;
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (!this.Visible)
            {
                return null;
            }
            List<ScriptReference> list = new List<ScriptReference>();
            list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType()));
            if (this.ScriptPath.Length > 0)
            {
                list.Add(new ScriptReference(this.ScriptPath));
            }
            return list;
        }

        protected virtual void LoadClientState(string clientState)
        {
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (this.SupportsClientState)
            {
                string str = postCollection[this.ClientStateFieldID];
                if (!string.IsNullOrEmpty(str))
                {
                    this.LoadClientState(str);
                }
            }
            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.EnsureScriptManager();
            this.ScriptManager.RegisterScriptControl<ScriptUserControl>(this);
            base.EnsureID();
            if (this.SupportsClientState)
            {
                System.Web.UI.ScriptManager.RegisterHiddenField(this, this.ClientStateFieldID, this.SaveClientState());
                this.Page.RegisterRequiresPostBack(this);
            }
        }

        protected virtual void RaiseCallbackEvent(string eventArgument)
        {
            this._callbackArgument = eventArgument;
        }

        protected virtual void RaisePostDataChangedEvent()
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderBeginTag(writer);
            this.RenderContents(writer);
            this.RenderEndTag(writer);
            this.ScriptManager.RegisterScriptDescriptors(this);
        }

        public virtual void RenderBeginTag(HtmlTextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.AddAttributesToRender(writer);
            HtmlTextWriterTag tagKey = this.TagKey;
            if (tagKey != HtmlTextWriterTag.Unknown)
            {
                writer.RenderBeginTag(tagKey);
            }
            else
            {
                writer.RenderBeginTag(this.TagName);
            }
        }

        protected virtual void RenderContents(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        public virtual void RenderEndTag(HtmlTextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.RenderEndTag();
        }

        public Control ResolveControl(string controlId) => 
            this.FindControl(controlId);

        protected virtual string SaveClientState() => 
            null;

        protected override object SaveViewState()
        {
            if (this.ControlStyleCreated)
            {
                ((IStateManager) this.ControlStyle).SaveViewState();
            }
            return base.SaveViewState();
        }

        string ICallbackEventHandler.GetCallbackResult() => 
            this.GetCallbackResult();

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            this.RaiseCallbackEvent(eventArgument);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors() => 
            this.GetScriptDescriptors();

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences() => 
            this.GetScriptReferences();

        bool IClientStateManager.SupportsClientState =>
            this.SupportsClientState;

        [DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
        public virtual Color BackColor
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Color.Empty;
                }
                return this._controlStyle.BackColor;
            }
            set
            {
                this.ControlStyle.BackColor = value;
            }
        }

        [DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
        public virtual Color BorderColor
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Color.Empty;
                }
                return this._controlStyle.BorderColor;
            }
            set
            {
                this.ControlStyle.BorderColor = value;
            }
        }

        [DefaultValue(1)]
        public virtual System.Web.UI.WebControls.BorderStyle BorderStyle
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.BorderStyle.None;
                }
                return this._controlStyle.BorderStyle;
            }
            set
            {
                this.ControlStyle.BorderStyle = value;
            }
        }

        [DefaultValue(typeof(Unit), "")]
        public virtual Unit BorderWidth
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Unit.Empty;
                }
                return this._controlStyle.BorderWidth;
            }
            set
            {
                this.ControlStyle.BorderWidth = value;
            }
        }

        protected virtual string ClientControlType
        {
            get
            {
                ClientScriptResourceAttribute attribute = (ClientScriptResourceAttribute) TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
                return attribute.ComponentType;
            }
        }

        protected string ClientStateFieldID
        {
            get
            {
                if (this._cachedClientStateFieldID == null)
                {
                    this._cachedClientStateFieldID = this.ClientID + "_ClientState";
                }
                return this._cachedClientStateFieldID;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public System.Web.UI.WebControls.Style ControlStyle
        {
            get
            {
                if (this._controlStyle == null)
                {
                    this._controlStyle = this.CreateControlStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._controlStyle).TrackViewState();
                    }
                }
                return this._controlStyle;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ControlStyleCreated =>
            (this._controlStyle != null);

        [DefaultValue("")]
        public virtual string CssClass
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return string.Empty;
                }
                return this._controlStyle.CssClass;
            }
            set
            {
                this.ControlStyle.CssClass = value;
            }
        }

        [NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual FontInfo Font =>
            this.ControlStyle.Font;

        [TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), "")]
        public virtual Color ForeColor
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Color.Empty;
                }
                return this.ControlStyle.ForeColor;
            }
            set
            {
                this.ControlStyle.ForeColor = value;
            }
        }

        [DefaultValue(typeof(Unit), "")]
        public virtual Unit Height
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Unit.Empty;
                }
                return this.ControlStyle.Height;
            }
            set
            {
                this.ControlStyle.Height = value;
            }
        }

        protected System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                this.EnsureScriptManager();
                return this._scriptManager;
            }
        }

        [DefaultValue("")]
        public virtual string ScriptPath
        {
            get => 
                (this.ViewState["ScriptPath"] ?? string.Empty);
            set
            {
                this.ViewState["ScriptPath"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public CssStyleCollection Style =>
            base.Attributes.CssStyle;

        protected virtual bool SupportsClientState =>
            this._enableClientState;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        protected virtual HtmlTextWriterTag TagKey =>
            this._tagKey;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        protected virtual string TagName
        {
            get
            {
                if ((this._tagName == null) && (this.TagKey != HtmlTextWriterTag.Unknown))
                {
                    this._tagName = Enum.Format(typeof(HtmlTextWriterTag), this.TagKey, "G").ToLower(CultureInfo.InvariantCulture);
                }
                return this._tagName;
            }
        }

        [DefaultValue(typeof(Unit), "")]
        public virtual Unit Width
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Unit.Empty;
                }
                return this.ControlStyle.Width;
            }
            set
            {
                this.ControlStyle.Width = value;
            }
        }
    }
}

