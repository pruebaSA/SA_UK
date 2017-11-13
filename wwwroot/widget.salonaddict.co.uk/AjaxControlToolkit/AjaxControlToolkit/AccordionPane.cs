namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ParseChildren(true), ToolboxBitmap(typeof(AccordionPane), "Accordion.Accordion.ico"), ToolboxData("<{0}:AccordionPane runat=\"server\"></{0}:AccordionPane>")]
    public class AccordionPane : WebControl
    {
        private AccordionContentPanel _content;
        private ITemplate _contentTemplate;
        private AccordionContentPanel _header;
        private ITemplate _headerTemplate;

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this._header = new AccordionContentPanel(null, -1, AccordionItemType.Header);
            this._header.ID = string.Format(CultureInfo.InvariantCulture, "{0}_header", new object[] { this.ID });
            this.Controls.Add(this._header);
            this._content = new AccordionContentPanel(null, -1, AccordionItemType.Content);
            this._content.ID = string.Format(CultureInfo.InvariantCulture, "{0}_content", new object[] { this.ID });
            this.Controls.Add(this._content);
            this._content.Collapsed = true;
            if (this._headerTemplate != null)
            {
                this._headerTemplate.InstantiateIn(this._header);
            }
            if (this._contentTemplate != null)
            {
                this._contentTemplate.InstantiateIn(this._content);
            }
        }

        public override Control FindControl(string id)
        {
            this.EnsureChildControls();
            return (base.FindControl(id) ?? (this._header.FindControl(id) ?? this._content.FindControl(id)));
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }

        [TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(AccordionContentPanel)), Browsable(false), DefaultValue((string) null), Description("Accordion Pane Content")]
        public virtual ITemplate Content
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AccordionContentPanel ContentContainer
        {
            get
            {
                this.EnsureChildControls();
                return this._content;
            }
        }

        [Browsable(true), Description("CSS class for Accordion Pane Content"), Category("Appearance")]
        public string ContentCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._content.CssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._content.CssClass = value;
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

        [PersistenceMode(PersistenceMode.InnerProperty), Description("Accordion Pane Header"), TemplateInstance(TemplateInstance.Single), DefaultValue((string) null), Browsable(false), TemplateContainer(typeof(AccordionContentPanel))]
        public virtual ITemplate Header
        {
            get => 
                this._headerTemplate;
            set
            {
                this._headerTemplate = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public AccordionContentPanel HeaderContainer
        {
            get
            {
                this.EnsureChildControls();
                return this._header;
            }
        }

        [Browsable(true), Description("CSS class for Accordion Pane Header"), Category("Appearance")]
        public string HeaderCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._header.CssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._header.CssClass = value;
            }
        }
    }
}

