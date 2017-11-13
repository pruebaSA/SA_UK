namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer("System.Web.UI.Design.WebControls.WebParts.DeclarativeCatalogPartDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DeclarativeCatalogPart : CatalogPart
    {
        private WebPartDescriptionCollection _descriptions;
        private string _webPartsListUserControlPath;
        private ITemplate _webPartsTemplate;

        private void AddControlToDescriptions(Control control, ArrayList descriptions)
        {
            WebPart webPart = control as WebPart;
            if ((webPart == null) && !(control is LiteralControl))
            {
                if (base.WebPartManager != null)
                {
                    webPart = base.WebPartManager.CreateWebPart(control);
                }
                else
                {
                    webPart = WebPartManager.CreateWebPartStatic(control);
                }
            }
            if ((webPart != null) && ((base.WebPartManager == null) || base.WebPartManager.IsAuthorized(webPart)))
            {
                WebPartDescription description = new WebPartDescription(webPart);
                descriptions.Add(description);
            }
        }

        public override WebPartDescriptionCollection GetAvailableWebPartDescriptions()
        {
            if (this._descriptions == null)
            {
                this.LoadAvailableWebParts();
            }
            return this._descriptions;
        }

        public override WebPart GetWebPart(WebPartDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            if (!this.GetAvailableWebPartDescriptions().Contains(description))
            {
                throw new ArgumentException(System.Web.SR.GetString("CatalogPart_UnknownDescription"), "description");
            }
            return description.WebPart;
        }

        private void LoadAvailableWebParts()
        {
            ArrayList descriptions = new ArrayList();
            if (this.WebPartsTemplate != null)
            {
                Control container = new NonParentingControl();
                this.WebPartsTemplate.InstantiateIn(container);
                if (container.HasControls())
                {
                    Control[] array = new Control[container.Controls.Count];
                    container.Controls.CopyTo(array, 0);
                    foreach (Control control2 in array)
                    {
                        this.AddControlToDescriptions(control2, descriptions);
                    }
                }
            }
            string webPartsListUserControlPath = this.WebPartsListUserControlPath;
            if (!string.IsNullOrEmpty(webPartsListUserControlPath) && !base.DesignMode)
            {
                Control control3 = this.Page.LoadControl(webPartsListUserControlPath);
                if ((control3 != null) && control3.HasControls())
                {
                    Control[] controlArray2 = new Control[control3.Controls.Count];
                    control3.Controls.CopyTo(controlArray2, 0);
                    foreach (Control control4 in controlArray2)
                    {
                        this.AddControlToDescriptions(control4, descriptions);
                    }
                }
            }
            this._descriptions = new WebPartDescriptionCollection(descriptions);
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string AccessKey
        {
            get => 
                base.AccessKey;
            set
            {
                base.AccessKey = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get => 
                base.BackColor;
            set
            {
                base.BackColor = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string BackImageUrl
        {
            get => 
                base.BackImageUrl;
            set
            {
                base.BackImageUrl = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false)]
        public override Color BorderColor
        {
            get => 
                base.BorderColor;
            set
            {
                base.BorderColor = value;
            }
        }

        [Browsable(false), Themeable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Web.UI.WebControls.BorderStyle BorderStyle
        {
            get => 
                base.BorderStyle;
            set
            {
                base.BorderStyle = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Themeable(false)]
        public override Unit BorderWidth
        {
            get => 
                base.BorderWidth;
            set
            {
                base.BorderWidth = value;
            }
        }

        [CssClassProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false)]
        public override string CssClass
        {
            get => 
                base.CssClass;
            set
            {
                base.CssClass = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultButton
        {
            get => 
                base.DefaultButton;
            set
            {
                base.DefaultButton = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false)]
        public override ContentDirection Direction
        {
            get => 
                base.Direction;
            set
            {
                base.Direction = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Themeable(false)]
        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [DefaultValue(false), Browsable(false), Themeable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool EnableTheming
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override FontInfo Font =>
            base.Font;

        [Themeable(false), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Color ForeColor
        {
            get => 
                base.ForeColor;
            set
            {
                base.ForeColor = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string GroupingText
        {
            get => 
                base.GroupingText;
            set
            {
                base.GroupingText = value;
            }
        }

        [Browsable(false), Themeable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Unit Height
        {
            get => 
                base.Height;
            set
            {
                base.Height = value;
            }
        }

        [Browsable(false), Themeable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Web.UI.WebControls.HorizontalAlign HorizontalAlign
        {
            get => 
                base.HorizontalAlign;
            set
            {
                base.HorizontalAlign = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Web.UI.WebControls.ScrollBars ScrollBars
        {
            get => 
                base.ScrollBars;
            set
            {
                base.ScrollBars = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(""), Themeable(false)]
        public override string SkinID
        {
            get => 
                string.Empty;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override short TabIndex
        {
            get => 
                base.TabIndex;
            set
            {
                base.TabIndex = value;
            }
        }

        [WebSysDefaultValue("DeclarativeCatalogPart_PartTitle")]
        public override string Title
        {
            get
            {
                string str = (string) this.ViewState["Title"];
                if (str == null)
                {
                    return System.Web.SR.GetString("DeclarativeCatalogPart_PartTitle");
                }
                return str;
            }
            set
            {
                this.ViewState["Title"] = value;
            }
        }

        [Browsable(false), Themeable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToolTip
        {
            get => 
                base.ToolTip;
            set
            {
                base.ToolTip = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Themeable(false), Browsable(false)]
        public override bool Visible
        {
            get => 
                base.Visible;
            set
            {
                base.Visible = value;
            }
        }

        [DefaultValue(""), Editor("System.Web.UI.Design.UserControlFileEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebSysDescription("DeclarativeCatlaogPart_WebPartsListUserControlPath"), Themeable(false), UrlProperty, WebCategory("Behavior")]
        public string WebPartsListUserControlPath
        {
            get
            {
                if (this._webPartsListUserControlPath == null)
                {
                    return string.Empty;
                }
                return this._webPartsListUserControlPath;
            }
            set
            {
                this._webPartsListUserControlPath = value;
                this._descriptions = null;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateContainer(typeof(DeclarativeCatalogPart))]
        public ITemplate WebPartsTemplate
        {
            get => 
                this._webPartsTemplate;
            set
            {
                this._webPartsTemplate = value;
                this._descriptions = null;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false)]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Wrap
        {
            get => 
                base.Wrap;
            set
            {
                base.Wrap = value;
            }
        }
    }
}

