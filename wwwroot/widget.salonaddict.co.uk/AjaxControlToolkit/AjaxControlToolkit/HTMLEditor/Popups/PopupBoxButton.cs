namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.PopupBoxButton", "HTMLEditor.Popups.PopupBoxButton.js"), PersistChildren(false)]
    internal class PopupBoxButton : PopupCommonButton
    {
        private Collection<Control> _content;
        private ITemplate _contentTemplate;

        public PopupBoxButton() : base(HtmlTextWriterTag.Div)
        {
            this.CssClass = "ajax__htmleditor_popup_boxbutton";
        }

        public PopupBoxButton(HtmlTextWriterTag tag) : base(tag)
        {
            this.CssClass = "ajax__htmleditor_popup_boxbutton";
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

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), MergableProperty(false), TemplateInstance(TemplateInstance.Single)]
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

