namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.PreviewMode", "HTMLEditor.Toolbar_buttons.PreviewMode.js"), ToolboxItem(false), ParseChildren(true)]
    public class PreviewMode : ModeButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_preview");
            base.ActiveMode = ActiveModeType.Preview;
            base.OnPreRender(e);
        }
    }
}

