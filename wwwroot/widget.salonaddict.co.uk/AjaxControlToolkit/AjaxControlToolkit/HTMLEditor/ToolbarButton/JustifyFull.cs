namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), ToolboxItem(false), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.JustifyFull", "HTMLEditor.Toolbar_buttons.JustifyFull.js")]
    public class JustifyFull : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_align_justify");
            base.OnPreRender(e);
        }
    }
}

