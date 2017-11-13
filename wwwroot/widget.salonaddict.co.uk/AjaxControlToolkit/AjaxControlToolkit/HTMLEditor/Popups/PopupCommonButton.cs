namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.PopupCommonButton", "HTMLEditor.Popups.PopupCommonButton.js")]
    public abstract class PopupCommonButton : ScriptControlBase
    {
        private Collection<Control> _exportedControls;
        private string _name;

        protected PopupCommonButton() : base(false, HtmlTextWriterTag.Div)
        {
            this._name = string.Empty;
        }

        protected PopupCommonButton(HtmlTextWriterTag tag) : base(false, tag)
        {
            this._name = string.Empty;
        }

        internal Collection<Control> ExportedControls
        {
            get
            {
                if (this._exportedControls == null)
                {
                    this._exportedControls = new Collection<Control>();
                }
                return this._exportedControls;
            }
        }

        protected bool IsDesign
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

        [DefaultValue(""), ExtenderControlProperty, ClientPropertyName("name"), Category("Behavior")]
        public string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }
    }
}

