namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Ltr", "HTMLEditor.Toolbar_buttons.Ltr.js"), ToolboxItem(false), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class Ltr : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_ltr");
            base.OnPreRender(e);
        }
    }
}

