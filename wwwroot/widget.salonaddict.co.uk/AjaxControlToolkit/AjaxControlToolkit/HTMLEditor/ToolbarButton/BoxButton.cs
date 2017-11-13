namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.BoxButton", "HTMLEditor.Toolbar_buttons.BoxButton.js"), ParseChildren(true)]
    public abstract class BoxButton : CommonButton
    {
        private Collection<Control> _content;
        private ITemplate _contentTemplate;

        protected BoxButton() : base(HtmlTextWriterTag.Div)
        {
        }

        protected override void CreateChildControls()
        {
            for (int i = 0; i < this.Content.Count; i++)
            {
                this.Controls.Add(this.Content[i]);
            }
            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this._contentTemplate != null)
            {
                Control container = new Control();
                this._contentTemplate.InstantiateIn(container);
                this.Content.Add(container);
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

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), TemplateInstance(TemplateInstance.Single)]
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

