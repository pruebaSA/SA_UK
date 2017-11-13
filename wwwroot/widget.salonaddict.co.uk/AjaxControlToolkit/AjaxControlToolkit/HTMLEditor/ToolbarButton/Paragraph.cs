namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.Paragraph", "HTMLEditor.Toolbar_buttons.Paragraph.js"), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class Paragraph : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_format_paragraph");
            base.OnPreRender(e);
        }
    }
}

