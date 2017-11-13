namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Rtl", "HTMLEditor.Toolbar_buttons.Rtl.js"), ToolboxItem(false), ParseChildren(true), PersistChildren(false)]
    public class Rtl : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_rtl");
            base.OnPreRender(e);
        }
    }
}

