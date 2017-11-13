namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.HtmlMode", "HTMLEditor.Toolbar_buttons.HtmlMode.js"), ParseChildren(true), PersistChildren(false)]
    public class HtmlMode : ModeButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_html");
            base.ActiveMode = ActiveModeType.Html;
            base.OnPreRender(e);
        }
    }
}

