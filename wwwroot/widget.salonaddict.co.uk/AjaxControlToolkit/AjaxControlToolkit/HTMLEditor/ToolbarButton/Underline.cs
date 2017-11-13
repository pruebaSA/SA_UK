namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline", "HTMLEditor.Toolbar_buttons.Underline.js"), ToolboxItem(false), ParseChildren(true), PersistChildren(false)]
    public class Underline : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_underline");
            base.OnPreRender(e);
        }
    }
}

