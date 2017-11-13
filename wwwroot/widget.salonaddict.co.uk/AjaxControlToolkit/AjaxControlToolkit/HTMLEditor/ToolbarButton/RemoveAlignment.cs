namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), ToolboxItem(false), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.RemoveAlignment", "HTMLEditor.Toolbar_buttons.RemoveAlignment.js")]
    public class RemoveAlignment : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_removealign");
            base.OnPreRender(e);
        }
    }
}

