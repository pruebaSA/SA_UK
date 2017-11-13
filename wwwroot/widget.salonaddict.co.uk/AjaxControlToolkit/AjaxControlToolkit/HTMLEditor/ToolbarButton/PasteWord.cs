namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.PasteWord", "HTMLEditor.Toolbar_buttons.PasteWord.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true)]
    public class PasteWord : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_pasteWord");
            base.OnPreRender(e);
        }
    }
}

