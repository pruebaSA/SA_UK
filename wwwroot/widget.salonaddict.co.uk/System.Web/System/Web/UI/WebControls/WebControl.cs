namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.Util;

    [PersistChildren(false), Themeable(true), ParseChildren(true), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebControl : Control, IAttributeAccessor
    {
        private SimpleBitVector32 _webControlFlags;
        private const int accessKeySet = 4;
        private System.Web.UI.AttributeCollection attrColl;
        private StateBag attrState;
        private System.Web.UI.WebControls.Style controlStyle;
        private const int deferStyleLoadViewState = 1;
        private const int disabledDirty = 2;
        private const int tabIndexSet = 0x10;
        private HtmlTextWriterTag tagKey;
        private string tagName;
        private const int toolTipSet = 8;

        protected WebControl() : this(HtmlTextWriterTag.Span)
        {
        }

        protected WebControl(string tag)
        {
            this.tagKey = HtmlTextWriterTag.Unknown;
            this.tagName = tag;
        }

        public WebControl(HtmlTextWriterTag tag)
        {
            this.tagKey = tag;
        }

        protected virtual void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.ID != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            }
            if (this._webControlFlags[4])
            {
                string accessKey = this.AccessKey;
                if (accessKey.Length > 0)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
                }
            }
            if (!this.Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            if (this._webControlFlags[0x10])
            {
                int tabIndex = this.TabIndex;
                if (tabIndex != 0)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(NumberFormatInfo.InvariantInfo));
                }
            }
            if (this._webControlFlags[8])
            {
                string toolTip = this.ToolTip;
                if (toolTip.Length > 0)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);
                }
            }
            if ((this.TagKey == HtmlTextWriterTag.Span) || (this.TagKey == HtmlTextWriterTag.A))
            {
                this.AddDisplayInlineBlockIfNeeded(writer);
            }
            if (this.ControlStyleCreated && !this.ControlStyle.IsEmpty)
            {
                this.ControlStyle.AddAttributesToRender(writer, this);
            }
            if (this.attrState != null)
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

        internal void AddDisplayInlineBlockIfNeeded(HtmlTextWriter writer)
        {
            if ((!this.RequiresLegacyRendering || !base.EnableLegacyRendering) && (((this.BorderStyle != System.Web.UI.WebControls.BorderStyle.NotSet) || !this.BorderWidth.IsEmpty) || (!this.Height.IsEmpty || !this.Width.IsEmpty)))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
            }
        }

        public void ApplyStyle(System.Web.UI.WebControls.Style s)
        {
            if ((s != null) && !s.IsEmpty)
            {
                this.ControlStyle.CopyFrom(s);
            }
        }

        public void CopyBaseAttributes(WebControl controlSrc)
        {
            if (controlSrc == null)
            {
                throw new ArgumentNullException("controlSrc");
            }
            if (controlSrc._webControlFlags[4])
            {
                this.AccessKey = controlSrc.AccessKey;
            }
            if (!controlSrc.Enabled)
            {
                this.Enabled = false;
            }
            if (controlSrc._webControlFlags[8])
            {
                this.ToolTip = controlSrc.ToolTip;
            }
            if (controlSrc._webControlFlags[0x10])
            {
                this.TabIndex = controlSrc.TabIndex;
            }
            if (controlSrc.HasAttributes)
            {
                foreach (string str in controlSrc.Attributes.Keys)
                {
                    this.Attributes[str] = controlSrc.Attributes[str];
                }
            }
        }

        protected virtual System.Web.UI.WebControls.Style CreateControlStyle() => 
            new System.Web.UI.WebControls.Style(this.ViewState);

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                Pair pair = (Pair) savedState;
                base.LoadViewState(pair.First);
                if (this.ControlStyleCreated || (this.ViewState["_!SB"] != null))
                {
                    this.ControlStyle.LoadViewState(null);
                }
                else
                {
                    this._webControlFlags.Set(1);
                }
                if (pair.Second != null)
                {
                    if (this.attrState == null)
                    {
                        this.attrState = new StateBag(true);
                        this.attrState.TrackViewState();
                    }
                    this.attrState.LoadViewState(pair.Second);
                }
            }
            object obj2 = this.ViewState["Enabled"];
            if (obj2 != null)
            {
                if (!((bool) obj2))
                {
                    this.flags.Set(0x80000);
                }
                else
                {
                    this.flags.Clear(0x80000);
                }
                this._webControlFlags.Set(2);
            }
            if (((IDictionary) this.ViewState).Contains("AccessKey"))
            {
                this._webControlFlags.Set(4);
            }
            if (((IDictionary) this.ViewState).Contains("TabIndex"))
            {
                this._webControlFlags.Set(0x10);
            }
            if (((IDictionary) this.ViewState).Contains("ToolTip"))
            {
                this._webControlFlags.Set(8);
            }
        }

        public void MergeStyle(System.Web.UI.WebControls.Style s)
        {
            if ((s != null) && !s.IsEmpty)
            {
                this.ControlStyle.MergeWith(s);
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.RenderBeginTag(writer);
            this.RenderContents(writer);
            this.RenderEndTag(writer);
        }

        public virtual void RenderBeginTag(HtmlTextWriter writer)
        {
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

        protected internal virtual void RenderContents(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        public virtual void RenderEndTag(HtmlTextWriter writer)
        {
            writer.RenderEndTag();
        }

        protected override object SaveViewState()
        {
            Pair pair = null;
            if (this._webControlFlags[2])
            {
                this.ViewState["Enabled"] = !this.flags[0x80000];
            }
            if (this.ControlStyleCreated)
            {
                this.ControlStyle.SaveViewState();
            }
            object x = base.SaveViewState();
            object y = null;
            if (this.attrState != null)
            {
                y = this.attrState.SaveViewState();
            }
            if ((x == null) && (y == null))
            {
                return pair;
            }
            return new Pair(x, y);
        }

        string IAttributeAccessor.GetAttribute(string name)
        {
            if (this.attrState == null)
            {
                return null;
            }
            return (string) this.attrState[name];
        }

        void IAttributeAccessor.SetAttribute(string name, string value)
        {
            this.Attributes[name] = value;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this.ControlStyleCreated)
            {
                this.ControlStyle.TrackViewState();
            }
            if (this.attrState != null)
            {
                this.attrState.TrackViewState();
            }
        }

        [DefaultValue(""), WebCategory("Accessibility"), WebSysDescription("WebControl_AccessKey")]
        public virtual string AccessKey
        {
            get
            {
                if (this._webControlFlags[4])
                {
                    string str = (string) this.ViewState["AccessKey"];
                    if (str != null)
                    {
                        return str;
                    }
                }
                return string.Empty;
            }
            set
            {
                if ((value != null) && (value.Length > 1))
                {
                    throw new ArgumentOutOfRangeException("value", System.Web.SR.GetString("WebControl_InvalidAccessKey"));
                }
                this.ViewState["AccessKey"] = value;
                this._webControlFlags.Set(4);
            }
        }

        [Browsable(false), WebSysDescription("WebControl_Attributes"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (this.attrColl == null)
                {
                    if (this.attrState == null)
                    {
                        this.attrState = new StateBag(true);
                        if (base.IsTrackingViewState)
                        {
                            this.attrState.TrackViewState();
                        }
                    }
                    this.attrColl = new System.Web.UI.AttributeCollection(this.attrState);
                }
                return this.attrColl;
            }
        }

        [DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter)), WebSysDescription("WebControl_BackColor"), WebCategory("Appearance")]
        public virtual Color BackColor
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Color.Empty;
                }
                return this.ControlStyle.BackColor;
            }
            set
            {
                this.ControlStyle.BackColor = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(typeof(Color), ""), WebSysDescription("WebControl_BorderColor"), TypeConverter(typeof(WebColorConverter))]
        public virtual Color BorderColor
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Color.Empty;
                }
                return this.ControlStyle.BorderColor;
            }
            set
            {
                this.ControlStyle.BorderColor = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(0), WebSysDescription("WebControl_BorderStyle")]
        public virtual System.Web.UI.WebControls.BorderStyle BorderStyle
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.BorderStyle.NotSet;
                }
                return this.ControlStyle.BorderStyle;
            }
            set
            {
                this.ControlStyle.BorderStyle = value;
            }
        }

        [WebSysDescription("WebControl_BorderWidth"), DefaultValue(typeof(Unit), ""), WebCategory("Appearance")]
        public virtual Unit BorderWidth
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return Unit.Empty;
                }
                return this.ControlStyle.BorderWidth;
            }
            set
            {
                this.ControlStyle.BorderWidth = value;
            }
        }

        [Browsable(false), WebSysDescription("WebControl_ControlStyle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Web.UI.WebControls.Style ControlStyle
        {
            get
            {
                if (this.controlStyle == null)
                {
                    this.controlStyle = this.CreateControlStyle();
                    if (base.IsTrackingViewState)
                    {
                        this.controlStyle.TrackViewState();
                    }
                    if (this._webControlFlags[1])
                    {
                        this._webControlFlags.Clear(1);
                        this.controlStyle.LoadViewState(null);
                    }
                }
                return this.controlStyle;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), WebSysDescription("WebControl_ControlStyleCreated"), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool ControlStyleCreated =>
            (this.controlStyle != null);

        [WebCategory("Appearance"), CssClassProperty, DefaultValue(""), WebSysDescription("WebControl_CSSClassName")]
        public virtual string CssClass
        {
            get
            {
                if (!this.ControlStyleCreated)
                {
                    return string.Empty;
                }
                return this.ControlStyle.CssClass;
            }
            set
            {
                this.ControlStyle.CssClass = value;
            }
        }

        [DefaultValue(true), WebCategory("Behavior"), Themeable(false), Bindable(true), WebSysDescription("WebControl_Enabled")]
        public virtual bool Enabled
        {
            get => 
                !this.flags[0x80000];
            set
            {
                bool flag = !this.flags[0x80000];
                if (flag != value)
                {
                    if (!value)
                    {
                        this.flags.Set(0x80000);
                    }
                    else
                    {
                        this.flags.Clear(0x80000);
                    }
                    if (base.IsTrackingViewState)
                    {
                        this._webControlFlags.Set(2);
                    }
                }
            }
        }

        [Browsable(true)]
        public override bool EnableTheming
        {
            get => 
                base.EnableTheming;
            set
            {
                base.EnableTheming = value;
            }
        }

        [WebCategory("Appearance"), WebSysDescription("WebControl_Font"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        public virtual FontInfo Font =>
            this.ControlStyle.Font;

        [WebSysDescription("WebControl_ForeColor"), DefaultValue(typeof(Color), ""), WebCategory("Appearance"), TypeConverter(typeof(WebColorConverter))]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool HasAttributes =>
            (((this.attrColl != null) && (this.attrColl.Count > 0)) || ((this.attrState != null) && (this.attrState.Count > 0)));

        [WebCategory("Layout"), DefaultValue(typeof(Unit), ""), WebSysDescription("WebControl_Height")]
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

        protected internal bool IsEnabled
        {
            get
            {
                for (Control control = this; control != null; control = control.Parent)
                {
                    if (control.flags[0x80000])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal virtual bool RequiresLegacyRendering =>
            false;

        [Browsable(true)]
        public override string SkinID
        {
            get => 
                base.SkinID;
            set
            {
                base.SkinID = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebSysDescription("WebControl_Style")]
        public CssStyleCollection Style =>
            this.Attributes.CssStyle;

        [DefaultValue((short) 0), WebSysDescription("WebControl_TabIndex"), WebCategory("Accessibility")]
        public virtual short TabIndex
        {
            get
            {
                if (this._webControlFlags[0x10])
                {
                    object obj2 = this.ViewState["TabIndex"];
                    if (obj2 != null)
                    {
                        return (short) obj2;
                    }
                }
                return 0;
            }
            set
            {
                this.ViewState["TabIndex"] = value;
                this._webControlFlags.Set(0x10);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        protected virtual HtmlTextWriterTag TagKey =>
            this.tagKey;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual string TagName
        {
            get
            {
                if ((this.tagName == null) && (this.TagKey != HtmlTextWriterTag.Unknown))
                {
                    this.tagName = Enum.Format(typeof(HtmlTextWriterTag), this.TagKey, "G").ToLower(CultureInfo.InvariantCulture);
                }
                return this.tagName;
            }
        }

        [WebCategory("Behavior"), DefaultValue(""), WebSysDescription("WebControl_Tooltip"), Localizable(true)]
        public virtual string ToolTip
        {
            get
            {
                if (this._webControlFlags[8])
                {
                    string str = (string) this.ViewState["ToolTip"];
                    if (str != null)
                    {
                        return str;
                    }
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ToolTip"] = value;
                this._webControlFlags.Set(8);
            }
        }

        [DefaultValue(typeof(Unit), ""), WebSysDescription("WebControl_Width"), WebCategory("Layout")]
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

