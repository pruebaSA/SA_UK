namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), ToolboxItem(false), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.JustifyLeft", "HTMLEditor.Toolbar_buttons.JustifyLeft.js")]
    public class JustifyLeft : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_align_left");
            base.OnPreRender(e);
        }
    }
}

