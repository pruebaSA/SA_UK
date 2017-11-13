namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DecreaseIndent", "HTMLEditor.Toolbar_buttons.DecreaseIndent.js"), ToolboxItem(false), ParseChildren(true)]
    public class DecreaseIndent : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_indent_less");
            base.OnPreRender(e);
        }
    }
}

