namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.IncreaseIndent", "HTMLEditor.Toolbar_buttons.IncreaseIndent.js"), ToolboxItem(false)]
    public class IncreaseIndent : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_indent_more");
            base.OnPreRender(e);
        }
    }
}

