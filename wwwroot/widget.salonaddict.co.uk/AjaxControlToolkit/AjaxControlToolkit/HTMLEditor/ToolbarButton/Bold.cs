namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Bold", "HTMLEditor.Toolbar_buttons.Bold.js"), ParseChildren(true), ToolboxItem(false), PersistChildren(false)]
    public class Bold : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_bold");
            base.OnPreRender(e);
        }
    }
}

