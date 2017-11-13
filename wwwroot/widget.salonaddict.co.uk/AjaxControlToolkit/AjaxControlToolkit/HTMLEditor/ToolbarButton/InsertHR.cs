namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.InsertHR", "HTMLEditor.Toolbar_buttons.InsertHR.js"), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class InsertHR : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_rule");
            base.OnPreRender(e);
        }
    }
}

