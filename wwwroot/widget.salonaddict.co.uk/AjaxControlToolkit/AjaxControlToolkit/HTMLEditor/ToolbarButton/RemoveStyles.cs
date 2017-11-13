namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.RemoveStyles", "HTMLEditor.Toolbar_buttons.RemoveStyles.js"), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class RemoveStyles : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_unformat");
            base.OnPreRender(e);
        }
    }
}

