namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.SelectButton", "HTMLEditor.Toolbar_buttons.SelectButton.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class SelectButton : CommonButton
    {
        private Collection<SelectOption> _options;

        public SelectButton() : base(HtmlTextWriterTag.Div)
        {
        }

        protected override void CreateChildControls()
        {
            HtmlGenericControl child = new HtmlGenericControl("nobr");
            HtmlGenericControl control2 = new HtmlGenericControl("span");
            control2.Attributes.Add("class", "ajax__htmleditor_toolbar_selectlable");
            control2.ID = "label";
            control2.Controls.Add(new LiteralControl(base.GetFromResource("label") + "&nbsp;"));
            child.Controls.Add(control2);
            HtmlGenericControl control3 = new HtmlGenericControl("select");
            control3.Attributes.Add("class", "ajax__htmleditor_toolbar_selectbutton");
            control3.ID = "select";
            if (!string.IsNullOrEmpty(this.SelectWidth))
            {
                control3.Style[HtmlTextWriterStyle.Width] = this.SelectWidth;
            }
            if (base.IgnoreTab)
            {
                control3.Attributes.Add("tabindex", "-1");
            }
            child.Controls.Add(control3);
            if (this.UseDefaultValue)
            {
                control3.Controls.Add(new LiteralControl("<option value=\"" + this.DefaultValue + "\">" + base.GetFromResource("defaultValue") + "</option>"));
            }
            for (int i = 0; i < this.Options.Count; i++)
            {
                control3.Controls.Add(new LiteralControl("<option value=\"" + this.Options[i].Value + "\">" + this.Options[i].Text + "</option>"));
            }
            this.Controls.Add(child);
        }

        protected override Style CreateControlStyle() => 
            new SelectButtonStyle(this.ViewState);

        [Category("Appearance"), DefaultValue("")]
        public virtual string DefaultValue =>
            string.Empty;

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Collection<SelectOption> Options
        {
            get
            {
                if (this._options == null)
                {
                    this._options = new Collection<SelectOption>();
                }
                return this._options;
            }
        }

        [Category("Appearance"), DefaultValue("")]
        public virtual string SelectWidth =>
            string.Empty;

        [Category("Appearance"), DefaultValue(true)]
        public virtual bool UseDefaultValue =>
            true;

        private sealed class SelectButtonStyle : Style
        {
            public SelectButtonStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Add("background-color", "transparent");
                attributes.Add("cursor", "text");
            }
        }
    }
}

