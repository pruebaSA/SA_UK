namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.PasteText", "HTMLEditor.Toolbar_buttons.PasteText.js"), ToolboxItem(false), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class PasteText : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_pasteText");
            base.OnPreRender(e);
        }
    }
}

