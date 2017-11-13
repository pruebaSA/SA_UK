namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.HorizontalSeparator", "HTMLEditor.Toolbar_buttons.HorizontalSeparator.js"), RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), PersistChildren(false)]
    public class HorizontalSeparator : DesignModeImageButton
    {
        protected override Style CreateControlStyle() => 
            new HorizontalSeparatorStyle(this.ViewState);

        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_sep");
            base.OnPreRender(e);
        }

        private sealed class HorizontalSeparatorStyle : Style
        {
            public HorizontalSeparatorStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Add("background-color", "transparent");
                attributes.Add("cursor", "text");
                attributes.Add("width", "13px");
            }
        }
    }
}

