namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    [RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.AttachedTemplatePopup", "HTMLEditor.Popups.AttachedTemplatePopup.js"), ToolboxItem(false)]
    public class AttachedTemplatePopup : AttachedPopup
    {
        private string _containerCSSClass = "ajax__htmleditor_attachedpopup_default";
        private Collection<Control> _content;
        private HtmlGenericControl _contentDiv;
        private ITemplate _contentTemplate;

        protected override void CreateChildControls()
        {
            this._contentDiv = new HtmlGenericControl("div");
            this._contentDiv.Style[HtmlTextWriterStyle.Display] = "none";
            HtmlGenericControl child = new HtmlGenericControl("div");
            child.Attributes.Add("class", this.ContainerCSSClass);
            this._contentDiv.Controls.Add(child);
            for (int i = 0; i < this.Content.Count; i++)
            {
                child.Controls.Add(this.Content[i]);
            }
            this.Controls.Add(this._contentDiv);
            base.CreateChildControls();
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddElementProperty("contentDiv", this._contentDiv.ClientID);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (base.CssPath.Length == 0)
            {
                base.CssPath = this.Page.ClientScript.GetWebResourceUrl(typeof(AttachedPopup), "HTMLEditor.Popups.AttachedTemplatePopup.css");
            }
            if (this._contentTemplate != null)
            {
                Control container = new Control();
                this._contentTemplate.InstantiateIn(container);
                this.Content.Add(container);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            this._contentDiv.Attributes.Add("id", this._contentDiv.ClientID);
            base.OnPreRender(e);
        }

        [DefaultValue("ajax__htmleditor_attachedpopup_default"), Category("Appearance")]
        public string ContainerCSSClass
        {
            get => 
                this._containerCSSClass;
            set
            {
                this._containerCSSClass = value;
            }
        }

        protected Collection<Control> Content
        {
            get
            {
                if (this._content == null)
                {
                    this._content = new Collection<Control>();
                }
                return this._content;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), TemplateInstance(TemplateInstance.Single), Browsable(false)]
        public ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
            }
        }
    }
}

