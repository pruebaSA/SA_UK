namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.ToolbarButton;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Toolbar", "HTMLEditor.Toolbar.js"), PersistChildren(false), ParseChildren(true)]
    public abstract class Toolbar : ScriptControlBase
    {
        private Collection<CommonButton> _buttons;
        private bool _wasPreRender;

        protected Toolbar() : base(false, HtmlTextWriterTag.Div)
        {
        }

        protected override void CreateChildControls()
        {
            for (int i = 0; i < this.Buttons.Count; i++)
            {
                this.Controls.Add(this.Buttons[i]);
                if (!this.AlwaysVisible && !this.IsDesign)
                {
                    if (!this.Buttons[i].PreservePlace)
                    {
                        this.Buttons[i].Style[HtmlTextWriterStyle.Display] = "none";
                    }
                    else
                    {
                        this.Buttons[i].Style[HtmlTextWriterStyle.Visibility] = "hidden";
                    }
                }
                for (int j = 0; j < this.Buttons[i].ExportedControls.Count; j++)
                {
                    this.Controls.Add(this.Buttons[i].ExportedControls[j]);
                }
            }
        }

        internal void CreateChilds(DesignerWithMapPath designer)
        {
            this.Controls.Clear();
            this.CreateChildControls();
            for (int i = 0; i < this.Controls.Count; i++)
            {
                CommonButton button = this.Controls[i] as CommonButton;
                if (button != null)
                {
                    button.CreateChilds(designer);
                }
            }
        }

        protected string LocalResolveUrl(string path)
        {
            string input = base.ResolveUrl(path);
            Regex regex = new Regex(@"(\(S\([A-Za-z0-9_]+\)\)/)", RegexOptions.Compiled);
            return regex.Replace(input, "");
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
            for (int i = 0; i < this.Controls.Count; i++)
            {
                CommonButton button = this.Controls[i] as CommonButton;
                if (button != null)
                {
                    if (!this.IsDesign)
                    {
                        if (!button.PreservePlace)
                        {
                            button.Style[HtmlTextWriterStyle.Display] = "none";
                        }
                        else
                        {
                            button.Style[HtmlTextWriterStyle.Visibility] = "hidden";
                        }
                    }
                    else
                    {
                        button.Style.Remove(HtmlTextWriterStyle.Display);
                        button.Style.Remove(HtmlTextWriterStyle.Visibility);
                    }
                }
            }
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
        public bool ShouldSerializeButtonIds() => 
            base.IsRenderingScript;

        [ClientPropertyName("alwaysVisible"), Category("Behavior"), ExtenderControlProperty, DefaultValue(false)]
        public bool AlwaysVisible
        {
            get => 
                (this.ViewState["AlwaysVisible"] ?? false);
            set
            {
                this.ViewState["AlwaysVisible"] = value;
            }
        }

        [ExtenderControlProperty, Browsable(false), ClientPropertyName("buttonIds")]
        public string ButtonIds
        {
            get
            {
                string str = "";
                for (int i = 0; i < this.Buttons.Count; i++)
                {
                    if (i > 0)
                    {
                        str = str + ";";
                    }
                    str = str + this.Buttons[i].ClientID;
                }
                return str;
            }
        }

        [Category("Appearance"), Description("Folder used for toolbar's buttons' images"), DefaultValue("")]
        public string ButtonImagesFolder
        {
            get => 
                (this.ViewState["ButtonImagesFolder"] ?? "");
            set
            {
                string str = this.LocalResolveUrl(value);
                if (str.Length > 0)
                {
                    string str2 = str.Substring(str.Length - 1, 1);
                    if ((str2 != @"\") && (str2 != "/"))
                    {
                        str = str + "/";
                    }
                    this.ViewState["ButtonImagesFolder"] = str;
                }
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Collection<CommonButton> Buttons
        {
            get
            {
                if (this._buttons == null)
                {
                    this._buttons = new Collection<CommonButton>();
                }
                return this._buttons;
            }
            internal set
            {
                this._buttons = value;
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
    }
}

