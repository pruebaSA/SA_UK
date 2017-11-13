namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.StrikeThrough", "HTMLEditor.Toolbar_buttons.StrikeThrough.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class StrikeThrough : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_strike");
            base.OnPreRender(e);
        }
    }
}

