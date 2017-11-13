namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Copy", "HTMLEditor.Toolbar_buttons.Copy.js"), ParseChildren(true), PersistChildren(false)]
    public class Copy : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_copy");
            base.OnPreRender(e);
        }
    }
}

