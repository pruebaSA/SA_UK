namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Italic", "HTMLEditor.Toolbar_buttons.Italic.js"), ToolboxItem(false), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false)]
    public class Italic : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_italic");
            base.OnPreRender(e);
        }
    }
}

