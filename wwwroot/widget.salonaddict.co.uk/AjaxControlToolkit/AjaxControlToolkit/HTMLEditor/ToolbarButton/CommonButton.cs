namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.CommonButton", "HTMLEditor.Toolbar_buttons.CommonButton.js"), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false)]
    public abstract class CommonButton : ScriptControlBase
    {
        private Collection<ActiveModeType> _activeModes;
        internal DesignerWithMapPath _designer;
        private Collection<Control> _exportedControls;
        private bool _IgnoreTab;
        private ResourceManager _rm;
        private bool _wasPreRender;

        protected CommonButton() : base(false, HtmlTextWriterTag.Div)
        {
        }

        protected CommonButton(HtmlTextWriterTag tag) : base(false, tag)
        {
            base.CssClass = "ajax__htmleditor_toolbar_button";
        }

        internal virtual void CreateChilds(DesignerWithMapPath designer)
        {
            this._designer = designer;
            this.Controls.Clear();
            this.CreateChildControls();
        }

        protected string GetFromResource(string name) => 
            this._rm.GetString("HTMLEditor_toolbar_button_" + base.GetType().Name + "_" + name);

        protected override void OnInit(EventArgs e)
        {
            this._rm = new ResourceManager("ScriptResources.BaseScriptsResources", Assembly.GetExecutingAssembly());
            this.ToolTip = this._rm.GetString("HTMLEditor_toolbar_button_" + base.GetType().Name + "_title");
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
            }
            catch
            {
            }
            this._wasPreRender = true;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this._wasPreRender)
            {
                this.OnPreRender(new EventArgs());
            }
            base.Render(writer);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeActiveModesIds() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Collection<ActiveModeType> ActiveModes
        {
            get
            {
                if (this._activeModes == null)
                {
                    this._activeModes = new Collection<ActiveModeType>();
                }
                return this._activeModes;
            }
        }

        [Browsable(false), ExtenderControlProperty, ClientPropertyName("activeModesIds")]
        public string ActiveModesIds
        {
            get
            {
                string str = "";
                for (int i = 0; i < this.ActiveModes.Count; i++)
                {
                    if (i > 0)
                    {
                        str = str + ";";
                    }
                    int num2 = this.ActiveModes[i];
                    str = str + num2.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
                }
                return str;
            }
        }

        [DefaultValue("ajax__htmleditor_toolbar_button")]
        public override string CssClass =>
            "ajax__htmleditor_toolbar_button";

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

        [Category("Behavior"), DefaultValue(false)]
        public bool IgnoreTab
        {
            get => 
                this._IgnoreTab;
            set
            {
                this._IgnoreTab = value;
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

        internal System.Web.UI.Page Page
        {
            get => 
                base.Page;
            set
            {
                base.Page = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("preservePlace"), DefaultValue(false)]
        public bool PreservePlace
        {
            get => 
                (this.ViewState["PreservePlace"] ?? false);
            set
            {
                this.ViewState["PreservePlace"] = value;
            }
        }
    }
}

