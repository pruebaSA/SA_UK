namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Resources;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.Popup", "HTMLEditor.Popups.Popup.js"), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true)]
    public abstract class Popup : ScriptControlBase
    {
        private bool _autoDimensions;
        private string _cssPath;
        private HtmlGenericControl _iframe;
        private string _initialContent;
        private Collection<RegisteredField> _registeredFields;
        private Collection<RegisteredField> _registeredHandlers;
        private ResourceManager _rm;
        private string _savedCSS;

        protected Popup() : base(false, HtmlTextWriterTag.Div)
        {
            this._initialContent = "";
            this._cssPath = "";
            this._autoDimensions = true;
        }

        protected override Style CreateControlStyle() => 
            new PopupStyle(this.ViewState, this);

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddElementProperty("iframe", this._iframe.ClientID);
            descriptor.AddProperty("registeredFields", this.RegisteredFieldsIds);
            descriptor.AddProperty("registeredHandlers", this.RegisteredHandlersIds);
        }

        protected string GetButton(string name) => 
            this._rm.GetString("HTMLEditor_toolbar_popup_" + base.GetType().Name + "_button_" + name);

        public static Popup GetExistingPopup(Control parent, Type type)
        {
            foreach (Control control in parent.Controls)
            {
                if (control.GetType().Equals(type))
                {
                    return (control as Popup);
                }
                Control existingPopup = GetExistingPopup(control, type);
                if (existingPopup != null)
                {
                    return (existingPopup as Popup);
                }
            }
            return null;
        }

        protected string GetField(string name) => 
            this._rm.GetString("HTMLEditor_toolbar_popup_" + base.GetType().Name + "_field_" + name);

        protected string GetField(string name, string subName) => 
            this.GetField(name + "_" + subName);

        protected override void OnInit(EventArgs e)
        {
            this._rm = new ResourceManager("ScriptResources.BaseScriptsResources", Assembly.GetExecutingAssembly());
            base.OnInit(e);
            if (!this.isDesign)
            {
                this._iframe = new HtmlGenericControl("iframe");
                this._iframe.Attributes.Add("scrolling", "no");
                this._iframe.Attributes.Add("marginHeight", "0");
                this._iframe.Attributes.Add("marginWidth", "0");
                this._iframe.Attributes.Add("frameborder", "0");
                this._iframe.Attributes.Add("tabindex", "-1");
                this.Controls.Add(this._iframe);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this._iframe != null)
            {
                string str = (this._savedCSS != null) ? this._savedCSS : base.Style.Value;
                if ((str != null) && (str.Length > 0))
                {
                    this._iframe.Style.Value = str;
                }
                if (this.Height.ToString().Length > 0)
                {
                    this._iframe.Style[HtmlTextWriterStyle.Height] = this.Height.ToString();
                }
                if (this.Width.ToString().Length > 0)
                {
                    this._iframe.Style[HtmlTextWriterStyle.Width] = this.Width.ToString();
                }
                this._iframe.Attributes.Add("id", this._iframe.ClientID);
            }
            this.Height = this.Height;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.isDesign)
            {
                base.Render(writer);
            }
        }

        [DefaultValue(true), ExtenderControlProperty, ClientPropertyName("autoDimensions"), Category("behavior")]
        public bool AutoDimensions
        {
            get => 
                this._autoDimensions;
            set
            {
                this._autoDimensions = value;
            }
        }

        [ExtenderControlProperty, Category("Appearance"), ClientPropertyName("cssPath"), DefaultValue("")]
        public string CssPath
        {
            get => 
                this._cssPath;
            set
            {
                this._cssPath = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("initialContent"), DefaultValue(""), Category("Appearance")]
        public string InitialContent
        {
            get => 
                this._initialContent;
            set
            {
                this._initialContent = value;
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

        public Collection<RegisteredField> RegisteredFields
        {
            get
            {
                if (this._registeredFields == null)
                {
                    this._registeredFields = new Collection<RegisteredField>();
                }
                return this._registeredFields;
            }
        }

        private string RegisteredFieldsIds
        {
            get
            {
                string str = "[";
                for (int i = 0; i < this.RegisteredFields.Count; i++)
                {
                    if (i > 0)
                    {
                        str = str + ",";
                    }
                    str = ((((str + "{name: ") + "'" + this.RegisteredFields[i].Name + "'") + ", clientID: ") + "'" + this.RegisteredFields[i].Control.ClientID + "'") + "}";
                }
                return (str + "]");
            }
        }

        public Collection<RegisteredField> RegisteredHandlers
        {
            get
            {
                if (this._registeredHandlers == null)
                {
                    this._registeredHandlers = new Collection<RegisteredField>();
                }
                return this._registeredHandlers;
            }
        }

        private string RegisteredHandlersIds
        {
            get
            {
                string str = "[";
                for (int i = 0; i < this.RegisteredHandlers.Count; i++)
                {
                    if (i > 0)
                    {
                        str = str + ",";
                    }
                    str = ((((str + "{name: ") + "'" + this.RegisteredHandlers[i].Name + "'") + ", clientID: ") + "'" + this.RegisteredHandlers[i].Control.ClientID + "'") + ", callMethod: null" + "}";
                }
                return (str + "]");
            }
        }

        private sealed class PopupStyle : Style
        {
            private Popup _popup;

            public PopupStyle(StateBag state, Popup popup) : base(state)
            {
                this._popup = popup;
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                this._popup._savedCSS = attributes.Value;
                attributes.Add(HtmlTextWriterStyle.Position, "absolute");
                attributes.Add(HtmlTextWriterStyle.Top, "-2000px");
                attributes.Add(HtmlTextWriterStyle.Left, "-2000px");
            }
        }
    }
}

