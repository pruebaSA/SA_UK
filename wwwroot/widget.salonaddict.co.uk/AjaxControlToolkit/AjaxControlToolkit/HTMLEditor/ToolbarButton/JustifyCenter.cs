namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.JustifyCenter", "HTMLEditor.Toolbar_buttons.JustifyCenter.js"), ParseChildren(true)]
    public class JustifyCenter : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_align_center");
            base.OnPreRender(e);
        }
    }
}

