namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.BackColorClear", "HTMLEditor.Toolbar_buttons.BackColorClear.js"), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), PersistChildren(false)]
    public class BackColorClear : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_color_bg_clear");
            base.OnPreRender(e);
        }
    }
}

