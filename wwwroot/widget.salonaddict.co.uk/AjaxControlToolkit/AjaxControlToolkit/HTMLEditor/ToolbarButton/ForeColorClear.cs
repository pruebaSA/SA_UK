namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ForeColorClear", "HTMLEditor.Toolbar_buttons.ForeColorClear.js"), ToolboxItem(false), ParseChildren(true)]
    public class ForeColorClear : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_color_fg_clear");
            base.OnPreRender(e);
        }
    }
}

