namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Selector", "HTMLEditor.Toolbar_buttons.Selector.js"), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class Selector : DesignModePopupImageButton
    {
        protected Selector()
        {
        }

        protected override Style CreateControlStyle() => 
            new SelectorStyle(this.ViewState);

        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_selector");
            base.OnPreRender(e);
        }

        private sealed class SelectorStyle : Style
        {
            public SelectorStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Add("width", "11px");
            }
        }
    }
}

