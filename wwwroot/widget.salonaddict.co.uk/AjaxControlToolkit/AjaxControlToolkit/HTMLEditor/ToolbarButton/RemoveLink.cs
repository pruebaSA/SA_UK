namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.RemoveLink", "HTMLEditor.Toolbar_buttons.RemoveLink.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class RemoveLink : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_unlink");
            base.OnPreRender(e);
        }
    }
}

